using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using AvenSuitesApi.Application.DTOs.Invoice;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using InvoiceItemInfo = AvenSuitesApi.Application.Services.Interfaces.InvoiceItemInfo;

namespace AvenSuitesApi.Application.Services.Implementations.Invoice;

public class IpmNfseService : IIpmNfseService
{
    private readonly IIpmCredentialsService _credentialsService;
    private readonly IHotelRepository _hotelRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IErpIntegrationLogRepository _erpLogRepository;
    private readonly IIpmHttpClient _httpClient;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<IpmNfseService> _logger;

    public IpmNfseService(
        IIpmCredentialsService credentialsService,
        IHotelRepository hotelRepository,
        IGuestRepository guestRepository,
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IInvoiceRepository invoiceRepository,
        IErpIntegrationLogRepository erpLogRepository,
        IIpmHttpClient httpClient,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        ILogger<IpmNfseService> logger)
    {
        _credentialsService = credentialsService;
        _hotelRepository = hotelRepository;
        _guestRepository = guestRepository;
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _invoiceRepository = invoiceRepository;
        _erpLogRepository = erpLogRepository;
        _httpClient = httpClient;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task<IpmNfseCreateResponse> GenerateInvoiceAsync(Guid hotelId, IpmNfseCreateRequest request)
    {
        // Buscar credenciais do hotel (senha já descriptografada automaticamente)
        var credentials = await _credentialsService.GetDecryptedByHotelIdAsync(hotelId);
        if (credentials == null || !credentials.Active)
            return new IpmNfseCreateResponse
            {
                Success = false,
                ErrorMessage = "Credenciais IPM não configuradas para este hotel"
            };

        // Buscar hotel
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);
        if (hotel == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Hotel não encontrado" };

        // Buscar booking (se fornecido)
        Domain.Entities.Booking? booking = null;
        if (request.BookingId.HasValue)
        {
            booking = await _bookingRepository.GetByIdAsync(request.BookingId.Value);
            if (booking == null)
                return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Reserva não encontrada" };
        }

        // Buscar tomador (hóspede)
        var guest = await _guestRepository.GetByIdWithPiiAsync(request.TomadorGuestId);
        if (guest == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Hóspede não encontrado" };

        // Gerar XML da NF-e
        var xmlContent = GenerateInvoiceXml(hotel, credentials, guest.GuestPii!, request);

        try
        {
            // Chamar webservice IPM
            var ipmResponse = await CallIpmWebServiceAsync(xmlContent, credentials.Username, credentials.Password);

            // Extrair dados da resposta
            var responseData = ExtractResponseData(ipmResponse);
            var isSuccess = responseData.Success || (!ipmResponse.Contains("erro") && !string.IsNullOrEmpty(responseData.NfseNumber));

            // Parsear resposta do IPM
            var invoice = new AvenSuitesApi.Domain.Entities.Invoice
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId, // Pode ser null
                HotelId = hotelId,
                Status = isSuccess ? "ISSUED" : "FAILED",
                IssueDate = ParseIssueDate(responseData.DataNfse, responseData.HoraNfse) ?? DateTime.UtcNow,
                NfseNumber = responseData.NfseNumber,
                NfseSeries = responseData.SerieNfse ?? credentials.SerieNfse,
                VerificationCode = responseData.VerificationCode,
                ErpProvider = "IPM",
                ErpProtocol = responseData.LinkNfse, // Armazena o link da NF-e no campo ErpProtocol
                TotalServices = request.TotalValue,
                TotalTaxes = CalculateTaxes(request),
                RawResponseJson = ipmResponse,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdInvoice = await _invoiceRepository.AddAsync(invoice);

            // Registrar no log
            var log = new AvenSuitesApi.Domain.Entities.ErpIntegrationLog
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                InvoiceId = createdInvoice.Id,
                Endpoint = "ipm/nfse/generate",
                Success = createdInvoice.Status == "ISSUED",
                RequestJson = xmlContent,
                ResponseJson = ipmResponse,
                CreatedAt = DateTime.UtcNow
            };
            await _erpLogRepository.AddAsync(log);

            return new IpmNfseCreateResponse
            {
                Success = createdInvoice.Status == "ISSUED",
                NfseNumber = createdInvoice.NfseNumber,
                SerieNfse = createdInvoice.NfseSeries,
                VerificationCode = createdInvoice.VerificationCode,
                XmlContent = ExtractXmlFromResponse(ipmResponse),
                PdfContent = ExtractPdfFromResponse(ipmResponse),
                RawResponse = ipmResponse
            };
        }
        catch (Exception ex)
        {
            return new IpmNfseCreateResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<IpmNfseCreateResponse> CancelInvoiceAsync(Guid hotelId, IpmNfseCancelRequest request)
    {
        var credentials = await _credentialsService.GetDecryptedByHotelIdAsync(hotelId);
        if (credentials == null || !credentials.Active)
            return new IpmNfseCreateResponse
            {
                Success = false,
                ErrorMessage = "Credenciais IPM não configuradas"
            };

        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        if (invoice == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Nota fiscal não encontrada" };

        // Gerar XML de cancelamento
        var xmlContent = GenerateCancelXml(credentials, request);

        // Chamar webservice
        // Implementar chamada

        invoice.Status = "CANCELLED";
        await _invoiceRepository.UpdateAsync(invoice);

        return new IpmNfseCreateResponse { Success = true };
    }

    public Task<IpmNfseCreateResponse> GetInvoiceByVerificationCodeAsync(Guid hotelId, string verificationCode)
    {
        throw new NotImplementedException();
    }

    public Task<IpmNfseCreateResponse> GetInvoiceByNumberAsync(Guid hotelId, string nfseNumber, string serie)
    {
        throw new NotImplementedException();
    }

    public async Task<IpmNfseCreateResponse> GenerateSimpleInvoiceAsync(Guid roomId, SimpleInvoiceCreateRequest request)
    {
        // 1. Buscar quarto e obter hotel
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Quarto não encontrado" };

        var hotelId = room.HotelId;

        // 2. Buscar hotel
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);
        if (hotel == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Hotel não encontrado" };

        // 3. Buscar credenciais IPM (já descriptografadas)
        var credentials = await _credentialsService.GetDecryptedByHotelIdAsync(hotelId);
        if (credentials == null || !credentials.Active)
            return new IpmNfseCreateResponse
            {
                Success = false,
                ErrorMessage = "Credenciais IPM não configuradas para este hotel"
            };

        // 4. Buscar hóspede com PII
        var guest = await _guestRepository.GetByIdWithPiiAsync(request.GuestId);
        if (guest == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Hóspede não encontrado" };

        if (guest.GuestPii == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Dados pessoais do hóspede não encontrados" };

        // 5. Buscar RoomType para calcular preço unitário
        var roomWithDetails = await _roomRepository.GetByIdWithDetailsAsync(roomId);
        var unitPrice = request.TotalValue / request.Days;
        var aliquotaISS = 2.01m; // Valor padrão para serviços de hospedagem (ajustar conforme necessidade)

        // 6. Montar request completo com dados preenchidos automaticamente
        var fullRequest = new IpmNfseCreateRequest
        {
            BookingId = null, // Não há booking neste caso simplificado
            Identifier = Guid.NewGuid().ToString(),
            SerieNfse = credentials.SerieNfse ?? "1",
            FatoGeradorDate = request.CheckInDate,
            TotalValue = request.TotalValue,
            DiscountValue = 0.00m,
            IrValue = 0.00m,
            InssValue = 0.00m,
            SocialContributionValue = 0.00m,
            RpsValue = 0.00m,
            PisValue = 0.00m,
            CofinsValue = 0.00m,
            Observations = request.Observations ?? $"Hospedagem - Quarto {room.RoomNumber} - {request.Description}",
            TomadorGuestId = request.GuestId,
            Items = new List<IpmNfseItemRequest>
            {
                new IpmNfseItemRequest
                {
                    TributaMunicipioPrestador = "N",
                    CodigoLocalPrestacao = credentials.CityCode,
                    UnidadeCodigo = "1", // Diária
                    UnidadeQuantidade = request.Days,
                    UnidadeValorUnitario = unitPrice,
                    CodigoItemListaServico = "901", // 901 = Serviços de hospedagem
                    Descritivo = $"{request.Description} - Quarto {room.RoomNumber} - {request.Adults} adulto(s), {request.Children} criança(s)",
                    AliquotaItemLista = aliquotaISS,
                    SituacaoTributaria = "0",
                    ValorTributavel = request.TotalValue
                }
            }
        };

        // 7. Gerar XML da NF-e
        var xmlContent = GenerateInvoiceXml(hotel, credentials, guest.GuestPii, fullRequest);

        try
        {
            // 8. Chamar webservice IPM
            var ipmResponse = await CallIpmWebServiceAsync(xmlContent, credentials.Username, credentials.Password);

            // 9. Extrair dados da resposta
            var responseData = ExtractResponseData(ipmResponse);
            var isSuccess = responseData.Success || (!ipmResponse.Contains("erro") && !string.IsNullOrEmpty(responseData.NfseNumber));

            // 10. Criar invoice no banco
            var invoice = new AvenSuitesApi.Domain.Entities.Invoice
            {
                Id = Guid.NewGuid(),
                BookingId = null, // Sem booking associado (invoice direta)
                HotelId = hotelId,
                Status = isSuccess ? "ISSUED" : "FAILED",
                IssueDate = ParseIssueDate(responseData.DataNfse, responseData.HoraNfse) ?? DateTime.UtcNow,
                NfseNumber = responseData.NfseNumber,
                NfseSeries = responseData.SerieNfse ?? credentials.SerieNfse ?? "1",
                VerificationCode = responseData.VerificationCode,
                ErpProvider = "IPM",
                ErpProtocol = responseData.LinkNfse, // Armazena o link da NF-e no campo ErpProtocol
                TotalServices = request.TotalValue,
                TotalTaxes = CalculateTaxes(fullRequest),
                RawResponseJson = ipmResponse,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdInvoice = await _invoiceRepository.AddAsync(invoice);

            // 10. Registrar no log
            var log = new AvenSuitesApi.Domain.Entities.ErpIntegrationLog
            {
                Id = Guid.NewGuid(),
                BookingId = null,
                InvoiceId = createdInvoice.Id,
                Endpoint = "ipm/nfse/generate-simple",
                Success = createdInvoice.Status == "ISSUED",
                RequestJson = xmlContent,
                ResponseJson = ipmResponse,
                CreatedAt = DateTime.UtcNow
            };
            await _erpLogRepository.AddAsync(log);

            return new IpmNfseCreateResponse
            {
                Success = createdInvoice.Status == "ISSUED",
                NfseNumber = createdInvoice.NfseNumber,
                SerieNfse = createdInvoice.NfseSeries,
                VerificationCode = createdInvoice.VerificationCode,
                XmlContent = ExtractXmlFromResponse(ipmResponse),
                PdfContent = ExtractPdfFromResponse(ipmResponse),
                RawResponse = ipmResponse
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar NF-e simplificada para quarto {RoomId}", roomId);
            return new IpmNfseCreateResponse
            {
                Success = false,
                ErrorMessage = $"Erro ao gerar NF-e: {ex.Message}"
            };
        }
    }

    private string GenerateInvoiceXml(AvenSuitesApi.Domain.Entities.Hotel hotel, IpmCredentials credentials, GuestPii guestPii, IpmNfseCreateRequest request)
    {
        var xml = new XElement("nfse", new XAttribute("id", "nota"),
            new XElement("identificador", request.Identifier ?? Guid.NewGuid().ToString()),
            new XElement("nf",
                new XElement("serie_nfse", request.SerieNfse),
                new XElement("data_fato_gerador", DateTime.Now.ToString("dd/MM/yyyy")),
                new XElement("valor_total", request.TotalValue.ToString("F2")),
                new XElement("valor_desconto", request.DiscountValue.ToString("F2")),
                new XElement("valor_ir", request.IrValue.ToString("F2")),
                new XElement("valor_inss", request.InssValue.ToString("F2")),
                new XElement("valor_contribuicao_social", request.SocialContributionValue.ToString("F2")),
                new XElement("valor_rps", request.RpsValue.ToString("F2")),
                new XElement("valor_pis", request.PisValue.ToString("F2")),
                new XElement("valor_cofins", request.CofinsValue.ToString("F2")),
                new XElement("observacao", request.Observations ?? "")
            ),
            new XElement("prestador",
                new XElement("cpfcnpj", CleanCpfCnpj(credentials.CpfCnpj ?? "")),
                new XElement("cidade", credentials.CityCode)
            ),
            new XElement("tomador",
                new XElement("endereco_informado", "1"),
                new XElement("tipo", guestPii.DocumentType == "CPF" ? "F" : "J"),
                new XElement("cpfcnpj", CleanCpfCnpj(guestPii.DocumentPlain ?? "")),
                new XElement("nome_razao_social", guestPii.FullName),
                new XElement("sobrenome_nome_fantasia", guestPii.FullName),
                new XElement("logradouro", guestPii.AddressLine1 ?? ""),
                new XElement("numero_residencia", ""),
                new XElement("bairro", guestPii.Neighborhood ?? ""),
                new XElement("cidade", credentials.CityCode),
                new XElement("cep", CleanCpfCnpj(guestPii.PostalCode ?? ""))
            ),
            new XElement("itens",
                request.Items.Select(item => new XElement("lista",
                    new XElement("tributa_municipio_prestador", item.TributaMunicipioPrestador),
                    new XElement("codigo_local_prestacao_servico", item.CodigoLocalPrestacao),
                    new XElement("unidade_codigo", item.UnidadeCodigo),
                    new XElement("unidade_quantidade", item.UnidadeQuantidade),
                    new XElement("unidade_valor_unitario", item.UnidadeValorUnitario.ToString("F2")),
                    new XElement("codigo_item_lista_servico", item.CodigoItemListaServico),
                    new XElement("descritivo", item.Descritivo),
                    new XElement("aliquota_item_lista_servico", item.AliquotaItemLista.ToString("F4")),
                    new XElement("situacao_tributaria", item.SituacaoTributaria),
                    new XElement("valor_tributavel", item.ValorTributavel.ToString("F2"))
                ))
            )
        );

        return xml.ToString();
    }

    private string GenerateCancelXml(IpmCredentials credentials, IpmNfseCancelRequest request)
    {
        var xml = new XElement("nfse", new XAttribute("id", "nota"),
            new XElement("nf",
                new XElement("numero", request.NfseNumber),
                new XElement("serie_nfse", request.SerieNfse),
                new XElement("situacao", "C"),
                new XElement("observacao", request.Observation ?? "Cancelamento")
            ),
            new XElement("prestador",
                new XElement("cpfcnpj", CleanCpfCnpj(credentials.CpfCnpj ?? "")),
                new XElement("cidade", credentials.CityCode)
            )
        );

        return xml.ToString();
    }
    
    private async Task<string> CallIpmWebServiceAsync(string xmlContent, string username, string password)
    {
        var endpoint = "https://nfse-saofranciscodosul.atende.net/?pg=rest&service=WNERestServiceNFSe"; // Configurar via appsettings
        
        try
        {
            var response = await _httpClient.PostAsync(endpoint, xmlContent, username, password);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao chamar webservice IPM");
            throw;
        }
    }
    
    private static decimal CalculateTaxes(IpmNfseCreateRequest request)
    {
        // Calcular impostos baseado nos itens
        return request.Items.Sum(i => 
            i.ValorTributavel * (i.AliquotaItemLista / 100));
    }
    
    /// <summary>
    /// Extrai informações do XML de resposta do IPM
    /// </summary>
    private static IpmResponseData ExtractResponseData(string xmlResponse)
    {
        try
        {
            var doc = XDocument.Parse(xmlResponse);
            var root = doc.Root;
            
            if (root == null)
                return new IpmResponseData();

            var retorno = root.Element("retorno");
            if (retorno == null)
                return new IpmResponseData();

            var mensagem = retorno.Element("mensagem");
            var codigo = mensagem?.Element("codigo")?.Value ?? "";
            
            var isSuccess = codigo.Contains("Sucesso") || codigo.Contains("00001");
            
            return new IpmResponseData
            {
                Success = isSuccess,
                Codigo = codigo,
                NfseNumber = retorno.Element("numero_nfse")?.Value,
                SerieNfse = retorno.Element("serie_nfse")?.Value,
                DataNfse = retorno.Element("data_nfse")?.Value,
                HoraNfse = retorno.Element("hora_nfse")?.Value,
                SituacaoCodigo = retorno.Element("situacao_codigo_nfse")?.Value,
                SituacaoDescricao = retorno.Element("situacao_descricao_nfse")?.Value,
                LinkNfse = retorno.Element("link_nfse")?.Value,
                VerificationCode = retorno.Element("cod_verificador_autenticidade")?.Value
            };
        }
        catch
        {
            // Se não conseguir parsear, retorna dados vazios
            return new IpmResponseData();
        }
    }
    
    private static string? ExtractNfseNumber(string response)
    {
        return ExtractResponseData(response).NfseNumber;
    }
    
    private static string? ExtractXmlFromResponse(string response)
    {
        // Retorna o XML completo da resposta
        return response;
    }
    
    private static string? ExtractPdfFromResponse(string response)
    {
        // PDF não está disponível na resposta XML
        return null;
    }
    
    /// <summary>
    /// Classe auxiliar para armazenar dados extraídos da resposta do IPM
    /// </summary>
    private class IpmResponseData
    {
        public bool Success { get; set; }
        public string? Codigo { get; set; }
        public string? NfseNumber { get; set; }
        public string? SerieNfse { get; set; }
        public string? DataNfse { get; set; }
        public string? HoraNfse { get; set; }
        public string? SituacaoCodigo { get; set; }
        public string? SituacaoDescricao { get; set; }
        public string? LinkNfse { get; set; }
        public string? VerificationCode { get; set; }
    }
    
    /// <summary>
    /// Converte data/hora do formato brasileiro para DateTime
    /// Formato esperado: "dd/MM/yyyy" e "HH:mm:ss"
    /// </summary>
    private static DateTime? ParseIssueDate(string? dataNfse, string? horaNfse)
    {
        if (string.IsNullOrWhiteSpace(dataNfse))
            return null;

        try
        {
            var dateOnly = DateTime.ParseExact(dataNfse, "dd/MM/yyyy", null);
            
            if (!string.IsNullOrWhiteSpace(horaNfse))
            {
                var timeOnly = TimeSpan.Parse(horaNfse);
                return dateOnly.Date.Add(timeOnly);
            }
            
            return dateOnly;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Remove pontos, traços e barras de CPF/CNPJ, deixando apenas números
    /// </summary>
    private static string CleanCpfCnpj(string cpfCnpj)
    {
        if (string.IsNullOrWhiteSpace(cpfCnpj))
            return string.Empty;

        // Remove pontos, traços, barras e espaços
        return cpfCnpj.Replace(".", "")
                      .Replace("-", "")
                      .Replace("/", "")
                      .Replace(" ", "");
    }
}


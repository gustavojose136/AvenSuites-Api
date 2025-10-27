using System.Text;
using System.Xml.Linq;
using AvenSuitesApi.Application.DTOs.Invoice;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Invoice;

public class IpmNfseService : IIpmNfseService
{
    private readonly IIpmCredentialsRepository _credentialsRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IErpIntegrationLogRepository _erpLogRepository;

    public IpmNfseService(
        IIpmCredentialsRepository credentialsRepository,
        IHotelRepository hotelRepository,
        IGuestRepository guestRepository,
        IBookingRepository bookingRepository,
        IInvoiceRepository invoiceRepository,
        IErpIntegrationLogRepository erpLogRepository)
    {
        _credentialsRepository = credentialsRepository;
        _hotelRepository = hotelRepository;
        _guestRepository = guestRepository;
        _bookingRepository = bookingRepository;
        _invoiceRepository = invoiceRepository;
        _erpLogRepository = erpLogRepository;
    }

    public async Task<IpmNfseCreateResponse> GenerateInvoiceAsync(Guid hotelId, IpmNfseCreateRequest request)
    {
        // Buscar credenciais do hotel
        var credentials = await _credentialsRepository.GetByHotelIdAsync(hotelId);
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

        // Buscar booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Reserva não encontrada" };

        // Buscar tomador (hóspede)
        var guest = await _guestRepository.GetByIdWithPiiAsync(request.TomadorGuestId);
        if (guest == null)
            return new IpmNfseCreateResponse { Success = false, ErrorMessage = "Hóspede não encontrado" };

        // Gerar XML da NF-e
        var xmlContent = GenerateInvoiceXml(hotel, credentials, guest.GuestPii!, request);

        try
        {
            // Chamar webservice IPM (aqui você implementaria a chamada SOAP/REST)
            // var response = await CallIpmWebService(xmlContent);

            // Por enquanto, retorna sucesso simulado
            var invoice = new AvenSuitesApi.Domain.Entities.Invoice
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                HotelId = hotelId,
                Status = "ISSUED",
                IssueDate = DateTime.UtcNow,
                NfseNumber = "12345", // Simulado
                NfseSeries = credentials.SerieNfse,
                TotalServices = request.TotalValue,
                TotalTaxes = 0,
                ErpProvider = "IPM",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _invoiceRepository.AddAsync(invoice);

            // Registrar no log
            var log = new AvenSuitesApi.Domain.Entities.ErpIntegrationLog
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                InvoiceId = invoice.Id,
                Endpoint = "ipm/nfse/generate",
                Success = true,
                RequestJson = xmlContent,
                ResponseJson = "{ \"status\": \"success\" }",
                CreatedAt = DateTime.UtcNow
            };
            await _erpLogRepository.AddAsync(log);

            return new IpmNfseCreateResponse
            {
                Success = true,
                NfseNumber = invoice.NfseNumber,
                SerieNfse = invoice.NfseSeries,
                RawResponse = xmlContent
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
        var credentials = await _credentialsRepository.GetByHotelIdAsync(hotelId);
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

    private string GenerateInvoiceXml(AvenSuitesApi.Domain.Entities.Hotel hotel, IpmCredentials credentials, GuestPii guestPii, IpmNfseCreateRequest request)
    {
        var xml = new XElement("nfse", new XAttribute("id", "nota"),
            new XElement("identificador", request.Identifier ?? Guid.NewGuid().ToString()),
            new XElement("nf",
                new XElement("serie_nfse", request.SerieNfse),
                new XElement("data_fato_gerador", request.FatoGeradorDate.ToString("dd/MM/yyyy")),
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
                new XElement("cpfcnpj", credentials.CpfCnpj ?? ""),
                new XElement("cidade", credentials.CityCode)
            ),
            new XElement("tomador",
                new XElement("endereco_informado", "1"),
                new XElement("tipo", guestPii.DocumentType == "CPF" ? "F" : "J"),
                new XElement("cpfcnpj", guestPii.DocumentPlain ?? ""),
                new XElement("nome_razao_social", guestPii.FullName),
                new XElement("sobrenome_nome_fantasia", guestPii.FullName),
                new XElement("logradouro", guestPii.AddressLine1 ?? ""),
                new XElement("numero_residencia", ""),
                new XElement("bairro", ""),
                new XElement("cidade", credentials.CityCode),
                new XElement("cep", guestPii.PostalCode ?? "")
            ),
            new XElement("itens",
                new XElement("lista",
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
                new XElement("cpfcnpj", credentials.CpfCnpj ?? ""),
                new XElement("cidade", credentials.CityCode)
            )
        );

        return xml.ToString();
    }
}


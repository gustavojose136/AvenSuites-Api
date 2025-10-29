using System.Text;
using Microsoft.Extensions.Logging;

namespace AvenSuitesApi.Application.Services.Implementations.Invoice;

public interface IIpmHttpClient
{
    Task<string> PostAsync(string endpoint, string xmlContent, string username, string password);
}

public class IpmHttpClient : IIpmHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IpmHttpClient> _logger;

    public IpmHttpClient(HttpClient httpClient, ILogger<IpmHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> PostAsync(string endpoint, string xmlContent, string username, string password)
    {
        try
        {
            // Limpar headers para evitar conflitos
            _httpClient.DefaultRequestHeaders.Clear();
            
            // Configurar autenticação Basic
            var authBytes = Encoding.ASCII.GetBytes($"{username}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            // Criar multipart/form-data com o XML como arquivo
            using var formData = new MultipartFormDataContent();
            
            // Criar o conteúdo do arquivo XML
            var xmlBytes = Encoding.UTF8.GetBytes(xmlContent);
            var xmlStreamContent = new StreamContent(new MemoryStream(xmlBytes));
            xmlStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
            
            // Adicionar como arquivo com a chave "note"
            formData.Add(xmlStreamContent, "note", "nota.xml");

            _logger.LogInformation("Enviando XML para IPM como multipart/form-data");
            
            var response = await _httpClient.PostAsync(endpoint, formData);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro na chamada IPM: {StatusCode} - {Error}", response.StatusCode, errorContent);
                throw new Exception($"Erro na chamada IPM: {response.StatusCode} - {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Resposta IPM recebida com sucesso");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao chamar webservice IPM");
            throw;
        }
    }
}


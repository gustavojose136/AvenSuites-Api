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
            // Configurar autenticação Basic
            var authBytes = Encoding.ASCII.GetBytes($"{username}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(authBytes)}");

            var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro na chamada IPM: {StatusCode} - {Error}", response.StatusCode, errorContent);
                throw new Exception($"Erro na chamada IPM: {response.StatusCode}");
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


namespace AvenSuitesApi.Application.Services.Interfaces;

/// <summary>
/// Interface para serviço de envio de e-mails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia um e-mail simples
    /// </summary>
    /// <param name="to">Destinatário</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do e-mail (texto ou HTML)</param>
    /// <param name="isHtml">Indica se o corpo é HTML</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o e-mail foi enviado com sucesso</returns>
    Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia um e-mail com múltiplos destinatários
    /// </summary>
    /// <param name="to">Lista de destinatários</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do e-mail (texto ou HTML)</param>
    /// <param name="isHtml">Indica se o corpo é HTML</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o e-mail foi enviado com sucesso</returns>
    Task<bool> SendEmailAsync(
        IEnumerable<string> to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia um e-mail com cópias (CC e BCC)
    /// </summary>
    /// <param name="to">Destinatário principal</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do e-mail (texto ou HTML)</param>
    /// <param name="cc">Lista de cópias (CC)</param>
    /// <param name="bcc">Lista de cópias ocultas (BCC)</param>
    /// <param name="isHtml">Indica se o corpo é HTML</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o e-mail foi enviado com sucesso</returns>
    Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null,
        bool isHtml = true,
        CancellationToken cancellationToken = default);
}





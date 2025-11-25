using AvenSuitesApi.Application.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AvenSuitesApi.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de envio de e-mails usando SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        return await SendEmailAsync(
            new[] { to },
            subject,
            body,
            isHtml,
            cancellationToken);
    }

    public async Task<bool> SendEmailAsync(
        IEnumerable<string> to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateSettings();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            
            // Adicionar todos os destinatários
            foreach (var toAddress in to.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                message.To.Add(MailboxAddress.Parse(toAddress));
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            await client.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_emailSettings.SmtpUsername))
            {
                await client.AuthenticateAsync(
                    _emailSettings.SmtpUsername,
                    _emailSettings.SmtpPassword,
                    cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            var toList = string.Join(", ", to);
            _logger.LogInformation(
                "E-mail enviado com sucesso para {To} com assunto: {Subject}",
                toList,
                subject);

            return true;
        }
        catch (Exception ex)
        {
            var toList = string.Join(", ", to);
            _logger.LogError(
                ex,
                "Erro ao enviar e-mail para {To} com assunto: {Subject}. Erro: {ErrorMessage}",
                toList,
                subject,
                ex.Message);

            return false;
        }
    }

    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateSettings();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            
            // Destinatário principal
            if (!string.IsNullOrWhiteSpace(to))
            {
                message.To.Add(MailboxAddress.Parse(to));
            }

            // Cópias (CC)
            if (cc != null)
            {
                foreach (var ccAddress in cc.Where(c => !string.IsNullOrWhiteSpace(c)))
                {
                    message.Cc.Add(MailboxAddress.Parse(ccAddress));
                }
            }

            // Cópias ocultas (BCC)
            if (bcc != null)
            {
                foreach (var bccAddress in bcc.Where(b => !string.IsNullOrWhiteSpace(b)))
                {
                    message.Bcc.Add(MailboxAddress.Parse(bccAddress));
                }
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            await client.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_emailSettings.SmtpUsername))
            {
                await client.AuthenticateAsync(
                    _emailSettings.SmtpUsername,
                    _emailSettings.SmtpPassword,
                    cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation(
                "E-mail enviado com sucesso para {To} com assunto: {Subject}",
                to,
                subject);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao enviar e-mail para {To} com assunto: {Subject}. Erro: {ErrorMessage}",
                to,
                subject,
                ex.Message);

            return false;
        }
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_emailSettings.SmtpServer))
        {
            throw new InvalidOperationException("SmtpServer não configurado no appsettings.json");
        }

        if (string.IsNullOrWhiteSpace(_emailSettings.FromEmail))
        {
            throw new InvalidOperationException("FromEmail não configurado no appsettings.json");
        }

        if (_emailSettings.SmtpPort <= 0)
        {
            throw new InvalidOperationException("SmtpPort deve ser maior que zero");
        }
    }
}


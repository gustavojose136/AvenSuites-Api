namespace AvenSuitesApi.Application.Services;

/// <summary>
/// Configurações de SMTP para envio de e-mails
/// </summary>
public class EmailSettings
{
    public const string SectionName = "Email";

    /// <summary>
    /// Servidor SMTP
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// Porta do servidor SMTP
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Usuário para autenticação SMTP
    /// </summary>
    public string SmtpUsername { get; set; } = string.Empty;

    /// <summary>
    /// Senha para autenticação SMTP
    /// </summary>
    public string SmtpPassword { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do remetente
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Nome do remetente
    /// </summary>
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// Habilitar SSL/TLS
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Usar credenciais padrão
    /// </summary>
    public bool UseDefaultCredentials { get; set; } = false;
}



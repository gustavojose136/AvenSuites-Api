using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations;

/// <summary>
/// Serviço para gerar templates HTML de e-mail
/// </summary>
public class EmailTemplateService : IEmailTemplateService
{
    public string GenerateWelcomeEmail(string guestName, string hotelName)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Bem-vindo ao {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; }}
        .message {{ font-size: 16px; color: #4a5568; margin-bottom: 25px; line-height: 1.8; }}
        .highlight-box {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 25px; border-radius: 12px; margin: 30px 0; text-align: center; color: white; }}
        .highlight-box h2 {{ font-size: 22px; margin-bottom: 10px; }}
        .highlight-box p {{ font-size: 16px; opacity: 0.95; }}
        .features {{ margin: 30px 0; }}
        .feature-item {{ display: flex; align-items: center; margin-bottom: 20px; padding: 15px; background-color: #f7fafc; border-radius: 8px; }}
        .feature-icon {{ font-size: 24px; margin-right: 15px; }}
        .feature-text {{ flex: 1; color: #2d3748; font-size: 15px; }}
        .cta-button {{ display: inline-block; padding: 15px 40px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; border-radius: 8px; font-weight: 600; font-size: 16px; margin: 20px 0; transition: transform 0.2s; }}
        .cta-button:hover {{ transform: translateY(-2px); }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
        .footer a {{ color: #667eea; text-decoration: none; }}
        .divider {{ height: 2px; background: linear-gradient(90deg, transparent, #e2e8f0, transparent); margin: 30px 0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🏨 {hotelName}</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">Sua experiência começa aqui</p>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div class=""message"">
                É com grande prazer que damos as boas-vindas ao <strong>{hotelName}</strong>! 
                Sua conta foi criada com sucesso e agora você tem acesso a todos os nossos serviços.
            </div>
            
            <div class=""highlight-box"">
                <h2>✨ Sua jornada começa agora</h2>
                <p>Explore todas as comodidades que preparamos para você</p>
            </div>
            
            <div class=""features"">
                <div class=""feature-item"">
                    <div class=""feature-icon"">📱</div>
                    <div class=""feature-text"">
                        <strong>Portal do Hóspede:</strong> Acesse suas reservas, perfil e muito mais
                    </div>
                </div>
                <div class=""feature-item"">
                    <div class=""feature-icon"">🎫</div>
                    <div class=""feature-text"">
                        <strong>Reservas Rápidas:</strong> Faça suas reservas de forma simples e segura
                    </div>
                </div>
                <div class=""feature-item"">
                    <div class=""feature-icon"">🔔</div>
                    <div class=""feature-text"">
                        <strong>Notificações:</strong> Receba lembretes e atualizações sobre suas reservas
                    </div>
                </div>
                <div class=""feature-item"">
                    <div class=""feature-icon"">💳</div>
                    <div class=""feature-text"">
                        <strong>Pagamento Seguro:</strong> Processe seus pagamentos com total segurança
                    </div>
                </div>
            </div>
            
            <div style=""text-align: center; margin: 30px 0;"">
                <a href=""#"" class=""cta-button"">Acessar Portal do Hóspede</a>
            </div>
            
            <div class=""divider""></div>
            
            <div class=""message"">
                Se você tiver alguma dúvida ou precisar de ajuda, nossa equipe está sempre pronta para atendê-lo.
                <br><br>
                Estamos ansiosos para proporcionar uma experiência inesquecível!
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Este é um e-mail automático, por favor não responda.</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateBookingConfirmationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        decimal totalAmount,
        string currency,
        List<BookingRoomInfo> rooms,
        string? hotelAddress = null,
        string? hotelPhone = null)
    {
        var roomsHtml = string.Join("", rooms.Select((room, index) => $@"
            <div style=""background-color: #f7fafc; padding: 20px; border-radius: 10px; margin-bottom: 15px; border-left: 4px solid #667eea;"">
                <div style=""display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap;"">
                    <div>
                        <div style=""font-size: 18px; font-weight: 600; color: #2d3748; margin-bottom: 5px;"">
                            🏨 Quarto {room.RoomNumber}
                        </div>
                        <div style=""color: #718096; font-size: 14px;"">{room.RoomTypeName}</div>
                    </div>
                    <div style=""text-align: right;"">
                        <div style=""font-size: 20px; font-weight: 700; color: #667eea;"">
                            {FormatCurrency(totalAmount / rooms.Count, currency)}
                        </div>
                    </div>
                </div>
            </div>"));

        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Confirmação de Reserva - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .ticket-badge {{ display: inline-block; background-color: rgba(255,255,255,0.2); padding: 8px 20px; border-radius: 20px; font-size: 14px; margin-top: 10px; }}
        .content {{ padding: 40px 30px; }}
        .success-icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .booking-code {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 20px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .booking-code-label {{ font-size: 14px; opacity: 0.9; margin-bottom: 8px; }}
        .booking-code-value {{ font-size: 32px; font-weight: 700; letter-spacing: 2px; }}
        .info-section {{ margin: 25px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 15px 0; border-bottom: 1px solid #e2e8f0; }}
        .info-label {{ color: #718096; font-size: 14px; }}
        .info-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .rooms-section {{ margin: 30px 0; }}
        .rooms-title {{ font-size: 20px; color: #2d3748; margin-bottom: 20px; font-weight: 600; }}
        .total-section {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 25px; border-radius: 12px; margin: 30px 0; text-align: center; color: white; }}
        .total-label {{ font-size: 16px; opacity: 0.9; margin-bottom: 8px; }}
        .total-value {{ font-size: 36px; font-weight: 700; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
        .footer-info {{ margin-top: 15px; line-height: 1.8; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🎫 Confirmação de Reserva</h1>
            <div class=""ticket-badge"">{hotelName}</div>
        </div>
        
        <div class=""content"">
            <div class=""success-icon"">✅</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div style=""text-align: center; color: #4a5568; margin-bottom: 30px; font-size: 16px;"">
                Sua reserva foi confirmada com sucesso! Guarde este e-mail como comprovante.
            </div>
            
            <div class=""booking-code"">
                <div class=""booking-code-label"">Código da Reserva</div>
                <div class=""booking-code-value"">{bookingCode}</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy} às 14:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy} às 12:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">🌙 Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div class=""rooms-section"">
                <div class=""rooms-title"">🏨 Quartos Reservados</div>
                {roomsHtml}
            </div>
            
            <div class=""total-section"">
                <div class=""total-label"">Valor Total</div>
                <div class=""total-value"">{FormatCurrency(totalAmount, currency)}</div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #f7fafc; padding: 20px; border-radius: 10px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2d3748; margin-bottom: 10px;"">📍 Localização</div>
                <div style=""color: #4a5568; line-height: 1.8;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>📞 {hotelPhone}")}
                </div>
            </div>")}
            
            <div style=""background-color: #fff5f5; border-left: 4px solid #f56565; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #c53030; margin-bottom: 10px;"">⚠️ Importante</div>
                <div style=""color: #742a2a; font-size: 14px; line-height: 1.8;"">
                    • Apresente este comprovante no check-in<br>
                    • Chegada: a partir das 14:00<br>
                    • Saída: até às 12:00<br>
                    • Em caso de dúvidas, entre em contato conosco
                </div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <div class=""footer-info"">
                {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p>{hotelAddress}</p>")}
                {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>📞 {hotelPhone}</p>")}
            </div>
            <p style=""margin-top: 15px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateBookingReminderEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        List<BookingRoomInfo> rooms,
        string? hotelAddress = null,
        string? hotelPhone = null)
    {
        var roomsHtml = string.Join("", rooms.Select(room => $@"
            <div style=""background-color: #f7fafc; padding: 15px; border-radius: 8px; margin-bottom: 10px; border-left: 4px solid #48bb78;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2d3748;"">
                    🏨 Quarto {room.RoomNumber} - {room.RoomTypeName}
                </div>
            </div>"));

        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Lembrete de Reserva - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .reminder-icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .countdown-box {{ background: linear-gradient(135deg, #f6ad55 0%, #ed8936 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .countdown-label {{ font-size: 14px; opacity: 0.9; margin-bottom: 8px; }}
        .countdown-value {{ font-size: 36px; font-weight: 700; }}
        .info-section {{ margin: 25px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 15px 0; border-bottom: 1px solid #e2e8f0; }}
        .info-label {{ color: #718096; font-size: 14px; }}
        .info-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .rooms-section {{ margin: 30px 0; }}
        .rooms-title {{ font-size: 20px; color: #2d3748; margin-bottom: 20px; font-weight: 600; }}
        .booking-code {{ background-color: #edf2f7; padding: 15px; border-radius: 8px; text-align: center; margin: 20px 0; }}
        .booking-code-label {{ font-size: 12px; color: #718096; margin-bottom: 5px; }}
        .booking-code-value {{ font-size: 24px; font-weight: 700; color: #2d3748; letter-spacing: 1px; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🔔 Lembrete de Reserva</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">Sua estadia está chegando!</p>
        </div>
        
        <div class=""content"">
            <div class=""reminder-icon"">⏰</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div style=""text-align: center; color: #4a5568; margin-bottom: 30px; font-size: 16px;"">
                Faltam apenas <strong>3 dias</strong> para sua reserva no <strong>{hotelName}</strong>!
            </div>
            
            <div class=""countdown-box"">
                <div class=""countdown-label"">Sua estadia começa em</div>
                <div class=""countdown-value"">{checkInDate:dd/MM/yyyy}</div>
            </div>
            
            <div class=""booking-code"">
                <div class=""booking-code-label"">Código da Reserva</div>
                <div class=""booking-code-value"">{bookingCode}</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy} às 14:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy} às 12:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">🌙 Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div class=""rooms-section"">
                <div class=""rooms-title"">🏨 Quartos Reservados</div>
                {roomsHtml}
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #22543d; margin-bottom: 10px;"">📍 Como Chegar</div>
                <div style=""color: #2f855a; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>📞 {hotelPhone}")}
                </div>
            </div>")}
            
            <div style=""background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px;"">💡 Dicas para sua estadia</div>
                <div style=""color: #2b6cb0; font-size: 14px; line-height: 1.8;"">
                    • Chegada: a partir das 14:00<br>
                    • Saída: até às 12:00<br>
                    • Traga um documento com foto para o check-in<br>
                    • Em caso de dúvidas, entre em contato conosco
                </div>
            </div>
            
            <div style=""text-align: center; margin: 30px 0; padding: 20px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 12px; color: white;"">
                <div style=""font-size: 18px; font-weight: 600; margin-bottom: 10px;"">Estamos ansiosos para recebê-lo!</div>
                <div style=""font-size: 14px; opacity: 0.9;"">Prepare-se para uma experiência inesquecível</div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p style=""margin-top: 10px;"">{hotelAddress}</p>")}
            {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>📞 {hotelPhone}</p>")}
            <p style=""margin-top: 15px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateHotelBookingNotificationEmail(
        string hotelName,
        string bookingCode,
        string guestName,
        string guestEmail,
        string? guestPhone,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        int adults,
        int children,
        decimal totalAmount,
        string currency,
        List<BookingRoomInfo> rooms,
        string? notes = null,
        string? channelRef = null)
    {
        var roomsHtml = string.Join("", rooms.Select((room, index) => $@"
            <div style=""background-color: #f7fafc; padding: 20px; border-radius: 10px; margin-bottom: 15px; border-left: 4px solid #4299e1;"">
                <div style=""display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap;"">
                    <div>
                        <div style=""font-size: 18px; font-weight: 600; color: #2d3748; margin-bottom: 5px;"">
                            🏨 Quarto {room.RoomNumber}
                        </div>
                        <div style=""color: #718096; font-size: 14px;"">{room.RoomTypeName}</div>
                    </div>
                    <div style=""text-align: right;"">
                        <div style=""font-size: 20px; font-weight: 700; color: #4299e1;"">
                            {FormatCurrency(room.PriceTotal, currency)}
                        </div>
                    </div>
                </div>
            </div>"));

        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Nova Reserva - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 700px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #4299e1 0%, #3182ce 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .alert-box {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .alert-box h2 {{ font-size: 24px; margin-bottom: 8px; }}
        .alert-box p {{ font-size: 16px; opacity: 0.95; }}
        .booking-code {{ background-color: #edf2f7; padding: 20px; border-radius: 10px; text-align: center; margin: 25px 0; border: 2px dashed #cbd5e0; }}
        .booking-code-label {{ font-size: 12px; color: #718096; margin-bottom: 8px; text-transform: uppercase; letter-spacing: 1px; }}
        .booking-code-value {{ font-size: 32px; font-weight: 700; color: #2d3748; letter-spacing: 3px; }}
        .info-section {{ margin: 25px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 15px 0; border-bottom: 1px solid #e2e8f0; }}
        .info-label {{ color: #718096; font-size: 14px; font-weight: 500; }}
        .info-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .guest-section {{ background-color: #f0f9ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0; }}
        .guest-title {{ font-size: 18px; font-weight: 600; color: #2c5282; margin-bottom: 15px; }}
        .guest-info {{ color: #2b6cb0; line-height: 1.8; }}
        .rooms-section {{ margin: 30px 0; }}
        .rooms-title {{ font-size: 20px; color: #2d3748; margin-bottom: 20px; font-weight: 600; }}
        .total-section {{ background: linear-gradient(135deg, #4299e1 0%, #3182ce 100%); padding: 25px; border-radius: 12px; margin: 30px 0; text-align: center; color: white; }}
        .total-label {{ font-size: 16px; opacity: 0.9; margin-bottom: 8px; }}
        .total-value {{ font-size: 36px; font-weight: 700; }}
        .notes-section {{ background-color: #fffaf0; border-left: 4px solid #ed8936; padding: 20px; border-radius: 8px; margin: 25px 0; }}
        .notes-title {{ font-size: 16px; font-weight: 600; color: #c05621; margin-bottom: 10px; }}
        .notes-content {{ color: #7c2d12; font-size: 14px; line-height: 1.8; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>📋 Nova Reserva Recebida</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""alert-box"">
                <h2>✨ Nova Reserva Confirmada!</h2>
                <p>Uma nova reserva foi realizada no sistema</p>
            </div>
            
            <div class=""booking-code"">
                <div class=""booking-code-label"">Código da Reserva</div>
                <div class=""booking-code-value"">{bookingCode}</div>
            </div>
            
            <div class=""guest-section"">
                <div class=""guest-title"">👤 Dados do Hóspede</div>
                <div class=""guest-info"">
                    <strong>Nome:</strong> {guestName}<br>
                    <strong>E-mail:</strong> {guestEmail}<br>
                    {(string.IsNullOrWhiteSpace(guestPhone) ? "" : $@"<strong>Telefone:</strong> {guestPhone}<br>")}
                </div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy} às 14:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy} às 12:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">🌙 Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">👥 Hóspedes</div>
                    <div class=""info-value"">{adults} {(adults == 1 ? "adulto" : "adultos")}{(children > 0 ? $", {children} {(children == 1 ? "criança" : "crianças")}" : "")}</div>
                </div>
                {(string.IsNullOrWhiteSpace(channelRef) ? "" : $@"
                <div class=""info-row"">
                    <div class=""info-label"">🔗 Canal</div>
                    <div class=""info-value"">{channelRef}</div>
                </div>")}
            </div>
            
            <div class=""rooms-section"">
                <div class=""rooms-title"">🏨 Quartos Reservados</div>
                {roomsHtml}
            </div>
            
            <div class=""total-section"">
                <div class=""total-label"">Valor Total da Reserva</div>
                <div class=""total-value"">{FormatCurrency(totalAmount, currency)}</div>
            </div>
            
            {(string.IsNullOrWhiteSpace(notes) ? "" : $@"
            <div class=""notes-section"">
                <div class=""notes-title"">📝 Observações</div>
                <div class=""notes-content"">{notes}</div>
            </div>")}
            
            <div style=""background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px;"">💡 Próximos Passos</div>
                <div style=""color: #2b6cb0; font-size: 14px; line-height: 1.8;"">
                    • Prepare os quartos para a chegada do hóspede<br>
                    • Verifique a disponibilidade dos serviços solicitados<br>
                    • Entre em contato com o hóspede se necessário<br>
                    • Confirme os detalhes da reserva no sistema
                </div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Sistema de Gestão Hoteleira</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateHotelInvoiceNotificationEmail(
        string hotelName,
        string? nfseNumber,
        string? nfseSeries,
        string? verificationCode,
        string? bookingCode,
        string guestName,
        decimal totalServices,
        decimal totalTaxes,
        decimal totalAmount,
        DateTime issueDate,
        List<InvoiceItemInfo> items,
        string? erpProvider = null,
        string? erpProtocol = null)
    {
        var itemsHtml = string.Join("", items.Select((item, index) => $@"
            <tr style=""border-bottom: 1px solid #e2e8f0;"">
                <td style=""padding: 15px; color: #4a5568;"">{item.Description}</td>
                <td style=""padding: 15px; text-align: center; color: #4a5568;"">{item.Quantity:N0}</td>
                <td style=""padding: 15px; text-align: right; color: #4a5568;"">{FormatCurrency(item.UnitPrice, "BRL")}</td>
                <td style=""padding: 15px; text-align: right; color: #2d3748; font-weight: 600;"">{FormatCurrency(item.Total, "BRL")}</td>
            </tr>"));

        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Nota Fiscal Gerada - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 700px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .success-box {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .success-box h2 {{ font-size: 24px; margin-bottom: 8px; }}
        .success-box p {{ font-size: 16px; opacity: 0.95; }}
        .invoice-info {{ background-color: #f7fafc; padding: 25px; border-radius: 10px; margin: 25px 0; border: 2px solid #e2e8f0; }}
        .invoice-row {{ display: flex; justify-content: space-between; margin-bottom: 15px; padding-bottom: 15px; border-bottom: 1px solid #e2e8f0; }}
        .invoice-row:last-child {{ border-bottom: none; margin-bottom: 0; padding-bottom: 0; }}
        .invoice-label {{ color: #718096; font-size: 14px; font-weight: 500; }}
        .invoice-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .nfse-number {{ font-size: 24px; font-weight: 700; color: #48bb78; letter-spacing: 2px; }}
        .guest-section {{ background-color: #f0f9ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0; }}
        .guest-title {{ font-size: 18px; font-weight: 600; color: #2c5282; margin-bottom: 10px; }}
        .guest-info {{ color: #2b6cb0; line-height: 1.8; }}
        .items-table {{ width: 100%; border-collapse: collapse; margin: 25px 0; background-color: white; }}
        .items-table th {{ background-color: #edf2f7; padding: 15px; text-align: left; color: #2d3748; font-weight: 600; font-size: 14px; border-bottom: 2px solid #cbd5e0; }}
        .items-table td {{ padding: 15px; }}
        .total-section {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 25px; border-radius: 12px; margin: 30px 0; }}
        .total-row {{ display: flex; justify-content: space-between; margin-bottom: 10px; color: white; }}
        .total-row:last-child {{ margin-bottom: 0; border-top: 2px solid rgba(255,255,255,0.3); padding-top: 15px; margin-top: 15px; }}
        .total-label {{ font-size: 16px; opacity: 0.9; }}
        .total-value {{ font-size: 20px; font-weight: 600; }}
        .total-final {{ font-size: 28px; font-weight: 700; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🧾 Nota Fiscal Gerada</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""success-box"">
                <h2>✅ Nota Fiscal Emitida com Sucesso!</h2>
                <p>A nota fiscal foi gerada e está disponível no sistema</p>
            </div>
            
            <div class=""invoice-info"">
                {(string.IsNullOrWhiteSpace(nfseNumber) ? "" : $@"
                <div class=""invoice-row"">
                    <div class=""invoice-label"">Número da NF-e</div>
                    <div class=""invoice-value nfse-number"">{nfseNumber}</div>
                </div>")}
                {(string.IsNullOrWhiteSpace(nfseSeries) ? "" : $@"
                <div class=""invoice-row"">
                    <div class=""invoice-label"">Série</div>
                    <div class=""invoice-value"">{nfseSeries}</div>
                </div>")}
                {(string.IsNullOrWhiteSpace(verificationCode) ? "" : $@"
                <div class=""invoice-row"">
                    <div class=""invoice-label"">Código de Verificação</div>
                    <div class=""invoice-value"">{verificationCode}</div>
                </div>")}
                {(string.IsNullOrWhiteSpace(bookingCode) ? "" : $@"
                <div class=""invoice-row"">
                    <div class=""invoice-label"">Código da Reserva</div>
                    <div class=""invoice-value"">{bookingCode}</div>
                </div>")}
                <div class=""invoice-row"">
                    <div class=""invoice-label"">Data de Emissão</div>
                    <div class=""invoice-value"">{issueDate:dd/MM/yyyy HH:mm}</div>
                </div>
                {(string.IsNullOrWhiteSpace(erpProvider) ? "" : $@"
                <div class=""invoice-row"">
                    <div class=""invoice-label"">Provedor ERP</div>
                    <div class=""invoice-value"">{erpProvider}</div>
                </div>")}
                {(string.IsNullOrWhiteSpace(erpProtocol) ? "" : $@"
                <div class=""invoice-row"">
                    <div class=""invoice-label"">Protocolo</div>
                    <div class=""invoice-value"">{erpProtocol}</div>
                </div>")}
            </div>
            
            <div class=""guest-section"">
                <div class=""guest-title"">👤 Dados do Tomador</div>
                <div class=""guest-info"">
                    <strong>Nome:</strong> {guestName}
                </div>
            </div>
            
            <div style=""margin: 25px 0;"">
                <div style=""font-size: 20px; color: #2d3748; margin-bottom: 20px; font-weight: 600;"">📋 Itens da Nota Fiscal</div>
                <table class=""items-table"">
                    <thead>
                        <tr>
                            <th>Descrição</th>
                            <th style=""text-align: center;"">Qtd</th>
                            <th style=""text-align: right;"">Valor Unit.</th>
                            <th style=""text-align: right;"">Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {itemsHtml}
                    </tbody>
                </table>
            </div>
            
            <div class=""total-section"">
                <div class=""total-row"">
                    <div class=""total-label"">Subtotal de Serviços</div>
                    <div class=""total-value"">{FormatCurrency(totalServices, "BRL")}</div>
                </div>
                <div class=""total-row"">
                    <div class=""total-label"">Impostos</div>
                    <div class=""total-value"">{FormatCurrency(totalTaxes, "BRL")}</div>
                </div>
                <div class=""total-row"">
                    <div class=""total-label total-final"">Valor Total</div>
                    <div class=""total-value total-final"">{FormatCurrency(totalAmount, "BRL")}</div>
                </div>
            </div>
            
            <div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #22543d; margin-bottom: 10px;"">💡 Informações Importantes</div>
                <div style=""color: #2f855a; font-size: 14px; line-height: 1.8;"">
                    • A nota fiscal foi emitida com sucesso<br>
                    • O XML e PDF estão disponíveis no sistema<br>
                    • O código de verificação pode ser usado para consulta<br>
                    • Mantenha os arquivos XML e PDF para fins de auditoria
                </div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Sistema de Gestão Hoteleira</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateBookingCancellationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal totalAmount,
        string currency,
        string? reason = null)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Cancelamento de Reserva - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #f56565 0%, #c53030 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .info-section {{ margin: 25px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 15px 0; border-bottom: 1px solid #e2e8f0; }}
        .info-label {{ color: #718096; font-size: 14px; }}
        .info-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .booking-code {{ background-color: #fed7d7; padding: 15px; border-radius: 8px; text-align: center; margin: 20px 0; }}
        .booking-code-label {{ font-size: 12px; color: #742a2a; margin-bottom: 5px; }}
        .booking-code-value {{ font-size: 24px; font-weight: 700; color: #c53030; letter-spacing: 1px; }}
        .reason-box {{ background-color: #fff5f5; border-left: 4px solid #f56565; padding: 20px; border-radius: 8px; margin: 25px 0; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>❌ Reserva Cancelada</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""icon"">🚫</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div style=""text-align: center; color: #4a5568; margin-bottom: 30px; font-size: 16px;"">
                Informamos que sua reserva foi cancelada conforme solicitado.
            </div>
            
            <div class=""booking-code"">
                <div class=""booking-code-label"">Código da Reserva Cancelada</div>
                <div class=""booking-code-value"">{bookingCode}</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy}</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy}</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">💰 Valor</div>
                    <div class=""info-value"">{FormatCurrency(totalAmount, currency)}</div>
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(reason) ? "" : $@"
            <div class=""reason-box"">
                <div style=""font-size: 16px; font-weight: 600; color: #c53030; margin-bottom: 10px;"">📝 Motivo do Cancelamento</div>
                <div style=""color: #742a2a; font-size: 14px; line-height: 1.8;"">{reason}</div>
            </div>")}
            
            <div style=""background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px;"">💡 Informações Importantes</div>
                <div style=""color: #2b6cb0; font-size: 14px; line-height: 1.8;"">
                    • O cancelamento foi processado com sucesso<br>
                    • Qualquer reembolso será processado conforme nossa política<br>
                    • Esperamos recebê-lo em uma futura oportunidade<br>
                    • Em caso de dúvidas, entre em contato conosco
                </div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Agradecemos sua compreensão</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateHotelBookingCancellationEmail(
        string hotelName,
        string bookingCode,
        string guestName,
        string guestEmail,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal totalAmount,
        string currency,
        string? reason = null)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reserva Cancelada - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 700px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #f56565 0%, #c53030 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .alert-box {{ background: linear-gradient(135deg, #f56565 0%, #c53030 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .alert-box h2 {{ font-size: 24px; margin-bottom: 8px; }}
        .info-section {{ margin: 25px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 15px 0; border-bottom: 1px solid #e2e8f0; }}
        .info-label {{ color: #718096; font-size: 14px; }}
        .info-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .guest-section {{ background-color: #fed7d7; border-left: 4px solid #f56565; padding: 20px; border-radius: 8px; margin: 25px 0; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>⚠️ Reserva Cancelada</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""alert-box"">
                <h2>❌ Reserva Cancelada</h2>
                <p>Uma reserva foi cancelada no sistema</p>
            </div>
            
            <div style=""background-color: #edf2f7; padding: 20px; border-radius: 10px; margin: 25px 0; text-align: center; border: 2px dashed #cbd5e0;"">
                <div style=""font-size: 12px; color: #718096; margin-bottom: 8px; text-transform: uppercase; letter-spacing: 1px;"">Código da Reserva</div>
                <div style=""font-size: 32px; font-weight: 700; color: #2d3748; letter-spacing: 3px;"">{bookingCode}</div>
            </div>
            
            <div class=""guest-section"">
                <div style=""font-size: 18px; font-weight: 600; color: #c53030; margin-bottom: 15px;"">👤 Dados do Hóspede</div>
                <div style=""color: #742a2a; line-height: 1.8;"">
                    <strong>Nome:</strong> {guestName}<br>
                    <strong>E-mail:</strong> {guestEmail}
                </div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy}</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy}</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">💰 Valor da Reserva</div>
                    <div class=""info-value"">{FormatCurrency(totalAmount, currency)}</div>
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(reason) ? "" : $@"
            <div style=""background-color: #fff5f5; border-left: 4px solid #f56565; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #c53030; margin-bottom: 10px;"">📝 Motivo do Cancelamento</div>
                <div style=""color: #742a2a; font-size: 14px; line-height: 1.8;"">{reason}</div>
            </div>")}
            
            <div style=""background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px;"">💡 Ações Recomendadas</div>
                <div style=""color: #2b6cb0; font-size: 14px; line-height: 1.8;"">
                    • Verificar política de cancelamento aplicável<br>
                    • Processar reembolso se necessário<br>
                    • Atualizar disponibilidade dos quartos<br>
                    • Registrar o motivo do cancelamento
                </div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Sistema de Gestão Hoteleira</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateCheckInConfirmationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        List<BookingRoomInfo> rooms,
        DateTime checkOutDate,
        string? hotelAddress = null,
        string? hotelPhone = null)
    {
        var roomsHtml = string.Join("", rooms.Select(room => $@"
            <div style=""background-color: #f0fff4; padding: 15px; border-radius: 8px; margin-bottom: 10px; border-left: 4px solid #48bb78;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2d3748;"">
                    🏨 Quarto {room.RoomNumber} - {room.RoomTypeName}
                </div>
            </div>"));

        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Check-in Realizado - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .success-icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .success-box {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .success-box h2 {{ font-size: 24px; margin-bottom: 8px; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>✅ Check-in Realizado</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""success-icon"">🎉</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div class=""success-box"">
                <h2>Bem-vindo ao {hotelName}!</h2>
                <p>Seu check-in foi realizado com sucesso</p>
            </div>
            
            <div style=""background-color: #edf2f7; padding: 15px; border-radius: 8px; text-align: center; margin: 20px 0;"">
                <div style=""font-size: 12px; color: #718096; margin-bottom: 5px;"">Código da Reserva</div>
                <div style=""font-size: 24px; font-weight: 700; color: #2d3748; letter-spacing: 1px;"">{bookingCode}</div>
            </div>
            
            <div style=""margin: 25px 0;"">
                <div style=""font-size: 20px; color: #2d3748; margin-bottom: 20px; font-weight: 600;"">🏨 Seus Quartos</div>
                {roomsHtml}
            </div>
            
            <div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #22543d; margin-bottom: 10px;"">📅 Informações Importantes</div>
                <div style=""color: #2f855a; font-size: 14px; line-height: 1.8;"">
                    • Check-out: {checkOutDate:dd/MM/yyyy} às 12:00<br>
                    • Desejamos uma estadia agradável!<br>
                    • Em caso de necessidade, nossa equipe está à disposição
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px;"">📍 Contato</div>
                <div style=""color: #2b6cb0; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>📞 {hotelPhone}")}
                </div>
            </div>")}
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Tenha uma excelente estadia!</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateCheckOutConfirmationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        decimal totalAmount,
        string currency,
        string? hotelAddress = null,
        string? hotelPhone = null)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Check-out Realizado - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .info-section {{ margin: 25px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 15px 0; border-bottom: 1px solid #e2e8f0; }}
        .info-label {{ color: #718096; font-size: 14px; }}
        .info-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .total-section {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 25px; border-radius: 12px; margin: 30px 0; text-align: center; color: white; }}
        .total-label {{ font-size: 16px; opacity: 0.9; margin-bottom: 8px; }}
        .total-value {{ font-size: 36px; font-weight: 700; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>👋 Check-out Realizado</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""icon"">✨</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div style=""text-align: center; color: #4a5568; margin-bottom: 30px; font-size: 16px;"">
                Seu check-out foi realizado com sucesso. Foi um prazer recebê-lo!
            </div>
            
            <div style=""background-color: #edf2f7; padding: 15px; border-radius: 8px; text-align: center; margin: 20px 0;"">
                <div style=""font-size: 12px; color: #718096; margin-bottom: 5px;"">Código da Reserva</div>
                <div style=""font-size: 24px; font-weight: 700; color: #2d3748; letter-spacing: 1px;"">{bookingCode}</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy}</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy}</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">🌙 Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div class=""total-section"">
                <div class=""total-label"">Valor Total da Estadia</div>
                <div class=""total-value"">{FormatCurrency(totalAmount, currency)}</div>
            </div>
            
            <div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #22543d; margin-bottom: 10px;"">💬 Avalie sua Experiência</div>
                <div style=""color: #2f855a; font-size: 14px; line-height: 1.8;"">
                    Sua opinião é muito importante para nós!<br>
                    Compartilhe sua experiência e nos ajude a melhorar nossos serviços.
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px;"">📍 Esperamos vê-lo novamente!</div>
                <div style=""color: #2b6cb0; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>📞 {hotelPhone}")}
                </div>
            </div>")}
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Obrigado por escolher {hotelName}!</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateBookingConfirmedEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        List<BookingRoomInfo> rooms,
        string? hotelAddress = null,
        string? hotelPhone = null)
    {
        var roomsHtml = string.Join("", rooms.Select(room => $@"
            <div style=""background-color: #f7fafc; padding: 15px; border-radius: 8px; margin-bottom: 10px; border-left: 4px solid #48bb78;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2d3748;"">
                    🏨 Quarto {room.RoomNumber} - {room.RoomTypeName}
                </div>
            </div>"));

        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reserva Confirmada - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .success-icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .success-box {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .success-box h2 {{ font-size: 24px; margin-bottom: 8px; }}
        .info-section {{ margin: 25px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 15px 0; border-bottom: 1px solid #e2e8f0; }}
        .info-label {{ color: #718096; font-size: 14px; }}
        .info-value {{ color: #2d3748; font-size: 16px; font-weight: 600; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>✅ Reserva Confirmada</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""success-icon"">🎉</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div class=""success-box"">
                <h2>Sua Reserva Foi Confirmada!</h2>
                <p>Estamos ansiosos para recebê-lo</p>
            </div>
            
            <div style=""background-color: #edf2f7; padding: 15px; border-radius: 8px; text-align: center; margin: 20px 0;"">
                <div style=""font-size: 12px; color: #718096; margin-bottom: 5px;"">Código da Reserva</div>
                <div style=""font-size: 24px; font-weight: 700; color: #2d3748; letter-spacing: 1px;"">{bookingCode}</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy} às 14:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">📅 Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy} às 12:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">🌙 Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div style=""margin: 25px 0;"">
                <div style=""font-size: 20px; color: #2d3748; margin-bottom: 20px; font-weight: 600;"">🏨 Quartos Confirmados</div>
                {roomsHtml}
            </div>
            
            <div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #22543d; margin-bottom: 10px;"">💡 Próximos Passos</div>
                <div style=""color: #2f855a; font-size: 14px; line-height: 1.8;"">
                    • Você receberá um lembrete 3 dias antes do check-in<br>
                    • Prepare-se para uma experiência incrível!<br>
                    • Em caso de dúvidas, entre em contato conosco
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px;"">📍 Como Chegar</div>
                <div style=""color: #2b6cb0; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>📞 {hotelPhone}")}
                </div>
            </div>")}
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Estamos ansiosos para recebê-lo!</p>
            <p style=""margin-top: 5px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateCheckOutReminderEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkOutDate,
        string? hotelAddress = null,
        string? hotelPhone = null)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Lembrete de Check-out - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #ed8936 0%, #dd6b20 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .reminder-icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .checkout-box {{ background: linear-gradient(135deg, #ed8936 0%, #dd6b20 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .checkout-label {{ font-size: 14px; opacity: 0.9; margin-bottom: 8px; }}
        .checkout-value {{ font-size: 36px; font-weight: 700; }}
        .info-section {{ margin: 25px 0; }}
        .info-box {{ background-color: #fff5f5; border-left: 4px solid #f56565; padding: 20px; border-radius: 8px; margin: 25px 0; }}
        .info-title {{ font-size: 16px; font-weight: 600; color: #c53030; margin-bottom: 10px; }}
        .info-content {{ color: #742a2a; font-size: 14px; line-height: 1.8; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>⏰ Lembrete de Check-out</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">Sua estadia está chegando ao fim</p>
        </div>
        
        <div class=""content"">
            <div class=""reminder-icon"">🕐</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div style=""text-align: center; color: #4a5568; margin-bottom: 30px; font-size: 16px;"">
                Faltam apenas <strong>1 dia</strong> para o check-out no <strong>{hotelName}</strong>!
            </div>
            
            <div class=""checkout-box"">
                <div class=""checkout-label"">Data e Hora do Check-out</div>
                <div class=""checkout-value"">{checkOutDate:dd/MM/yyyy}</div>
                <div style=""font-size: 18px; margin-top: 10px; opacity: 0.95;"">até às 12:00</div>
            </div>
            
            <div class=""info-box"">
                <div class=""info-title"">⚠️ Informações Importantes</div>
                <div class=""info-content"">
                    • Check-out: até às <strong>12:00</strong><br>
                    • Verifique se não deixou pertences pessoais no quarto<br>
                    • Se precisar de mais tempo, entre em contato conosco<br>
                    • A chave do quarto deve ser devolvida na recepção
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #22543d; margin-bottom: 10px;"">📍 Localização</div>
                <div style=""color: #2f855a; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>📞 {hotelPhone}")}
                </div>
            </div>")}
            
            <div style=""text-align: center; margin: 30px 0; padding: 20px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 12px; color: white;"">
                <div style=""font-size: 18px; font-weight: 600; margin-bottom: 10px;"">Foi um prazer recebê-lo!</div>
                <div style=""font-size: 14px; opacity: 0.9;"">Esperamos vê-lo novamente em breve</div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p style=""margin-top: 10px;"">{hotelAddress}</p>")}
            {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>📞 {hotelPhone}</p>")}
            <p style=""margin-top: 15px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    public string GeneratePostStayThankYouEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        string? hotelAddress = null,
        string? hotelPhone = null)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Obrigado pela sua estadia - {hotelName}</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; color: white; }}
        .header h1 {{ font-size: 28px; margin-bottom: 10px; font-weight: 600; }}
        .content {{ padding: 40px 30px; }}
        .thank-you-icon {{ text-align: center; font-size: 64px; margin-bottom: 20px; }}
        .greeting {{ font-size: 24px; color: #2d3748; margin-bottom: 20px; font-weight: 600; text-align: center; }}
        .message-box {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 25px; border-radius: 12px; text-align: center; color: white; margin: 25px 0; }}
        .message-box h2 {{ font-size: 22px; margin-bottom: 10px; }}
        .message-box p {{ font-size: 16px; opacity: 0.95; }}
        .stats-section {{ margin: 30px 0; }}
        .stat-item {{ background-color: #f7fafc; padding: 20px; border-radius: 10px; margin-bottom: 15px; text-align: center; }}
        .stat-label {{ color: #718096; font-size: 14px; margin-bottom: 5px; }}
        .stat-value {{ color: #2d3748; font-size: 24px; font-weight: 700; }}
        .review-box {{ background-color: #ebf8ff; border-left: 4px solid #4299e1; padding: 20px; border-radius: 8px; margin: 25px 0; }}
        .review-title {{ font-size: 16px; font-weight: 600; color: #2c5282; margin-bottom: 10px; }}
        .review-content {{ color: #2b6cb0; font-size: 14px; line-height: 1.8; }}
        .footer {{ background-color: #2d3748; padding: 30px; text-align: center; color: #a0aec0; font-size: 14px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🙏 Obrigado pela sua estadia!</h1>
            <p style=""margin-top: 10px; opacity: 0.95;"">{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""thank-you-icon"">💙</div>
            
            <div class=""greeting"">Olá, {guestName}!</div>
            
            <div class=""message-box"">
                <h2>Foi um prazer recebê-lo!</h2>
                <p>Esperamos que tenha tido uma experiência maravilhosa conosco</p>
            </div>
            
            <div class=""stats-section"">
                <div class=""stat-item"">
                    <div class=""stat-label"">Reserva</div>
                    <div class=""stat-value"">{bookingCode}</div>
                </div>
                <div class=""stat-item"">
                    <div class=""stat-label"">Período</div>
                    <div class=""stat-value"">{checkInDate:dd/MM} - {checkOutDate:dd/MM/yyyy}</div>
                </div>
                <div class=""stat-item"">
                    <div class=""stat-label"">Noites</div>
                    <div class=""stat-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div class=""review-box"">
                <div class=""review-title"">⭐ Avalie sua experiência</div>
                <div class=""review-content"">
                    Sua opinião é muito importante para nós! Compartilhe sua experiência e nos ajude a melhorar nossos serviços.
                    <br><br>
                    <strong>O que você mais gostou?</strong><br>
                    • Atendimento<br>
                    • Comodidades<br>
                    • Localização<br>
                    • Limpeza
                </div>
            </div>
            
            <div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #22543d; margin-bottom: 10px;"">💡 Próxima visita</div>
                <div style=""color: #2f855a; font-size: 14px; line-height: 1.8;"">
                    Esperamos recebê-lo novamente em breve!<br>
                    Fique atento às nossas promoções e ofertas especiais.
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #fff5f5; border-left: 4px solid #f56565; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #c53030; margin-bottom: 10px;"">📍 Nosso Contato</div>
                <div style=""color: #742a2a; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>📞 {hotelPhone}")}
                </div>
            </div>")}
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p style=""margin-top: 10px;"">Obrigado por escolher {hotelName}!</p>
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p style=""margin-top: 10px;"">{hotelAddress}</p>")}
            {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>📞 {hotelPhone}</p>")}
            <p style=""margin-top: 15px; font-size: 12px;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string FormatCurrency(decimal amount, string currency)
    {
        return currency switch
        {
            "BRL" => $"R$ {amount:N2}",
            "USD" => $"${amount:N2}",
            "EUR" => $"€{amount:N2}",
            _ => $"{amount:N2} {currency}"
        };
    }
}


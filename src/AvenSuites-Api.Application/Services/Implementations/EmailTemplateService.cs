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
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background-color: #f8f9fa; line-height: 1.6; color: #212529; }}
        .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }}
        .header {{ background-color: #1a1a1a; padding: 50px 40px; text-align: center; border-bottom: 3px solid #d4af37; }}
        .header h1 {{ font-size: 32px; font-weight: 300; letter-spacing: 2px; color: #ffffff; margin-bottom: 8px; }}
        .header p {{ color: #d4af37; font-size: 14px; letter-spacing: 1px; text-transform: uppercase; }}
        .content {{ padding: 50px 40px; }}
        .greeting {{ font-size: 28px; color: #1a1a1a; margin-bottom: 24px; font-weight: 400; letter-spacing: -0.5px; }}
        .message {{ font-size: 16px; color: #495057; margin-bottom: 32px; line-height: 1.8; }}
        .divider {{ height: 1px; background-color: #e9ecef; margin: 40px 0; }}
        .footer {{ background-color: #1a1a1a; padding: 40px; text-align: center; color: #adb5bd; font-size: 13px; }}
        .footer p {{ margin: 8px 0; }}
        .footer strong {{ color: #ffffff; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>{hotelName}</h1>
            <p>Bem-vindo</p>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">Olá, {guestName}</div>
            
            <div class=""message"">
                Sua conta foi criada com sucesso. Agora você pode fazer reservas, gerenciar seu perfil e acompanhar suas estadias através do nosso sistema.
            </div>
            
            <div class=""divider""></div>
            
            <div class=""message"">
                Em caso de dúvidas, nossa equipe está disponível para ajudar.
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            <p>Este é um e-mail automático. Por favor, não responda.</p>
            <p style=""margin-top: 20px; font-size: 11px; color: #6c757d;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
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
            <div style=""background-color: #f8f9fa; padding: 20px; margin-bottom: 12px; border-left: 3px solid #1a1a1a;"">
                <div style=""display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap;"">
                    <div>
                        <div style=""font-size: 17px; font-weight: 500; color: #1a1a1a; margin-bottom: 4px;"">
                            Quarto {room.RoomNumber}
                        </div>
                        <div style=""color: #6c757d; font-size: 14px; font-weight: 300;"">{room.RoomTypeName}</div>
                    </div>
                    <div style=""text-align: right;"">
                        <div style=""font-size: 18px; font-weight: 500; color: #1a1a1a;"">
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
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background-color: #f8f9fa; line-height: 1.6; color: #212529; }}
        .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }}
        .header {{ background-color: #1a1a1a; padding: 50px 40px; text-align: center; border-bottom: 3px solid #d4af37; }}
        .header h1 {{ font-size: 28px; font-weight: 300; letter-spacing: 1px; color: #ffffff; }}
        .header p {{ color: #d4af37; font-size: 12px; letter-spacing: 1px; text-transform: uppercase; margin-top: 8px; }}
        .content {{ padding: 50px 40px; }}
        .greeting {{ font-size: 24px; color: #1a1a1a; margin-bottom: 32px; font-weight: 400; }}
        .booking-code {{ background-color: #1a1a1a; padding: 30px; text-align: center; margin: 32px 0; }}
        .booking-code-label {{ font-size: 11px; color: #d4af37; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 12px; }}
        .booking-code-value {{ font-size: 28px; font-weight: 300; letter-spacing: 3px; color: #ffffff; }}
        .info-section {{ margin: 32px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 16px 0; border-bottom: 1px solid #e9ecef; }}
        .info-label {{ color: #6c757d; font-size: 14px; font-weight: 400; }}
        .info-value {{ color: #1a1a1a; font-size: 15px; font-weight: 500; }}
        .rooms-section {{ margin: 32px 0; }}
        .rooms-title {{ font-size: 16px; color: #1a1a1a; margin-bottom: 20px; font-weight: 500; letter-spacing: 0.5px; text-transform: uppercase; }}
        .total-section {{ background-color: #1a1a1a; padding: 30px; margin: 32px 0; text-align: center; }}
        .total-label {{ font-size: 12px; color: #d4af37; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 12px; }}
        .total-value {{ font-size: 32px; font-weight: 300; color: #ffffff; }}
        .footer {{ background-color: #1a1a1a; padding: 40px; text-align: center; color: #adb5bd; font-size: 13px; }}
        .footer p {{ margin: 8px 0; }}
        .footer strong {{ color: #ffffff; }}
        .notice {{ background-color: #f8f9fa; border-left: 3px solid #1a1a1a; padding: 20px; margin: 32px 0; }}
        .notice-title {{ font-size: 14px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 0.5px; }}
        .notice-text {{ color: #495057; font-size: 14px; line-height: 1.8; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Confirmação de Reserva</h1>
            <p>{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">Olá, {guestName}</div>
            
            <div style=""color: #495057; margin-bottom: 32px; font-size: 15px; line-height: 1.8;"">
                Sua reserva foi confirmada. Guarde este e-mail como comprovante.
            </div>
            
            <div class=""booking-code"">
                <div class=""booking-code-label"">Código da Reserva</div>
                <div class=""booking-code-value"">{bookingCode}</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy} às 14:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy} às 12:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div class=""rooms-section"">
                <div class=""rooms-title"">Quartos Reservados</div>
                {roomsHtml}
            </div>
            
            <div class=""total-section"">
                <div class=""total-label"">Valor Total</div>
                <div class=""total-value"">{FormatCurrency(totalAmount, currency)}</div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #f8f9fa; padding: 24px; margin: 32px 0; border-left: 3px solid #1a1a1a;"">
                <div style=""font-size: 14px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 0.5px;"">Localização</div>
                <div style=""color: #495057; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>{hotelPhone}")}
                </div>
            </div>")}
            
            <div class=""notice"">
                <div class=""notice-title"">Informações Importantes</div>
                <div class=""notice-text"">
                    Apresente este comprovante no check-in. Horário de chegada: a partir das 14:00. Horário de saída: até às 12:00. Em caso de dúvidas, entre em contato conosco.
                </div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p>{hotelAddress}</p>")}
            {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>{hotelPhone}</p>")}
            <p style=""margin-top: 20px; font-size: 11px; color: #6c757d;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
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
            <div style=""background-color: #f8f9fa; padding: 16px; margin-bottom: 10px; border-left: 3px solid #1a1a1a;"">
                <div style=""font-size: 15px; font-weight: 500; color: #1a1a1a;"">
                    Quarto {room.RoomNumber} - {room.RoomTypeName}
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
        .header {{ background-color: #1a1a1a; padding: 50px 40px; text-align: center; border-bottom: 3px solid #ffc107; }}
        .header h1 {{ font-size: 28px; font-weight: 300; letter-spacing: 1px; color: #ffffff; }}
        .header p {{ color: #ffc107; font-size: 12px; letter-spacing: 1px; text-transform: uppercase; margin-top: 8px; }}
        .content {{ padding: 50px 40px; }}
        .greeting {{ font-size: 24px; color: #1a1a1a; margin-bottom: 32px; font-weight: 400; }}
        .booking-code {{ background-color: #f8f9fa; padding: 30px; text-align: center; margin: 32px 0; border: 1px solid #dee2e6; }}
        .booking-code-label {{ font-size: 11px; color: #6c757d; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 12px; }}
        .booking-code-value {{ font-size: 28px; font-weight: 300; letter-spacing: 3px; color: #1a1a1a; }}
        .info-section {{ margin: 32px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 16px 0; border-bottom: 1px solid #e9ecef; }}
        .info-label {{ color: #6c757d; font-size: 14px; font-weight: 400; }}
        .info-value {{ color: #1a1a1a; font-size: 15px; font-weight: 500; }}
        .rooms-section {{ margin: 32px 0; }}
        .rooms-title {{ font-size: 16px; color: #1a1a1a; margin-bottom: 20px; font-weight: 500; letter-spacing: 0.5px; text-transform: uppercase; }}
        .notice {{ background-color: #f8f9fa; border-left: 3px solid #1a1a1a; padding: 20px; margin: 32px 0; }}
        .notice-title {{ font-size: 14px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 0.5px; }}
        .notice-text {{ color: #495057; font-size: 14px; line-height: 1.8; }}
        .footer {{ background-color: #1a1a1a; padding: 40px; text-align: center; color: #adb5bd; font-size: 13px; }}
        .footer p {{ margin: 8px 0; }}
        .footer strong {{ color: #ffffff; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Lembrete de Reserva</h1>
            <p>{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">Olá, {guestName}</div>
            
            <div style=""color: #495057; margin-bottom: 32px; font-size: 15px; line-height: 1.8;"">
                Faltam 3 dias para sua reserva. Seguem os detalhes da sua estadia.
            </div>
            
            <div class=""booking-code"">
                <div class=""booking-code-label"">Código da Reserva</div>
                <div class=""booking-code-value"">{bookingCode}</div>
            </div>
            
            <div style=""background-color: #1a1a1a; padding: 24px; margin: 32px 0; text-align: center; color: #ffffff;"">
                <div style=""font-size: 11px; color: #d4af37; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 8px;"">Check-in</div>
                <div style=""font-size: 24px; font-weight: 300; letter-spacing: 1px;"">{checkInDate:dd/MM/yyyy}</div>
                <div style=""font-size: 13px; color: #adb5bd; margin-top: 8px;"">A partir das 14:00</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy} às 12:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div class=""rooms-section"">
                <div class=""rooms-title"">Quartos Reservados</div>
                {roomsHtml}
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #f8f9fa; padding: 24px; margin: 32px 0; border-left: 3px solid #1a1a1a;"">
                <div style=""font-size: 14px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 0.5px;"">Localização</div>
                <div style=""color: #495057; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>{hotelPhone}")}
                </div>
            </div>")}
            
            <div class=""notice"">
                <div class=""notice-title"">Informações</div>
                <div class=""notice-text"">
                    Chegada: a partir das 14:00. Saída: até às 12:00. Traga um documento com foto para o check-in. Em caso de dúvidas, entre em contato conosco.
                </div>
            </div>
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p>{hotelAddress}</p>")}
            {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>{hotelPhone}</p>")}
            <p style=""margin-top: 20px; font-size: 11px; color: #6c757d;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
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
            <div style=""background-color: #f8f9fa; padding: 16px; margin-bottom: 10px; border-left: 3px solid #1a1a1a;"">
                <div style=""font-size: 15px; font-weight: 500; color: #1a1a1a;"">
                    Quarto {room.RoomNumber} - {room.RoomTypeName}
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
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background-color: #f8f9fa; line-height: 1.6; color: #212529; }}
        .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }}
        .header {{ background-color: #1a1a1a; padding: 50px 40px; text-align: center; border-bottom: 3px solid #28a745; }}
        .header h1 {{ font-size: 28px; font-weight: 300; letter-spacing: 1px; color: #ffffff; }}
        .header p {{ color: #28a745; font-size: 12px; letter-spacing: 1px; text-transform: uppercase; margin-top: 8px; }}
        .content {{ padding: 50px 40px; }}
        .greeting {{ font-size: 24px; color: #1a1a1a; margin-bottom: 32px; font-weight: 400; }}
        .booking-code {{ background-color: #f8f9fa; padding: 30px; text-align: center; margin: 32px 0; border: 1px solid #dee2e6; }}
        .booking-code-label {{ font-size: 11px; color: #6c757d; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 12px; }}
        .booking-code-value {{ font-size: 28px; font-weight: 300; letter-spacing: 3px; color: #1a1a1a; }}
        .info-section {{ margin: 32px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 16px 0; border-bottom: 1px solid #e9ecef; }}
        .info-label {{ color: #6c757d; font-size: 14px; font-weight: 400; }}
        .info-value {{ color: #1a1a1a; font-size: 15px; font-weight: 500; }}
        .rooms-section {{ margin: 32px 0; }}
        .rooms-title {{ font-size: 16px; color: #1a1a1a; margin-bottom: 20px; font-weight: 500; letter-spacing: 0.5px; text-transform: uppercase; }}
        .footer {{ background-color: #1a1a1a; padding: 40px; text-align: center; color: #adb5bd; font-size: 13px; }}
        .footer p {{ margin: 8px 0; }}
        .footer strong {{ color: #ffffff; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Reserva Confirmada</h1>
            <p>{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">Olá, {guestName}</div>
            
            <div style=""color: #495057; margin-bottom: 32px; font-size: 15px; line-height: 1.8;"">
                Sua reserva foi confirmada. Aguardamos sua chegada.
            </div>
            
            <div class=""booking-code"">
                <div class=""booking-code-label"">Código da Reserva</div>
                <div class=""booking-code-value"">{bookingCode}</div>
            </div>
            
            <div class=""info-section"">
                <div class=""info-row"">
                    <div class=""info-label"">Check-in</div>
                    <div class=""info-value"">{checkInDate:dd/MM/yyyy} às 14:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">Check-out</div>
                    <div class=""info-value"">{checkOutDate:dd/MM/yyyy} às 12:00</div>
                </div>
                <div class=""info-row"">
                    <div class=""info-label"">Noites</div>
                    <div class=""info-value"">{nights} {(nights == 1 ? "noite" : "noites")}</div>
                </div>
            </div>
            
            <div class=""rooms-section"">
                <div class=""rooms-title"">Quartos Confirmados</div>
                {roomsHtml}
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #f8f9fa; padding: 24px; margin: 32px 0; border-left: 3px solid #1a1a1a;"">
                <div style=""font-size: 14px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 0.5px;"">Localização</div>
                <div style=""color: #495057; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>{hotelPhone}")}
                </div>
            </div>")}
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p>{hotelAddress}</p>")}
            {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>{hotelPhone}</p>")}
            <p style=""margin-top: 20px; font-size: 11px; color: #6c757d;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
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
        .header {{ background-color: #1a1a1a; padding: 50px 40px; text-align: center; border-bottom: 3px solid #ffc107; }}
        .header h1 {{ font-size: 28px; font-weight: 300; letter-spacing: 1px; color: #ffffff; }}
        .header p {{ color: #ffc107; font-size: 12px; letter-spacing: 1px; text-transform: uppercase; margin-top: 8px; }}
        .content {{ padding: 50px 40px; }}
        .greeting {{ font-size: 24px; color: #1a1a1a; margin-bottom: 32px; font-weight: 400; }}
        .checkout-box {{ background-color: #1a1a1a; padding: 30px; text-align: center; margin: 32px 0; color: #ffffff; }}
        .checkout-label {{ font-size: 11px; color: #ffc107; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 12px; }}
        .checkout-value {{ font-size: 32px; font-weight: 300; letter-spacing: 1px; }}
        .info-section {{ margin: 32px 0; }}
        .info-box {{ background-color: #f8f9fa; border-left: 3px solid #1a1a1a; padding: 20px; margin: 32px 0; }}
        .info-title {{ font-size: 14px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 0.5px; }}
        .info-content {{ color: #495057; font-size: 14px; line-height: 1.8; }}
        .footer {{ background-color: #1a1a1a; padding: 40px; text-align: center; color: #adb5bd; font-size: 13px; }}
        .footer p {{ margin: 8px 0; }}
        .footer strong {{ color: #ffffff; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Lembrete de Check-out</h1>
            <p>{hotelName}</p>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">Olá, {guestName}</div>
            
            <div style=""color: #495057; margin-bottom: 32px; font-size: 15px; line-height: 1.8;"">
                Este é um lembrete de que seu check-out está programado para amanhã.
            </div>
            
            <div class=""checkout-box"">
                <div class=""checkout-label"">Data de Check-out</div>
                <div class=""checkout-value"">{checkOutDate:dd/MM/yyyy}</div>
                <div style=""font-size: 13px; color: #adb5bd; margin-top: 8px;"">Até às 12:00</div>
            </div>
            
            <div class=""info-box"">
                <div class=""info-title"">Horário de Saída</div>
                <div class=""info-content"">
                    Por favor, realize o check-out até às 12:00 para evitar cobranças adicionais.
                </div>
            </div>
            
            <div class=""info-box"">
                <div class=""info-title"">Informações</div>
                <div class=""info-content"">
                    Verifique se não deixou nenhum pertence no quarto. Devolva as chaves na recepção. Em caso de dúvidas sobre o pagamento, entre em contato conosco.
                </div>
            </div>
            
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"
            <div style=""background-color: #f8f9fa; padding: 24px; margin: 32px 0; border-left: 3px solid #1a1a1a;"">
                <div style=""font-size: 14px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 0.5px;"">Localização</div>
                <div style=""color: #495057; line-height: 1.8; font-size: 14px;"">
                    {hotelName}<br>
                    {hotelAddress}
                    {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<br>{hotelPhone}")}
                </div>
            </div>")}
        </div>
        
        <div class=""footer"">
            <p><strong>{hotelName}</strong></p>
            {(string.IsNullOrWhiteSpace(hotelAddress) ? "" : $@"<p>{hotelAddress}</p>")}
            {(string.IsNullOrWhiteSpace(hotelPhone) ? "" : $@"<p>{hotelPhone}</p>")}
            <p style=""margin-top: 20px; font-size: 11px; color: #6c757d;"">© {DateTime.Now.Year} {hotelName}. Todos os direitos reservados.</p>
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


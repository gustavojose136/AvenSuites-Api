using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using AvenSuitesApi.Application.DTOs;
using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Application.Utils;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Guest;

public class GuestRegistrationService : IGuestRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IGuestPiiRepository _guestPiiRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<GuestRegistrationService> _logger;

    public GuestRegistrationService(
        IUserRepository userRepository,
        IGuestRepository guestRepository,
        IGuestPiiRepository guestPiiRepository,
        IRoleRepository roleRepository,
        IHotelRepository hotelRepository,
        IJwtService jwtService,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        ILogger<GuestRegistrationService> logger)
    {
        _userRepository = userRepository;
        _guestRepository = guestRepository;
        _guestPiiRepository = guestPiiRepository;
        _roleRepository = roleRepository;
        _hotelRepository = hotelRepository;
        _jwtService = jwtService;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task<LoginResponse> RegisterAsync(GuestRegisterRequest request)
    {
        try
        {
            // Verificar se o email já existe
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new InvalidOperationException("Email já cadastrado");
            }

            // Verificar se o hotel existe
            var hotel = await _hotelRepository.GetByIdAsync(request.HotelId);
            if (hotel == null)
            {
                throw new InvalidOperationException("Hotel não encontrado");
            }

            // Obter o role Guest
            var guestRole = await _roleRepository.GetByNameAsync("Guest");
            if (guestRole == null)
            {
                throw new InvalidOperationException("Role 'Guest' não encontrado");
            }

            // Criar o usuário
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = Argon2PasswordHasher.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            // Adicionar role Guest ao usuário
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = guestRole.Id,
                AssignedAt = DateTime.UtcNow
            };

            user.UserRoles.Add(userRole);
            await _userRepository.UpdateAsync(user);

            // Criar o guest
            var guest = new AvenSuitesApi.Domain.Entities.Guest
            {
                Id = Guid.NewGuid(),
                HotelId = request.HotelId,
                UserId = user.Id,
                MarketingConsent = request.MarketingConsent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _guestRepository.AddAsync(guest);

            // Criar GuestPii
            var guestPii = new GuestPii
            {
                GuestId = guest.Id,
                FullName = request.Name,
                Email = request.Email,
                EmailSha256 = ComputeSha256Hash(request.Email),
                PhoneE164 = request.Phone,
                PhoneSha256 = ComputeSha256Hash(request.Phone),
                DocumentType = request.DocumentType,
                DocumentPlain = request.Document,
                DocumentSha256 = ComputeSha256Hash(request.Document),
                BirthDate = request.BirthDate,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Neighborhood = request.Neighborhood,
                State = request.State,
                PostalCode = request.PostalCode,
                CountryCode = request.CountryCode,
                DocumentKeyVersion = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _guestPiiRepository.AddOrUpdateAsync(guestPii);

            // Atualizar user com a navegação para guest para o token
            user.Guest = guest;
            user.UserRoles = new List<UserRole> { userRole };
            userRole.Role = guestRole;

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("Hóspede registrado com sucesso: {Email}", request.Email);

            try
            {
                var emailBody = _emailTemplateService.GenerateWelcomeEmail(request.Name, hotel.Name);
                await _emailService.SendEmailAsync(
                    to: request.Email,
                    subject: $"Bem-vindo ao {hotel.Name}!",
                    body: emailBody,
                    isHtml: true,
                    cc: null,
                    bcc: null);
                
                _logger.LogInformation("E-mail de boas-vindas enviado para {Email}", request.Email);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Erro ao enviar e-mail de boas-vindas para {Email}", request.Email);
            }

            return new LoginResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = new List<string> { "Guest" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar hóspede: {Email}", request.Email);
            throw;
        }
    }

    public async Task<GuestProfileResponse> GetProfileAsync(Guid guestId)
    {
        var guest = await _guestRepository.GetByIdWithPiiAsync(guestId);
        if (guest == null)
        {
            throw new InvalidOperationException("Hóspede não encontrado");
        }

        return new GuestProfileResponse
        {
            Id = guest.Id,
            UserId = guest.UserId ?? Guid.Empty,
            HotelId = guest.HotelId,
            HotelName = guest.Hotel?.Name ?? string.Empty,
            Name = guest.GuestPii?.FullName ?? string.Empty,
            Email = guest.GuestPii?.Email ?? string.Empty,
            Phone = guest.GuestPii?.PhoneE164 ?? string.Empty,
            DocumentType = guest.GuestPii?.DocumentType ?? string.Empty,
            Document = guest.GuestPii?.DocumentPlain ?? string.Empty,
            BirthDate = guest.GuestPii?.BirthDate,
            AddressLine1 = guest.GuestPii?.AddressLine1 ?? string.Empty,
            AddressLine2 = guest.GuestPii?.AddressLine2,
            City = guest.GuestPii?.City ?? string.Empty,
            Neighborhood = guest.GuestPii?.Neighborhood,
            State = guest.GuestPii?.State ?? string.Empty,
            PostalCode = guest.GuestPii?.PostalCode ?? string.Empty,
            CountryCode = guest.GuestPii?.CountryCode ?? string.Empty,
            MarketingConsent = guest.MarketingConsent,
            CreatedAt = guest.CreatedAt
        };
    }

    public async Task<GuestProfileResponse> UpdateProfileAsync(Guid guestId, GuestRegisterRequest request)
    {
        var guest = await _guestRepository.GetByIdWithPiiAsync(guestId);
        if (guest == null)
        {
            throw new InvalidOperationException("Hóspede não encontrado");
        }

        // Atualizar GuestPii
        if (guest.GuestPii != null)
        {
            guest.GuestPii.FullName = request.Name;
            guest.GuestPii.Email = request.Email;
            guest.GuestPii.EmailSha256 = ComputeSha256Hash(request.Email);
            guest.GuestPii.PhoneE164 = request.Phone;
            guest.GuestPii.PhoneSha256 = ComputeSha256Hash(request.Phone);
            guest.GuestPii.DocumentType = request.DocumentType;
            guest.GuestPii.DocumentPlain = request.Document;
            guest.GuestPii.DocumentSha256 = ComputeSha256Hash(request.Document);
            guest.GuestPii.BirthDate = request.BirthDate;
            guest.GuestPii.AddressLine1 = request.AddressLine1;
            guest.GuestPii.AddressLine2 = request.AddressLine2;
            guest.GuestPii.City = request.City;
            guest.GuestPii.Neighborhood = request.Neighborhood;
            guest.GuestPii.State = request.State;
            guest.GuestPii.PostalCode = request.PostalCode;
            guest.GuestPii.CountryCode = request.CountryCode;
            guest.GuestPii.UpdatedAt = DateTime.UtcNow;

            await _guestPiiRepository.AddOrUpdateAsync(guest.GuestPii);
        }

        // Atualizar marketing consent
        guest.MarketingConsent = request.MarketingConsent;
        guest.UpdatedAt = DateTime.UtcNow;
        await _guestRepository.UpdateAsync(guest);

        // Atualizar nome do usuário se houver
        if (guest.UserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(guest.UserId.Value);
            if (user != null)
            {
                user.Name = request.Name;
                user.Email = request.Email;
                await _userRepository.UpdateAsync(user);
            }
        }

        return await GetProfileAsync(guestId);
    }

    private static string ComputeSha256Hash(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData.ToLowerInvariant()));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}


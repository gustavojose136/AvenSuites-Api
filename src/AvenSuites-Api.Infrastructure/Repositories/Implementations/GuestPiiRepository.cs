using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class GuestPiiRepository : IGuestPiiRepository
{
    private readonly ApplicationDbContext _context;

    public GuestPiiRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GuestPii?> GetByGuestIdAsync(Guid guestId)
    {
        return await _context.GuestPii.FirstOrDefaultAsync(gp => gp.GuestId == guestId);
    }

    public async Task<GuestPii> AddOrUpdateAsync(GuestPii guestPii)
    {
        var existing = await GetByGuestIdAsync(guestPii.GuestId);
        
        if (existing == null)
        {
            guestPii.CreatedAt = DateTime.UtcNow;
            guestPii.UpdatedAt = DateTime.UtcNow;
            _context.GuestPii.Add(guestPii);
        }
        else
        {
            existing.FullName = guestPii.FullName;
            existing.Email = guestPii.Email;
            existing.EmailSha256 = guestPii.EmailSha256;
            existing.PhoneE164 = guestPii.PhoneE164;
            existing.PhoneSha256 = guestPii.PhoneSha256;
            existing.DocumentType = guestPii.DocumentType;
            existing.DocumentPlain = guestPii.DocumentPlain;
            existing.DocumentSha256 = guestPii.DocumentSha256;
            existing.BirthDate = guestPii.BirthDate;
            existing.AddressLine1 = guestPii.AddressLine1;
            existing.AddressLine2 = guestPii.AddressLine2;
            existing.City = guestPii.City;
            existing.Neighborhood = guestPii.Neighborhood;
            existing.State = guestPii.State;
            existing.PostalCode = guestPii.PostalCode;
            existing.CountryCode = guestPii.CountryCode;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return existing ?? guestPii;
    }

    public async Task<bool> ExistsByEmailSha256Async(string emailSha256)
    {
        return await _context.GuestPii.AnyAsync(gp => gp.EmailSha256 == emailSha256);
    }

    public async Task<bool> ExistsByPhoneSha256Async(string phoneSha256)
    {
        return await _context.GuestPii.AnyAsync(gp => gp.PhoneSha256 == phoneSha256);
    }

    public async Task<bool> ExistsByDocumentSha256Async(string documentSha256)
    {
        return await _context.GuestPii.AnyAsync(gp => gp.DocumentSha256 == documentSha256);
    }
}


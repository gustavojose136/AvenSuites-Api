using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Domain.Interfaces;

public interface IGuestPiiRepository
{
    Task<GuestPii?> GetByGuestIdAsync(Guid guestId);
    Task<GuestPii> AddOrUpdateAsync(GuestPii guestPii);
    Task<bool> ExistsByEmailSha256Async(string emailSha256);
    Task<bool> ExistsByPhoneSha256Async(string phoneSha256);
    Task<bool> ExistsByDocumentSha256Async(string documentSha256);
}


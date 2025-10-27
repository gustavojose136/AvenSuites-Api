using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.MainGuest)
            .ThenInclude(g => g.GuestPii)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.RoomType)
            .Include(b => b.Payments)
            .Include(b => b.StatusHistory)
            .Include(b => b.Invoice)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking?> GetByCodeAsync(Guid hotelId, string code)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.HotelId == hotelId && b.Code == code);
    }

    public async Task<IEnumerable<Booking>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.Bookings
            .Include(b => b.MainGuest)
            .ThenInclude(g => g.GuestPii)
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByHotelIdAndDatesAsync(Guid hotelId, DateTime startDate, DateTime endDate)
    {
        return await _context.Bookings
            .Where(b => b.HotelId == hotelId
                && b.CheckInDate <= endDate
                && b.CheckOutDate >= startDate)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByGuestIdAsync(Guid guestId)
    {
        return await _context.Bookings
            .Where(b => b.MainGuestId == guestId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByStatusAsync(Guid hotelId, string status)
    {
        return await _context.Bookings
            .Where(b => b.HotelId == hotelId && b.Status == status)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        booking.CreatedAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking> UpdateAsync(Booking booking)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            booking.Status = "CANCELLED";
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Bookings.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> IsCodeUniqueAsync(Guid hotelId, string code)
    {
        return !await _context.Bookings.AnyAsync(b => b.HotelId == hotelId && b.Code == code);
    }
}


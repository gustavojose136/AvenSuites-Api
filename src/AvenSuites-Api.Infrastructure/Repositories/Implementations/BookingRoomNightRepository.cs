using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class BookingRoomNightRepository : IBookingRoomNightRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRoomNightRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingRoomNight>> GetByBookingRoomIdAsync(Guid bookingRoomId)
    {
        return await _context.BookingRoomNights
            .Where(brn => brn.BookingRoomId == bookingRoomId)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingRoomNight>> GetByRoomIdAndDateRangeAsync(Guid roomId, DateTime startDate, DateTime endDate)
    {
        return await _context.BookingRoomNights
            .Include(brn => brn.BookingRoom)
                .ThenInclude(br => br.Booking)
            .Where(brn => brn.RoomId == roomId
                && brn.StayDate >= startDate.Date
                && brn.StayDate < endDate.Date)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(Guid roomId, DateTime startDate, DateTime endDate, Guid? excludeBookingRoomId = null)
    {
        var query = _context.BookingRoomNights
            .Include(brn => brn.BookingRoom)
                .ThenInclude(br => br.Booking)
            .Where(brn => brn.RoomId == roomId
                && brn.BookingRoom.Booking.Status != "CANCELLED"
                && brn.StayDate >= startDate.Date
                && brn.StayDate < endDate.Date);

        if (excludeBookingRoomId.HasValue)
        {
            query = query.Where(brn => brn.BookingRoomId != excludeBookingRoomId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<BookingRoomNight> AddAsync(BookingRoomNight bookingRoomNight)
    {
        bookingRoomNight.CreatedAt = DateTime.UtcNow;
        _context.BookingRoomNights.Add(bookingRoomNight);
        await _context.SaveChangesAsync();
        return bookingRoomNight;
    }

    public async Task AddRangeAsync(IEnumerable<BookingRoomNight> bookingRoomNights)
    {
        var nightsList = bookingRoomNights.ToList();
        foreach (var night in nightsList)
        {
            night.CreatedAt = DateTime.UtcNow;
        }
        _context.BookingRoomNights.AddRange(nightsList);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var bookingRoomNight = await _context.BookingRoomNights.FindAsync(id);
        if (bookingRoomNight != null)
        {
            _context.BookingRoomNights.Remove(bookingRoomNight);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByBookingRoomIdAsync(Guid bookingRoomId)
    {
        var nights = await _context.BookingRoomNights
            .Where(brn => brn.BookingRoomId == bookingRoomId)
            .ToListAsync();
        
        if (nights.Any())
        {
            _context.BookingRoomNights.RemoveRange(nights);
            await _context.SaveChangesAsync();
        }
    }

}


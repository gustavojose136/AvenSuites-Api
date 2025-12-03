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

    public async Task<BookingRoomNight?> GetByIdAsync(Guid id)
    {
        return await _context.BookingRoomNights
            .Include(brn => brn.BookingRoom)
            .Include(brn => brn.Room)
            .FirstOrDefaultAsync(brn => brn.Id == id);
    }

    public async Task<IEnumerable<BookingRoomNight>> GetByBookingRoomIdAsync(Guid bookingRoomId)
    {
        return await _context.BookingRoomNights
            .Include(brn => brn.Room)
            .Where(brn => brn.BookingRoomId == bookingRoomId)
            .OrderBy(brn => brn.StayDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingRoomNight>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.BookingRoomNights
            .Include(brn => brn.BookingRoom)
                .ThenInclude(br => br.Booking)
            .Where(brn => brn.RoomId == roomId)
            .OrderBy(brn => brn.StayDate)
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
            .OrderBy(brn => brn.StayDate)
            .ToListAsync();
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
        var nights = bookingRoomNights.ToList();
        foreach (var night in nights)
        {
            night.CreatedAt = DateTime.UtcNow;
        }
        await _context.BookingRoomNights.AddRangeAsync(nights);
        await _context.SaveChangesAsync();
    }

    public async Task<BookingRoomNight> UpdateAsync(BookingRoomNight bookingRoomNight)
    {
        _context.BookingRoomNights.Update(bookingRoomNight);
        await _context.SaveChangesAsync();
        return bookingRoomNight;
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

    public async Task<bool> HasConflictAsync(Guid roomId, DateTime startDate, DateTime endDate, Guid? excludeBookingRoomId = null)
    {
        var query = _context.BookingRoomNights
            .Where(brn => brn.RoomId == roomId
                && brn.StayDate >= startDate.Date
                && brn.StayDate < endDate.Date);

        // Excluir noites de um BookingRoom específico (útil ao atualizar uma reserva)
        if (excludeBookingRoomId.HasValue)
        {
            query = query.Where(brn => brn.BookingRoomId != excludeBookingRoomId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.BookingRoomNights.AnyAsync(brn => brn.Id == id);
    }
}




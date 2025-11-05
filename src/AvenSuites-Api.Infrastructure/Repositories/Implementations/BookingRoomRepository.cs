using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class BookingRoomRepository : IBookingRoomRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookingRoom?> GetByIdAsync(Guid id)
    {
        return await _context.BookingRooms
            .Include(br => br.Booking)
            .Include(br => br.Room)
            .Include(br => br.RoomType)
            .FirstOrDefaultAsync(br => br.Id == id);
    }

    public async Task<IEnumerable<BookingRoom>> GetByBookingIdAsync(Guid bookingId)
    {
        return await _context.BookingRooms
            .Include(br => br.Room)
            .Include(br => br.RoomType)
            .Where(br => br.BookingId == bookingId)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingRoom>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.BookingRooms
            .Include(br => br.Booking)
            .Where(br => br.RoomId == roomId)
            .OrderByDescending(br => br.CreatedAt)
            .ToListAsync();
    }

    public async Task<BookingRoom> AddAsync(BookingRoom bookingRoom)
    {
        bookingRoom.CreatedAt = DateTime.UtcNow;
        bookingRoom.UpdatedAt = DateTime.UtcNow;
        _context.BookingRooms.Add(bookingRoom);
        await _context.SaveChangesAsync();
        return bookingRoom;
    }

    public async Task<BookingRoom> UpdateAsync(BookingRoom bookingRoom)
    {
        bookingRoom.UpdatedAt = DateTime.UtcNow;
        _context.BookingRooms.Update(bookingRoom);
        await _context.SaveChangesAsync();
        return bookingRoom;
    }

    public async Task DeleteAsync(Guid id)
    {
        var bookingRoom = await _context.BookingRooms.FindAsync(id);
        if (bookingRoom != null)
        {
            _context.BookingRooms.Remove(bookingRoom);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.BookingRooms.AnyAsync(br => br.Id == id);
    }
}


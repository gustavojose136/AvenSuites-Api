using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;

namespace AvenSuitesApi.Infrastructure.Repositories.Implementations;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Room>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(Guid hotelId, DateTime startDate, DateTime endDate, Guid? roomTypeId = null)
    {
        var query = _context.Rooms
            .Include(r => r.RoomType)
            .Where(r => r.HotelId == hotelId && r.Status == "ACTIVE");

        if (roomTypeId.HasValue)
            query = query.Where(r => r.RoomTypeId == roomTypeId.Value);

        // Implementar lógica completa de verificação de disponibilidade
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetByStatusAsync(Guid hotelId, string status)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .Where(r => r.HotelId == hotelId && r.Status == status)
            .ToListAsync();
    }

    public async Task<Room> AddAsync(Room room)
    {
        room.CreatedAt = DateTime.UtcNow;
        room.UpdatedAt = DateTime.UtcNow;
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        room.UpdatedAt = DateTime.UtcNow;
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task DeleteAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room != null)
        {
            room.Status = "INACTIVE";
            room.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == id);
    }

    public async Task<bool> IsRoomNumberUniqueAsync(Guid hotelId, string roomNumber, Guid? excludeRoomId = null)
    {
        var query = _context.Rooms.Where(r => r.HotelId == hotelId && r.RoomNumber == roomNumber);
        
        if (excludeRoomId.HasValue)
            query = query.Where(r => r.Id != excludeRoomId.Value);

        return !await query.AnyAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        var room = await GetByIdAsync(roomId);
        if (room == null || room.Status != "ACTIVE")
            return false;

        // Verificar conflitos com blocos de manutenção
        var hasMaintenance = await _context.MaintenanceBlocks
            .AnyAsync(mb => mb.RoomId == roomId 
                && mb.Status == "ACTIVE"
                && mb.StartDate <= checkOutDate 
                && mb.EndDate >= checkInDate);

        if (hasMaintenance)
            return false;

        // Verificar conflitos com reservas existentes
        var hasBooking = await _context.BookingRooms
            .Include(br => br.Booking)
            .AnyAsync(br => br.RoomId == roomId
                && br.Booking.Status != "CANCELLED"
                && br.Booking.CheckInDate < checkOutDate
                && br.Booking.CheckOutDate > checkInDate);

        return !hasBooking;
    }
    
    public async Task<IEnumerable<Room>> GetAvailableRoomsForPeriodAsync(
        Guid hotelId, DateTime startDate, DateTime endDate, Guid? roomTypeId = null)
    {
        var query = _context.Rooms
            .Include(r => r.RoomType)
            .Where(r => r.HotelId == hotelId && r.Status == "ACTIVE");

        if (roomTypeId.HasValue)
            query = query.Where(r => r.RoomTypeId == roomTypeId.Value);

        var rooms = await query.ToListAsync();
        
        var availableRooms = new List<Room>();
        
        foreach (var room in rooms)
        {
            var isAvailable = await IsRoomAvailableAsync(room.Id, startDate, endDate);
            if (isAvailable)
                availableRooms.Add(room);
        }

        return availableRooms;
    }
}


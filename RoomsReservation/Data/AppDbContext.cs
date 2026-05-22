using Microsoft.EntityFrameworkCore;
using RoomsReservation.Data.Tables;

namespace RoomsReservation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RoomEquipment> RoomEquipments => Set<RoomEquipment>();
        public DbSet<Equipment> Equipments => Set<Equipment>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
    }
}

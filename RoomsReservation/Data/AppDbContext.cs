using Microsoft.EntityFrameworkCore;
using RoomsReservation.Data.Tables;

namespace RoomsReservation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
    }
}

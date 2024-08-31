using EZ_Site.Models;
using Microsoft.EntityFrameworkCore;

namespace EZ_Site.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){}

        public DbSet<User> Users { get; set; }
    }
}

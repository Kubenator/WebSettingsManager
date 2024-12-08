using Microsoft.EntityFrameworkCore;
using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Models
{
    public class WebSettingsManagerDbContext : DbContext, IWebSettingsManagerDbContext
    {
        public DbSet<User> Users { get; set; }

        public DbContext Instance => this;

        public WebSettingsManagerDbContext(DbContextOptions<WebSettingsManagerDbContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        public void Load()
        {
        }
    }
}

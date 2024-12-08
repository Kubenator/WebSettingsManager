using Microsoft.EntityFrameworkCore;
using WebSettingsManager.Models;

namespace WebSettingsManager.Interfaces
{
    public interface IWebSettingsManagerDbContext : IDisposable
    {
        DbContext Instance { get; }
        DbSet<User> Users { get; }
        public void Load();
    }
}

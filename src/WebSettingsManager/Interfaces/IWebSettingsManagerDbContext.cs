using Microsoft.EntityFrameworkCore;
using WebSettingsManager.Models;

namespace WebSettingsManager.Interfaces
{
    public interface IWebSettingsManagerDbContext : IDisposable
    {
        DbContext Instance { get; }
        public DbSet<User> Users { get; }
        public DbSet<UserTextConfiguration> UserTextConfigurations { get; }
        //public DbSet<TextConfigurationState> TextConfigurationStates { get; }
        //public DbSet<TextConfigurationOptions> TextConfigurationOptions { get; }
    }
}

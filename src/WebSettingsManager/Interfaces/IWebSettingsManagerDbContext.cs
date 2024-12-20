using Microsoft.EntityFrameworkCore;
using WebSettingsManager.Models;

namespace WebSettingsManager.Interfaces
{
    public interface IWebSettingsManagerDbContext : IDisposable
    {
        DbContext Instance { get; }
        public DbSet<User_Db> Users { get; }
        public DbSet<UserTextConfiguration_Db> UserTextConfigurations { get; }
        public DbSet<TextConfigurationActualState_Db> TextConfigurationActualStates { get; }
        public DbSet<TextConfigurationSavedState_Db> TextConfigurationSavedStates { get; }
        public DbSet<TextConfigurationOptions_Actual_Db> TextConfigurationOptionsForActualStates { get; }
        public DbSet<TextConfigurationOptions_Saved_Db> TextConfigurationOptionsForSavedStates { get; }
    }
}

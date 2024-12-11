using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Models
{
    public class WebSettingsManagerDbContext : DbContext, IWebSettingsManagerDbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserTextConfiguration> UserTextConfigurations { get; set; } = null!;
        public DbSet<TextConfigurationActualState> TextConfigurationActualStates { get; set; } = null!;
        public DbSet<TextConfigurationSavedState> TextConfigurationSavedStates { get; set; } = null!;
        //public DbSet<TextConfigurationOptions> TextConfigurationOptions { get; set; } = null!;

        public DbContext Instance => this;

        public WebSettingsManagerDbContext(DbContextOptions<WebSettingsManagerDbContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            Console.WriteLine("DBContext created!");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username);

            modelBuilder.Entity<UserTextConfiguration>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(textConfiguration => textConfiguration.UserUsername)
                .HasPrincipalKey(user => user.Username);
        }
    }

    public class User
    {
        [Key]
        public UInt64 Id { get; set; }

        /// <summary>
        /// Уникальное имя пользователя
        /// </summary>
        public string Username { get; set; } = null!; //В FluentAPI настроена уникальность по Username

        /// <summary>
        /// Имя из инициалов пользователя
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Конфигурации пользователя
        /// </summary>
        public List<UserTextConfiguration> TextConfigurations { get; set; } = new();
    }

    [Index(nameof(UserUsername), nameof(ConfigurationName), IsUnique = true)] //Название конфигурации должно быть уникально в пределах одного пользователя
    public class UserTextConfiguration
    {
        [Key]
        public UInt64 Id { get; set; }
        public User User { get; set; } = null!;
        public string UserUsername { get; set; } = null!;

        public string ConfigurationName { get; set; } = null!;
        public DateTime CreationDateTime { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public DateTime SaveDateTime { get; set; }
        
        public TextConfigurationActualState TextConfigurationActualState { get; set; } = null!;
        public List<TextConfigurationSavedState> TextConfigurationSavedStates { get; set; } = new();
    }
    public class TextConfigurationActualState
    {
        [Key]
        public UInt64 Id { get; set; }
        public string FontName { get; set; } = null!;
        public string FontSize { get; set; } = null!;

        public UInt64 TextConfigurationId { get; set; }
        public UserTextConfiguration TextConfiguration { get; set; } = null!;
    }
    public class TextConfigurationSavedState
    {
        [Key]
        public UInt64 Id { get; set; }
        public DateTime SaveDateTime { get; set; }
        public string FontName { get; set; } = null!;
        public string FontSize { get; set; } = null!;

        public UserTextConfiguration TextConfiguration { get; set; } = null!;
    }
}

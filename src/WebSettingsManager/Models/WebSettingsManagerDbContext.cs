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
            modelBuilder.Entity<UserTextConfiguration>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(textConfiguration => textConfiguration.UserUsername)
                .HasPrincipalKey(user => user.Username);
        }
    }

    [Index(nameof(Username), IsUnique = true)] //Никнейм пользователя должен быть уникален
    public class User
    {
        [Key]
        public UInt64 Id { get; set; }

        /// <summary>
        /// Уникальное имя пользователя<br/>
        /// Альтернативный ключ
        /// </summary>
        public string Username { get; set; } = null!;

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

        /// <summary>
        /// Никнейм пользователя<br/>
        /// Соотносится со свойством ведущей сущности <see cref="User.Username"/>
        /// </summary>
        public string UserUsername { get; set; } = null!;

        /// <summary>
        /// Название конфигурации
        /// </summary>
        public string ConfigurationName { get; set; } = null!;     

        /// <summary>
        /// Текущее состояние конфигурации
        /// </summary>
        public TextConfigurationActualState TextConfigurationActualState { get; set; } = null!;

        /// <summary>
        /// Множество сохранненых версий конфигурации
        /// </summary>
        public List<TextConfigurationSavedState> TextConfigurationSavedStates { get; set; } = new();
    }
    public class TextConfigurationActualState
    {
        [Key]
        public UInt64 Id { get; set; }

        /// <summary>
        /// Момент создания конфигурации
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// Момент последней модификации конфигурации
        /// </summary>
        public DateTime ModificationDateTime { get; set; }

        ///// <summary>
        ///// Актуальные параметры кофигурации
        ///// </summary>
        //public TextConfigurationOptions TextConfigurationOptions { get; set; } = null!;
        public string FontName { get; set; } = null!;
        public int FontSize { get; set; }

        ///// <summary>
        ///// Сохраненное состояние
        ///// </summary>
        //public TextConfigurationSavedState? TextConfigurationSavedState { get; set; } = null!;

        public UInt64 UserTextConfigurationId { get; set; }
        public UserTextConfiguration UserTextConfiguration { get; set; } = null!;
    }
    public class TextConfigurationSavedState
    {
        [Key]
        public UInt64 Id { get; set; }
        public DateTime SaveDateTime { get; set; }
        //TextConfigurationOptions TextConfigurationOptions { get; set; } = null!;
        public string FontName { get; set; } = null!;
        public int FontSize { get; set; }

        public UserTextConfiguration UserTextConfiguration { get; set; } = null!;
    }
    //public class TextConfigurationOptions
    //{
    //    [Key]
    //    public UInt64 Id { get; set; }

    //    public string FontName { get; set; } = null!;
    //    public int FontSize { get; set; }

    //}
}

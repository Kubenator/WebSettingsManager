using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using WebSettingsManager.Interfaces;

#pragma warning disable CS1591
namespace WebSettingsManager.Models
{
    public class WebSettingsManagerDbContext : DbContext, IWebSettingsManagerDbContext
    {
        public DbSet<User_Db> Users { get; set; } = null!;
        public DbSet<UserTextConfiguration_Db> UserTextConfigurations { get; set; } = null!;
        public DbSet<TextConfigurationActualState_Db> TextConfigurationActualStates { get; set; } = null!;
        public DbSet<TextConfigurationSavedState_Db> TextConfigurationSavedStates { get; set; } = null!;
        public DbSet<TextConfigurationOptions_Actual_Db> TextConfigurationOptionsForActualStates { get; set; } = null!;
        public DbSet<TextConfigurationOptions_Saved_Db> TextConfigurationOptionsForSavedStates { get; set; } = null!;

        public DbContext Instance => this;

        public WebSettingsManagerDbContext(DbContextOptions<WebSettingsManagerDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            Console.WriteLine("DBContext created: " + Database.GetConnectionString());
        }
    }

    [Index(nameof(Username), IsUnique = true)] //Никнейм пользователя должен быть уникален
    public class User_Db : IUser
    {
        [Key]
        public UInt64 Id { get; set; }

        /// <summary>
        /// Уникальное имя пользователя<br/>
        /// Альтернативный ключ
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// Имя из ФИО пользователя
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Конфигурации пользователя
        /// </summary>
        public List<UserTextConfiguration_Db> TextConfigurations { get; set; } = new();
    }

    [Index(nameof(UserId), nameof(ConfigurationName), IsUnique = true)] //Название конфигурации должно быть уникально в пределах одного пользователя
    public class UserTextConfiguration_Db : ITextConfiguration
    {
        public UserTextConfiguration_Db() { }
        public UserTextConfiguration_Db(UInt64 userId, ITextConfiguration textConfiguration)
        {
            UserId = userId;
            ConfigurationName = textConfiguration.ConfigurationName;

            TextConfigurationActualState = new TextConfigurationActualState_Db(textConfiguration.TextConfigurationOptions, this);
        }
        [Key]
        public UInt64 Id { get; set; }


        public UInt64 UserId { get; set; }
        public User_Db User { get; set; } = null!;

        /// <summary>
        /// Название конфигурации
        /// </summary>
        public string ConfigurationName { get; set; } = null!;

        /// <summary>
        /// Текущее состояние конфигурации
        /// </summary>
        [Required]
        public TextConfigurationActualState_Db TextConfigurationActualState { get; set; } = null!;

        /// <summary>
        /// Множество сохранненых версий конфигурации
        /// </summary>
        public List<TextConfigurationSavedState_Db> TextConfigurationSavedStates { get; set; } = new();

        [NotMapped]
        ITextConfigurationOptions ITextConfiguration.TextConfigurationOptions => TextConfigurationActualState.TextConfigurationOptions;
    }
    public class TextConfigurationActualState_Db
    {
        public TextConfigurationActualState_Db() { }
        public TextConfigurationActualState_Db(ITextConfigurationOptions textConfigurationOptions, UserTextConfiguration_Db userTextConfiguration)
        {
            var dt = DateTime.UtcNow;
            CreationDateTime = dt;
            ModificationDateTime = dt;
            UserTextConfigurationId = userTextConfiguration.Id;
            TextConfigurationOptions = new TextConfigurationOptions_Actual_Db(this, textConfigurationOptions);            
        }

        [Key]
        public UInt64 Id { get; set; }

        public UInt64 UserTextConfigurationId { get; set; }
        public UserTextConfiguration_Db UserTextConfiguration { get; set; } = null!;
        

        /// <summary>
        /// Момент создания конфигурации
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// Момент последней модификации конфигурации
        /// </summary>
        public DateTime ModificationDateTime { get; set; }

        /// <summary>
        /// Опции конфигурации текста
        /// </summary>
        [Required]
        public TextConfigurationOptions_Actual_Db TextConfigurationOptions { get; set; } = null!;

        /// <summary>
        /// Сохраненное состояние
        /// </summary>        
        public TextConfigurationSavedState_Db? TextConfigurationSavedState { get; set; } = null!;
        
    }
    public class TextConfigurationSavedState_Db
    {
        public TextConfigurationSavedState_Db() { }
        public TextConfigurationSavedState_Db(TextConfigurationActualState_Db textConfigurationActualState, DateTime saveDateTime)
        {
            SaveDateTime = saveDateTime;
            UserTextConfigurationId = textConfigurationActualState.UserTextConfigurationId;
            TextConfigurationOptions = new TextConfigurationOptions_Saved_Db(textConfigurationActualState);
        }
        [Key]
        public UInt64 Id { get; set; }

        public UInt64 UserTextConfigurationId { get; set; }
        public UserTextConfiguration_Db UserTextConfiguration { get; set; } = null!;


        /// <summary>
        /// Врямя сохранения состояния
        /// </summary>
        public DateTime SaveDateTime { get; set; }

        /// <summary>
        /// Опции конфигурации текста
        /// </summary>
        [Required]
        public TextConfigurationOptions_Saved_Db TextConfigurationOptions { get; set; } = null!;
    }
    public class TextConfigurationOptions_Actual_Db : ITextConfigurationOptions
    {
        public TextConfigurationOptions_Actual_Db() { }
        public TextConfigurationOptions_Actual_Db(TextConfigurationActualState_Db textConfigurationActualState, ITextConfigurationOptions textConfigurationOptions)
        { 
            TextConfigurationActualStateId = textConfigurationActualState.Id;
            FontName = textConfigurationOptions.FontName;
            FontSize = textConfigurationOptions.FontSize;
        }
        [Key]
        public UInt64 Id { get; set; }

        public UInt64 TextConfigurationActualStateId { get; set; }
        public TextConfigurationActualState_Db TextConfigurationActualState { get; set; } = null!;

        public string FontName { get; set; } = null!;
        public int FontSize { get; set; }
    }
    public class TextConfigurationOptions_Saved_Db : ITextConfigurationOptions
    {
        public TextConfigurationOptions_Saved_Db() { }
        public TextConfigurationOptions_Saved_Db(TextConfigurationActualState_Db textConfigurationActualState/*, ITextConfigurationOptions textConfigurationOptions*/)
        {
            TextConfigurationSavedStateId = textConfigurationActualState.Id;
            FontName = textConfigurationActualState.TextConfigurationOptions.FontName;
            FontSize = textConfigurationActualState.TextConfigurationOptions.FontSize;
        }
        [Key]
        public UInt64 Id { get; set; }

        public UInt64 TextConfigurationSavedStateId { get; set; }
        public TextConfigurationSavedState_Db TextConfigurationSavedState { get; set; } = null!;

        public string FontName { get; set; } = null!;
        public int FontSize { get; set; }
    }
}

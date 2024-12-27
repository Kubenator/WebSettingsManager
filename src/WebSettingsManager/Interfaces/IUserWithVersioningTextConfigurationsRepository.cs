using WebSettingsManager.Models;
using static WebSettingsManager.Controllers.UsersController;

namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс пользователя с множеством версионируемых конфигураций
    /// </summary>
    public interface IUserWithVersioningTextConfigurationsRepository
    {
        #region ManageUseres
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyList<User_Db>> GetAllUsers();

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<User_Db> AddUser(User_RequestData user);

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<User_Db> RemoveUser(UInt64 userId);

        /// <summary>
        /// Получить пользователя по id
        /// </summary>
        /// <returns></returns>
        public Task<User_Db> GetUser(UInt64 userId);

        /// <summary>
        /// Обновить пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<User_Db> UpdateUser(UInt64 userId, User_RequestData user);
        #endregion ManageUseres


        #region ManageUserConfigurations
        /// <summary>
        /// Получить список конфигураций, связанных с пользователем
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="configurationFilterOptions"></param>
        /// <returns></returns>
        public Task<IReadOnlyList<UserTextConfiguration_Db>> GetUserConfigurations(UInt64 userId, ConfigurationFilterOptions configurationFilterOptions);

        /// <summary>
        /// Получить конкретную конфигурацию для конкретного пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public Task<UserTextConfiguration_Db> GetUserConfiguration(UInt64 userId, UInt64 confId);

        /// <summary>
        /// Создать новую конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="configurationData"></param>
        /// <returns></returns>
        public Task<UserTextConfiguration_Db> AddUserConfiguration(UInt64 userId, UserTextConfiguration_RequestData configurationData);

        /// <summary>
        /// Обновить конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="configurationData"></param>
        public Task<UserTextConfiguration_Db> UpdateUserConfiguration(UInt64 userId, UInt64 confId, UserTextConfiguration_RequestData configurationData);

        /// <summary>
        /// Удалить конфигурацию у пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public Task<UserTextConfiguration_Db> RemoveUserConfiguration(UInt64 userId, UInt64 confId);

        /// <summary>
        /// Восстановить последнее сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public Task<UserTextConfiguration_Db> RestoreUserConfigurationLastSavedState(UInt64 userId, UInt64 confId);

        /// <summary>
        /// Восстановить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        public Task<UserTextConfiguration_Db> RestoreUserConfigurationSavedState(UInt64 userId, UInt64 confId, UInt64 savedStateId);

        /// <summary>
        /// Сохранить состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public Task<TextConfigurationSavedState_Db> SaveUserConfigurationState(UInt64 userId, UInt64 confId);
        #endregion ManageUserConfigurations


        #region ManageUserConfigurationSavedStates
        /// <summary>
        /// Получить сохраненные состояния конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public Task<IReadOnlyList<TextConfigurationSavedState_Db>> GetUserConfigurationSavedStates(UInt64 userId, UInt64 confId);

        /// <summary>
        /// Получить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        public Task<TextConfigurationSavedState_Db> GetUserConfigurationSavedState(UInt64 userId, UInt64 confId, UInt64 savedStateId);

        /// <summary>
        /// Удалить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        public Task<TextConfigurationSavedState_Db> RemoveUserConfigurationSavedState(UInt64 userId, UInt64 confId, UInt64 savedStateId);
        #endregion ManageUserConfigurationSavedStates

        /// <summary>
        /// Событие обновлления конфигурации текста у пользователя
        /// </summary>
        public event EventHandler<UserTextConfigurationUpdatedEventArgs>? UserTextConfigurationUpdated;
    }

    /// <summary>
    /// Данные события обновления конфигурации текста у пользователя
    /// </summary>
    public class UserTextConfigurationUpdatedEventArgs : EventArgs
    {
        public UserTextConfigurationUpdatedEventArgs(UInt64 userId, UInt64 configurationId, string configurationName, string fontName, int fontSize)
        { 
            this.UserId = userId;
            this.ConfigurationId = configurationId;
            this.ConfigurationName = configurationName;
            this.FontName = fontName;
            this.FontSize = fontSize;
        }
        /// <summary>
        /// Id пользователя
        /// </summary>
        public UInt64 UserId { get; }

        /// <summary>
        /// Id конфигурации
        /// </summary>
        public UInt64 ConfigurationId { get; }

        /// <summary>
        /// Название конфигурации
        /// </summary>
        public string ConfigurationName { get; }

        /// <summary>
        /// Название шрифта
        /// </summary>
        public string FontName { get; }

        /// <summary>
        /// Размер шрифта
        /// </summary>
        public int FontSize { get; }
    }

}

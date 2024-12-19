using Microsoft.AspNetCore.Mvc;
using WebSettingsManager.Controllers;
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
        public Task<User_Db> AddUser(UserData user);

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
        public Task<User_Db> UpdateUser(UInt64 userId, UserData user);
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
        public Task<UserTextConfiguration_Db> AddUserConfiguration(UInt64 userId, TextConfigurationData configurationData);

        /// <summary>
        /// Обновить конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="configurationData"></param>
        public Task<UserTextConfiguration_Db> UpdateUserConfiguration(UInt64 userId, UInt64 confId, TextConfigurationData configurationData);

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
    }
}

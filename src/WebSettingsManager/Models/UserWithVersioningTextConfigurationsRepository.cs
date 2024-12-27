using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebSettingsManager.Interfaces;
using static WebSettingsManager.Controllers.UsersController;

namespace WebSettingsManager.Models
{
    public class UserWithVersioningTextConfigurationsRepository : IUserWithVersioningTextConfigurationsRepository
    {
        private IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// <inheritdoc cref="IUserWithVersioningTextConfigurationsRepository.UserTextConfigurationUpdated"/>
        /// </summary>
        public event EventHandler<UserTextConfigurationUpdatedEventArgs>? UserTextConfigurationUpdated;

        public UserWithVersioningTextConfigurationsRepository(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        #region ManageUseres
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<User_Db>> GetAllUsers()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var users = await dbContext.Users
                .ToListAsync().ConfigureAwait(false);

            return users;
        }

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User_Db> AddUser(User_RequestData user)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var userToAdd = new User_Db() { Username = user.Username, Name = user.Name };
            var addedUser = dbContext.Users.Add(userToAdd);
            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            return addedUser.Entity;
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User_Db> RemoveUser(UInt64 userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (existingUser == null)
                throw new UserNotFoundException(userId);
            dbContext.Users.Remove(existingUser);
            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            return existingUser;
        }

        /// <summary>
        /// Получить пользователя по id
        /// </summary>
        /// <returns></returns>
        public async Task<User_Db> GetUser(UInt64 userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            if (existingUser == null)
                throw new UserNotFoundException(userId);

            return existingUser;
        }

        /// <summary>
        /// Обновить пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User_Db> UpdateUser(UInt64 userId, User_RequestData user)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
            if (existingUser == null)
                throw new UserNotFoundException(userId);
            existingUser.Username = user.Username;
            existingUser.Name = user.Name;
            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            return existingUser;
        }
        #endregion ManageUseres


        #region ManageUserConfigurations
        /// <summary>
        /// Получить список конфигураций, связанных с пользователем
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="configurationFilterOptions"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<UserTextConfiguration_Db>> GetUserConfigurations(UInt64 userId, ConfigurationFilterOptions configurationFilterOptions)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var iQueryableConfigurations = dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationOptions)
                .Where(x => x.UserId == userId);
            ConfigureQuery(ref iQueryableConfigurations, configurationFilterOptions);
            var configurations = await iQueryableConfigurations
                .ToListAsync().ConfigureAwait(false);

            return configurations;

            void ConfigureQuery(ref IQueryable<UserTextConfiguration_Db> items, ConfigurationFilterOptions filterOptions)
            {
                if (filterOptions.CreationDateTimeOlderThanOrEqual != null)
                    items = items.Where(c => c.TextConfigurationActualState.CreationDateTime >= filterOptions.CreationDateTimeOlderThanOrEqual);
                if (filterOptions.CreationDateTimeEarlierThanOrEqual != null)
                    items = items.Where(c => c.TextConfigurationActualState.CreationDateTime <= filterOptions.CreationDateTimeEarlierThanOrEqual);
                if (filterOptions.ConfigurationNameTemplate != null)
                    items = items.Where(c => Regex.IsMatch(c.ConfigurationName, filterOptions.ConfigurationNameTemplate.Replace("*", ".*")));
            }
        }

        /// <summary>
        /// Получить конкретную конфигурацию для конкретного пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public async Task<UserTextConfiguration_Db> GetUserConfiguration(UInt64 userId, UInt64 confId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var configuration = await dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(c => c.TextConfigurationOptions)
                .Include(ss => ss.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationSavedState)
                        .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (configuration == null)
                throw new UserConfigurationNotFoundException(userId, confId);

            return configuration;
        }

        /// <summary>
        /// Создать новую конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="configurationData"></param>
        /// <returns></returns>
        public async Task<UserTextConfiguration_Db> AddUserConfiguration(UInt64 userId, UserTextConfiguration_RequestData configurationData)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
            if (existingUser == null)
                throw new UserNotFoundException(userId);
            var newConfiguration = new UserTextConfiguration_Db(userId, configurationData);
            var addedConfiguration = dbContext.UserTextConfigurations.Add(newConfiguration);
            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            return addedConfiguration.Entity;
        }

        /// <summary>
        /// Обновить конфигурацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="configurationData"></param>
        public async Task<UserTextConfiguration_Db> UpdateUserConfiguration(UInt64 userId, UInt64 confId, UserTextConfiguration_RequestData configurationData)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
            if (existingUser == null)
                throw new UserNotFoundException(userId);
            var existingConfiguration = await dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(c => c.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            existingConfiguration.ConfigurationName = configurationData.ConfigurationName;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize = configurationData.TextConfigurationOptions.FontSize;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName = configurationData.TextConfigurationOptions.FontName;
            var isAnyChangesExists = dbContext.Instance.ChangeTracker.HasChanges();
            if (isAnyChangesExists)
                existingConfiguration.TextConfigurationActualState.ModificationDateTime = DateTime.Now;

            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            if (isAnyChangesExists)
                UserTextConfigurationUpdated?.Invoke(this, new UserTextConfigurationUpdatedEventArgs(
                    userId,
                    confId,
                    existingConfiguration.ConfigurationName,
                    existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName,
                    existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize
                    ));

            

            return existingConfiguration;
        }

        /// <summary>
        /// Удалить конфигурацию у пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public async Task<UserTextConfiguration_Db> RemoveUserConfiguration(UInt64 userId, UInt64 confId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingConfiguration = await dbContext.UserTextConfigurations
               .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            var removedConfiguration = dbContext.UserTextConfigurations
                .Remove(existingConfiguration);
            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            return removedConfiguration.Entity;
        }

        /// <summary>
        /// Восстановить последнее сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public async Task<UserTextConfiguration_Db> RestoreUserConfigurationLastSavedState(UInt64 userId, UInt64 confId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingConfiguration = await dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationSavedState)
                        .ThenInclude(ss => ss.TextConfigurationOptions)
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            if (existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState == null)
                throw new UserConfigurationNoLastSavedStateException(confId);

            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize = existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState.TextConfigurationOptions.FontSize;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName = existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState.TextConfigurationOptions.FontName;
            var isAnyChangesExists = dbContext.Instance.ChangeTracker.HasChanges();
            if (isAnyChangesExists)
                existingConfiguration.TextConfigurationActualState.ModificationDateTime = DateTime.Now;

            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            if (isAnyChangesExists)
                UserTextConfigurationUpdated?.Invoke(this, new UserTextConfigurationUpdatedEventArgs(
                    userId,
                    confId,
                    existingConfiguration.ConfigurationName,
                    existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName,
                    existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize
                    ));

            return existingConfiguration;
        }

        /// <summary>
        /// Восстановить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        public async Task<UserTextConfiguration_Db> RestoreUserConfigurationSavedState(UInt64 userId, UInt64 confId, UInt64 savedStateId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingConfiguration = await dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationSavedState)
                        .ThenInclude(ss => ss.TextConfigurationOptions)
                .Include(c => c.TextConfigurationActualState)
                    .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            var savedStateToRestore = await dbContext.TextConfigurationSavedStates
                .Include(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(ss => ss.Id == savedStateId && ss.UserTextConfigurationId == confId);
            if (savedStateToRestore == null)
                throw new UserConfigurationSavedStateNotFoundException(confId, savedStateId);
            existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState = savedStateToRestore;
            existingConfiguration.TextConfigurationActualState.ModificationDateTime = DateTime.Now;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize = savedStateToRestore.TextConfigurationOptions.FontSize;
            existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName = savedStateToRestore.TextConfigurationOptions.FontName;

            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            UserTextConfigurationUpdated?.Invoke(this, new UserTextConfigurationUpdatedEventArgs(
                    userId,
                    confId,
                    existingConfiguration.ConfigurationName,
                    existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontName,
                    existingConfiguration.TextConfigurationActualState.TextConfigurationOptions.FontSize
                    ));

            return existingConfiguration;
        }

        /// <summary>
        /// Сохранить состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public async Task<TextConfigurationSavedState_Db> SaveUserConfigurationState(UInt64 userId, UInt64 confId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingConfiguration = await dbContext.UserTextConfigurations
                .Include(c => c.TextConfigurationActualState)
                .ThenInclude(s => s.TextConfigurationOptions)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            var saveDateTime = DateTime.Now;
            var newSavedState = new TextConfigurationSavedState_Db(existingConfiguration.TextConfigurationActualState, saveDateTime);
            var addedSavedState = dbContext.TextConfigurationSavedStates.Add(newSavedState);
            existingConfiguration.TextConfigurationActualState.TextConfigurationSavedState = addedSavedState.Entity;
            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            return addedSavedState.Entity;
        }
        #endregion ManageUserConfigurations


        #region ManageUserConfigurationSavedStates
        /// <summary>
        /// Получить сохраненные состояния конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<TextConfigurationSavedState_Db>> GetUserConfigurationSavedStates(UInt64 userId, UInt64 confId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingConfiguration = await dbContext.UserTextConfigurations
               .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            var configurationSavedStates = await dbContext.TextConfigurationSavedStates
                .Where(ss => ss.UserTextConfigurationId == confId)
                .Include(ss => ss.TextConfigurationOptions)
                .ToListAsync().ConfigureAwait(false);

            return configurationSavedStates;
        }

        /// <summary>
        /// Получить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        public async Task<TextConfigurationSavedState_Db> GetUserConfigurationSavedState(UInt64 userId, UInt64 confId, UInt64 savedStateId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingConfiguration = await dbContext.UserTextConfigurations
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            var savedState = await dbContext.TextConfigurationSavedStates
                .FirstOrDefaultAsync(ss => ss.Id == savedStateId && ss.UserTextConfigurationId == confId).ConfigureAwait(false);
            if (savedState == null)
                throw new UserConfigurationSavedStateNotFoundException(confId, savedStateId);

            return savedState;
        }

        /// <summary>
        /// Удалить конкретное сохраненное состояние конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <param name="savedStateId"></param>
        /// <returns></returns>
        public async Task<TextConfigurationSavedState_Db> RemoveUserConfigurationSavedState(UInt64 userId, UInt64 confId, UInt64 savedStateId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IWebSettingsManagerDbContext>();

            var existingConfiguration = await dbContext.UserTextConfigurations
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == confId).ConfigureAwait(false);
            if (existingConfiguration == null)
                throw new UserConfigurationNotFoundException(userId, confId);
            var savedState = await dbContext.TextConfigurationSavedStates
                .FirstOrDefaultAsync(ss => ss.Id == savedStateId && ss.UserTextConfigurationId == confId);
            if (savedState == null)
                throw new UserConfigurationSavedStateNotFoundException(confId, savedStateId);
            var removedState = dbContext.TextConfigurationSavedStates
                .Remove(savedState);
            await dbContext.Instance.SaveChangesAsync().ConfigureAwait(false);

            return removedState.Entity;
        }
        #endregion ManageUserConfigurationSavedStates


        /// <summary>
        /// Ошибка при попытке обнаружить указанный элемент репозитория
        /// </summary>
        public class UserRepositoryItemNotFoundExceprion : Exception
        {
            public UserRepositoryItemNotFoundExceprion(string message) : base(message) { }
        }

        /// <summary>
        /// Ошибка при невозможности найти указанного пользователя
        /// </summary>
        public class UserNotFoundException : UserRepositoryItemNotFoundExceprion
        {
            public UserNotFoundException(UInt64 userId) : base($"Не удалось обнаружить пользователя по id: '{userId}'") { }
        }

        /// <summary>
        /// Ошибка при невозможности найти указанную конфигурацию пользователя
        /// </summary>
        public class UserConfigurationNotFoundException : UserRepositoryItemNotFoundExceprion
        {
            public UserConfigurationNotFoundException(UInt64 confId) : base($"Не удалось обнаружить конфигурацию пользователя по id конфигурации: '{confId}'") { }
            public UserConfigurationNotFoundException(UInt64 userId, UInt64 confId) : base($"Не удалось обнаружить конфигурацию пользователя по id пользователя: '{userId}' и id конфигурации: '{confId}'") { }
        }

        /// <summary>
        /// Ошибка при отсутствии запрошенного последнего сохраненного состояни конфигурации пользователя
        /// </summary>
        public class UserConfigurationNoLastSavedStateException : UserRepositoryItemNotFoundExceprion
        {
            public UserConfigurationNoLastSavedStateException(UInt64 confId) : base($"Не удалось обнаружить последнее сохраненное состояние для конфигурации с id: '{confId}'") { }
        }

        /// <summary>
        /// Ошибка при невозможности найти запрошенное сохраненное состояние конфигурации пользователя
        /// </summary>
        public class UserConfigurationSavedStateNotFoundException : UserRepositoryItemNotFoundExceprion
        {
            public UserConfigurationSavedStateNotFoundException(UInt64 confId, UInt64 savedStateId) : base($"Не удалось обнаружить сохраненное состояние с id '{savedStateId}' для конфигурации с id: '{confId}'") { }
        }
    }
}


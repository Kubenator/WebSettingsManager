using WebSettingsManager.Models;

namespace WebSettingsManager.Interfaces
{
    public interface IUserWithVersioningTextConfigurationsRepository
    {
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyList<IUserWithVersioningTextConfigurations>> GetAllUsers();

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Task<IUserWithVersioningTextConfigurations> AddUser(string username);

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Task<IUserWithVersioningTextConfigurations> RemoveUser(string username);

        /// <summary>
        /// Добавить существующую конфигурацию пользователю
        /// </summary>
        /// <param name="username"></param>
        /// <param name="versioningTextConfiguration"></param>
        /// <returns></returns>
        public Task<IUserWithVersioningTextConfigurations> AddConfigurationToUser(string username, IVersioningTextConfiguration versioningTextConfiguration);

        /// <summary>
        /// Загрузить пользователей
        /// </summary>
        /// <returns></returns>
        public Task LoadUsers();

        /// <summary>
        /// Сохранить пользователей
        /// </summary>
        /// <returns></returns>
        public Task SaveUsers();

    }
}

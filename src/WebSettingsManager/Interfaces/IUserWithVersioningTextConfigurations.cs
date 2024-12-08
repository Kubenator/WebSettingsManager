using System.Collections.Immutable;

namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс пользователя с версионируемыми конфигурациями текста
    /// </summary>
    public interface IUserWithVersioningTextConfigurations : IUser
    {
        /// <summary>
        /// Конфигурации пользователя
        /// </summary>
        public IReadOnlyDictionary<string, IVersioningTextConfiguration> TextConfigurationsByNameDict { get; }

        /// <summary>
        /// Создать новую конфигурацию
        /// </summary>
        /// <param name="configurationName"></param>
        /// <returns></returns>
        IVersioningTextConfiguration CreateVersioningConfiguration(string configurationName);

        /// <summary>
        /// Добавить существующую конфигурацию<br/>
        /// Создается новая конфигурация на основе входящей
        /// </summary>
        /// <param name="versioningTextConfiguration"></param>
        /// <returns></returns>
        public IVersioningTextConfiguration AddVersioningConfiguration(IVersioningTextConfiguration versioningTextConfiguration);

        /// <summary>
        /// Удалить конфигурацию
        /// </summary>
        /// <param name="versioningTextConfigurationName"></param>
        /// <returns></returns>
        public IVersioningTextConfiguration RemoveVersioningConfiguration(string versioningTextConfigurationName);

        /// <summary>
        /// Получить конфигурацию
        /// </summary>
        /// <param name="versioningTextConfigurationName"></param>
        /// <returns></returns>
        public IVersioningTextConfiguration GetVersioningConfiguration(string versioningTextConfigurationName);
    }
}

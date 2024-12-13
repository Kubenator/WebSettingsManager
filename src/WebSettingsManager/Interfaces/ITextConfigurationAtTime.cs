namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс конфигурации текста
    /// </summary>
    public interface ITextConfigurationAtTime : ITextConfiguration
    {
        /// <summary>
        /// Время создание конфигурации
        /// </summary>
        public DateTime CreationDateTime { get; }

        /// <summary>
        /// Время модификации конфигурации
        /// </summary>
        public DateTime ModificationDateTime { get; }

        /// <summary>
        /// Время сохранения версии конфигурации
        /// </summary>
        public DateTime SaveDateTime { get; }       
    }
}

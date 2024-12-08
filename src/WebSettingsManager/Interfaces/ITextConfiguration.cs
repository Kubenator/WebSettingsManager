namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс конфигурации текста
    /// </summary>
    public interface ITextConfiguration
    {
        /// <summary>
        /// Название конфигурации
        /// </summary>
        public string Name { get; }

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

        /// <summary>
        /// Параметры конфигурации текста
        /// </summary>
        public ITextConfigurationData TextConfigurationData { get; }
       
    }
}

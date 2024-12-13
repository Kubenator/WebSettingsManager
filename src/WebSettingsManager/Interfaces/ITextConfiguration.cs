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
        public string ConfigurationName { get; }

        /// <summary>
        /// Параметры конфигурации текста
        /// </summary>
        public ITextConfigurationOptions TextConfigurationOptions { get; }
       
    }
}

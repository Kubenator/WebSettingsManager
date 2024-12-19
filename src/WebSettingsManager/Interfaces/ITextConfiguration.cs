namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс конфигурации текста
    /// </summary>
    public interface ITextConfiguration
    {
        /// <summary>
        /// Идентификатор конфигурации
        /// </summary>
        public UInt64 Id { get; set; }

        /// <summary>
        /// Название конфигурации
        /// </summary>
        public string ConfigurationName { get; }

        ///// <summary>
        ///// Актуальное состояние конфигурации
        ///// </summary>
        //ITextConfigurationActualState TextConfigurationActualState { get; }
    }
}

namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс параметров конфигурации текста
    /// </summary>
    public interface ITextConfigurationOptions
    {
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

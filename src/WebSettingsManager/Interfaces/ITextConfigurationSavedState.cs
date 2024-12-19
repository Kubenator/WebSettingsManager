namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Сохраненное состояние конфигурации пользвателя на момент времени
    /// </summary>
    public interface ITextConfigurationSavedState
    {
        /// <summary>
        /// Идентификатор сохраненного состояния конфигурации
        /// </summary>
        public UInt64 Id { get; }

        /// <summary>
        /// Момент сохранения конфигурации
        /// </summary>
        public DateTime SaveDateTime { get; }

        /// <summary>
        /// Опции конфигурации
        /// </summary>
        public ITextConfigurationOptions TextConfigurationOptions { get; }
    }
}

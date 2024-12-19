namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Актуальное состояние конфигурации пользвателя на момент времени
    /// </summary>
    public interface ITextConfigurationActualState
    {
        /// <summary>
        /// Идентификатор сохраненного состояния конфигурации
        /// </summary>
        public UInt64 Id { get; }

        /// <summary>
        /// Момент создания конфигурации
        /// </summary>
        public DateTime CreationDateTime { get; }

        /// <summary>
        /// Момент последней модификации конфигурации
        /// </summary>
        public DateTime ModificationDateTime { get; }

        ///// <summary>
        ///// Опции конфигурации
        ///// </summary>
        //public ITextConfigurationOptions TextConfigurationOptions { get; }
    }
}

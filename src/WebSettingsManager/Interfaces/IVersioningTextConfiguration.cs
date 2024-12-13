namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс версионируемой конфигурации текста
    /// </summary>
    public interface IVersioningTextConfiguration : ITextConfiguration, IVersioningObject<ITextConfigurationOptions>
    {

    }
}

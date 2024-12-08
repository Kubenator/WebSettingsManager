using Microsoft.AspNetCore.Components.Web;

namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс объекта управления версиями
    /// </summary>
    public interface IVersioningObject<T>
    {
        /// <summary>
        /// Словарь состояния конфигурации на момент времени
        /// </summary>
        IReadOnlyDictionary<DateTime, T> VersionsByPointInTimeDict { get; }

        /// <summary>
        /// Существующие версии конфигурации
        /// </summary>
        public ICollection<DateTime> VersionsTimes { get; }

        /// <summary>
        /// Обновить текущее состояние без сохранения
        /// </summary>
        /// <param name="newState"></param>
        /// <returns>произведено ли фактическое данных</returns>
        public bool UpdateCurrentState(T newState);

        /// <summary>
        /// Сохранить состояние конфигурации
        /// </summary>
        /// <returns>присутствуют ли изменения относительно прошлого сохранения</returns>
        public bool SaveState();

        /// <summary>
        /// Восстановить последнюю версию конфигурации
        /// </summary>
        /// <returns>происходит ли изменение данных при восстановлении</returns>
        public bool RestoreLastVersion();

        /// <summary>
        /// Восстановить версию конфигурации на момент времени
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>происходит ли изменение данных при восстановлении</returns>
        public bool RestoreVersion(DateTime dateTime);

        /// <summary>
        /// Получить версию на момент времени
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public T GetVersion(DateTime dateTime);

        /// <summary>
        /// Получить последнюю сохраненную версию
        /// </summary>
        /// <returns></returns>
        public T GetLastVersion();

        /// <summary>
        /// Удалить версию на момент времени
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>удаленный элемент</returns>
        public T RemoveVersion(DateTime dateTime);

        /// <summary>
        /// Удалить версию на момент времени
        /// </summary>
        /// <returns>удаленный элемент</returns>
        public T RemoveLastVersion();
    }
}

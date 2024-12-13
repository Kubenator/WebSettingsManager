using System.Collections.Immutable;

namespace WebSettingsManager.Interfaces
{
    /// <summary>
    /// Интерфейс пользователя
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Имя из ФИО пользователя
        /// </summary>
        public string Name { get; }
    }
}

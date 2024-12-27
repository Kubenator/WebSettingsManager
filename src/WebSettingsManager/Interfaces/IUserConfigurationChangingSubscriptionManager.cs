namespace WebSettingsManager.Interfaces
{
    public interface IUserConfigurationChangingSubscriptionManager
    {
        /// <summary>
        /// Подписаться на обновление конфигурации пользователя
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="configurationId"></param>
        /// <returns>
        /// true  --> произошла подписка<br/>
        /// false --> подписка уже существует<br/>
        /// </returns>
        public Task<bool> SubscribeClientToUserConfigurationChanges(string clientId, UInt64 userId, UInt64 configurationId);

        /// <summary>
        /// Отписать клиента от обновлений конфигурации пользователя
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="configurationId"></param>
        /// <returns>
        /// true  --> произошла отписка<br/>
        /// false --> подписки не существовало<br/>
        /// </returns>
        public Task<bool> UnsubscribeClientToUserConfigurationChanges(string clientId, UInt64 userId, UInt64 configurationId);

        /// <summary>
        /// Удалить клиента с его подписками
        /// </summary>
        /// <param name="clientId"></param>
        /// true  --> произошло удаление клиента с соответствующими подписками<br/>
        /// false --> клиента с указанным Id не существовало<br/>
        public Task<bool> RemoveClient(string clientId);

        /// <summary>
        /// Получить всех клиентов
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> GetClients();

        /// <summary>
        /// Получить подписки для существующего клиента
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public Task<List<(UInt64 userId, UInt64 configurationId)>> GetSubscriptionsForClient(string clientId);
    }
}

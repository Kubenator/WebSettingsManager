using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WebSettingsManager.Hubs;
using WebSettingsManager.Interfaces;
using static WebSettingsManager.Models.UserWithVersioningTextConfigurationsRepository;

namespace WebSettingsManager.Models
{
    public class UserConfigurationChangingSubscriptionManager : IUserConfigurationChangingSubscriptionManager
    {
        private IHubContext<UsersHub> _hubContext;
        private IUserWithVersioningTextConfigurationsRepository _userRepository;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private Dictionary<(UInt64 userId, UInt64 configurationId), Dictionary<string, OnUserConfigurationUpdateClientSender>> _configurationChangedEventHandlersByUserIdAndConfId = new();
        private Dictionary<string, HashSet<(UInt64 userId, UInt64 configurationId)>> _userConfigurationsSetByConnectionId = new();
        public UserConfigurationChangingSubscriptionManager(IHubContext<UsersHub> hubContext, IUserWithVersioningTextConfigurationsRepository userRepository)
        { 
            _hubContext = hubContext;
            _userRepository = userRepository;
        }
        /// <summary>
        /// <inheritdoc cref="IUserConfigurationChangingSubscriptionManager.SubscribeClientToUserConfigurationChanges(string, ulong, ulong)"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        /// <exception cref="UserConfigurationNotFoundException"></exception>
        public async Task<bool> SubscribeClientToUserConfigurationChanges(string clientId, UInt64 userId, UInt64 configurationId)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_configurationChangedEventHandlersByUserIdAndConfId.TryGetValue(new (userId, configurationId), out var existingSubscribers))
                {
                    if (existingSubscribers.TryGetValue(clientId, out var existingHandler))
                        return false;
                    else
                    {
                        var updatedConfigurationToClientSender = new OnUserConfigurationUpdateClientSender(
                            _hubContext,
                            clientId,
                            userId,
                            configurationId
                            );
                        existingSubscribers.Add(clientId, updatedConfigurationToClientSender);

                        if (_userConfigurationsSetByConnectionId.TryGetValue(clientId, out var existingConnectionSubscriptions))
                            existingConnectionSubscriptions.Add(new (userId, configurationId));
                        else
                            _userConfigurationsSetByConnectionId.Add(clientId, new HashSet<(ulong, ulong)> { new (userId, configurationId) });

                        _userRepository.UserTextConfigurationUpdated += updatedConfigurationToClientSender.EventHandler;
                        return true;
                    }
                }
                else
                {
                    var existingUserConfiguration = await _userRepository.GetUserConfiguration(userId, configurationId).ConfigureAwait(false);
                    if (existingUserConfiguration is null)
                        throw new UserConfigurationNotFoundException(userId, configurationId);
                    var updatedConfigurationToClientSender = new OnUserConfigurationUpdateClientSender(
                            _hubContext,
                            clientId,
                            userId,
                            configurationId
                            );
                    _configurationChangedEventHandlersByUserIdAndConfId.Add(
                        new (userId, configurationId),
                        new Dictionary<string, OnUserConfigurationUpdateClientSender> { { clientId, updatedConfigurationToClientSender } });

                    if (_userConfigurationsSetByConnectionId.TryGetValue(clientId, out var existingConnectionSubscriptions))
                        existingConnectionSubscriptions.Add(new (userId, configurationId));
                    else
                        _userConfigurationsSetByConnectionId.Add(clientId, new HashSet<(ulong, ulong)> { new (userId, configurationId) });

                    _userRepository.UserTextConfigurationUpdated += updatedConfigurationToClientSender.EventHandler;
                    return true;
                }
            }
            finally
            { 
                _semaphore.Release();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IUserConfigurationChangingSubscriptionManager.UnsubscribeClientToUserConfigurationChanges(string, ulong, ulong)"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        public async Task<bool> UnsubscribeClientToUserConfigurationChanges(string clientId, UInt64 userId, UInt64 configurationId)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_configurationChangedEventHandlersByUserIdAndConfId.TryGetValue(new (userId, configurationId), out var existingSubscribers))
                {
                    if (existingSubscribers.TryGetValue(clientId, out var existingHandler))
                    {
                        existingSubscribers.Remove(clientId);

                        if (_userConfigurationsSetByConnectionId.TryGetValue(clientId, out var existingConnectionSubscriptions))
                            existingConnectionSubscriptions.Remove(new (userId, configurationId));

                        _userRepository.UserTextConfigurationUpdated -= existingHandler.EventHandler;
                        return true;
                    }
                    else
                        return false;                
                }
                else
                    return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IUserConfigurationChangingSubscriptionManager.RemoveClient(string)"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveClient(string clientId)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_userConfigurationsSetByConnectionId.TryGetValue(clientId, out var existingConnectionSubscriptions))
                    return false;
                if (existingConnectionSubscriptions.Count == 0)
                {
                    _userConfigurationsSetByConnectionId.Remove(clientId);
                    return true;
                }
                else
                {
                    foreach (var existingConnectionSubscription in existingConnectionSubscriptions)
                    {
                        if (_configurationChangedEventHandlersByUserIdAndConfId.TryGetValue(existingConnectionSubscription, out var existingSubscribers))
                        {
                            if (existingSubscribers.TryGetValue(clientId, out var existingHandler))
                            {
                                existingSubscribers.Remove(clientId);
                                _userRepository.UserTextConfigurationUpdated -= existingHandler.EventHandler;
                            }
                        }
                    }
                    _userConfigurationsSetByConnectionId.Remove(clientId);
                    return true;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IUserConfigurationChangingSubscriptionManager.GetClients()"/>
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetClients()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                return _userConfigurationsSetByConnectionId.Keys.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IUserConfigurationChangingSubscriptionManager.GetSubscriptionsForClient(string)"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <exception cref="ClientNotFoundExceprion"></exception>
        public async Task<List<(UInt64 userId, UInt64 configurationId)>> GetSubscriptionsForClient(string clientId)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_userConfigurationsSetByConnectionId.TryGetValue(clientId, out var existingConnectionSubscriptions))
                return existingConnectionSubscriptions.ToList();
                throw new ClientNotFoundExceprion(clientId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private class OnUserConfigurationUpdateClientSender
        {
            private IHubContext<UsersHub> _hubContext;
            private string _clientId;
            private UInt64 _userId;
            private UInt64 _configurationId;
            public readonly EventHandler<UserTextConfigurationUpdatedEventArgs> EventHandler;

            public OnUserConfigurationUpdateClientSender(IHubContext<UsersHub> hubContext, string clientId, UInt64 userId, UInt64 configurationId)
            {
                _hubContext = hubContext;
                _clientId = clientId;
                _userId = userId;
                _configurationId = configurationId;

                EventHandler = new EventHandler<UserTextConfigurationUpdatedEventArgs>(async (o, args) =>
                {
                    if (args.UserId == _userId && args.ConfigurationId == _configurationId)
                        await _hubContext.Clients.Client(_clientId).SendAsync("ReceiveConfigurationUpdateArgs", args);
                });
            }
        }

        /// <summary>
        /// Ошибка при попытке обнаружить указанный элемент менеджера подписок на изменения конфигураций пользователей
        /// </summary>
        public class UserConfigurationChangingSubscriptionManagerItemNotFoundExceprion : Exception
        {
            public UserConfigurationChangingSubscriptionManagerItemNotFoundExceprion(string message) : base(message) { }
        }

        /// <summary>
        /// Ошибка при невозможности найти указанного клиента
        /// </summary>
        public class ClientNotFoundExceprion : UserConfigurationChangingSubscriptionManagerItemNotFoundExceprion
        {
            public ClientNotFoundExceprion(string clientId) : base($"Не удалось обнаружить клиента по id: '{clientId}'") { }
        }
    }

}

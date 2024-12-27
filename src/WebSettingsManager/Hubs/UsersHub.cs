using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Text.Json;
using WebSettingsManager.Interfaces;
using WebSettingsManager.Models;

namespace WebSettingsManager.Hubs
{
    public class UsersHub : Hub
    {
        private readonly IUserConfigurationChangingSubscriptionManager _userConfigurationChangingSubscriptionManager;
        public UsersHub(IUserConfigurationChangingSubscriptionManager userConfigurationChangingSubscriptionManager, IHubContext<UsersHub> hubContext)
        {
            _userConfigurationChangingSubscriptionManager = userConfigurationChangingSubscriptionManager;
        }
        
        /// <summary>
        /// Подписаться на обновления конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public async Task SubscribeUserConfigurationUpdates(string userId, string confId)
        {
            var connectionActualId = this.Context.ConnectionId;
            try
            {
                var subscriptionResult = await _userConfigurationChangingSubscriptionManager.SubscribeClientToUserConfigurationChanges(connectionActualId, Convert.ToUInt64(userId), Convert.ToUInt64(confId));
                if (subscriptionResult)
                {
                    await this.Clients.Caller.SendAsync("ReceiveSubscriptionUpdateArgs", JsonSerializer.Serialize(
                       new SubscriptionEventData(
                           true,
                           nameof(SubscribeUserConfigurationUpdates),
                           false,
                           Convert.ToUInt64(userId),
                           Convert.ToUInt64(confId))));
                }
                else
                    await this.Clients.Caller.SendAsync("ReceiveSubscriptionUpdateArgs", JsonSerializer.Serialize(
                       new SubscriptionEventData(
                           true,
                           nameof(SubscribeUserConfigurationUpdates),
                           true,
                           Convert.ToUInt64(userId),
                           Convert.ToUInt64(confId))));
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync("ReceiveSubscriptionUpdateArgs", JsonSerializer.Serialize(
                    new ErrorEventData(
                        false,
                        nameof(SubscribeUserConfigurationUpdates),
                        ex.Message
                        )
                    ));
            }
        }

        /// <summary>
        /// Отписаться от обновлений конфигурации пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confId"></param>
        /// <returns></returns>
        public async Task UnsubscribeUserConfigurationUpdates(string userId, string confId)
        {
            var connectionActualId = this.Context.ConnectionId;
            try
            {
                var unsubscriptionResult = await _userConfigurationChangingSubscriptionManager.UnsubscribeClientToUserConfigurationChanges(connectionActualId, Convert.ToUInt64(userId), Convert.ToUInt64(confId));
                if (unsubscriptionResult)
                    await this.Clients.Caller.SendAsync("ReceiveSubscriptionUpdateArgs", JsonSerializer.Serialize(
                        new SubscriptionEventData(
                            true,
                            nameof(UnsubscribeUserConfigurationUpdates),
                            true,
                            Convert.ToUInt64(userId),
                            Convert.ToUInt64(confId))));
                else
                    await this.Clients.Caller.SendAsync("ReceiveSubscriptionUpdateArgs", JsonSerializer.Serialize(
                        new SubscriptionEventData(
                            true,
                            nameof(UnsubscribeUserConfigurationUpdates),
                            false,
                            Convert.ToUInt64(userId),
                            Convert.ToUInt64(confId))));
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync("ReceiveSubscriptionUpdateArgs", JsonSerializer.Serialize(new ErrorEventData(
                    false,
                    nameof(UnsubscribeUserConfigurationUpdates),
                    ex.Message
                    )));
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _userConfigurationChangingSubscriptionManager.RemoveClient(this.Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        private record SubscriptionEventData(bool Success, string Action, bool WasPreviouslySubscribed, UInt64 userId, UInt64 configurationID);
        private record ErrorEventData(bool Success, string Action, string Error);

    }
}

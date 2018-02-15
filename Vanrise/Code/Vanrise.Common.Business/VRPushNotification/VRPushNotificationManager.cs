using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRPushNotificationManager
    {
        #region Singleton

        static VRPushNotificationManager s_current = new VRPushNotificationManager();

        public static VRPushNotificationManager Current
        {
            get
            {
                return s_current;
            }
        }

        private VRPushNotificationManager()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = false;
        }

        System.Timers.Timer _timer;
        ExtensionConfigurationManager _extensionConfigurationManager = new ExtensionConfigurationManager();
        List<VRSocketInfo> _registeredSockets = new List<VRSocketInfo>();
        HandlerRuntimeStatesByHandlerId _handlerRuntimeStatesByHandler = new HandlerRuntimeStatesByHandlerId();

        #endregion

        #region Public Methods

        public async Task RegisterSocket(WebSocket socket)
        {
            int userId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            VRSocketInfo socketInfo = new VRSocketInfo(socket, userId);
            _registeredSockets.Add(socketInfo);
            lock (_timer)
            {
                if (!_timer.Enabled)
                {
                    _timer.Enabled = true;
                }
            }
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    string serializedMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    Object message = Serializer.Deserialize(serializedMessage);
                    VRPushNotificationClientSubscription clientSubscription = message as VRPushNotificationClientSubscription;
                    if(clientSubscription != null)
                    {
                        AddClientSubscription(clientSubscription, socketInfo);
                    }
                    else
                    {
                        VRPushNotificationClientUnsubscribe unsubscribePayload = message as VRPushNotificationClientUnsubscribe;
                        RemoveClientSubscription(unsubscribePayload, socketInfo);
                    }
                }
                else
                {
                    lock (this)
                    {
                        foreach (var subscriptionInfo in socketInfo.Subscriptions.Values)
                        {
                            HandlerRuntimeState handlerRuntimeState;
                            if (_handlerRuntimeStatesByHandler.TryGetValue(subscriptionInfo.ClientSubscription.HandlerId, out handlerRuntimeState))
                            {
                                handlerRuntimeState.RemoveSubscriptionIfExists(subscriptionInfo);
                            }
                        }
                        _registeredSockets.Remove(socketInfo);
                    }
                    lock (_timer)
                    {
                        if (_registeredSockets.Count == 0)
                        {
                            _timer.Enabled = false;
                        }
                    }
                    break;
                }
            }
        }

        #endregion

        #region Private Methods

        bool _isTimerRunning;

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock(this)
            {
                if (_isTimerRunning)
                    return;
                _isTimerRunning = true;
            }
            try
            {
                Dictionary<Guid, VRPushNotificationHandler> handlers = _extensionConfigurationManager.GetExtensionConfigurationsByType<VRPushNotificationHandler>(VRPushNotificationHandler.EXTENSION_TYPE);
                foreach (var handlerRuntimeStateEntry in _handlerRuntimeStatesByHandler)
                {
                    RunHandlerIfNotRunning(handlerRuntimeStateEntry.Key, handlerRuntimeStateEntry.Value, handlers);
                }
            }
            catch(Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            finally
            {
                lock (this)
                {
                    _isTimerRunning = false;
                }
            }
        }

        private void RunHandlerIfNotRunning(Guid handlerId, HandlerRuntimeState handlerRuntimeState, Dictionary<Guid, VRPushNotificationHandler> handlers)
        {
            if (!handlerRuntimeState.IsRunning)
            {
                handlerRuntimeState.IsRunning = true;
                Task task = new Task(() =>
                {
                    try
                    {
                        if (!handlerRuntimeState.EmptySubscriptions)
                        {
                            VRPushNotificationHandler handler;
                            if (handlers.TryGetValue(handlerId, out handler))
                            {
                                handler.HandlerSettings.Execute(handlerRuntimeState);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerFactory.GetExceptionLogger().WriteException(ex);
                    }
                    finally
                    {
                        handlerRuntimeState.IsRunning = false;
                    }
                });
                task.Start();
            }
        }

        private void AddClientSubscription(VRPushNotificationClientSubscription clientSubscription, VRSocketInfo socketInfo)
        {
            SubscriptionInfo subscriptionInfo = new SubscriptionInfo(clientSubscription, socketInfo);
            string clientSubscriptionId = clientSubscription.ClientSubscriptionId;
            lock (this)
            {
                if (socketInfo.Subscriptions.ContainsKey(clientSubscriptionId))
                    throw new Exception(String.Format("ClientSubscriptionId '{0}' already exists for current web socket", clientSubscriptionId));
                socketInfo.Subscriptions.Add(clientSubscriptionId, subscriptionInfo);
                _handlerRuntimeStatesByHandler.GetOrCreateItem(clientSubscription.HandlerId).AddSubscription(subscriptionInfo);
            }
        }

        private void RemoveClientSubscription(VRPushNotificationClientUnsubscribe unsubscribePayload, VRSocketInfo socketInfo)
        {
            string clientSubscriptionId = unsubscribePayload.ClientSubscriptionId;
            lock (this)
            {
                SubscriptionInfo matchSubscriptionInfo;
                if (socketInfo.Subscriptions.TryGetValue(clientSubscriptionId, out matchSubscriptionInfo))
                {
                    socketInfo.Subscriptions.Remove(clientSubscriptionId);
                    HandlerRuntimeState handlerRuntimeState;
                    if (_handlerRuntimeStatesByHandler.TryGetValue(matchSubscriptionInfo.ClientSubscription.HandlerId, out handlerRuntimeState))
                    {
                        handlerRuntimeState.RemoveSubscriptionIfExists(matchSubscriptionInfo);
                    }
                }
            }
        }

        #endregion

        #region Private Classes

        private class VRSocketInfo
        {
            public VRSocketInfo(WebSocket socket, int userId)
            {
                this.Socket = socket;
                this.UserId = userId;
                this.Subscriptions = new SubscriptionInfosBySubscriptionId();
            }

            public WebSocket Socket { get; private set; }

            public int UserId { get; private set; }

            public SubscriptionInfosBySubscriptionId Subscriptions { get; private set; }
        }

        private class SubscriptionInfo : IVRPushNotificationSubscription
        {
            public SubscriptionInfo(VRPushNotificationClientSubscription clientSubscription, VRSocketInfo socket)
            {
                this.ClientSubscription = clientSubscription;
                this.Socket = socket;
            }

            public VRPushNotificationClientSubscription ClientSubscription { get; private set; }

            public VRSocketInfo Socket { get; private set; }

            public int UserId
            {
                get { return this.Socket.UserId; }
            }
        }

        private class SubscriptionInfosBySubscriptionId : Dictionary<string, SubscriptionInfo>
        {

        }

        private class HandlerRuntimeState : IVRPushNotificationHandlerExecuteContext
        {
            List<IVRPushNotificationSubscription> _subscriptions = new List<IVRPushNotificationSubscription>();
            public HandlerRuntimeState()
            {
            }
            
            public bool IsRunning { get; set; }

            public List<IVRPushNotificationSubscription> Subscriptions
            {
                get { return _subscriptions; }
            }

            public void SendMessage(VRPushNotificationMessage message, IVRPushNotificationSubscription subscription)
            {
                SubscriptionInfo subscriptionInfo = subscription.CastWithValidate<SubscriptionInfo>("subscription");
                var clientMessage = new VRPushNotificationClientMessage
                {
                    Message = message,
                    ClientSubscriptionId = subscriptionInfo.ClientSubscription.ClientSubscriptionId
                };
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(Serializer.Serialize(clientMessage)));
                //await 
                subscriptionInfo.Socket.Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }

            public object HandlerState { get; set; }

            public void AddSubscription(SubscriptionInfo subscriptionInfo)
            {
                _subscriptions.Add(subscriptionInfo);
            }

            public void RemoveSubscriptionIfExists(SubscriptionInfo subscriptionInfo)
            {
                if (_subscriptions.Contains(subscriptionInfo))
                    _subscriptions.Remove(subscriptionInfo);
            }

            public bool EmptySubscriptions
            {
                get
                {
                    return _subscriptions.Count == 0;
                }
            }
        }

        private class HandlerRuntimeStatesByHandlerId : Dictionary<Guid, HandlerRuntimeState>
        {

        }

        #endregion
    }
}

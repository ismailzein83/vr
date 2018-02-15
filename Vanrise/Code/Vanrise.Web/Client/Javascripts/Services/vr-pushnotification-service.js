(function (app) {

    "use strict";

    PushNotificationService.$inject = ['$websocket', 'UtilsService'];

    function PushNotificationService($websocket, UtilsService) {

        var socket;
        var isSocketOpen;
        var subscriptionInfos = [];
        var lastClientSubscriptionId = 0;

        function subscribeToNotification(payload, scope) {
            var clientSubscription = payload.subscription;
            clientSubscription.ClientSubscriptionId = 'subsId_' + (++lastClientSubscriptionId);
            var subscriptionInfo = {
                clientSubscriptionId: clientSubscription.ClientSubscriptionId,
                clientSubscription: clientSubscription,
                onMessageReceived: payload.onMessageReceived
            };
            subscriptionInfos.push(subscriptionInfo);
            registerPendingSubscriptions();
            scope.$on("$destroy", function () {
                unsubscribe({
                    subscription: payload.subscription
                });                
            });
        }

        function registerPendingSubscriptions() {
            if (socket == undefined) {
                socket = $websocket("ws://" + window.location.hostname + ":" + window.location.port + "/api/VRCommon/VRPushNotification/Register");
                socket.onOpen(function () {
                    isSocketOpen = true;
                    console.log('socket opened ' + new Date());
                });
                socket.onMessage(function (evt) {
                    var clientMessage = JSON.parse(evt.data);
                    var clientSubscriptionId = clientMessage.ClientSubscriptionId;
                    var matchSubscriptionInfo = UtilsService.getItemByVal(subscriptionInfos, clientSubscriptionId, "clientSubscriptionId");
                    if (matchSubscriptionInfo != null) {
                        matchSubscriptionInfo.onMessageReceived({
                            message: clientMessage.Message
                        });
                    }
                });
                socket.onError(function (evt) {
                    console.log('error: ' + evt);
                    if (socket != null && isSocketOpen) {
                        socket.close(true);
                    }
                    else {
                        socket = null;
                        isSocketOpen = false;
                        setSubscriptionsToPendingAndReregister();
                    }
                });
                socket.onClose(function () {
                    socket = null;
                    isSocketOpen = false;
                    setSubscriptionsToPendingAndReregister();
                    console.log("socket closed");
                });
                registerPendingSubscriptions();
            }
            else if(isSocketOpen)
            {
                for (var i = 0; i < subscriptionInfos.length; i++) {
                    var subscriptionInfo = subscriptionInfos[i];
                    if (!subscriptionInfo.isRegistered) {
                        socket.send(subscriptionInfo.clientSubscription);
                        subscriptionInfo.isRegistered = true;
                    }
                }
            }
            else
            {
                scheduleRegisterPendingSubscriptions();
            }
        }

        var isRegisterPendingSubscriptionsScheduled = false;
        function scheduleRegisterPendingSubscriptions() {
            if (isRegisterPendingSubscriptionsScheduled)
                return;
            isRegisterPendingSubscriptionsScheduled = true;
            setTimeout(function () {
                isRegisterPendingSubscriptionsScheduled = false;
                registerPendingSubscriptions();
            }, 500);
        }

        function setSubscriptionsToPendingAndReregister() {
            if (subscriptionInfos.length > 0) {
                for (var i = 0; i < subscriptionInfos.length; i++) {
                    subscriptionInfos[i].isRegistered = false;
                }
                scheduleRegisterPendingSubscriptions();
            }
        }

        function unsubscribe(payload) {
            var clientSubscriptionId = payload.subscription.ClientSubscriptionId;
            
            var matchSubscriptionInfo = UtilsService.getItemByVal(subscriptionInfos, clientSubscriptionId, "clientSubscriptionId");
            if (matchSubscriptionInfo != null) {
                if (socket != undefined && isSocketOpen) {
                    var unregisterSubscriptionPayload = {
                        "$type": "Vanrise.Entities.VRPushNotificationClientUnsubscribe, Vanrise.Entities",
                        "ClientSubscriptionId": clientSubscriptionId
                    };
                    socket.send(unregisterSubscriptionPayload);
                }
                var subscriptionIndex = subscriptionInfos.indexOf(matchSubscriptionInfo);
                subscriptionInfos.splice(subscriptionIndex, 1);
                if (subscriptionInfos.length == 0 && socket != undefined && isSocketOpen)
                    socket.close(true);
            }
        }

        return ({
            subscribeToNotification: subscribeToNotification,
            unsubscribe: unsubscribe
        });
    }

    app.service('PushNotificationService', PushNotificationService);

})(app);



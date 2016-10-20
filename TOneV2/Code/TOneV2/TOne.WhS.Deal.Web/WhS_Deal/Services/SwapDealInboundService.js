(function (appControllers) {

    'use strict';

    SwapDealInboundService.$inject = ['VRModalService', 'VRNotificationService'];

    function SwapDealInboundService(VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealInboundEditor.html';

        function addSwapDealInbound(onSwapDealInboundAdded, sellingNumberPlanId) {
            var settings = {};
            var parameters = {
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealInboundAdded = onSwapDealInboundAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editSwapDealInbound(swapDealInbound, sellingNumberPlanId, onSwapDealInboundUpdated) {
            var parameters = {
                swapDealInbound: swapDealInbound,
                sellingNumberPlanId: sellingNumberPlanId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealInboundUpdated = onSwapDealInboundUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function deleteSwapDealInbound($scope, swapDealInbound, onSwapDealInboundDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onSwapDealInboundDeleted(swapDealInbound);
                    }
                });
        }

        return {
            addSwapDealInbound: addSwapDealInbound,
            editSwapDealInbound: editSwapDealInbound,
            deleteSwapDealInbound: deleteSwapDealInbound
        };
    }

    appControllers.service('WhS_Deal_SwapDealInboundService', SwapDealInboundService);

})(appControllers);
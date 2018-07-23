(function (appControllers) {

    'use strict';

    SwapDealInboundService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

    function SwapDealInboundService(VRModalService, VRNotificationService, UtilsService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Directives/SwapDeal/Templates/SwapDealInboundEditor.html';

        function viewSwapDealInbound(swapDealInbound, sellingNumberPlanId, context) {
            var parameters = {
                swapDealInbound: swapDealInbound,
                sellingNumberPlanId: sellingNumberPlanId,
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function addSwapDealInbound(onSwapDealInboundAdded, sellingNumberPlanId, context, carrierAccountId, dealId) {
            var settings = {};
            var parameters = {
                sellingNumberPlanId: sellingNumberPlanId,
                context: context,
                carrierAccountId: carrierAccountId,
                dealId: dealId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealInboundAdded = onSwapDealInboundAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editSwapDealInbound(swapDealInbound, sellingNumberPlanId, onSwapDealInboundUpdated, context, carrierAccountId,dealId) {
            var parameters = {
                swapDealInbound: swapDealInbound,
                sellingNumberPlanId: sellingNumberPlanId,
                context: context,
                carrierAccountId: carrierAccountId,
                dealId: dealId,
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
            viewSwapDealInbound: viewSwapDealInbound,
            addSwapDealInbound: addSwapDealInbound,
            editSwapDealInbound: editSwapDealInbound,
            deleteSwapDealInbound: deleteSwapDealInbound
        };
    }

    appControllers.service('WhS_Deal_SwapDealInboundService', SwapDealInboundService);

})(appControllers);
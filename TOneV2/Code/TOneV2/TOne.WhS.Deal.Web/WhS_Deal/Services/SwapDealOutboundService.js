(function (appControllers) {

    'use strict';

    SwapDealOutboundService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

    function SwapDealOutboundService(VRModalService, VRNotificationService, UtilsService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Directives/SwapDeal/Templates/SwapDealOutboundEditor.html';

        function addSwapDealOutbound(onSwapDealOutboundAdded, supplierId, context, carrierAccountId, dealId, dealBED, dealEED) {
            var settings = {};
            var parameters = {
                supplierId: supplierId,
                context: context,
                carrierAccountId: carrierAccountId,
                dealId: dealId,
                dealBED: dealBED,
                dealEED: dealEED
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealOutboundAdded = onSwapDealOutboundAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }
        function viewSwapDealOutbound(swapDealOutbound, sellingNumberPlanId, context) {
            var parameters = {
                swapDealOutbound: swapDealOutbound,
                sellingNumberPlanId: sellingNumberPlanId,
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editSwapDealOutbound(swapDealOutbound, supplierId, onSwapDealOutboundUpdated, context, carrierAccountId, dealId, dealBED, dealEED) {
            var parameters = {
                swapDealOutbound: swapDealOutbound,
                supplierId: supplierId,
                context: context,
                carrierAccountId: carrierAccountId,
                dealId: dealId,
                dealBED: dealBED,
                dealEED: dealEED
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealOutboundUpdated = onSwapDealOutboundUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function deleteSwapDealOutbound($scope, swapDealOutbound, onSwapDealOutboundDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onSwapDealOutboundDeleted(swapDealOutbound);
                    }
                });
        }

        return {
            addSwapDealOutbound: addSwapDealOutbound,
            editSwapDealOutbound: editSwapDealOutbound,
            deleteSwapDealOutbound: deleteSwapDealOutbound,
            viewSwapDealOutbound: viewSwapDealOutbound
        };
    }

    appControllers.service('WhS_Deal_SwapDealOutboundService', SwapDealOutboundService);

})(appControllers);
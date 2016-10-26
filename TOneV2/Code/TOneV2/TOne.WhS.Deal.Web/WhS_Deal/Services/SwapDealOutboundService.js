(function (appControllers) {

    'use strict';

    SwapDealOutboundService.$inject = ['VRModalService', 'VRNotificationService'];

    function SwapDealOutboundService(VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Directives/SwapDeal/Templates/SwapDealOutboundEditor.html';

        function addSwapDealOutbound(onSwapDealOutboundAdded, supplierId) {
            var settings = {};
            var parameters = {
                supplierId: supplierId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealOutboundAdded = onSwapDealOutboundAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editSwapDealOutbound(swapDealOutbound, supplierId, onSwapDealOutboundUpdated) {
            var parameters = {
                swapDealOutbound: swapDealOutbound,
                supplierId: supplierId
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
            deleteSwapDealOutbound: deleteSwapDealOutbound
        };
    }

    appControllers.service('WhS_Deal_SwapDealOutboundService', SwapDealOutboundService);

})(appControllers);
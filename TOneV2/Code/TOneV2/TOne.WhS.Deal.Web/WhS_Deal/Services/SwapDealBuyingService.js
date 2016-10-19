(function (appControllers) {

    'use strict';

    SwapDealBuyingService.$inject = ['VRModalService', 'VRNotificationService'];

    function SwapDealBuyingService(VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealBuyingEditor.html';

        function addSwapDealBuying(onSwapDealBuyingAdded, supplierId) {
            var settings = {};
            var parameters = {
                supplierId: supplierId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealBuyingAdded = onSwapDealBuyingAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editSwapDealBuying(swapDealBuying, supplierId, onSwapDealBuyingUpdated) {
            var parameters = {
                swapDealBuying: swapDealBuying,
                supplierId: supplierId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealBuyingUpdated = onSwapDealBuyingUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function deleteSwapDealBuying($scope, swapDealBuying, onSwapDealBuyingDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onSwapDealBuyingDeleted(swapDealBuying);
                    }
                });
        }

        return {
            addSwapDealBuying: addSwapDealBuying,
            editSwapDealBuying: editSwapDealBuying,
            deleteSwapDealBuying: deleteSwapDealBuying
        };
    }

    appControllers.service('WhS_Deal_SwapDealBuyingService', SwapDealBuyingService);

})(appControllers);
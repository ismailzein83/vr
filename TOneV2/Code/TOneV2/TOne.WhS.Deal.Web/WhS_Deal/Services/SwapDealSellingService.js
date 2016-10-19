(function (appControllers) {

    'use strict';

    SwapDealSellingService.$inject = ['VRModalService', 'VRNotificationService'];

    function SwapDealSellingService(VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealSellingEditor.html';

        function addSwapDealSelling(onSwapDealSellingAdded, sellingNumberPlanId) {
            var settings = {};
            var parameters = {
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealSellingAdded = onSwapDealSellingAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editSwapDealSelling(swapDealSelling, sellingNumberPlanId, onSwapDealSellingUpdated) {
            var parameters = {
                swapDealSelling: swapDealSelling,
                sellingNumberPlanId: sellingNumberPlanId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealSellingUpdated = onSwapDealSellingUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function deleteSwapDealSelling($scope, swapDealSelling, onSwapDealSellingDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onSwapDealSellingDeleted(swapDealSelling);
                    }
                });
        }

        return {
            addSwapDealSelling: addSwapDealSelling,
            editSwapDealSelling: editSwapDealSelling,
            deleteSwapDealSelling: deleteSwapDealSelling
        };
    }

    appControllers.service('WhS_Deal_SwapDealSellingService', SwapDealSellingService);

})(appControllers);
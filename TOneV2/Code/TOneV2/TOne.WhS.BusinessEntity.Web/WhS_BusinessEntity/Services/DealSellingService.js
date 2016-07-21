(function (appControllers) {

    'use strict';

    DealSellingService.$inject = ['VRModalService', 'VRNotificationService'];

    function DealSellingService(VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/Deal/DealSellingEditor.html';

        function addDealSelling(onDealSellingAdded, sellingNumberPlanId) {
            var settings = {};
            var parameters = {
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealSellingAdded = onDealSellingAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editDealSelling(dealSelling, sellingNumberPlanId, onDealSellingUpdated) {
            var parameters = {
                dealSelling: dealSelling,
                sellingNumberPlanId: sellingNumberPlanId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealSellingUpdated = onDealSellingUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function deleteDealSelling($scope, dealSelling, onDealSellingDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onDealSellingDeleted(dealSelling);
                    }
                });
        }

        return {
            addDealSelling: addDealSelling,
            editDealSelling: editDealSelling,
            deleteDealSelling: deleteDealSelling
        };
    }

    appControllers.service('WhS_BE_DealSellingService', DealSellingService);

})(appControllers);
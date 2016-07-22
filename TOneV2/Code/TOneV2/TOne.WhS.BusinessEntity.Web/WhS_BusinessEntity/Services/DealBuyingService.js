(function (appControllers) {

    'use strict';

    DealBuyingService.$inject = ['VRModalService', 'VRNotificationService'];

    function DealBuyingService(VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/Deal/DealBuyingEditor.html';

        function addDealBuying(onDealBuyingAdded, supplierId) {
            var settings = {};
            var parameters = {
                supplierId: supplierId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealBuyingAdded = onDealBuyingAdded;
            };
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function editDealBuying(dealBuying, supplierId, onDealBuyingUpdated) {
            var parameters = {
                dealBuying: dealBuying,
                supplierId: supplierId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealBuyingUpdated = onDealBuyingUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function deleteDealBuying($scope, dealBuying, onDealBuyingDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onDealBuyingDeleted(dealBuying);
                    }
                });
        }

        return {
            addDealBuying: addDealBuying,
            editDealBuying: editDealBuying,
            deleteDealBuying: deleteDealBuying
        };
    }

    appControllers.service('WhS_BE_DealBuyingService', DealBuyingService);

})(appControllers);
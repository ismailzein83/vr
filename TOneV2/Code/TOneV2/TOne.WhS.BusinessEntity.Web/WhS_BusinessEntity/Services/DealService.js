(function (appControllers) {

    'use strict';

    DealService.$inject = ['VRModalService', 'VRNotificationService'];

    function DealService(VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/Deal/DealEditor.html';

        function addDeal(onDealAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealAdded = onDealAdded;
            };

            VRModalService.showModal(editorUrl, null, settings);
        }

        function editDeal(dealId, onDealUpdated) {
            var parameters = {
                dealId: dealId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealUpdated = onDealUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        return {
            addDeal: addDeal,
            editDeal: editDeal
        };
    }

    appControllers.service('WhS_BE_DealService', DealService);

})(appControllers);
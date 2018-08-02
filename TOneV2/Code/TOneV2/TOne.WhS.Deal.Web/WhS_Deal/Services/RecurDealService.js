(function (appControllers) {
    'use strict';
    RecurDealService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

    function RecurDealService(VRModalService, VRNotificationService, UtilsService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Directives/RecurDeal/RecurDealEditor.html';

        function recurDeal(dealId, dealName, onRecur) {
            var parameters = {
                dealId: dealId,
                dealName: dealName,
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRecur = onRecur;
            };

            VRModalService.showModal(editorUrl, parameters, modalSettings);
        }

        return {
            recurDeal: recurDeal
        };
     }

    appControllers.service('WhS_Deal_RecurDealService', RecurDealService);

})(appControllers);
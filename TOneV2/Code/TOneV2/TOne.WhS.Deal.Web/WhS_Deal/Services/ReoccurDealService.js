(function (appControllers) {
    'use strict';
    ReoccurDealService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

    function ReoccurDealService(VRModalService, VRNotificationService, UtilsService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Directives/ReoccurDeal/ReoccurDealEditor.html';

        function reoccurDeal(dealId, dealName, onReoccur) {
            var parameters = {
                dealId: dealId,
                dealName: dealName,
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onReoccur = onReoccur;
            };

            VRModalService.showModal(editorUrl, parameters, modalSettings);
        }

        return {
            reoccurDeal: reoccurDeal
        };
     }

    appControllers.service('WhS_Deal_ReoccurDealService', ReoccurDealService);

})(appControllers);
(function (appControllers) {

    'use strict';

    DealService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

    function DealService(VRModalService, VRNotificationService, UtilsService) {

        function ViewLastDealProgress(dealInfo) {

            var parameters = {
                dealInfo: dealInfo,
            };

            var settings = {};
            VRModalService.showModal("/Client/Modules/WhS_Deal/Views/DealProgressViewTemplate.html", parameters, settings);
        }

        return {
            ViewLastDealProgress: ViewLastDealProgress
        };
    }

    appControllers.service('WhS_Deal_DealService', DealService);
})(appControllers);
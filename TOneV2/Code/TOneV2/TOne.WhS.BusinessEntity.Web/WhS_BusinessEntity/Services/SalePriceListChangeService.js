(function (appControllers) {

    'use strict';

    salePriceListPreviewService.$inject = ['VRModalService'];

    function salePriceListPreviewService(vrModalService) {
        return ({
            previewPriceList: previewPriceList
        });

        function previewPriceList(priceListId) {
            var modalParameters = {
                PriceListId: priceListId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {

            };
            vrModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SalePricelist/SalePriceListChange.html', modalParameters, modalSettings);
        }
    };
    appControllers.service('WhS_BE_SalePriceListChangeService', salePriceListPreviewService);

})(appControllers);

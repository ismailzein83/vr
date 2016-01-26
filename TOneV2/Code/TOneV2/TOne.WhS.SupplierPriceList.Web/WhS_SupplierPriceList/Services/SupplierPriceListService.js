(function (appControllers) {

    'use strict';

    SupplierPriceListService.$inject = ['VRModalService'];

    function SupplierPriceListService(VRModalService) {
        return ({
            previewSupplierPriceList: previewSupplierPriceList
        });

        function previewSupplierPriceList(priceListId) {
           
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Supplier PriceList Preview";

            };
            var parameters = {
                PriceListId: priceListId
            };

            VRModalService.showModal('/Client/Modules/WhS_SupplierPriceList/Views/SupplierPriceListPreview.html', parameters, settings);
        }
    }

    appControllers.service('WhS_SupPL_SupplierPriceListService', SupplierPriceListService);

})(appControllers);

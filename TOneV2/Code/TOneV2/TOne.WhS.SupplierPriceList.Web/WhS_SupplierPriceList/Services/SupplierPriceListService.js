(function (appControllers) {

    'use strict';

    SupplierPriceListService.$inject = ['VRModalService', 'WhS_BE_SupplierPriceListService'];

    function SupplierPriceListService(VRModalService, WhS_BE_SupplierPriceListService) {

        return ({
            previewSupplierPriceListTask: previewSupplierPriceListTask,
            previewSupplierPriceList: previewSupplierPriceList,
            registerPreviewActionToSupplierPricelistGrid: registerPreviewActionToSupplierPricelistGrid
        });

        function registerPreviewActionToSupplierPricelistGrid() {
            var previewAction = {
                name: "Preview",
                clicked: function (payload) {
                    previewSupplierPriceList(payload);
                }
            };
            WhS_BE_SupplierPriceListService.registerAdditionalActionToSupplierPricelistGrid(previewAction);
        }

        function previewSupplierPriceListTask(priceListId) {

            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Supplier PriceList Preview";

            };
            var parameters = {
                PriceListId: priceListId
            };

            VRModalService.showModal('/Client/Modules/WhS_SupplierPriceList/Views/SupplierPriceListPreviewTask.html', parameters, settings);
        }

        function previewSupplierPriceList(payload) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Supplier PriceList Preview";

            };
            var parameters = {
                processInstanceId: payload.processInstanceId,
                fileId: payload.fileId,
                supplierPricelistType: payload.supplierPricelistType,
                pricelistDate: payload.pricelistDate,
                currencyId: payload.currencyId,
                supplierName : payload.supplierName
            };
            VRModalService.showModal('/Client/Modules/WhS_SupplierPriceList/Views/SupplierPriceListPreview.html', parameters, settings);
        }

    }

    appControllers.service('WhS_SupPL_SupplierPriceListService', SupplierPriceListService);

})(appControllers);

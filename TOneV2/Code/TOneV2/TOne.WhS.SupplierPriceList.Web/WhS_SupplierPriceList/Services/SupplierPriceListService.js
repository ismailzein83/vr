﻿(function (appControllers) {

    'use strict';

    SupplierPriceListService.$inject = ['VRModalService', 'WhS_BE_SupplierPriceListService'];

    function SupplierPriceListService(VRModalService, WhS_BE_SupplierPriceListService) {

        return ({
            previewSupplierPriceList: previewSupplierPriceList,
            previewSupplierPriceListForGrid: previewSupplierPriceListForGrid,
            registerPreviewActionToSupplierPricelistGrid: registerPreviewActionToSupplierPricelistGrid
        });

        function registerPreviewActionToSupplierPricelistGrid() {
            var previewAction = {
                name: "Preview",
                clicked: function (payload) {
                    previewSupplierPriceListForGrid(payload.Entity.ProcessInstanceId);
                }
            };
            WhS_BE_SupplierPriceListService.registerAdditionalActionToSupplierPricelistGrid(previewAction);
        }

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

        function previewSupplierPriceListForGrid(processInstanceId) {

            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Supplier PriceList Preview";

            };
            var parameters = {
                processInstanceId: processInstanceId
            };

            VRModalService.showModal('/Client/Modules/WhS_SupplierPriceList/Views/SupplierPriceListPreviewForGrid.html', parameters, settings);
        }

    }

    appControllers.service('WhS_SupPL_SupplierPriceListService', SupplierPriceListService);

})(appControllers);

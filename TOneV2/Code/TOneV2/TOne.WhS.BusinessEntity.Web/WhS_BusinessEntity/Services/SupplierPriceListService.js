(function (appControllers) {

    'use strict';

    SupplierPriceListService.$inject = [];

    function SupplierPriceListService() {
        var supplierPricelistGridAdditionalActions = [];

        return ({
            registerAdditionalActionToSupplierPricelistGrid: registerAdditionalActionToSupplierPricelistGrid,
            getAdditionalActionOfSupplierPricelistGrid: getAdditionalActionOfSupplierPricelistGrid
        });

        function registerAdditionalActionToSupplierPricelistGrid(additionalMenuAction) {
            supplierPricelistGridAdditionalActions.push(additionalMenuAction);
        }

        function getAdditionalActionOfSupplierPricelistGrid() {
            return supplierPricelistGridAdditionalActions;
        }

    }

    appControllers.service('WhS_BE_SupplierPriceListService', SupplierPriceListService);

})(appControllers);

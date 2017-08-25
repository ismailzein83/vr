(function (appControllers) {

    'use strict';

    SupplierPriceListService.$inject = [];

    function SupplierPriceListService() {
        var supplierPricelistGridAdditionalActions = [];

        return ({
            registerAdditionalActionToSupplierPricelistGrid: registerAdditionalActionToSupplierPricelistGrid,
            getAdditionalActionForSupplierPricelistGrid: getAdditionalActionForSupplierPricelistGrid
        });

        function registerAdditionalActionToSupplierPricelistGrid(newMenuAction) {
            supplierPricelistGridAdditionalActions.push(newMenuAction);
        }

        function getAdditionalActionForSupplierPricelistGrid() {
            return supplierPricelistGridAdditionalActions;
        }

    }

    appControllers.service('WhS_BE_SupplierPriceListService', SupplierPriceListService);

})(appControllers);

(function (appControllers) {

    'use stict';

    ProductService.$inject = ['VRModalService'];

    function ProductService(VRModalService) {

        function addProduct(onProductAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductAdded = onProductAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Product/ProductEditor.html', null, settings);
        };
        function editProduct(productId, onProductUpdated) {

            var parameters = {
                productId: productId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductUpdated = onProductUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Product/ProductEditor.html', parameters, settings);
        }

        function addProductPackageItem(productDefinitionId, excludedPackageIds, onPackageItemAdded) {

            var parameters = {
                productDefinitionId: productDefinitionId,
                excludedPackageIds: excludedPackageIds
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPackageItemAdded = onPackageItemAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/Templates/ProductPackageItemEditor.html', parameters, settings);
        };
        function editProductPackageItem(packageItem, productDefinitionId, excludedPackageIds, onPackageItemUpdated) {

            var parameters = {
                packageItem: packageItem,
                productDefinitionId: productDefinitionId,
                excludedPackageIds: excludedPackageIds
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPackageItemUpdated = onPackageItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/Templates/ProductPackageItemEditor.html', parameters, settings);
        }


        return {
            addProduct: addProduct,
            editProduct: editProduct,
            addProductPackageItem: addProductPackageItem,
            editProductPackageItem: editProductPackageItem
        };
    }

    appControllers.service('Retail_BE_ProductService', ProductService);

})(appControllers);
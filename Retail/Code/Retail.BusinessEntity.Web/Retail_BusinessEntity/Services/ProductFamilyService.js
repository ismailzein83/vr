(function (appControllers) {

    'use stict';

    ProductFamilyService.$inject = ['VRModalService'];

    function ProductFamilyService(VRModalService) {

        function addProductFamily(onProductFamilyAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductFamilyAdded = onProductFamilyAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ProductFamily/ProductFamilyEditor.html', null, settings);
        };
        function editProductFamily(productFamilyId, onProductFamilyUpdated) {

            var parameters = {
                productFamilyId: productFamilyId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductFamilyUpdated = onProductFamilyUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ProductFamily/ProductFamilyEditor.html', parameters, settings);
        }

        function addProductFamilyPackageItem(productDefinitionId, excludedPackageIds, onProductFamilyPackageItemAdded) {

            var parameters = {
                productDefinitionId: productDefinitionId,
                excludedPackageIds: excludedPackageIds
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductFamilyPackageItemAdded = onProductFamilyPackageItemAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/ProductFamily/Templates/ProductFamilyPackageItemEditor.html', parameters, settings);
        };
        function editProductFamilyPackageItem(productFamilyPackageItem, productDefinitionId, excludedPackageIds, onProductFamilyPackageItemUpdated) {

            var parameters = {
                productFamilyPackageItem: productFamilyPackageItem,
                productDefinitionId: productDefinitionId,
                excludedPackageIds: excludedPackageIds
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductFamilyPackageItemUpdated = onProductFamilyPackageItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/ProductFamily/Templates/ProductFamilyPackageItemEditor.html', parameters, settings);
        }


        return {
            addProductFamily: addProductFamily,
            editProductFamily: editProductFamily,
            addProductFamilyPackageItem: addProductFamilyPackageItem,
            editProductFamilyPackageItem: editProductFamilyPackageItem
        };
    }

    appControllers.service('Retail_BE_ProductFamilyService', ProductFamilyService);

})(appControllers);
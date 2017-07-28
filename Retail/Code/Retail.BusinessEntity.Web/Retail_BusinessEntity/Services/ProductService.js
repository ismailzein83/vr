(function (appControllers) {

    'use stict';

    ProductService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function ProductService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addProduct(onProductAdded, productFamilyId, productDefinitionId) {

            var parameters = {
                productFamilyId: productFamilyId,
                productDefinitionId: productDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductAdded = onProductAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Product/ProductEditor.html', parameters, settings);
        };
        function editProduct(onProductUpdated, productId, productFamilyId, productDefinitionId) {

            var parameters = {
                productId: productId,
                productFamilyId: productFamilyId,
                productDefinitionId: productDefinitionId
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

        function addRecurringChargeRuleSet(onRecurringChargeRuleSetAdded) {

            var parameters = {
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeRuleSetAdded = onRecurringChargeRuleSetAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/Templates/RecurringChargeRuleSetEditor.html', parameters, settings);
        };
        function editRecurringChargeRuleSet(recurringChargeRuleSet, onRecurringChargeRuleSetUpdated) {

            var parameters = {
                recurringChargeRuleSet: recurringChargeRuleSet
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPackageItemUpdated = onPackageItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/Templates/RecurringChargeRuleSetEditor.html', parameters, settings);
        }
        function getEntityUniqueName(accountBEDefinitionId) {
            return "Retail_BusinessEntity_Product_" + accountBEDefinitionId;
        }

        function registerObjectTrackingDrillDownToProduct() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, productItem) {

                productItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: productItem.Entity.ProductId,
                    EntityUniqueName: getEntityUniqueName(productItem.AccountBEDefinitionId),

                };
                return productItem.objectTrackingGridAPI.load(query);
            };
                

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {

            return drillDownDefinitions;
        }


        return {
            addProduct: addProduct,
            editProduct: editProduct,
            addProductPackageItem: addProductPackageItem,
            editProductPackageItem: editProductPackageItem,
            addRecurringChargeRuleSet: addRecurringChargeRuleSet,
            editRecurringChargeRuleSet: editRecurringChargeRuleSet,
            getEntityUniqueName: getEntityUniqueName,
            registerObjectTrackingDrillDownToProduct: registerObjectTrackingDrillDownToProduct,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('Retail_BE_ProductService', ProductService);

})(appControllers);
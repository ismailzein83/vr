(function (appControllers) {

    "use strict";
    sellingProductAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];
    function sellingProductAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = "SellingProduct";
        function GetFilteredSellingProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSellingProducts"), input);
        }

        function GetSellingProduct(sellingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSellingProduct"), {
                sellingProductId: sellingProductId
            });
        }
        function GetSellingProductCurrencyId(sellingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSellingProductCurrencyId"), {
                sellingProductId: sellingProductId
            });
        }
        function AddSellingProduct(sellingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddSellingProduct"), sellingProductObject);
        }

        function UpdateSellingProduct(sellingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSellingProduct"), sellingProductObject);
        }
        function GetSellingProductsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSellingProductsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function HasUpdateSellingProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateSellingProduct']));
        }

        function HasAddSellingProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddSellingProduct']));
        }
        function GetSellingProductHistoryDetailbyHistoryId(sellingProductHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSellingProductHistoryDetailbyHistoryId'), {
                sellingProductHistoryId: sellingProductHistoryId
            });
        }
        function GetSellingProductName(sellingProductId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSellingProductName'), {
                sellingProductId: sellingProductId
            });
        }
        
        function GetSellingProductPricingSettings(sellingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSellingProductPricingSettings'), {
                sellingProductId: sellingProductId
            });
        }

        return ({
            GetFilteredSellingProducts: GetFilteredSellingProducts,
            AddSellingProduct: AddSellingProduct,
            UpdateSellingProduct: UpdateSellingProduct,
            GetSellingProduct: GetSellingProduct,
            GetSellingProductsInfo: GetSellingProductsInfo,
            HasUpdateSellingProductPermission: HasUpdateSellingProductPermission,
            HasAddSellingProductPermission: HasAddSellingProductPermission,
            GetSellingProductHistoryDetailbyHistoryId: GetSellingProductHistoryDetailbyHistoryId,
            GetSellingProductName: GetSellingProductName,
            GetSellingProductCurrencyId: GetSellingProductCurrencyId,
            GetSellingProductPricingSettings: GetSellingProductPricingSettings
        });
    }

    appControllers.service('WhS_BE_SellingProductAPIService', sellingProductAPIService);
})(appControllers);
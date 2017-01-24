(function (appControllers) {

    "use strict";

    ProductFamilyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function ProductFamilyAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "ProductFamily";

        function GetFilteredProductFamilies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredProductFamilies'), input);
        }

        function GetProductFamily(productFamilyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetProductFamily'), {
                productFamilyId: productFamilyId
            });
        }

        function AddProductFamily(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddProductFamily'), statusDefinitionItem);
        }

        function HasAddProductFamilyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddProductFamily']));

        }

        function UpdateProductFamily(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateProductFamily'), statusDefinitionItem);
        }

        function HasUpdateProductFamilyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateProductFamily']));
        }

        function GetProductFamiliesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductFamiliesInfo"), {
                serializedFilter: serializedFilter
            });
        }

        return ({
            GetFilteredProductFamilies: GetFilteredProductFamilies,
            GetProductFamily: GetProductFamily,
            AddProductFamily: AddProductFamily,
            HasAddProductFamilyPermission: HasAddProductFamilyPermission,
            UpdateProductFamily: UpdateProductFamily,
            HasUpdateProductFamilyPermission: HasUpdateProductFamilyPermission,
            GetProductFamiliesInfo: GetProductFamiliesInfo
        });
    }

    appControllers.service('Retail_BE_ProductFamilyAPIService', ProductFamilyAPIService);

})(appControllers);
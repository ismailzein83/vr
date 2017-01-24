(function (appControllers) {

    "use strict";

    ProductAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function ProductAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "Product";

        function GetFilteredProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredProducts'), input);
        }

        function GetProductEditorRuntime(productId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetProductEditorRuntime'), {
                productId: productId
            });
        }

        function AddProduct(product) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddProduct'), product);
        }

        function HasAddProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddProduct']));

        }

        function UpdateProduct(product) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateProduct'), product);
        }

        function HasUpdateProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateProduct']));
        }

        function GetProductsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        return ({
            GetFilteredProducts: GetFilteredProducts,
            GetProductEditorRuntime: GetProductEditorRuntime,
            AddProduct: AddProduct,
            HasAddProductPermission: HasAddProductPermission,
            UpdateProduct: UpdateProduct,
            HasUpdateProductPermission: HasUpdateProductPermission,
            GetProductsInfo: GetProductsInfo
        });
    }

    appControllers.service('Retail_BE_ProductAPIService', ProductAPIService);

})(appControllers);
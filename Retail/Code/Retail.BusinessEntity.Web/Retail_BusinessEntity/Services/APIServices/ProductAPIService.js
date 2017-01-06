
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

        function AddProduct(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddProduct'), statusDefinitionItem);
        }

        function HasAddProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddProduct']));

        }

        function UpdateProduct(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateProduct'), statusDefinitionItem);
        }

        function HasUpdateProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateProduct']));
        }

        //function GetProductesInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductesInfo"), {
        //        filter: filter
        //    });
        //}

        return ({
            GetFilteredProducts: GetFilteredProducts,
            GetProductEditorRuntime: GetProductEditorRuntime,
            AddProduct: AddProduct,
            HasAddProductPermission: HasAddProductPermission,
            UpdateProduct: UpdateProduct,
            HasUpdateProductPermission: HasUpdateProductPermission
            //GetProductesInfo: GetProductesInfo,
        });
    }

    appControllers.service('Retail_BE_ProductAPIService', ProductAPIService);

})(appControllers);
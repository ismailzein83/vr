(function (appControllers) {
    'use strict';

    productAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function productAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var moduleName = Demo_Module_ModuleConfig.moduleName;
        var controller = 'Demo_Product';

        function GetFilteredProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(moduleName, controller, 'GetFilteredProducts'), input);
        }

        function GetProductById(productId) {
            return BaseAPIService.get(UtilsService.getServiceURL(moduleName, controller, 'GetProductById'), {
                productId: productId
            });
        }

        function AddProduct(product) {
            return BaseAPIService.post(UtilsService.getServiceURL(moduleName, controller, 'AddProduct'), product);
        }

        function UpdateProduct(product) {
            return BaseAPIService.post(UtilsService.getServiceURL(moduleName, controller, 'UpdateProduct'), product);
        }

        function GetProductSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(moduleName, controller, 'GetProductSettingsConfigs'));
        }

        function GetOperatingSystemConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(moduleName, controller, 'GetOperatingSystemConfigs'));
        }

        return {
            GetFilteredProducts: GetFilteredProducts,
            GetProductById: GetProductById,
            AddProduct: AddProduct,
            UpdateProduct: UpdateProduct,
            GetProductSettingsConfigs: GetProductSettingsConfigs,
            GetOperatingSystemConfigs: GetOperatingSystemConfigs
        };
    }

    appControllers.service('Demo_Module_ProductAPIService', productAPIService);
})(appControllers);
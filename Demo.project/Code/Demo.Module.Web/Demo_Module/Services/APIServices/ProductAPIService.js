(function (appControllers) {

    'use strict';

    ProductAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function ProductAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'VRProduct';

        function GetFilteredProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredProducts"), input);
        }

        function GetProductById(productId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetProductById'),
                {
                    productId: productId
                });
        }

        function GetProductsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetProductsInfo"));
        }

        function AddProduct(product) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddProduct"), product);
        } 
        function UpdateProduct(product) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateProduct"), product);
        }


        return ({
            GetProductById: GetProductById,
            GetProductsInfo: GetProductsInfo,
            GetFilteredProducts: GetFilteredProducts,
            AddProduct: AddProduct,
            UpdateProduct: UpdateProduct
        });
    }


    appControllers.service('Demo_Module_ProductAPIService', ProductAPIService);
})(appControllers);
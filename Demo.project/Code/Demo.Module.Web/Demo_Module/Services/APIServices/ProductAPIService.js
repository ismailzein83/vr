(function (appControllers) {

    'use strict';

    ProductAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function ProductAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'VRProduct';

        function GetFilteredProducts(input) {
           //console.log(input);
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredProducts"), input);
        }

        function GetProductById(Id) {
            console.log(Id);
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetProductById'),
                { productId: Id }
                );
        }


        function GetProductsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetProductsInfo"));
        }

        function AddProduct(product) {
            console.log("a");
            console.log(product);
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddProduct"), product);
        } 
        function UpdateProduct(product) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateProduct"), product);
        }
        function DeleteProduct(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteProduct'), {
                productId: Id
            });
        }

        return ({
            GetProductById: GetProductById,
            GetProductsInfo: GetProductsInfo,
            GetFilteredProducts: GetFilteredProducts,
            AddProduct: AddProduct,
            UpdateProduct: UpdateProduct,
            DeleteProduct: DeleteProduct
        });
    }


    appControllers.service('Demo_Module_ProductAPIService', ProductAPIService);
})(appControllers);
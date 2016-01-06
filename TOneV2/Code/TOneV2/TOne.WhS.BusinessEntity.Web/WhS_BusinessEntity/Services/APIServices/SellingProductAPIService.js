(function (appControllers) {

    "use strict";
    sellingProductAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];
    function sellingProductAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredSellingProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingProduct","GetFilteredSellingProducts"), input);
        }

        function GetSellingProduct(sellingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingProduct","GetSellingProduct"), {
                sellingProductId: sellingProductId
            });
        }
        function AddSellingProduct(sellingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingProduct","AddSellingProduct"), sellingProductObject);
        }

        function UpdateSellingProduct(sellingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingProduct","UpdateSellingProduct"), sellingProductObject);
        }
        function DeleteSellingProduct(sellingProductId) {
            
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingProduct","DeleteSellingProduct"), { sellingProductId: sellingProductId });
        }
        function GetSellingProductsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingProduct", "GetSellingProductsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        return ({
            GetFilteredSellingProducts: GetFilteredSellingProducts,
            AddSellingProduct: AddSellingProduct,
            UpdateSellingProduct: UpdateSellingProduct,
            DeleteSellingProduct: DeleteSellingProduct,
            GetSellingProduct: GetSellingProduct,
            GetSellingProductsInfo: GetSellingProductsInfo
        });
    }

    appControllers.service('WhS_BE_SellingProductAPIService', sellingProductAPIService);
})(appControllers);
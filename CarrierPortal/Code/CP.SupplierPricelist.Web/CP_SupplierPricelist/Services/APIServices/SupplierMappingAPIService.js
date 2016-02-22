(function (appControllers) {

    "use strict";
    function supplierMappingAPIService(baseApiService, utilsService, moduleConfig) {
        function GetCustomerSuppliers() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "GetCustomerSuppliers"));
        }
        return ({
            GetCustomerSuppliers: GetCustomerSuppliers
        });
       
    }
    supplierMappingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_SupplierMappingAPIService', supplierMappingAPIService);

})(appControllers);
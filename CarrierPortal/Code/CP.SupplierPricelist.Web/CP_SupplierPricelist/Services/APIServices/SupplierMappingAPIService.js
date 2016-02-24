(function (appControllers) {

    "use strict";
    function supplierMappingAPIService(baseApiService, utilsService, moduleConfig) {
        function GetCustomerSuppliers() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "GetCustomerSuppliers"));
        }
        function GetFilteredCustomerSupplierMappings(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "GetFilteredCustomerSupplierMappings"), input);
        }
        function GetCustomerSupplierMapping(supplierMappingId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "GetCustomerSupplierMapping"), {
                supplierMappingId: supplierMappingId
            });
        }
        function AddCustomerSupplierMapping(supplierMapping) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "AddCustomerSupplierMapping"), supplierMapping);
        }

        return ({
            GetCustomerSuppliers: GetCustomerSuppliers,
            GetFilteredCustomerSupplierMappings:GetFilteredCustomerSupplierMappings,
            AddCustomerSupplierMapping: AddCustomerSupplierMapping
        });
       
    }
    supplierMappingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_SupplierMappingAPIService', supplierMappingAPIService);

})(appControllers);
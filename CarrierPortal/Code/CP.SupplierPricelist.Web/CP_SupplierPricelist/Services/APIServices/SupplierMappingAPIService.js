(function (appControllers) {

    "use strict";
    function supplierMappingAPIService(baseApiService, utilsService, moduleConfig) {
        function GetCustomerSuppliers(serializedFilter) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "GetCustomerSuppliers"), {
                   serializedFilter: serializedFilter
            });
        }
        function GetFilteredCustomerSupplierMappings(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "GetFilteredCustomerSupplierMappings"), input);
        }
        function GetCustomerSupplierMapping(userId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "GetCustomerSupplierMapping"), {
                userId: userId
            });
        }
        function AddCustomerSupplierMapping(supplierMapping) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "AddCustomerSupplierMapping"), supplierMapping);
        }
        function UpdateCustomerSupplierMapping(supplierMapping) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "UpdateCustomerSupplierMapping"), supplierMapping);
        }
        return ({
            GetCustomerSuppliers: GetCustomerSuppliers,
            GetFilteredCustomerSupplierMappings: GetFilteredCustomerSupplierMappings,
            GetCustomerSupplierMapping:GetCustomerSupplierMapping,
            AddCustomerSupplierMapping: AddCustomerSupplierMapping,
            UpdateCustomerSupplierMapping: UpdateCustomerSupplierMapping
        });
       
    }
    supplierMappingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_SupplierMappingAPIService', supplierMappingAPIService);

})(appControllers);
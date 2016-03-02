(function (appControllers) {

    "use strict";
    function supplierMappingAPIService(baseApiService, utilsService, SecurityService , moduleConfig) {
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
        function HasAddCustomerSupplierMapping() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(moduleConfig.moduleName, "SupplierMapping", ['AddCustomerSupplierMapping']));
        }
        function UpdateCustomerSupplierMapping(supplierMapping) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "SupplierMapping", "UpdateCustomerSupplierMapping"), supplierMapping);
        }       
        function HasUpdateCustomerSupplierMapping() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(moduleConfig.moduleName, "SupplierMapping", ['UpdateCustomerSupplierMapping']));
        }
        return ({
            GetCustomerSuppliers: GetCustomerSuppliers,
            GetFilteredCustomerSupplierMappings: GetFilteredCustomerSupplierMappings,
            GetCustomerSupplierMapping:GetCustomerSupplierMapping,
            AddCustomerSupplierMapping: AddCustomerSupplierMapping,
            HasAddCustomerSupplierMapping: HasAddCustomerSupplierMapping,
            UpdateCustomerSupplierMapping: UpdateCustomerSupplierMapping,           
            HasUpdateCustomerSupplierMapping: HasUpdateCustomerSupplierMapping
        });
       
    }
    supplierMappingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_SupplierMappingAPIService', supplierMappingAPIService);

})(appControllers);
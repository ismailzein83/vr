(function (appControllers) {

    "use strict";
    function customerManagmentApiService(baseApiService, utilsService, moduleConfig) {

        function UpdateCustomer(object) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "UpdateCustomer"), object);
        }
        function AddCustomer(object) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "AddCustomer"), object);
        }
        function GetCustomerTemplates() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "GetCustomerTemplates"));
        }
        function GetFilteredCustomers(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "GetFilteredCustomers"), input);
        }
        return ({
            GetCustomerTemplates: GetCustomerTemplates,
            UpdateCustomer: UpdateCustomer,
            AddCustomer: AddCustomer,
            GetFilteredCustomers: GetFilteredCustomers

        });
    }
    customerManagmentApiService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_CustomerManagmentAPIService', customerManagmentApiService);

})(appControllers);
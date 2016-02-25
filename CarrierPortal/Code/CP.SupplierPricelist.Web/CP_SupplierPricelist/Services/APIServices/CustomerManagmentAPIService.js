(function (appControllers) {

    "use strict";
    function customerManagmentApiService(baseApiService, utilsService, moduleConfig) {
        function AddUser(object) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "AddUser"), object);
        }
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
        function GetCustomer(customerId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "GetCustomer"), {
                customerId: customerId
            });
        }
        return ({
            GetCustomerTemplates: GetCustomerTemplates,
            UpdateCustomer: UpdateCustomer,
            AddCustomer: AddCustomer,
            GetFilteredCustomers: GetFilteredCustomers,
            GetCustomer: GetCustomer,
            AddUser: AddUser
        });
    }
    customerManagmentApiService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_CustomerManagmentAPIService', customerManagmentApiService);

})(appControllers);
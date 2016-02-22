(function (appControllers) {

    "use strict";
    function customerManagmentApiService(baseApiService, utilsService, moduleConfig) {

        function UpdateCustomer() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "UpdateCustomer"));
        }
        function AddCustomer() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "AddCustomer"));
        }
        function GetCustomerTemplates() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "GetCustomerTemplates"));
        }
        return ({
            GetCustomerTemplates: GetCustomerTemplates,
            UpdateCustomer: UpdateCustomer,
            AddCustomer: AddCustomer

        });
    }
    customerManagmentApiService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_CustomerManagmentAPIService', customerManagmentApiService);

})(appControllers);
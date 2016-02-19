(function (appControllers) {

    "use strict";
    function customerManagmentApiService(baseApiService, utilsService, moduleConfig) {

        function GetCustomers(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "GetCustomers"), input);
        }
        return ({
            GetCustomers: GetCustomers
        });
    }
    customerManagmentApiService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_CustomerManagmentAPIService', customerManagmentApiService);

})(appControllers);
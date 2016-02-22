(function (appControllers) {

    "use strict";
    function customerManagmentApiService(baseApiService, utilsService, moduleConfig) {

        function GetCustomerTemplates() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "Customer", "GetCustomerTemplates"));
        }
        return ({
            GetCustomerTemplates: GetCustomerTemplates
        });
    }
    customerManagmentApiService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_CustomerManagmentAPIService', customerManagmentApiService);

})(appControllers);
(function (appControllers) {

    "use strict";
    function customerUserAPIService(baseApiService, utilsService, moduleConfig) {
       
        function GetFilteredCustomerUsers(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "GetFilteredCustomerUsers"), input);
        }

        function AddCustomerUser(object) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "AddCustomerUser"), object);
        }
        function DeleteCustomerUser(userId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "DeleteCustomerUser"), { userId: userId });
        }

        return ({
            GetFilteredCustomerUsers: GetFilteredCustomerUsers,
            AddCustomerUser: AddCustomerUser,
            DeleteCustomerUser: DeleteCustomerUser
        });
       
    }

    customerUserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_CustomerUserAPIService', customerUserAPIService);

})(appControllers);
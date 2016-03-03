(function (appControllers) {

    "use strict";
    function customerUserAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {
       
        function GetFilteredCustomerUsers(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "GetFilteredCustomerUsers"), input);
        }
        function GetHasCurrentCustomerId(){
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "GetHasCurrentCustomerId"));
        }
        function AddCustomerUser(object) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "AddCustomerUser"), object);
        }

        function DeleteCustomerUser(userId) {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "CustomerUser", "DeleteCustomerUser"), { userId: userId });
        }

        function HasAddCustomerUser() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(moduleConfig.moduleName, "CustomerUser", ['AddCustomerUser']));
        }

        function HasDeleteCustomerUser() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(moduleConfig.moduleName, "CustomerUser", ['DeleteCustomerUser']));
        }

        return ({
            GetFilteredCustomerUsers: GetFilteredCustomerUsers,
            AddCustomerUser: AddCustomerUser,
            DeleteCustomerUser: DeleteCustomerUser,
            HasAddCustomerUser: HasAddCustomerUser,
            HasDeleteCustomerUser: HasDeleteCustomerUser,
            GetHasCurrentCustomerId: GetHasCurrentCustomerId
        });
       
    }

    customerUserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'CP_SupPriceList_ModuleConfig'];
    appControllers.service('CP_SupplierPricelist_CustomerUserAPIService', customerUserAPIService);

})(appControllers);
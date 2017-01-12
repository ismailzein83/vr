(function (appControllers) {

    "use strict";
    AccountServiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountServiceAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "AccountService";

        function GetFilteredAccountServices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredAccountServices"), input);
        }

        function GetAccountService(accountServiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountService"), {
                accountServiceId: accountServiceId
            });
        }

        function AddAccountService(accountServiceObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "AddAccountService"), accountServiceObject);
        }

        function UpdateAccountService(accountServiceObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "UpdateAccountService"), accountServiceObject);
        }

        function HasUpdateAccountServicePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateAccountService']));
        }

        function HasAddAccountServicePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAccountService']));
        }
        function GetAccountServiceDetail(accountServiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountServiceDetail"), {
                accountServiceId: accountServiceId
            });
        }
        return ({
            GetFilteredAccountServices: GetFilteredAccountServices,
            GetAccountService: GetAccountService,
            AddAccountService: AddAccountService,
            UpdateAccountService: UpdateAccountService,
            HasUpdateAccountServicePermission: HasUpdateAccountServicePermission,
            HasAddAccountServicePermission: HasAddAccountServicePermission,
            GetAccountServiceDetail: GetAccountServiceDetail
        });
    }

    appControllers.service('Retail_BE_AccountServiceAPIService', AccountServiceAPIService);

})(appControllers);
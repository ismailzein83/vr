(function (appControllers) {

    'use strict';

    AccountPackageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountPackageAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService)
    {
        var controllerName = 'AccountPackage';

        function GetFilteredAccountPackages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredAccountPackages'), input);
        }

        function GetAccountPackage(accountPackageId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAccountPackage'), {
                accountPackageId: accountPackageId
            });
        }

        function AddAccountPackage(accountPackage) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAccountPackage'), accountPackage);
        }

        function HasAddAccountPackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAccount']));
        }

        return {
            GetFilteredAccountPackages: GetFilteredAccountPackages,
            AddAccountPackage: AddAccountPackage,
            HasAddAccountPackagePermission: HasAddAccountPackagePermission
        };
    }

    appControllers.service('Retail_BE_AccountPackageAPIService', AccountPackageAPIService);

})(appControllers);
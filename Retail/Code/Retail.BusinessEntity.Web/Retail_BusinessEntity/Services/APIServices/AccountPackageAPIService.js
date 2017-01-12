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
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountPackage'), {
                accountPackageId: accountPackageId
            });
        }

        function AddAccountPackage(accountPackageToAdd) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAccountPackage'), accountPackageToAdd);
        }

        function HasViewAccountPackagesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredAccountPackages']));
        }

        function HasAddAccountPackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAccountPackage']));
        }

        return {
            GetFilteredAccountPackages: GetFilteredAccountPackages,
            GetAccountPackage: GetAccountPackage,
            AddAccountPackage: AddAccountPackage,
            HasViewAccountPackagesPermission: HasViewAccountPackagesPermission,
            HasAddAccountPackagePermission: HasAddAccountPackagePermission
        };
    }

    appControllers.service('Retail_BE_AccountPackageAPIService', AccountPackageAPIService);

})(appControllers);
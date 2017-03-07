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

        function DoesUserHaveAddAccess(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'DoesUserHaveAddAccess'), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }

        return {
            GetFilteredAccountPackages: GetFilteredAccountPackages,
            GetAccountPackage: GetAccountPackage,
            AddAccountPackage: AddAccountPackage,
            DoesUserHaveAddAccess: DoesUserHaveAddAccess
        };
    }

    appControllers.service('Retail_BE_AccountPackageAPIService', AccountPackageAPIService);

})(appControllers);
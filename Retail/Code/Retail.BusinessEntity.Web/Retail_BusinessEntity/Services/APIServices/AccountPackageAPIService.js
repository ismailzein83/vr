(function (appControllers) {

    'use strict';

    AccountPackageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountPackageAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
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

        function UpdateAccountPackage(accountPackage) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateAccountPackage'), accountPackage);
        }


        function ExportRates(accountBEDefinitionId, accountId) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'ExportRates'), { AccountBEDefinitionId: accountBEDefinitionId, AccountId: accountId },
            {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        return {
            GetFilteredAccountPackages: GetFilteredAccountPackages,
            GetAccountPackage: GetAccountPackage,
            AddAccountPackage: AddAccountPackage,
            DoesUserHaveAddAccess: DoesUserHaveAddAccess,
            UpdateAccountPackage: UpdateAccountPackage,
            ExportRates: ExportRates
        };
    }

    appControllers.service('Retail_BE_AccountPackageAPIService', AccountPackageAPIService);

})(appControllers);
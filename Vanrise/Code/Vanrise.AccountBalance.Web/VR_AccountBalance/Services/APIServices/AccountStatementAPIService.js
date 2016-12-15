(function (appControllers) {

    'use strict';

    AccountStatementAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function AccountStatementAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'AccountStatement';
        function GetFilteredAccountStatments(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetFilteredAccountStatments"), input);
        }

        return {
            GetFilteredAccountStatments: GetFilteredAccountStatments,
        };
    }

    appControllers.service('VR_AccountBalance_AccountStatementAPIService', AccountStatementAPIService);

})(appControllers);
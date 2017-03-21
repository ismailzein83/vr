(function (appControllers) {

    'use strict';

    AccountTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function AccountTypeAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'AccountType';
        function GetAccountSelector(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountSelector"), { accountTypeId: accountTypeId });
        }
        function GetAccountTypeSettings(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountTypeSettings"), { accountTypeId: accountTypeId });
        }
        function GetAccountTypeSourceSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountTypeSourceSettingsConfigs"));
        }
        function GetAccountTypeSourcesFields(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountTypeSourcesFields"), query);
        }
        function GetAccountTypeSourceFields(source) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountTypeSourceFields"),source);
        }
        function ConvertToGridColumnAttribute(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "ConvertToGridColumnAttribute"), {
                accountTypeId: accountTypeId
            });
        }
        return {
            GetAccountSelector: GetAccountSelector,
            GetAccountTypeSettings: GetAccountTypeSettings,
            GetAccountTypeSourceSettingsConfigs: GetAccountTypeSourceSettingsConfigs,
            GetAccountTypeSourcesFields: GetAccountTypeSourcesFields,
            GetAccountTypeSourceFields: GetAccountTypeSourceFields,
            ConvertToGridColumnAttribute: ConvertToGridColumnAttribute
        };
    }

    appControllers.service('VR_AccountBalance_AccountTypeAPIService', AccountTypeAPIService);

})(appControllers);
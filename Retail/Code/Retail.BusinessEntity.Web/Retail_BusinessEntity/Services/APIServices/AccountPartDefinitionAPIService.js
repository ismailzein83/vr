(function (appControllers) {

    'use strict';

    AccountPartDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountPartDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService)
    {
        var controllerName = 'AccountPartDefinition';

        function GetFilteredAccountPartDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredAccountPartDefinitions'), input);
        }

        function GetAccountPartDefinition(accountPartDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountPartDefinition'), {
                accountPartDefinitionId: accountPartDefinitionId
            });
        }

        function GetAccountPartDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountPartDefinitionsInfo"), {
                filter: filter
            });
        }

        function GetAccountPartDefinitionPartDefinitionExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountPartDefinitionPartDefinitionExtensionConfigs"));
        }

        function AddAccountPartDefinition(accountPartDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAccountPartDefinition'), accountPartDefinition);
        }

        function UpdateAccountPartDefinition(accountPartDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateAccountPartDefinition'), accountPartDefinition);
        }

        function HasViewAccountPartDefinitionsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredAccountPartDefinitions']));
        }

        function HasAddAccountPartDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAccountPartDefinition']));
        }

        function HasUpdateAccountPartDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateAccountPartDefinition']));
        }

        return {
            GetFilteredAccountPartDefinitions: GetFilteredAccountPartDefinitions,
            GetAccountPartDefinition: GetAccountPartDefinition,
            GetAccountPartDefinitionsInfo: GetAccountPartDefinitionsInfo,
            GetAccountPartDefinitionPartDefinitionExtensionConfigs: GetAccountPartDefinitionPartDefinitionExtensionConfigs,
            AddAccountPartDefinition: AddAccountPartDefinition,
            UpdateAccountPartDefinition: UpdateAccountPartDefinition,
            HasViewAccountPartDefinitionsPermission: HasViewAccountPartDefinitionsPermission,
            HasAddAccountPartDefinitionPermission: HasAddAccountPartDefinitionPermission,
            HasUpdateAccountPartDefinitionPermission: HasUpdateAccountPartDefinitionPermission
        };
    }

    appControllers.service('Retail_BE_AccountPartDefinitionAPIService', AccountPartDefinitionAPIService);

})(appControllers);
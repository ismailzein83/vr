(function (appControllers) {
    'use strict';

    AgentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AgentAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'Agent';

        function GetFilteredAgents(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredAgents'), input);
        }

        function AddAgent(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAgent'), account);
        }

        function UpdateAgent(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateAgent'), account);
        }

        function GetAgentsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAgentsInfo"));
        }

        function HasAddAgentPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAgent']));
        }

        function HasViewAgentsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredAgents']));
        }

        function HasUpdateAgentPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateAgent']));
        }


        return {
            GetFilteredAgents: GetFilteredAgents,
            AddAgent: AddAgent,
            UpdateAgent: UpdateAgent,
            GetAgentsInfo: GetAgentsInfo,
            HasAddAgentPermission: HasAddAgentPermission,
            HasViewAgentsPermission: HasViewAgentsPermission,
            HasUpdateAgentPermission: HasUpdateAgentPermission
        };

    }
    appControllers.service('Retail_BE_AgentAPIService', AgentAPIService);

})(appControllers);
(function (appControllers) {

    "use strict";

    RingoAgentNumberRequestAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Ringo_ModuleConfig', 'SecurityService'];

    function RingoAgentNumberRequestAPIService(BaseAPIService, UtilsService, Retail_Ringo_ModuleConfig, SecurityService) {

        var controllerName = "RingoAgentNumberRequest";

        function GetFilteredAgentNumberRequests(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Ringo_ModuleConfig.moduleName, controllerName, 'GetFilteredAgentNumberRequests'), input);
        }

        function AddAgentNumberRequest(agentNumberRequest) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Ringo_ModuleConfig.moduleName, controllerName, 'AddAgentNumberRequest'), agentNumberRequest);
        }
                
        function UpdateAgentNumberRequest(agentNumberRequest) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Ringo_ModuleConfig.moduleName, controllerName, 'UpdateAgentNumberRequest'), agentNumberRequest);
        }
        function HasUpdateAgentNumberRequestPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_Ringo_ModuleConfig.moduleName, controllerName, ['UpdateAgentNumberRequest']));
        }
        function GetAgentNumberRequest(agentNumberRequestId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Ringo_ModuleConfig.moduleName, controllerName, 'GetAgentNumberRequest'), {
                agentNumberRequestId: agentNumberRequestId
            });
        }
        return ({
            GetFilteredAgentNumberRequests: GetFilteredAgentNumberRequests,
            AddAgentNumberRequest: AddAgentNumberRequest,
            UpdateAgentNumberRequest: UpdateAgentNumberRequest,
            HasUpdateAgentNumberRequestPermission: HasUpdateAgentNumberRequestPermission,
            GetAgentNumberRequest: GetAgentNumberRequest
        });
    }

    appControllers.service('Retail_Ringo_AgentNumberRequestAPIService', RingoAgentNumberRequestAPIService);

})(appControllers);
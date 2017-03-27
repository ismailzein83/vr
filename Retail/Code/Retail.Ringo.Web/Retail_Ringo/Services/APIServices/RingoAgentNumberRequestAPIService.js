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
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateAgentNumberRequest'), agentNumberRequest);
        }
        return ({
            GetFilteredAgentNumberRequests: GetFilteredAgentNumberRequests,
            AddAgentNumberRequest: AddAgentNumberRequest,
            UpdateAgentNumberRequest: UpdateAgentNumberRequest
        });
    }

    appControllers.service('Retail_Ringo_AgentNumberRequestAPIService', RingoAgentNumberRequestAPIService);

})(appControllers);
(function (appControllers) {

    "use strict";
    AgentNumbersAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'CP_Ringo_ModuleConfig'];

    function AgentNumbersAPIService(BaseAPIService, UtilsService, SecurityService, CP_Ringo_ModuleConfig) {
        var controllerName = "AgentNumbers";

        function GetFilteredAgentNumbers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CP_Ringo_ModuleConfig.moduleName, controllerName, 'GetFilteredAgentNumbers'), input);
        }

        function AddAgentNumbersRequest(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CP_Ringo_ModuleConfig.moduleName, controllerName, "AddAgentNumbersRequest"), input);
        }
        return ({
            GetFilteredAgentNumbers: GetFilteredAgentNumbers,
            AddAgentNumbersRequest: AddAgentNumbersRequest
        });
    }
    appControllers.service('CP_Ringo_AgentNumbersAPIService', AgentNumbersAPIService);

})(appControllers);
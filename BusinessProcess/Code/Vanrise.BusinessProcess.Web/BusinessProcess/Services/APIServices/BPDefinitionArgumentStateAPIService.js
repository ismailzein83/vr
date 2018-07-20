(function (appControllers) {

    "use strict";

    BusinessProcess_BPDefinitionArgumentStateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPDefinitionArgumentStateAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {
        var controllerName = 'BPDefinitionArgumentState';

        function GetBPDefinitionArgumentState(bpDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPDefinitionArgumentState"), {
                bpDefinitionId: bpDefinitionId
            });
        }

        return ({
            GetBPDefinitionArgumentState: GetBPDefinitionArgumentState
        });

    }

    appControllers.service('BusinessProcess_BPDefinitionArgumentStateAPIService', BusinessProcess_BPDefinitionArgumentStateAPIService);

})(appControllers);
(function (appControllers) {

    "use strict";
    BusinessProcess_BPInstanceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPInstanceAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetUpdated"), input);
        }

        function GetBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetBeforeId"), input);
        }

        return ({
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId
        });
    }

    appControllers.service('BusinessProcess_BPInstanceAPIService', BusinessProcess_BPInstanceAPIService);

})(appControllers);
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

        function GetFilteredBPInstances(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetFilteredBPInstances"), input);
        }

        function GetBPInstance(id) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetBPInstance"), {
                id: id
            });
        }

        return ({
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId,
            GetFilteredBPInstances: GetFilteredBPInstances,
            GetBPInstance: GetBPInstance
        });
    }

    appControllers.service('BusinessProcess_BPInstanceAPIService', BusinessProcess_BPInstanceAPIService);

})(appControllers);
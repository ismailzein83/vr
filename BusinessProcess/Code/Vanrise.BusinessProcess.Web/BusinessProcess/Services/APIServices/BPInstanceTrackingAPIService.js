(function (appControllers) {

    "use strict";
    BusinessProcess_BPInstanceTrackingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPInstanceTrackingAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {


        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstanceTracking", "GetUpdated"), input);
        }

        function GetBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstanceTracking", "GetBeforeId"), input);
        }

        return ({
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId
        });
    }

    appControllers.service('BusinessProcess_BPInstanceTrackingAPIService', BusinessProcess_BPInstanceTrackingAPIService);

})(appControllers);
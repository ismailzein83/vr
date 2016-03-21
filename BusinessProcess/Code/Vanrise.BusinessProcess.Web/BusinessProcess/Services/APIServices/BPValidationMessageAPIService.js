(function (appControllers) {

    "use strict";
    BusinessProcess_BPValidationMessageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPValidationMessageAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {
        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPValidationMessage", "GetUpdated"), input);
        }

        function GetBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPValidationMessage", "GetBeforeId"), input);
        }

        function GetFilteredBPValidationMessage(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPValidationMessage", "GetFilteredBPValidationMessage"), input);
        }

        return ({
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId,
            GetFilteredBPValidationMessage: GetFilteredBPValidationMessage
        });
    }

    appControllers.service('BusinessProcess_BPValidationMessageAPIService', BusinessProcess_BPValidationMessageAPIService);

})(appControllers);
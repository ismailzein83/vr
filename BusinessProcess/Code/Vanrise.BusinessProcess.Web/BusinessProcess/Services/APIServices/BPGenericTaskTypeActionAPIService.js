(function (appControllers) {

    "use strict";
    BusinessProcess_BPGenericTaskTypeActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPGenericTaskTypeActionAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        var controller = "BPGenericTaskTypeAction";
        function GetTaskTypeActions(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controller, "GetTaskTypeActions"), {
                taskId: taskId
            });
        }


        return ({
            GetTaskTypeActions: GetTaskTypeActions
        });
    }

    appControllers.service('BusinessProcess_BPGenericTaskTypeActionAPIService', BusinessProcess_BPGenericTaskTypeActionAPIService);

})(appControllers);
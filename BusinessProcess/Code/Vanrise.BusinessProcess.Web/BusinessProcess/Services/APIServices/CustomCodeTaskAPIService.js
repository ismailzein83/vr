(function (appControllers) {

    "use strict";
    CustomCodeTaskAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function CustomCodeTaskAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {
        var conttrolerName = "CustomCodeTask";

        function TryCompileCustomCodeTask(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, conttrolerName, "TryCompileCustomCodeTask"), input);
        }

        return ({
            TryCompileCustomCodeTask: TryCompileCustomCodeTask
        });
    }

    appControllers.service('BusinessProcess_CustomCodeTaskAPIService', CustomCodeTaskAPIService);

})(appControllers);
(function (appControllers) {

    "use strict";
    whSJazzSwitchCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzSwitchCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "WhSJazzSwitchCode";

        function GetAllSwitchCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllSwitchCodes'), {
            });
        }
        function GetSwitchCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetSwitchCodesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllSwitchCodes: GetAllSwitchCodes,
            GetSwitchCodesInfo: GetSwitchCodesInfo
        });
    }
    
    appControllers.service("WhS_Jazz_SwitchCodeAPIService", whSJazzSwitchCodeAPIService);
})(appControllers);
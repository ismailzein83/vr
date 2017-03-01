
(function (appControllers) {

    "use strict";
    SwitchDashboardAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig'];

    function SwitchDashboardAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig) {

        var controllerName = "SwitchDashboard";

        function GetSwitchDashboardManagerResult() {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetSwitchDashboardManagerResult"));
        }

        return ({
            GetSwitchDashboardManagerResult: GetSwitchDashboardManagerResult,
        });
    }

    appControllers.service('NP_IVSwitch_SwitchDashboardAPIService', SwitchDashboardAPIService);

})(appControllers);
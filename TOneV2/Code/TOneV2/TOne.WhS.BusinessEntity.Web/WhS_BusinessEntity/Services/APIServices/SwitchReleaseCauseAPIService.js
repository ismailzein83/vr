(function (appControllers) {
    "use strict";
    switchReleaseCauseAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];
    function switchReleaseCauseAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = "SwitchReleaseCause";
        function GetFilteredSwitchReleaseCauses(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSwitchReleaseCauses"), input);
        }
        function AddSwitchReleaseCause(switchReleaseCause)
        {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddSwitchReleaseCause"), switchReleaseCause);
        }
        function GetSwitchReleaseCause(switchReleaseCauseId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSwitchReleaseCause"), { switchReleaseCauseId: switchReleaseCauseId });
        }
        function UpdateSwitchReleaseCause(switchReleaseCause) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSwitchReleaseCause"), switchReleaseCause);
        }
        return ({
            GetFilteredSwitchReleaseCauses: GetFilteredSwitchReleaseCauses,
            AddSwitchReleaseCause: AddSwitchReleaseCause,
            GetSwitchReleaseCause: GetSwitchReleaseCause,
            UpdateSwitchReleaseCause: UpdateSwitchReleaseCause
        });
    }
    appControllers.service("WhS_BE_SwitchReleaseCauseAPIService", switchReleaseCauseAPIService);
})(appControllers);
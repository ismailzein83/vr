(function (appControllers) {

    "use strict";

    switchAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];

    function switchAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = "Switch";

        function GetFilteredSwitches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSwitches"), input);
        }

        function GetSwitch(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSwitch"), {
                switchId: switchId
            });
        }

        function GetSwitchesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSwitchesInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function AddSwitch(switchObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddSwitch"), switchObject);
        }

        function UpdateSwitch(switchObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSwitch"), switchObject);
        }

        function DeleteSwitch(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "DeleteSwitch"), { switchId: switchId });
        }

        function HasUpdateSwitchPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateSwitch']));
        }

        function HasAddSwitchPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddSwitch']));
        }

        function HasDeleteSwitchPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['DeleteSwitch']));
        }
       
        return ({
            GetFilteredSwitches: GetFilteredSwitches,
            GetSwitch: GetSwitch,
            AddSwitch: AddSwitch,
            UpdateSwitch: UpdateSwitch,
            DeleteSwitch: DeleteSwitch,
            GetSwitchesInfo: GetSwitchesInfo,
            HasUpdateSwitchPermission: HasUpdateSwitchPermission,
            HasAddSwitchPermission: HasAddSwitchPermission,
            HasDeleteSwitchPermission: HasDeleteSwitchPermission
        });
    }

    appControllers.service("WhS_BE_SwitchAPIService", switchAPIService);
})(appControllers);
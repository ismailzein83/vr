(function (appControllers) {

    "use strict";
    switchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function switchAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredSwitches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Switch", "GetFilteredSwitches"), input);
        }

        function GetSwitch(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Switch", "GetSwitch"), {
                switchId: switchId
            });
        }

        function AddSwitch(switchObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Switch", "AddSwitch"), switchObject);
        }

        function UpdateSwitch(switchObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Switch", "UpdateSwitch"), switchObject);
        }

        function DeleteSwitch(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Switch", "DeleteSwitch"), { switchId: switchId });
        }


        return ({
            GetFilteredSwitches: GetFilteredSwitches,
            GetSwitch: GetSwitch,
            AddSwitch: AddSwitch,
            UpdateSwitch: UpdateSwitch,
            DeleteSwitch: DeleteSwitch
        });
    }

    appControllers.service('WhS_BE_SwitchAPIService', switchAPIService);
})(appControllers);
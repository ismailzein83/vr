(function (appControllers) {

    "use strict";
    switchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PSTN_BE_ModuleConfig'];

    function switchAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig) {

        var controllerName = 'Switch';

        function GetFilteredSwitches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSwitches"), input);
        }

        function GetSwitchById(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetSwitchById"), {
                switchId: switchId
            });
        }
        function GetSwitchesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetSwitchesInfo"), {
                serializedFilter: filter
            });
        }
        function GetSwitches() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetSwitches"));
        }

        function GetSwitchAssignedDataSources() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetSwitchAssignedDataSources"));
        }

        function UpdateSwitch(switchObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "UpdateSwitch"), switchObj);
        }

        function AddSwitch(switchObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "AddSwitch"), switchObj);
        }
        function DeleteSwitch(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "DeleteSwitch"), {
                switchId: switchId
            });
        }


        return ({
            GetFilteredSwitches: GetFilteredSwitches,
            GetSwitchById: GetSwitchById,
            GetSwitches: GetSwitches,
            GetSwitchesInfo: GetSwitchesInfo,
            GetSwitchAssignedDataSources: GetSwitchAssignedDataSources,
            UpdateSwitch: UpdateSwitch,
            AddSwitch: AddSwitch,
            DeleteSwitch: DeleteSwitch

        });
    }

    appControllers.service('CDRAnalysis_PSTN_SwitchAPIService', switchAPIService);

})(appControllers);
(function (appControllers) {

    "use strict";
    switchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PSTN_BE_ModuleConfig'];

    function switchAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig) {


        function GetFilteredSwitches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","GetFilteredSwitches"), input);
        }

        function GetSwitchById(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","GetSwitchById"), {
                switchId: switchId
            });
        }
        function GetSwitchesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","GetSwitchesInfo"), {
                serializedFilter: filter
            });
        }
        function GetSwitches() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","GetSwitches"));
        }

        function GetSwitchAssignedDataSources() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","GetSwitchAssignedDataSources"));
        }

        function UpdateSwitch(switchObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","UpdateSwitch"), switchObj);
        }

        function AddSwitch(switchObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","AddSwitch"), switchObj);
        }
        function DeleteSwitch(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Switch","DeleteSwitch"), {
                switchId: switchId
            });
        }
        return ({
            GetFilteredSwitches: GetFilteredSwitches,
            GetSwitchById: GetSwitchById,
            GetSwitches: GetSwitches,
            GetSwitchesInfo:GetSwitchesInfo,
            GetSwitchAssignedDataSources: GetSwitchAssignedDataSources,
            UpdateSwitch: UpdateSwitch,
            AddSwitch: AddSwitch,
            DeleteSwitch: DeleteSwitch

        });
    }

    appControllers.service('CDRAnalysis_PSTN_SwitchAPIService', switchAPIService);

})(appControllers);
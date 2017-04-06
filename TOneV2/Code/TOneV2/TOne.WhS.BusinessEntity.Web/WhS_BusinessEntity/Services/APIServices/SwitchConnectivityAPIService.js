(function (appControllers) {

    'use strict';

    SwitchConnectivityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function SwitchConnectivityAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = 'SwitchConnectivity';

        function GetFilteredSwitchConnectivities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredSwitchConnectivities'), input);
        }

        function GetSwitchConnectivity(switchConnectivityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSwitchConnectivity'), {
                switchConnectivityId: switchConnectivityId
            });
        }
        function GetSwitcheConnectivitiesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSwitcheConnectivitiesInfo'));
        }
        function AddSwitchConnectivity(switchConnectivity) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'AddSwitchConnectivity'), switchConnectivity);
        }

        function UpdateSwitchConnectivity(switchConnectivity) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'UpdateSwitchConnectivity'), switchConnectivity);
        }

        function HasAddSwitchConnectivityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddSwitchConnectivity']));
        }

        function HasEditSwitchConnectivityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateSwitchConnectivity']));
        }
        function GetSwitchConnectivityHistoryDetailbyHistoryId(switchConnectivityHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSwitchConnectivityHistoryDetailbyHistoryId'), {
                switchConnectivityHistoryId: switchConnectivityHistoryId
            });
        }

        return ({
            GetFilteredSwitchConnectivities: GetFilteredSwitchConnectivities,
            GetSwitchConnectivity: GetSwitchConnectivity,
            AddSwitchConnectivity: AddSwitchConnectivity,
            UpdateSwitchConnectivity: UpdateSwitchConnectivity,
            HasAddSwitchConnectivityPermission: HasAddSwitchConnectivityPermission,
            HasEditSwitchConnectivityPermission: HasEditSwitchConnectivityPermission,
            GetSwitcheConnectivitiesInfo: GetSwitcheConnectivitiesInfo,
            GetSwitchConnectivityHistoryDetailbyHistoryId: GetSwitchConnectivityHistoryDetailbyHistoryId
        });
    }

    appControllers.service('WhS_BE_SwitchConnectivityAPIService', SwitchConnectivityAPIService);

})(appControllers);
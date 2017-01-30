(function (appControllers) {

    "use strict";

    vrConnectionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function vrConnectionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'VRConnection';

        function GetFilteredVRConnections(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRConnections'), input);
        }
        function GetVRConnectionInfos(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRConnectionInfos"), {
                filter: filter
            });
        };
        function GetVRConnection(vrConnectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRConnection'), {
                VRConnectionId: vrConnectionId
            });
        }

        function AddVRConnection(vrConnectionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddVRConnection'), vrConnectionItem);
        }

        function UpdateVRConnection(vrConnectionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateVRConnection'), vrConnectionItem);
        }


        function HasAddVRConnectionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddVRConnection']));
        }

        function HasEditVRConnectionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateVRConnection']));
        }
        function GetVRConnectionConfigTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRConnectionConfigTypes"));
        };

        return ({
            GetVRConnectionInfos: GetVRConnectionInfos,
            GetVRConnectionConfigTypes: GetVRConnectionConfigTypes,
            GetFilteredVRConnections: GetFilteredVRConnections,
            GetVRConnection: GetVRConnection,
            AddVRConnection: AddVRConnection,
            UpdateVRConnection: UpdateVRConnection,
            HasAddVRConnectionPermission: HasAddVRConnectionPermission,
            HasEditVRConnectionPermission: HasEditVRConnectionPermission
        });
    }

    appControllers.service('VRCommon_VRConnectionAPIService', vrConnectionAPIService);

})(appControllers);
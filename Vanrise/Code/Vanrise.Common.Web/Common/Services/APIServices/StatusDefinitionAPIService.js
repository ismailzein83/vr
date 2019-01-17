﻿
(function (appControllers) {

    "use strict";
    StatusDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function StatusDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "StatusDefinition";


        function GetFilteredStatusDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredStatusDefinitions'), input);
        }

        function AddStatusDefinition(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddStatusDefinition'), statusDefinitionItem);
        }
        function GetStatusDefinitionStylesByBusinessEntityId(businessEntityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetStatusDefinitionStylesByBusinessEntityId'), {
                businessEntityId: businessEntityId
            });
        }

        function UpdateStatusDefinition(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateStatusDefinition'), statusDefinitionItem);
        }

        function HasAddStatusDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddStatusDefinition']));
        }

        function HasUpdateStatusDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateStatusDefinition']));
        }
        function GetStatusDefinition(statusDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetStatusDefinition'), {
                statusDefinitionId: statusDefinitionId
            });
        }

        function GetStatusDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetStatusDefinitionsInfo"), {
                filter: filter
            });
        }

        function GetRemoteStatusDefinitionsInfo(connectionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRemoteStatusDefinitionsInfo"), {
                connectionId: connectionId,
                filter: filter
            });
        }

        return ({
            GetFilteredStatusDefinitions: GetFilteredStatusDefinitions,
            AddStatusDefinition: AddStatusDefinition,
            UpdateStatusDefinition: UpdateStatusDefinition,
            HasAddStatusDefinitionPermission: HasAddStatusDefinitionPermission,
            HasUpdateStatusDefinitionPermission: HasUpdateStatusDefinitionPermission,
            GetStatusDefinition: GetStatusDefinition,
            GetStatusDefinitionsInfo: GetStatusDefinitionsInfo,
            GetRemoteStatusDefinitionsInfo: GetRemoteStatusDefinitionsInfo,
            GetStatusDefinitionStylesByBusinessEntityId: GetStatusDefinitionStylesByBusinessEntityId
        });
    }

    appControllers.service('VR_Common_StatusDefinitionAPIService', StatusDefinitionAPIService);

})(appControllers);
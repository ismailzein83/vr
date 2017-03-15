﻿(function (appControllers) {

    "use strict";
    beRecieveDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_BEBridge_ModuleConfig', 'SecurityService'];

    function beRecieveDefinitionAPIService(baseApiService, utilsService, vrBeBridgeModuleConfig, SecurityService) {
        var controllerName = 'BERecieveDefinition';

        function GetBERecieveDefinitionsInfo(filter) {
            return baseApiService.get(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, "GetBERecieveDefinitionsInfo"), { filter: filter });
        }
        function GetFilteredBeReceiveDefinitions(input) {
            return baseApiService.post(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, 'GetFilteredBeReceiveDefinitions'), input);
        }
        function GetReceiveDefinition(beReceiveDefinitionId) {
            return baseApiService.get(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, 'GetReceiveDefinition'), {
                receiveDefinitionId: beReceiveDefinitionId
            });
        }
        function AddReceiveDefinition(beReceiveDeinitionItem) {
            return baseApiService.post(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, 'AddReceiveDefinition'), beReceiveDeinitionItem);
        }
        function UpdateReceiveDefinition(beReceiveDeinitionItem) {
            return baseApiService.post(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, 'UpdateReceiveDefinition'), beReceiveDeinitionItem);
        }
        function GetSourceReaderExtensionConfigs() {
            return baseApiService.get(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, "GetSourceReaderExtensionConfigs"));
        }
        function GetTargetSynchronizerExtensionConfigs() {
            return baseApiService.get(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, "GetTargetSynchronizerExtensionConfigs"));
        }
        function GetTargetConvertorExtensionConfigs() {
            return baseApiService.get(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, "GetTargetConvertorExtensionConfigs"));
        }
        function HasAddReceiveDefinitionPermission() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(vrBeBridgeModuleConfig.moduleName, controllerName, ['AddReceiveDefinition']));
        }
        function HasUpdateReceiveDefinitionPermission() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(vrBeBridgeModuleConfig.moduleName, controllerName, ['UpdateReceiveDefinition']));
        }

        return ({
            GetBERecieveDefinitionsInfo: GetBERecieveDefinitionsInfo,
            GetFilteredBeReceiveDefinitions: GetFilteredBeReceiveDefinitions,
            GetReceiveDefinition: GetReceiveDefinition,
            UpdateReceiveDefinition: UpdateReceiveDefinition,
            AddReceiveDefinition: AddReceiveDefinition,
            GetSourceReaderExtensionConfigs: GetSourceReaderExtensionConfigs,
            GetTargetSynchronizerExtensionConfigs: GetTargetSynchronizerExtensionConfigs,
            GetTargetConvertorExtensionConfigs: GetTargetConvertorExtensionConfigs,
            HasAddReceiveDefinitionPermission: HasAddReceiveDefinitionPermission,
            HasUpdateReceiveDefinitionPermission: HasUpdateReceiveDefinitionPermission
        });
    }

    appControllers.service('VR_BEBridge_BERecieveDefinitionAPIService', beRecieveDefinitionAPIService);

})(appControllers);
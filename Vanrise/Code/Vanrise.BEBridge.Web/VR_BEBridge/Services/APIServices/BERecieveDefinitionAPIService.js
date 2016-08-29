(function (appControllers) {

    "use strict";
    beRecieveDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_BEBridge_ModuleConfig', 'SecurityService'];

    function beRecieveDefinitionAPIService(baseApiService, utilsService, vrBeBridgeModuleConfig, SecurityService) {
        var controllerName = 'BERecieveDefinition';

        function GetBERecieveDefinitionsInfo(filter) {
            return baseApiService.get(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, "GetBERecieveDefinitionsInfo"));
        }
        function GetFilteredBeReceiveDefinitions(input) {
            return baseApiService.post(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, 'GetFilteredBeReceiveDefinitions'), input);
        }
        function GetReceiveDefinition(beReceiveDefinitionId) {
            return baseApiService.get(utilsService.getServiceURL(vrBeBridgeModuleConfig.moduleName, controllerName, 'GetReceiveDefinition'), {
                BEReceiveDefinitionId: beReceiveDefinitionId
            });
        }
        return ({
            GetBERecieveDefinitionsInfo: GetBERecieveDefinitionsInfo,
            GetFilteredBeReceiveDefinitions: GetFilteredBeReceiveDefinitions,
            GetReceiveDefinition: GetReceiveDefinition
        });
    }

    appControllers.service('VR_BEBridge_BERecieveDefinitionAPIService', beRecieveDefinitionAPIService);

})(appControllers);
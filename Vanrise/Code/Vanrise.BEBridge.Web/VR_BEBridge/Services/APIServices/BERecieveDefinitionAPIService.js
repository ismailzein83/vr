(function (appControllers) {

    "use strict";
    beRecieveDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_BEBridge_ModuleConfig', 'SecurityService'];

    function beRecieveDefinitionAPIService(BaseAPIService, UtilsService, VR_BEBridge_ModuleConfig, SecurityService) {
        var controllerName = 'BERecieveDefinition';

        function GetBERecieveDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_BEBridge_ModuleConfig.moduleName, controllerName, "GetBERecieveDefinitionsInfo"));
        }

        return ({
            GetBERecieveDefinitionsInfo: GetBERecieveDefinitionsInfo
        });
    }

    appControllers.service('VR_BEBridge_BERecieveDefinitionAPIService', beRecieveDefinitionAPIService);

})(appControllers);
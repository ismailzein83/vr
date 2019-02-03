(function (appControllers) {

    "use strict";
    VRTileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRTileAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRTile';

        function GetTileExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetTileExtendedSettingsConfigs"));
        }
        function GetFiguresTilesDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFiguresTilesDefinitionSettingsConfigs"));
        }
        function GetFiguresTileItemsToDiplayInfo(queriesInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFiguresTileItemsToDiplayInfo"), queriesInput);
        }
        function GetFigureItemsValue(figureStyleInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFigureItemsValue"), figureStyleInput);
        }
        return ({
            GetTileExtendedSettingsConfigs: GetTileExtendedSettingsConfigs,
            GetFiguresTilesDefinitionSettingsConfigs: GetFiguresTilesDefinitionSettingsConfigs,
            GetFiguresTileItemsToDiplayInfo: GetFiguresTileItemsToDiplayInfo,
            GetFigureItemsValue: GetFigureItemsValue

        });
    }

    appControllers.service('VRCommon_VRTileAPIService', VRTileAPIService);

})(appControllers);
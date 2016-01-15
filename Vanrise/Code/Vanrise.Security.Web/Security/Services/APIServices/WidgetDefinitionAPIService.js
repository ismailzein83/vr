(function (appControllers) {

    "use strict";
    widgetDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function widgetDefinitionAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {

        function GetWidgetsDefinition() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "WidgetDefinition", "GetWidgetsDefinition"));
        }
      
        return ({
            GetWidgetsDefinition:GetWidgetsDefinition
        });
    }

    appControllers.service('WidgetDefinitionAPIService', widgetDefinitionAPIService);

})(appControllers);
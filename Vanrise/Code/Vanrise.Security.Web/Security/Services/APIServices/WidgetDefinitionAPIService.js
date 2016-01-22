(function (appControllers) {

    'use strict';

    WidgetDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function WidgetDefinitionAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        return ({
            GetWidgetsDefinition: GetWidgetsDefinition
        });

        function GetWidgetsDefinition() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'WidgetDefinition', 'GetWidgetsDefinition'));
        }
    }

    appControllers.service('VR_Sec_WidgetDefinitionAPIService', WidgetDefinitionAPIService);

})(appControllers);

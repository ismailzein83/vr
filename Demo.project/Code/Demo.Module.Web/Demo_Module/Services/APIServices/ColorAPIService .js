(function (appControllers) {
    "use strict";
    colorAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function colorAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Color";

        function GetColorsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetColorsInfo"), { filter: filter });
        };


        function GetColorById(colorId) {

            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetColorById"),
                {
                    colorId: colorId

                });
        }

        return {
            GetColorsInfo: GetColorsInfo,
            GetColorById: GetColorById
        };
    };
    appControllers.service("Demo_Module_ColorAPIService", colorAPIService);

})(appControllers);
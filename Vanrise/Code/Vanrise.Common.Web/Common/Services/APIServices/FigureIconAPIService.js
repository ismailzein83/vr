(function (appControllers) {

    "use strict";
    FigureIconAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function FigureIconAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'FigureIcon';

        function GetFigureIconsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFigureIconsInfo"));
        }


        return ({
            GetFigureIconsInfo: GetFigureIconsInfo

        });
    }

    appControllers.service('VRCommon_FigureIconAPIService', FigureIconAPIService);

})(appControllers);
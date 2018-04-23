(function (appControllers) {

    "use strict";

    EricssonSwitchLoggerAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function EricssonSwitchLoggerAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'EricssonSwitchLogger';

        function GetSwitchLoggerTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetSwitchLoggerTemplates"));
        }

        return ({
            GetSwitchLoggerTemplates: GetSwitchLoggerTemplates
        });
    }

    appControllers.service('WhS_RouteSync_EricssonSwitchLoggerAPIService', EricssonSwitchLoggerAPIService);

})(appControllers);
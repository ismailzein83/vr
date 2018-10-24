(function (appControllers) {

    "use strict";

    HuaweiSwitchLoggerAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function HuaweiSwitchLoggerAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'HuaweiSwitchLogger';

        function GetSwitchLoggerTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetSwitchLoggerTemplates"));
        }

        return ({
            GetSwitchLoggerTemplates: GetSwitchLoggerTemplates
        });
    }

    appControllers.service('WhS_RouteSync_HuaweiSwitchLoggerAPIService', HuaweiSwitchLoggerAPIService);

})(appControllers);
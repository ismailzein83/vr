(function (appControllers) {

    'use strict';

    HttpConnectionCallInterceptorAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function HttpConnectionCallInterceptorAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'HttpCallConnectionInterceptors';

        function GetHttpConnectionCallInterceptorTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetHttpConnectionCallInterceptorTemplateConfigs"));
        }

        return {
            GetHttpConnectionCallInterceptorTemplateConfigs: GetHttpConnectionCallInterceptorTemplateConfigs
        };
    }

    appControllers.service('VRCommon_HttpConnectionCallInterceptorAPIService', HttpConnectionCallInterceptorAPIService);

})(appControllers);
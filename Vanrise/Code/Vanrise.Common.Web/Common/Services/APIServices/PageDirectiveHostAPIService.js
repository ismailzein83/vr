(function (appControllers) {

    "use strict";
    PageDirectiveHostAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function PageDirectiveHostAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'PageDirectiveHost';

        function GetPageDirectiveHostInfo(viewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetPageDirectiveHostInfo"), { viewId: viewId });
        }
        return ({
            GetPageDirectiveHostInfo: GetPageDirectiveHostInfo
        });
    }

    appControllers.service('VRCommon_PageDirectiveHostAPIService', PageDirectiveHostAPIService);

})(appControllers);
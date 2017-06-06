(function (appControllers) {

    "use strict";
    LiveCdrAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig'];

    function LiveCdrAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig) {

        var controllerName = "LiveCdr";

        function GetFilteredLiveCdrs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetFilteredLiveCdrs"), input);
        }

        return ({
            GetFilteredLiveCdrs: GetFilteredLiveCdrs
        });
    }

    appControllers.service('NP_IVSwitch_LiveCdrAPIService', LiveCdrAPIService);
})(appControllers);
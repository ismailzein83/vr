(function (appControllers) {

    "use strict";
    LOBAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function LOBAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controller = "LOB";

        function GetLOBInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controller, "GetLOBInfo"), {
                filter: filter
            });
        }

        return {
            GetLOBInfo: GetLOBInfo
        };

    }
    appControllers.service("WhS_BE_LOBAPIService", LOBAPIService);
})(appControllers);
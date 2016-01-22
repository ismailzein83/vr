(function (appControllers) {

    "use strict";
    BIConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_BI_ModuleConfig'];

    function BIConfigurationAPIService(BaseAPIService, UtilsService, VR_BI_ModuleConfig) {
        function GetMeasuresInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BIConfiguration", "GetMeasuresInfo"));
        }

        function GetEntitiesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BIConfiguration", "GetEntitiesInfo"));
        }
        function GetTimeEntitiesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BIConfiguration", "GetTimeEntitiesInfo"));
        }

        return ({
            GetMeasuresInfo: GetMeasuresInfo,
            GetEntitiesInfo: GetEntitiesInfo,
            GetTimeEntitiesInfo: GetTimeEntitiesInfo
        });
    }

    appControllers.service('VR_BI_BIConfigurationAPIService', BIConfigurationAPIService);

})(appControllers);
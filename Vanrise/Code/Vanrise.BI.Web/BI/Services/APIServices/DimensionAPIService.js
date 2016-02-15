(function (appControllers) {

    "use strict";
    BIDimensionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_BI_ModuleConfig'];

    function BIDimensionAPIService(BaseAPIService, UtilsService, VR_BI_ModuleConfig) {
        function GetDimensionInfo(entityName) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "Dimension", "GetDimensionInfo"), { entityName: entityName });
        }

        return ({
            GetDimensionInfo: GetDimensionInfo,
        });
    }

    appControllers.service('VR_BI_BIDimensionAPIService', BIDimensionAPIService);

})(appControllers);
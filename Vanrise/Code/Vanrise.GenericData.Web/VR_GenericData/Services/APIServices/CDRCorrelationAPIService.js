(function (appControllers) {

    "use strict";

    CDRCorrelationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function CDRCorrelationAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = "CDRCorrelation";

        function GetCDRCorrelationDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetCDRCorrelationDefinitionsInfo"), {
                filter: filter
            });
        }

        return ({
            GetCDRCorrelationDefinitionsInfo: GetCDRCorrelationDefinitionsInfo
        });
    }

   appControllers.service('VR_GenericData_CDRCorrelationAPIService', CDRCorrelationAPIService);
})(appControllers);
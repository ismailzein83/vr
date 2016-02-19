(function (appControllers) {

    "use strict";
    genericAnalyticAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function genericAnalyticAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFiltered(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "GenericAnalytic", "GetFiltered"), input);
        }

        return ({
            GetFiltered: GetFiltered
        });
    }

    appControllers.service('WhS_Analytics_GenericAnalyticAPIService', genericAnalyticAPIService);
})(appControllers);
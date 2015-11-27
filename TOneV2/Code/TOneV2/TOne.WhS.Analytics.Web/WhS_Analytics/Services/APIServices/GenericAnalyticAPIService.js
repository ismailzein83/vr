(function (appControllers) {

    "use strict";
    genericAnalyticAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function genericAnalyticAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        function GetFiltered(input) {
          
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "GenericAnalytic", "GetFiltered"), input);
        }

        return ({
            GetFiltered: GetFiltered
        });
    }

    appControllers.service('WhS_Analytics_GenericAnalyticAPIService', genericAnalyticAPIService);
})(appControllers);
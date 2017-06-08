(function (appControllers) {


    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName ="CodeCompare";

        function GetFilteredCodeCompare(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredCodeCompare"), input);
        }
       

        return ({
            GetFilteredCodeCompare: GetFilteredCodeCompare
        });
    }

    appControllers.service('WhS_Sales_CodeCompareAPIService', countryAPIService);

})(appControllers);
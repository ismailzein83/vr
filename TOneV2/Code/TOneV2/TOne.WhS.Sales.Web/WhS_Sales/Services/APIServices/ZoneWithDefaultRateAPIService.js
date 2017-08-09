(function (appControllers) {


    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName = "ZoneWithDefaultRate";

        function GetFilteredZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredZones"), input);
        }


        return ({
            GetFilteredZones: GetFilteredZones
        });
    }

    appControllers.service('WhS_Sales_ZoneWithDefaultRateAPIService', countryAPIService);

})(appControllers);
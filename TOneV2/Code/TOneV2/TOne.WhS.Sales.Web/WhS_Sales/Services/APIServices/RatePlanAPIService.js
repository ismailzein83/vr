(function (appControllers) {

    "use strict";

    ratePlanAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function ratePlanAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {

        function GetRatePlanItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetRatePlanItems"), input);
        }

        return ({
            GetRatePlanItems: GetRatePlanItems
        });
    }

    appControllers.service("WhS_Sales_RatePlanAPIService", ratePlanAPIService);

})(appControllers);
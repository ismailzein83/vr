(function (appControllers) {

    "use strict";

    RatePlanPreviewAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function RatePlanPreviewAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig)
    {
        var controllerName = "RatePlanPreview";

        function GetRatePlanPreviewSummary(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetRatePlanPreviewSummary"), query);
        }

        function GetFilteredRatePreviews(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredRatePreviews"), input);
        }

        function GetFilteredSaleZoneRoutingProductPreviews(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredSaleZoneRoutingProductPreviews"), input);
        }

        return {
            GetRatePlanPreviewSummary: GetRatePlanPreviewSummary,
            GetFilteredRatePreviews: GetFilteredRatePreviews,
            GetFilteredSaleZoneRoutingProductPreviews: GetFilteredSaleZoneRoutingProductPreviews
        };
    }

    appControllers.service("WhS_Sales_RatePlanPreviewAPIService", RatePlanPreviewAPIService);

})(appControllers);
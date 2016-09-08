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

        function GetDefaultRoutingProductPreview(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetDefaultRoutingProductPreview"), query);
        }
        
        function GetFilteredSaleZoneServicePreviews(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredSaleZoneServicePreviews"), query);
        }
        
        function GetDefaultServicePreview(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetDefaultServicePreview"), query);
        }

        return {
            GetRatePlanPreviewSummary: GetRatePlanPreviewSummary,
            GetFilteredRatePreviews: GetFilteredRatePreviews,
            GetFilteredSaleZoneRoutingProductPreviews: GetFilteredSaleZoneRoutingProductPreviews,
            GetDefaultRoutingProductPreview: GetDefaultRoutingProductPreview,
            GetFilteredSaleZoneServicePreviews: GetFilteredSaleZoneServicePreviews,
            GetDefaultServicePreview: GetDefaultServicePreview
        };
    }

    appControllers.service("WhS_Sales_RatePlanPreviewAPIService", RatePlanPreviewAPIService);

})(appControllers);
(function (appControllers) {

    "use strict";

    RatePlanPreviewAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function RatePlanPreviewAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig)
    {
        var controllerName = "RatePlanPreview";
        
        function GetCustomerRatePlanPreviewSummary(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetCustomerRatePlanPreviewSummary"), query);
        }
        
        function GetProductRatePlanPreviewSummary(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetProductRatePlanPreviewSummary"), query);
        }
        function GetFilteredRatePreviews(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredRatePreviews"), input);
        }
        function GetSubscriberPreviews(processInstanceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetSubscriberPreviews"), { processInstanceId: processInstanceId });
        }
        function GetFilteredCustomerRatePreviews(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerRatePreviews"), input);
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

        function GetFilteredChangedCustomerCountryPreviews(query) {
        	return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredChangedCustomerCountryPreviews"), query);
        }

        function GetFilteredNewCustomerCountryPreviews(query) {
        	return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredNewCustomerCountryPreviews"), query);
        }

        return {
            GetProductRatePlanPreviewSummary:GetProductRatePlanPreviewSummary,
            GetCustomerRatePlanPreviewSummary: GetCustomerRatePlanPreviewSummary,
            GetFilteredRatePreviews: GetFilteredRatePreviews,
            GetFilteredSaleZoneRoutingProductPreviews: GetFilteredSaleZoneRoutingProductPreviews,
            GetDefaultRoutingProductPreview: GetDefaultRoutingProductPreview,
            GetFilteredSaleZoneServicePreviews: GetFilteredSaleZoneServicePreviews,
            GetDefaultServicePreview: GetDefaultServicePreview,
            GetFilteredChangedCustomerCountryPreviews: GetFilteredChangedCustomerCountryPreviews,
            GetFilteredNewCustomerCountryPreviews: GetFilteredNewCustomerCountryPreviews,
            GetFilteredCustomerRatePreviews: GetFilteredCustomerRatePreviews,
            GetSubscriberPreviews: GetSubscriberPreviews,
        };
    }

    appControllers.service("WhS_Sales_RatePlanPreviewAPIService", RatePlanPreviewAPIService);

})(appControllers);
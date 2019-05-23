(function (appControllers) {

    "use strict";
    sellingRuleAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function sellingRuleAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {

        var controllerName = "SellingRule";
   
        function GetSellingRuleThresholdTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetSellingRuleThresholdTemplates"));
        }
        function GetSellingRuleActionTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetSellingRuleActionTemplates"));
        }
        return ({
            GetSellingRuleThresholdTemplates: GetSellingRuleThresholdTemplates,
            GetSellingRuleActionTemplates: GetSellingRuleActionTemplates
        });
    }

    appControllers.service("WhS_Sales_SellingRuleAPIService", sellingRuleAPIService);
})(appControllers);
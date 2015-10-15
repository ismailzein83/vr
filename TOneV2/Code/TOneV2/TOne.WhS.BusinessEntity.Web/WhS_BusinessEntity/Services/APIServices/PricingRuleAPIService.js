(function (appControllers) {

    "use strict";
    pricingRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function pricingRuleAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

     
        function GetPricingRuleTODTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PricingRule", "GetPricingRuleTODTemplates"));

        }
        function GetPricingRuleTariffTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PricingRule", "GetPricingRuleTariffTemplates"));

        }

       
        return ({
            GetPricingRuleTODTemplates: GetPricingRuleTODTemplates,
            GetPricingRuleTariffTemplates: GetPricingRuleTariffTemplates
        });
    }

    appControllers.service('WhS_BE_PricingRuleAPIService', pricingRuleAPIService);

})(appControllers);
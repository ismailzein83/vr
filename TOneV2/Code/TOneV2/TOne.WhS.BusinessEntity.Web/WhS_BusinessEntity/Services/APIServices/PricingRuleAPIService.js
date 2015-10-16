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
        function GetPricingRuleExtraChargeTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PricingRule", "GetPricingRuleExtraChargeTemplates"));

        }

       
        return ({
            GetPricingRuleTODTemplates: GetPricingRuleTODTemplates,
            GetPricingRuleTariffTemplates: GetPricingRuleTariffTemplates,
            GetPricingRuleExtraChargeTemplates: GetPricingRuleExtraChargeTemplates
        });
    }

    appControllers.service('WhS_BE_PricingRuleAPIService', pricingRuleAPIService);

})(appControllers);
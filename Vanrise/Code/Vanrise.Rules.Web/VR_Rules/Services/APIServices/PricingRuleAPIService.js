(function (appControllers) {

    "use strict";
    pricingRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Rules_ModuleConfig'];

    function pricingRuleAPIService(BaseAPIService, UtilsService, VR_Rules_ModuleConfig) {


        function GetPricingRuleRateTypeTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Rules_ModuleConfig.moduleName, "PricingRule", "GetPricingRuleRateTypeTemplates"));

        }
        function GetPricingRuleTariffTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Rules_ModuleConfig.moduleName, "PricingRule", "GetPricingRuleTariffTemplates"));

        }
        function GetPricingRuleExtraChargeTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Rules_ModuleConfig.moduleName, "PricingRule", "GetPricingRuleExtraChargeTemplates"));

        }

        function GetPricingRuleRateValueTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Rules_ModuleConfig.moduleName, "PricingRule", "GetPricingRuleRateValueTemplates"));
        }

        return ({
            GetPricingRuleRateTypeTemplates: GetPricingRuleRateTypeTemplates,
            GetPricingRuleTariffTemplates: GetPricingRuleTariffTemplates,
            GetPricingRuleExtraChargeTemplates: GetPricingRuleExtraChargeTemplates,
            GetPricingRuleRateValueTemplates: GetPricingRuleRateValueTemplates
        });
    }

    appControllers.service('VR_Rules_PricingRuleAPIService', pricingRuleAPIService);

})(appControllers);
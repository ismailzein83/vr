(function (appControllers) {

    'use strict';

    RulesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_RA_ModuleConfig'];

    function RulesAPIService(BaseAPIService, UtilsService, Retail_RA_ModuleConfig) {
        var controllerName = 'RaRules';

        function GetVoiceTaxRuleTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_RA_ModuleConfig.moduleName, controllerName, "GetVoiceTaxRuleTemplates"));
        }

        function GetSMSTaxRuleTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_RA_ModuleConfig.moduleName, controllerName, "GetSMSTaxRuleTemplates"));
        }

        function GetTransactionTaxRuleTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_RA_ModuleConfig.moduleName, controllerName, "GetTransactionTaxRuleTemplates"));
        }


        return {
            GetVoiceTaxRuleTemplates: GetVoiceTaxRuleTemplates,
            GetSMSTaxRuleTemplates: GetSMSTaxRuleTemplates,
            GetTransactionTaxRuleTemplates: GetTransactionTaxRuleTemplates
        };
    }

    appControllers.service('RA_RulesAPIService', RulesAPIService);

})(appControllers);
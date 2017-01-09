
(function (appControllers) {

    "use strict";

    RecurringChargeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function RecurringChargeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "RecurringCharge";

        function GetAccountRecurringChargeEvaluatorExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountRecurringChargeEvaluatorExtensionConfigs'));
        }

        function GetAccountRecurringChargeRuleSetSettingsExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountRecurringChargeRuleSetSettingsExtensionConfigs'));
        }


        return ({
            GetAccountRecurringChargeEvaluatorExtensionConfigs: GetAccountRecurringChargeEvaluatorExtensionConfigs,
            GetAccountRecurringChargeRuleSetSettingsExtensionConfigs: GetAccountRecurringChargeRuleSetSettingsExtensionConfigs
        });
    }

    appControllers.service('Retail_BE_RecurringChargeAPIService', RecurringChargeAPIService);

})(appControllers);
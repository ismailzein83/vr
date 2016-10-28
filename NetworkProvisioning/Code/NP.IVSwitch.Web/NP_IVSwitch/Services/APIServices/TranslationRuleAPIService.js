
(function (appControllers) {

    "use strict";
    TranslationRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function TranslationRuleAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "TranslationRule";


        function GetFilteredTranslationRules(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredTranslationRules'), input);

        }

        function GetTranslationRule(TranslationRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetTranslationRule'), {
                TranslationRuleId: TranslationRuleId
            });
        }

        function AddTranslationRule(TranslationRuleItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddTranslationRule'), TranslationRuleItem);
        }

        function UpdateTranslationRule(TranslationRuleItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateTranslationRule'), TranslationRuleItem);
        }
          


        function HasAddTranslationRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['AddTranslationRule']));
        }

        function HasEditTranslationRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['UpdateTranslationRule']));
        }


        return ({
            GetFilteredTranslationRules: GetFilteredTranslationRules,
            GetTranslationRule: GetTranslationRule,
            AddTranslationRule: AddTranslationRule,
            UpdateTranslationRule: UpdateTranslationRule, 
            HasAddTranslationRulePermission: HasAddTranslationRulePermission,
            HasEditTranslationRulePermission: HasEditTranslationRulePermission,
        });
    }

    appControllers.service('NP_IVSwitch_TranslationRuleAPIService', TranslationRuleAPIService);

})(appControllers);
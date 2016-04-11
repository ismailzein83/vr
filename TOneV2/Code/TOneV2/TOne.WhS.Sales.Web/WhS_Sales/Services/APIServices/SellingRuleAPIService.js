(function (appControllers) {

    "use strict";
    sellingRuleAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig", "SecurityService"];

    function sellingRuleAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig, SecurityService) {

        var controllerName = "SellingRule";

        function GetFilteredSellingRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredSellingRules"), input);
        }

        function GetRule(sellingRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetRule"), {
                ruleId: sellingRuleId
            });
        }

        function AddRule(sellingRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "AddRule"), sellingRuleObject);
        }

        function UpdateRule(sellingRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "UpdateRule"), sellingRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "DeleteRule"), { ruleId: ruleId });
        }

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetCodeCriteriaGroupTemplates"));
        }

        function GetSellingRuleSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetSellingRuleSettingsTemplates"));
        }


        function HasUpdateRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Sales_ModuleConfig.moduleName, controllerName, ['UpdateRule']));
        }

        function HasAddRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Sales_ModuleConfig.moduleName, controllerName, ['AddRule']));
        }

        function HasDeleteRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Sales_ModuleConfig.moduleName, controllerName, ['DeleteRule']));
        }


        return ({
            GetFilteredSellingRules: GetFilteredSellingRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates,
            GetSellingRuleSettingsTemplates: GetSellingRuleSettingsTemplates,
            HasUpdateRulePermission: HasUpdateRulePermission,
            HasAddRulePermission: HasAddRulePermission,
            HasDeleteRulePermission: HasDeleteRulePermission
        });
    }

    appControllers.service("WhS_Sales_SellingRuleAPIService", sellingRuleAPIService);
})(appControllers);
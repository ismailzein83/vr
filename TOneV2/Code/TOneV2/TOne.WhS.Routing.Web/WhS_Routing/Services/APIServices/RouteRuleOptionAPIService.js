(function (appControllers) {

    "use strict";
    routeOptionRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig', 'SecurityService'];

    function routeOptionRuleAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig, SecurityService) {

        var controllerName = "RouteOptionRule";

        function GetFilteredRouteOptionRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRouteOptionRules"), input);
        }

        function GetOptionRule(routeOptionRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetOptionRule"), {
                optionRuleId: routeOptionRuleId
            });
        }

        function AddRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "AddRule"), routeRuleObject);
        }

        function UpdateRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "UpdateRule"), routeRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "DeleteRule"), { ruleId: ruleId });
        }

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetCodeCriteriaGroupTemplates"));
        }

        function GetRouteOptionRuleSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteOptionRuleSettingsTemplates"));
        }

        function HasAddRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Routing_ModuleConfig.moduleName, controllerName, ['AddRule']));
        }

        function HasUpdateRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Routing_ModuleConfig.moduleName, controllerName, ['UpdateRule']));
        }

        function HasDeleteRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Routing_ModuleConfig.moduleName, controllerName, ['DeleteRule']));
        }

        return ({
            GetFilteredRouteOptionRules: GetFilteredRouteOptionRules,
            GetOptionRule: GetOptionRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates,
            GetRouteOptionRuleSettingsTemplates: GetRouteOptionRuleSettingsTemplates,
            HasAddRulePermission: HasAddRulePermission,
            HasUpdateRulePermission: HasUpdateRulePermission,
            HasDeleteRulePermission: HasDeleteRulePermission
        });

    }

    appControllers.service('WhS_Routing_RouteOptionRuleAPIService', routeOptionRuleAPIService);
})(appControllers);
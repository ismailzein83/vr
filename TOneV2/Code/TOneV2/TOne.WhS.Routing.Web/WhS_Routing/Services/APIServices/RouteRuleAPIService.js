(function (appControllers) {

    "use strict";
    routeRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig', 'SecurityService'];

    function routeRuleAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig, SecurityService) {

        var controllerName = "RouteRule";

        function GetFilteredRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRouteRules"), input);
        };

        function GetRule(routeRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRule"), {
                ruleId: routeRuleId
            });
        };

        function AddRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "AddRule"), routeRuleObject);
        };

        function UpdateRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "UpdateRule"), routeRuleObject);
        };

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "DeleteRule"), { ruleId: ruleId });
        };
        
        function SetRouteRulesDeleted(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "SetRouteRulesDeleted"), input);
        };

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetCodeCriteriaGroupTemplates"));
        };

        function GetRouteRuleSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteRuleSettingsTemplates"));
        };

        function GetRouteRuleCriteriaTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteRuleCriteriaTemplates"));
        };

        function HasAddRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Routing_ModuleConfig.moduleName, controllerName, ['AddRule']));
        };

        function HasUpdateRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Routing_ModuleConfig.moduleName, controllerName, ['UpdateRule']));
        };

        function HasDeleteRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Routing_ModuleConfig.moduleName, controllerName, ['DeleteRule']));
        };

        function BuildLinkedRouteRule(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "BuildLinkedRouteRule"), input);
        };
        function GetRouteRuleHistoryDetailbyHistoryId(routeRuleHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, 'GetRouteRuleHistoryDetailbyHistoryId'), {
                routeRuleHistoryId: routeRuleHistoryId
            });
        }
        return ({
            GetFilteredRouteRules: GetFilteredRouteRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates,
            GetRouteRuleSettingsTemplates: GetRouteRuleSettingsTemplates,
            GetRouteRuleCriteriaTemplates: GetRouteRuleCriteriaTemplates,
            HasAddRulePermission: HasAddRulePermission,
            HasUpdateRulePermission: HasUpdateRulePermission,
            HasDeleteRulePermission: HasDeleteRulePermission,
            BuildLinkedRouteRule: BuildLinkedRouteRule,
            GetRouteRuleHistoryDetailbyHistoryId: GetRouteRuleHistoryDetailbyHistoryId,
            SetRouteRulesDeleted:SetRouteRulesDeleted
        });
    };

    appControllers.service('WhS_Routing_RouteRuleAPIService', routeRuleAPIService);
})(appControllers);
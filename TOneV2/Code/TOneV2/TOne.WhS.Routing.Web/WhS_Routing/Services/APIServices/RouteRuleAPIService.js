﻿(function (appControllers) {

    "use strict";

    routeRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig', 'SecurityService'];

    function routeRuleAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig, SecurityService) {

        var controllerName = "RouteRule";

        function GetFilteredRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRouteRules"), input);
        }

        function GetRule(routeRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRule"), {
                ruleId: routeRuleId
            });
        }

        function AddRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "AddRule"), routeRuleObject);
        }

        function UpdateRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "UpdateRule"), routeRuleObject);
        }

        function SetRouteRuleDeleted(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "SetRouteRuleDeleted"), { ruleId: ruleId });
        }
        
        function SetRouteRulesDeleted(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "SetRouteRulesDeleted"), input);
        }

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetCodeCriteriaGroupTemplates"));
        }

        function GetRouteRuleSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteRuleSettingsTemplates"));
        }

        function GetRouteRuleCriteriaTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteRuleCriteriaTemplates"));
        }

        function GetRouteRuleHistoryDetailbyHistoryId(routeRuleHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, 'GetRouteRuleHistoryDetailbyHistoryId'), {
                routeRuleHistoryId: routeRuleHistoryId
            });
        }

        function GetRouteRuleConfiguration() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, 'GetRouteRuleConfiguration'));
        }

        function BuildLinkedRouteRule(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "BuildLinkedRouteRule"), input);
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

        function HasViewRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Routing_ModuleConfig.moduleName, controllerName, ['GetFilteredRouteRules']));
        }

        function ExtendSuppliersList(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "ExtendSuppliersList"), input);
        }

        return ({
            GetFilteredRouteRules: GetFilteredRouteRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            SetRouteRuleDeleted: SetRouteRuleDeleted,
            SetRouteRulesDeleted: SetRouteRulesDeleted,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates,
            GetRouteRuleSettingsTemplates: GetRouteRuleSettingsTemplates,
            GetRouteRuleCriteriaTemplates: GetRouteRuleCriteriaTemplates,
            GetRouteRuleHistoryDetailbyHistoryId: GetRouteRuleHistoryDetailbyHistoryId,
            GetRouteRuleConfiguration: GetRouteRuleConfiguration,
            BuildLinkedRouteRule: BuildLinkedRouteRule,
            HasAddRulePermission: HasAddRulePermission,
            HasUpdateRulePermission: HasUpdateRulePermission,
            HasDeleteRulePermission: HasDeleteRulePermission,
            HasViewRulePermission: HasViewRulePermission,
            ExtendSuppliersList: ExtendSuppliersList
        });
    }

    appControllers.service('WhS_Routing_RouteRuleAPIService', routeRuleAPIService);
})(appControllers);
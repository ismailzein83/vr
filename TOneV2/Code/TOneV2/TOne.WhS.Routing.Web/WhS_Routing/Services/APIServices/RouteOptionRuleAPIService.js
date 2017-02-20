﻿(function (appControllers) {

    "use strict";
    routeOptionRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig', 'SecurityService'];

    function routeOptionRuleAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig, SecurityService) {

        var controllerName = "RouteOptionRule";

        function GetFilteredRouteOptionRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRouteOptionRules"), input);
        };

        function GetRuleEditorRuntime(routeOptionRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRuleEditorRuntime"), {
                ruleId: routeOptionRuleId
            });
        };

        function GetRule(routeOptionRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRule"), {
                ruleId: routeOptionRuleId
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


        function GetRouteOptionRuleSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteOptionRuleSettingsTemplates"));
        };

        function GetRouteOptionRuleSettingsTemplatesByProcessType(routingProcessType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteOptionRuleSettingsTemplatesByProcessType"), {
                routingProcessType: routingProcessType
            });
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

        function BuildLinkedRouteOptionRule(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "BuildLinkedRouteOptionRule "), input);
        };

        return ({
            GetFilteredRouteOptionRules: GetFilteredRouteOptionRules,
            GetRuleEditorRuntime: GetRuleEditorRuntime,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
            GetRouteOptionRuleSettingsTemplates: GetRouteOptionRuleSettingsTemplates,
            GetRouteOptionRuleSettingsTemplatesByProcessType: GetRouteOptionRuleSettingsTemplatesByProcessType,
            HasAddRulePermission: HasAddRulePermission,
            HasUpdateRulePermission: HasUpdateRulePermission,
            HasDeleteRulePermission: HasDeleteRulePermission,
            BuildLinkedRouteOptionRule: BuildLinkedRouteOptionRule
        });

    }

    appControllers.service('WhS_Routing_RouteOptionRuleAPIService', routeOptionRuleAPIService);
})(appControllers);
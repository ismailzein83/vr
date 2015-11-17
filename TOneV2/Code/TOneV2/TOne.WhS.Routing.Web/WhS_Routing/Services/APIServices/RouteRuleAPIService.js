(function (appControllers) {

    "use strict";
    routeRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function routeRuleAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        function GetFilteredRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RouteRule", "GetFilteredRouteRules"), input);
        }

        function GetRule(routeRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RouteRule", "GetRule"), {
                ruleId: routeRuleId
            });
        }

        function AddRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RouteRule", "AddRule"), routeRuleObject);
        }

        function UpdateRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RouteRule", "UpdateRule"), routeRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RouteRule", "DeleteRule"), { ruleId: ruleId });
        }

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RouteRule", "GetCodeCriteriaGroupTemplates"));
        }

        function GetRouteRuleSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RouteRule", "GetRouteRuleSettingsTemplates"));
        }

        return ({
            GetFilteredRouteRules: GetFilteredRouteRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates,
            GetRouteRuleSettingsTemplates: GetRouteRuleSettingsTemplates
        });
    }

    appControllers.service('WhS_Routing_RouteRuleAPIService', routeRuleAPIService);
})(appControllers);
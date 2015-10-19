(function (appControllers) {

    "use strict";
    routeRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function routeRuleAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "GetFilteredRouteRules", input));
        }

        function GetRouteRule(routeRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "GetRouteRule", {
                routeRuleId: routeRuleId
            }));
        }

        function AddRouteRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "AddRouteRule", routeRuleObject));
        }

        function UpdateRouteRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "UpdateRouteRule", routeRuleObject));
        }

        function DeleteRouteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "DeleteRouteRule", { ruleId: ruleId }));
        }

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "GetCodeCriteriaGroupTemplates"));
        }

        return ({
            GetFilteredRouteRules: GetFilteredRouteRules,
            GetRouteRule: GetRouteRule,
            AddRouteRule: AddRouteRule,
            UpdateRouteRule: UpdateRouteRule,
            DeleteRouteRule: DeleteRouteRule,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates
        });
    }

    appControllers.service('WhS_BE_RouteRuleAPIService', routeRuleAPIService);
})(appControllers);
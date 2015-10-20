(function (appControllers) {

    "use strict";
    routeRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function routeRuleAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "GetFilteredRouteRules"), input);
        }

        function GetRule(routeRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "GetRouteRule"), {
                ruleId: routeRuleId
            });
        }

        function AddRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "AddRouteRule"), routeRuleObject);
        }

        function UpdateRule(routeRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "UpdateRouteRule"), routeRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "DeleteRouteRule"), { ruleId: ruleId });
        }

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RouteRule", "GetCodeCriteriaGroupTemplates"));
        }

        return ({
            GetFilteredRouteRules: GetFilteredRouteRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates
        });
    }

    appControllers.service('WhS_BE_RouteRuleAPIService', routeRuleAPIService);
})(appControllers);
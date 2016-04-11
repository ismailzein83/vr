﻿(function (appControllers) {

    "use strict";
    routRuleSettingsAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Routing_ModuleConfig"];

    function routRuleSettingsAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "RouteRuleSettings";

        function GetRouteOptionSettingsGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteOptionSettingsGroupTemplates"));
        }

        function GetRouteOptionOrderSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteOptionOrderSettingsTemplates"));
        }

        function GetRouteOptionFilterSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteOptionFilterSettingsTemplates"));
        }

        function GetRouteOptionPercentageSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRouteOptionPercentageSettingsTemplates"));
        }
        
        return ({
            GetRouteOptionSettingsGroupTemplates: GetRouteOptionSettingsGroupTemplates,
            GetRouteOptionOrderSettingsTemplates: GetRouteOptionOrderSettingsTemplates,
            GetRouteOptionFilterSettingsTemplates: GetRouteOptionFilterSettingsTemplates,
            GetRouteOptionPercentageSettingsTemplates: GetRouteOptionPercentageSettingsTemplates
        });
    }

    appControllers.service("WhS_Routing_RoutRuleSettingsAPIService", routRuleSettingsAPIService);

})(appControllers);
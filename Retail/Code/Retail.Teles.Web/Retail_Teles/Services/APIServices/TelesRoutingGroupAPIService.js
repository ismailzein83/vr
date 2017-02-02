(function (appControllers) {

    "use strict";
    TelesRoutingGroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function TelesRoutingGroupAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesRoutingGroup";

        function GetRoutingGroupConditionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetRoutingGroupConditionConfigs"));
        }
        return ({
            GetRoutingGroupConditionConfigs: GetRoutingGroupConditionConfigs,
        });
    }

    appControllers.service('Retail_Teles_TelesRoutingGroupAPIService', TelesRoutingGroupAPIService);

})(appControllers);
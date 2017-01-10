
(function (appControllers) {

    "use strict";

    RecurringChargeDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function RecurringChargeDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "RecurringChargeDefinition";

        function GetRecurringChargeDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetRecurringChargeDefinitionsInfo"), {
                filter: filter
            });
        }

        return ({
            GetRecurringChargeDefinitionsInfo: GetRecurringChargeDefinitionsInfo
        });
    }

    appControllers.service('Retail_BE_RecurringChargeDefinitionAPIService', RecurringChargeDefinitionAPIService);

})(appControllers);
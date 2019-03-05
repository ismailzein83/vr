(function (appControllers) {

    'use strict';

    RecurringChargeAPIService.$inject = ["BaseAPIService", "UtilsService", "Retail_BE_ModuleConfig"];
    function RecurringChargeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "FinancialRecurringCharge";

        function GetRecurringChargePeriodsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetRecurringChargePeriodsConfigs"));
        }

        return ({
            GetRecurringChargePeriodsConfigs: GetRecurringChargePeriodsConfigs
        });
    }

    appControllers.service("Retail_BE_FinacialRecurringChargeAPIService", RecurringChargeAPIService);

})(appControllers);
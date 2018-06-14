(function (appControllers) {

    'use strict';

    RecurringChargeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];
    function RecurringChargeAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "RecurringCharge";

        function GetRecurringChargePeriodsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetRecurringChargePeriodsConfigs"));
        }

        function HasViewRecurringChargePermission(financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'DoesUserHaveViewAccess'), { financialAccountId: financialAccountId });
        }

        return ({
            GetRecurringChargePeriodsConfigs: GetRecurringChargePeriodsConfigs,
            HasViewRecurringChargePermission: HasViewRecurringChargePermission
        });
    }

    appControllers.service("WhS_BE_RecurringChargeAPIService", RecurringChargeAPIService);

})(appControllers);
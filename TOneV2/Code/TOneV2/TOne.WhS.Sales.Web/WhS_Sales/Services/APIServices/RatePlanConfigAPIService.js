(function (appControllers) {

    'use strict';

    RatePlanConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function RatePlanConfigAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName = 'RatePlanConfig';

        function GetGeneralSettingsLongPrecisionValue() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'GetGeneralSettingsLongPrecisionValue'));
        }

        return {
            GetGeneralSettingsLongPrecisionValue: GetGeneralSettingsLongPrecisionValue
        };
    }

    appControllers.service('WhS_Sales_RatePlanConfigAPIService', RatePlanConfigAPIService);

})(appControllers);
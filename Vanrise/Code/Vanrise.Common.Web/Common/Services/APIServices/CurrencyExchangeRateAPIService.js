(function (appControllers) {

    "use strict";
    currencyExchangeRateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function currencyExchangeRateAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        function GetFilteredExchangeRateCurrencies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", "GetFilteredExchangeRateCurrencies"), input);
        }
        function GetCurrencyExchangeRate(currencyExchangeRateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", "GetCurrencyExchangeRate"),
                {
                    currencyExchangeRateId: currencyExchangeRateId
                });
        }
        function AddCurrencyExchangeRate(object) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", "AddCurrencyExchangeRate"), object);
        }
        function UpdateCurrencyExchangeRate(object) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", "UpdateCurrencyExchangeRate"), object);
        }

        function HasAddCurrencyExchangeRatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", ['AddCurrencyExchangeRate']));
        }

        function HasEditCurrencyExchangeRatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", ['UpdateCurrencyExchangeRate']));
        }

        return ({
            GetFilteredExchangeRateCurrencies: GetFilteredExchangeRateCurrencies,
            UpdateCurrencyExchangeRate: UpdateCurrencyExchangeRate,
            GetCurrencyExchangeRate:GetCurrencyExchangeRate,
            AddCurrencyExchangeRate: AddCurrencyExchangeRate,
            HasAddCurrencyExchangeRatePermission: HasAddCurrencyExchangeRatePermission,
            HasEditCurrencyExchangeRatePermission: HasEditCurrencyExchangeRatePermission
        });
    }

    appControllers.service('VRCommon_CurrencyExchangeRateAPIService', currencyExchangeRateAPIService);

})(appControllers);
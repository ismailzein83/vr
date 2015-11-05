(function (appControllers) {

    "use strict";
    currencyExchangeRateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function currencyExchangeRateAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        function GetFilteredExchangeRateCurrencies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", "GetFilteredExchangeRateCurrencies"), input);
        }

        function AddCurrencyExchangeRate(object) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "CurrencyExchangeRate", "AddCurrencyExchangeRate"), object);
        }

        return ({
            GetFilteredExchangeRateCurrencies: GetFilteredExchangeRateCurrencies,
            AddCurrencyExchangeRate: AddCurrencyExchangeRate
        });
    }

    appControllers.service('VRCommon_CurrencyExchangeRateAPIService', currencyExchangeRateAPIService);

})(appControllers);
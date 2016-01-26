app.run(['VRCommon_CityService', 'VRCommon_CurrencyExchangeRateService', function (VRCommon_CityService, VRCommon_CurrencyExchangeRateService) {
    VRCommon_CityService.registerDrillDownToCountry();
    VRCommon_CurrencyExchangeRateService.registerDrillDownToCurrency();
}]);
app.run(['VRCommon_CityService', 'VRCommon_CurrencyExchangeRateService', 'VRCommon_LogEntryService', function (VRCommon_CityService, VRCommon_CurrencyExchangeRateService, VRCommon_LogEntryService) {
    VRCommon_CityService.registerDrillDownToCountry();
    VRCommon_CurrencyExchangeRateService.registerDrillDownToCurrency();
    VRCommon_LogEntryService.registerLogToMaster();
}]);
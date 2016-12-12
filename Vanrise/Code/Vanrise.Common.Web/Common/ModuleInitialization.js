app.run(['VRCommon_CityService', 'VRCommon_CurrencyExchangeRateService', 'VRCommon_LogEntryService', 'VRCommon_UserActionAuditService', function (VRCommon_CityService, VRCommon_CurrencyExchangeRateService, VRCommon_LogEntryService, VRCommon_UserActionAuditService) {
    VRCommon_CityService.registerDrillDownToCountry();
    VRCommon_CurrencyExchangeRateService.registerDrillDownToCurrency();
    VRCommon_LogEntryService.registerLogToMaster();
    VRCommon_UserActionAuditService.registerLogToMaster();
}]);
app.run(['VRCommon_CityService', 'VRCommon_CurrencyExchangeRateService', 'VRCommon_LogEntryService', 'VRCommon_UserActionAuditService', 'VRCommon_CountryService', 'VRCommon_VRConnectionService', 'VRCommon_SettingsService', 'VRCommon_VRMailMessageTemplateService', 'VRCommon_RateTypeService', 'VRCommon_CurrencyService', function (VRCommon_CityService, VRCommon_CurrencyExchangeRateService, VRCommon_LogEntryService, VRCommon_UserActionAuditService, VRCommon_CountryService, VRCommon_VRConnectionService, VRCommon_SettingsService, VRCommon_VRMailMessageTemplateService, VRCommon_RateTypeService, VRCommon_CurrencyService) {
    VRCommon_CityService.registerDrillDownToCountry();
    VRCommon_CurrencyExchangeRateService.registerDrillDownToCurrency();
    VRCommon_LogEntryService.registerLogToMaster();
    VRCommon_UserActionAuditService.registerLogToMaster();
    VRCommon_CountryService.registerObjectTrackingDrillDownToCountry();
    VRCommon_CityService.registerObjectTrackingDrillDownToCity();
    VRCommon_VRConnectionService.registerObjectTrackingDrillDownToConnection();
    VRCommon_SettingsService.registerObjectTrackingDrillDownToSetting();
    VRCommon_VRMailMessageTemplateService.registerObjectTrackingDrillDownToVRMailTemplateMessage();
    VRCommon_RateTypeService.registerObjectTrackingDrillDownToRateType();
    VRCommon_CurrencyService.registerObjectTrackingDrillDownToCurrency();
}]);
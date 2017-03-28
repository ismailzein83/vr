app.run(['VRCommon_VRObjectTypeDefinitionService', 'VRCommon_VRMailMessageTypeService', 'VRCommon_VRTimeZoneService', 'VRCommon_CityService', 'VRCommon_CurrencyExchangeRateService', 'VRCommon_LogEntryService', 'VRCommon_UserActionAuditService', 'VRCommon_CountryService', 'VRCommon_VRConnectionService', 'VRCommon_SettingsService', 'VRCommon_VRMailMessageTemplateService', 'VRCommon_RateTypeService', 'VRCommon_CurrencyService', function (VRCommon_VRObjectTypeDefinitionService,VRCommon_VRMailMessageTypeService, VRCommon_VRTimeZoneService, VRCommon_CityService, VRCommon_CurrencyExchangeRateService, VRCommon_LogEntryService, VRCommon_UserActionAuditService, VRCommon_CountryService, VRCommon_VRConnectionService, VRCommon_SettingsService, VRCommon_VRMailMessageTemplateService, VRCommon_RateTypeService, VRCommon_CurrencyService) {
    VRCommon_CityService.registerDrillDownToCountry();
    VRCommon_CurrencyExchangeRateService.registerDrillDownToCurrency();
    VRCommon_LogEntryService.registerLogToMaster();
    VRCommon_UserActionAuditService.registerLogToMaster();
    VRCommon_CityService.registerObjectTrackingDrillDownToCity();
    VRCommon_VRConnectionService.registerObjectTrackingDrillDownToConnection();
    VRCommon_SettingsService.registerObjectTrackingDrillDownToSetting();
    VRCommon_VRMailMessageTemplateService.registerObjectTrackingDrillDownToVRMailTemplateMessage();
    VRCommon_RateTypeService.registerObjectTrackingDrillDownToRateType();
    VRCommon_CurrencyService.registerObjectTrackingDrillDownToCurrency();
    VRCommon_VRTimeZoneService.registerObjectTrackingDrillDownToTimeZone();
    VRCommon_VRMailMessageTypeService.registerObjectTrackingDrillDownToVRMailMessageMailType();
    VRCommon_VRObjectTypeDefinitionService.registerObjectTrackingDrillDownToVRObjectTypeDefinition();
}]);
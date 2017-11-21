﻿app.run(['VRCommon_VRLocalizationTextResourceService', 'VRCommon_CountryService', 'VRCommon_StyleDefinitionService', 'VRCommon_VRComponentTypeService', 'VR_Common_StatusDefinitionService', 'VRCommon_VRObjectTypeDefinitionService', 'VRCommon_VRMailMessageTypeService', 'VRCommon_VRTimeZoneService', 'VRCommon_CityService', 'VRCommon_CurrencyExchangeRateService', 'VRCommon_LogEntryService', 'VRCommon_CountryService', 'VRCommon_VRConnectionService', 'VRCommon_SettingsService', 'VRCommon_VRMailMessageTemplateService', 'VRCommon_RateTypeService', 'VRCommon_CurrencyService', 'VRCommon_RegionService', 'VR_Common_GenericLKUPService', function (VRCommon_VRLocalizationTextResourceService,VRCommon_CountryService, VRCommon_StyleDefinitionService, VRCommon_VRComponentTypeService, VR_Common_StatusDefinitionService, VRCommon_VRObjectTypeDefinitionService, VRCommon_VRMailMessageTypeService, VRCommon_VRTimeZoneService, VRCommon_CityService, VRCommon_CurrencyExchangeRateService, VRCommon_LogEntryService, VRCommon_CountryService, VRCommon_VRConnectionService, VRCommon_SettingsService, VRCommon_VRMailMessageTemplateService, VRCommon_RateTypeService, VRCommon_CurrencyService, VRCommon_RegionService, VR_Common_GenericLKUPService) {
    VRCommon_RegionService.registerDrillDownToCountry();
    VRCommon_CityService.registerDrillDownToCountry();
    VRCommon_VRLocalizationTextResourceService.registerDrillDownToModule();
    VRCommon_CityService.registerDrillDownToRegion();
    VRCommon_CurrencyExchangeRateService.registerDrillDownToCurrency();
    VRCommon_LogEntryService.registerLogToMaster();
    VRCommon_CityService.registerObjectTrackingDrillDownToCity();
    VRCommon_RegionService.registerObjectTrackingDrillDownToRegion();
    VRCommon_VRConnectionService.registerObjectTrackingDrillDownToConnection();
    VRCommon_SettingsService.registerObjectTrackingDrillDownToSetting();
    VRCommon_VRMailMessageTemplateService.registerObjectTrackingDrillDownToVRMailTemplateMessage();
    VRCommon_RateTypeService.registerObjectTrackingDrillDownToRateType();
    VRCommon_CurrencyService.registerObjectTrackingDrillDownToCurrency();
    VRCommon_VRTimeZoneService.registerObjectTrackingDrillDownToTimeZone();
    VRCommon_VRMailMessageTypeService.registerObjectTrackingDrillDownToVRMailMessageMailType();
    VRCommon_VRObjectTypeDefinitionService.registerObjectTrackingDrillDownToVRObjectTypeDefinition();
    VR_Common_StatusDefinitionService.registerObjectTrackingDrillDownToStatusDefinition();
    VRCommon_VRComponentTypeService.registerObjectTrackingDrillDownToVRComponentType();
    VRCommon_StyleDefinitionService.registerObjectTrackingDrillDownToStyleDefinition();
    VR_Common_GenericLKUPService.registerObjectTrackingDrillDownToGenericLKUP();
    VRCommon_CountryService.registerHistoryViewAction();
    VRCommon_CityService.registerHistoryViewAction();
    VRCommon_SettingsService.registerHistoryViewAction();
    VRCommon_RateTypeService.registerHistoryViewAction();
    VRCommon_RegionService.registerHistoryViewAction();
    VR_Common_GenericLKUPService.registerHistoryViewAction();
}]);
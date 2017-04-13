app.run(['WhS_BE_SellingNumberPlanService', 'WhS_BE_CarrierProfileService', 'WhS_BE_ZoneServiceConfigService', 'WhS_BE_SalePriceListTemplateService', 'WhS_BE_SwitchConnectivityService', 'WhS_BE_SwitchService', 'WhS_BE_CarrierAccountService', 'WhS_BE_CodeGroupService', 'WhS_BE_SaleZoneService', 'WhS_BE_SupplierRateService', 'WhS_BE_SupplierCodeService', 'WhS_BE_SupplierZoneService_Service', 'WhS_BE_CustomerSellingProductService',
function (WhS_BE_SellingNumberPlanService,WhS_BE_CarrierProfileService, WhS_BE_ZoneServiceConfigService, WhS_BE_SalePriceListTemplateService, WhS_BE_SwitchConnectivityService, WhS_BE_SwitchService, WhS_BE_CarrierAccountService, WhS_BE_CodeGroupService, WhS_BE_SaleZoneService, WhS_BE_SupplierRateService, WhS_BE_SupplierCodeService, WhS_BE_SupplierZoneService_Service, WhS_BE_CustomerSellingProductService) {
    WhS_BE_CodeGroupService.registerDrillDownToCountry();
    WhS_BE_SaleZoneService.registerDrillDownToSellingNumberPlan();
    WhS_BE_SupplierRateService.registerDrillDownToSupplierZone();
    WhS_BE_SupplierCodeService.registerDrillDownToSupplierZone();
    WhS_BE_SupplierZoneService_Service.registerDrillDownToSupplierZone();
    WhS_BE_CustomerSellingProductService.registerDrillDownToCarrierAccount();
  
    WhS_BE_CodeGroupService.registerObjectTrackingDrillDownToCodeGroupe();
    WhS_BE_SwitchService.registerObjectTrackingDrillDownToSwitch();
    WhS_BE_SwitchConnectivityService.registerObjectTrackingDrillDownToSwitchConnectivity();
    WhS_BE_SalePriceListTemplateService.registerObjectTrackingDrillDownToSalePriceListTemplate();
    WhS_BE_ZoneServiceConfigService.registerObjectTrackingDrillDownToZoneServiceConfig();
    WhS_BE_CarrierAccountService.registerHistoryViewAction();
    WhS_BE_CarrierProfileService.registerHistoryViewAction();
    WhS_BE_SalePriceListTemplateService.registerHistoryViewAction();
    WhS_BE_SwitchConnectivityService.registerHistoryViewAction();
    WhS_BE_SellingNumberPlanService.registerHistoryViewAction();
}]);

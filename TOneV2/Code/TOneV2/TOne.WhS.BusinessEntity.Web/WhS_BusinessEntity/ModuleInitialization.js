app.run(['WhS_BE_CodeGroupService', 'VRCommon_SaleZoneService', 'WhS_BE_SupplierRateService', function (WhS_BE_CodeGroupService, VRCommon_SaleZoneService, WhS_BE_SupplierRateService) {
    WhS_BE_CodeGroupService.registerDrillDownToCountry();
    VRCommon_SaleZoneService.registerDrillDownToSellingNumberPlan();
    WhS_BE_SupplierRateService.registerDrillDownToSupplierZone();
}]);

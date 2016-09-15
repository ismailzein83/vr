app.run(['WhS_BE_CodeGroupService', 'WhS_BE_SaleZoneService', 'WhS_BE_SupplierRateService', 'WhS_BE_SupplierCodeService','WhS_BE_SupplierZoneService_Service',
function (WhS_BE_CodeGroupService, WhS_BE_SaleZoneService, WhS_BE_SupplierRateService, WhS_BE_SupplierCodeService , WhS_BE_SupplierZoneService_Service) {
    WhS_BE_CodeGroupService.registerDrillDownToCountry();
    WhS_BE_SaleZoneService.registerDrillDownToSellingNumberPlan();
    WhS_BE_SupplierRateService.registerDrillDownToSupplierZone();
    WhS_BE_SupplierCodeService.registerDrillDownToSupplierZone();
    WhS_BE_SupplierZoneService_Service.registerDrillDownToSupplierZone();
}]);

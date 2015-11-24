app.run(['WhS_BE_CodeGroupService', 'VRCommon_SaleZoneService', function (WhS_BE_CodeGroupService, VRCommon_SaleZoneService) {
    WhS_BE_CodeGroupService.registerDrillDownToCountry();
    VRCommon_SaleZoneService.registerDrillDownToSellingNumberPlan();
}]);

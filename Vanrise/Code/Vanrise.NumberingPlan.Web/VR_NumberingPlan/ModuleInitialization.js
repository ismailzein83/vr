app.run(['Vr_NP_CodeGroupService','Vr_NP_SaleZoneService',function (Vr_NP_CodeGroupService, Vr_NP_SaleZoneService) {
    Vr_NP_CodeGroupService.registerDrillDownToCountry();
    Vr_NP_SaleZoneService.registerDrillDownToSellingNumberPlan();
}]);

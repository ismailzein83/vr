﻿app.run(['WhS_BE_SwitchConnectivityService', 'WhS_BE_SwitchService', 'WhS_BE_CarrierAccountService', 'WhS_BE_CodeGroupService', 'WhS_BE_SaleZoneService', 'WhS_BE_SupplierRateService', 'WhS_BE_SupplierCodeService', 'WhS_BE_SupplierZoneService_Service', 'WhS_BE_CustomerSellingProductService',
function (WhS_BE_SwitchConnectivityService, WhS_BE_SwitchService, WhS_BE_CarrierAccountService, WhS_BE_CodeGroupService, WhS_BE_SaleZoneService, WhS_BE_SupplierRateService, WhS_BE_SupplierCodeService, WhS_BE_SupplierZoneService_Service, WhS_BE_CustomerSellingProductService) {
    WhS_BE_CodeGroupService.registerDrillDownToCountry();
    WhS_BE_SaleZoneService.registerDrillDownToSellingNumberPlan();
    WhS_BE_SupplierRateService.registerDrillDownToSupplierZone();
    WhS_BE_SupplierCodeService.registerDrillDownToSupplierZone();
    WhS_BE_SupplierZoneService_Service.registerDrillDownToSupplierZone();
    WhS_BE_CustomerSellingProductService.registerDrillDownToCarrierAccount();
    WhS_BE_CarrierAccountService.registerObjectTrackingDrillDownToCarrierAccount();
    WhS_BE_CodeGroupService.registerObjectTrackingDrillDownToCodeGroupe();
    WhS_BE_SwitchService.registerObjectTrackingDrillDownToSwitch();
    WhS_BE_SwitchConnectivityService.registerObjectTrackingDrillDownToSwitchConnectivity();
}]);

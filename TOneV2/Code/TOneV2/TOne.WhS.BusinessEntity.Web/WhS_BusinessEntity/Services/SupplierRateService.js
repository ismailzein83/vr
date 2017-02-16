(function (appControllers) {

    'use strict';

    SupplierRateService.$inject = ['WhS_BE_SupplierZoneService'];

    function SupplierRateService(WhS_BE_SupplierZoneService) {
        var drillDownDefinitions = [];

        return ({
            registerDrillDownToSupplierZone: registerDrillDownToSupplierZone
        });

        function registerDrillDownToSupplierZone() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Supplier Rates";
            drillDownDefinition.directive = "vr-whs-be-supplierrate-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, supplierZoneItem) {
                supplierZoneItem.supplierRateGridAPI = directiveAPI;
                var payload = {
                    $type: "TOne.WhS.BusinessEntity.Business.SupplierRateQueryHandler,TOne.WhS.BusinessEntity.Business",
                    Query: {
                        ZoneIds: [supplierZoneItem.Entity.SupplierZoneId],
                        EffectiveOn: supplierZoneItem.EffectiveOn,
                        HideHistory: true
                    }
                };
                return supplierZoneItem.supplierRateGridAPI.loadGrid(payload);
            };

            WhS_BE_SupplierZoneService.addDrillDownDefinition(drillDownDefinition);
        }
    }

    appControllers.service('WhS_BE_SupplierRateService', SupplierRateService);

})(appControllers);

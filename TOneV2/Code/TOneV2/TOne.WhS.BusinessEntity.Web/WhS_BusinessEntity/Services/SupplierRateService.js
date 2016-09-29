(function (appControllers) {

    'use stict';

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
              

                var query = {
                    ZoneIds: [supplierZoneItem.Entity.SupplierZoneId],
                    EffectiveOn: supplierZoneItem.EffectiveOn
                };
               
                return supplierZoneItem.supplierRateGridAPI.loadGrid(query);
            };

            WhS_BE_SupplierZoneService.addDrillDownDefinition(drillDownDefinition);
        }
    }

    appControllers.service('WhS_BE_SupplierRateService', SupplierRateService);

})(appControllers);

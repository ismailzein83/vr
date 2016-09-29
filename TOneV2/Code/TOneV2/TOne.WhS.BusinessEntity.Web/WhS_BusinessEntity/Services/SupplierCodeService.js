(function (appControllers) {

    'use stict';

    SupplierCodeService.$inject = ['WhS_BE_SupplierZoneService'];

    function SupplierCodeService(WhS_BE_SupplierZoneService) {
        var drillDownDefinitions = [];

        return ({
            registerDrillDownToSupplierZone: registerDrillDownToSupplierZone
        });

        function registerDrillDownToSupplierZone() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Supplier Codes";
            drillDownDefinition.directive = "vr-whs-be-suppliercode-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, supplierZoneItem) {
                supplierZoneItem.supplierCodeGridAPI = directiveAPI;
                
                var query = {
                    SupplierId: supplierZoneItem.Entity.SupplierId,
                    ZoneIds: [supplierZoneItem.Entity.SupplierZoneId],
                    EffectiveOn: supplierZoneItem.EffectiveOn
                };
               
                return supplierZoneItem.supplierCodeGridAPI.loadGrid(query);
            };

            WhS_BE_SupplierZoneService.addDrillDownDefinition(drillDownDefinition);
        }
    }

    appControllers.service('WhS_BE_SupplierCodeService', SupplierCodeService);

})(appControllers);

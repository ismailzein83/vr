(function (appControllers) {

    'use strict';

    SupplierZoneService_Service.$inject = ['WhS_BE_SupplierZoneService'];

    function SupplierZoneService_Service(WhS_BE_SupplierZoneService) {
        var drillDownDefinitions = [];

        return ({
            registerDrillDownToSupplierZone: registerDrillDownToSupplierZone
        });

        function registerDrillDownToSupplierZone() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Supplier Services";
            drillDownDefinition.directive = "vr-whs-be-supplierzoneservice-grid";

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

    appControllers.service('WhS_BE_SupplierZoneService_Service', SupplierZoneService_Service);

})(appControllers);

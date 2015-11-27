
app.service('WhS_BE_SupplierCodeService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'WhS_BE_SupplierZoneService',
    function (VRModalService, VRNotificationService, UtilsService, WhS_BE_SupplierZoneService) {
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
                    ZoneIds: [supplierZoneItem.Entity.SupplierZoneId],
                };
               
                return supplierZoneItem.supplierCodeGridAPI.loadGrid(query);
            };

            WhS_BE_SupplierZoneService.addDrillDownDefinition(drillDownDefinition);
        }
      

    }]);

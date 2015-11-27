
app.service('WhS_BE_SupplierRateService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'WhS_BE_SupplierZoneService',
    function (VRModalService, VRNotificationService, UtilsService, WhS_BE_SupplierZoneService) {
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
                };
               
                return supplierZoneItem.supplierRateGridAPI.loadGrid(query);
            };

            WhS_BE_SupplierZoneService.addDrillDownDefinition(drillDownDefinition);
        }
      

    }]);

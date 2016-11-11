(function (appControllers) {

    'use strict';

    SupplierZoneService.$inject = [];

    function SupplierZoneService() {
        var drillDownDefinitions = [];

        return ({
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        });

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('WhS_BE_SupplierZoneService', SupplierZoneService);

})(appControllers);

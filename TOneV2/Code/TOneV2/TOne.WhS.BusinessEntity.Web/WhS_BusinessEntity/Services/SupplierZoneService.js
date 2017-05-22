(function (appControllers) {

    'use strict';

    SupplierZoneService.$inject = ["VRModalService"];

    function SupplierZoneService(VRModalService) {
        var drillDownDefinitions = [];

        return ({
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            editSupplierService: editSupplierService
        });

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function editSupplierService(supplierServiceObj, supplierId, effectiveOn, onSupplierServiceUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSupplierServiceUpdated = onSupplierServiceUpdated;
            };
            var parameters = {
                SupplierServiceObj: supplierServiceObj,
                SupplierId: supplierId,
                EffectiveOn: effectiveOn
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SupplierService/SupplierServiceEditor.html', parameters, settings);
        }
    }

    appControllers.service('WhS_BE_SupplierZoneService', SupplierZoneService);

})(appControllers);


app.service('QM_BE_SupplierService', ['QM_BE_SupplierAPIService', 'VRModalService', 'VRNotificationService',
    function (QM_BE_SupplierAPIService, VRModalService, VRNotificationService) {

        return ({
            addSupplier: addSupplier,
            editSupplier: editSupplier
        });

        function addSupplier(onSupplierAdded) {
            var settings = {
            };

            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSupplierAdded = onSupplierAdded;
            };

            VRModalService.showModal('/Client/Modules/QM_BusinessEntity/Views/Supplier/SupplierEditor.html', parameters, settings);
        }

        function editSupplier(supplierId, onSupplierUpdated) {
            var modalSettings = {
            };
            var parameters = {
                supplierId: supplierId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSupplierUpdated = onSupplierUpdated;
            };
            VRModalService.showModal('/Client/Modules/QM_BusinessEntity/Views/Supplier/SupplierEditor.html', parameters, modalSettings);
        }

    }]);

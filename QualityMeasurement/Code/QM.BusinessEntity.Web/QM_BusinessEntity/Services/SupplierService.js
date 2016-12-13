(function (appControllers) {

    'use strict';

    SupplierService.$inject = ['QM_BE_SupplierAPIService', 'VRModalService', 'VRNotificationService'];

    function SupplierService(QM_BE_SupplierAPIService, VRModalService, VRNotificationService) {
        return {
            addSupplier: addSupplier,
            editSupplier: editSupplier,
            uploadNewSuppliers: uploadNewSuppliers
        };

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

        function uploadNewSuppliers(modalScope) {

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Upload Suppliers"


            }
            var parameters = {};


            VRModalService.showModal('/Client/Modules/QM_BusinessEntity/Views/Supplier/SupplierUploader.html', parameters, modalSettings);
        }
    }
    appControllers.service('QM_BE_SupplierService', SupplierService);

})(appControllers);

app.service('CP_SupplierPricelist_SupplierMappingService', ['VRModalService', 'VRNotificationService', 'CP_SupplierPricelist_SupplierMappingAPIService',
    function (VRModalService, vRNotificationService, supplierMappingAPIService) {

        function addSupplierMapping(onSupplierMappingAdded) {
            var settings = {
            
            };
            var parameters = {
            };

            settings.onScopeReady = function(modalScope) {
                modalScope.onSupplierMappingAdded = onSupplierMappingAdded;
            };
            VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/SupplierMapping/SupplierMappingEditor.html', parameters, settings);
        }

        function editSupplierMapping(userId, onSupplierMappingUpdated) {
                var modalSettings = {
                };
                var parameters = {
                    UserId: userId
                };

                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.onSupplierMappingUpdated = onSupplierMappingUpdated
                };
                VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/SupplierMapping/SupplierMappingEditor.html', parameters, modalSettings);
        }
        function deleteSupplierMapping(scope, supplierMappingId, onCustomerSupplierMappingDeleted) {
            vRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return supplierMappingAPIService.DeleteCustomerSupplierMapping(supplierMappingId)
                            .then(function (deletionResponse) {
                                vRNotificationService.notifyOnItemDeleted("Supplier Mapping", deletionResponse);
                                onCustomerSupplierMappingDeleted();
                            })
                            .catch(function (error) {
                                vRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }
        return ({
            addSupplierMapping: addSupplierMapping,
            editSupplierMapping: editSupplierMapping,
            deleteSupplierMapping: deleteSupplierMapping
        });
    }]);

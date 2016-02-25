
app.service('CP_SupplierPricelist_SupplierMappingService', ['VRModalService',
    function ( VRModalService) {

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
        return ({
            addSupplierMapping: addSupplierMapping,
            editSupplierMapping: editSupplierMapping
        });
    }]);

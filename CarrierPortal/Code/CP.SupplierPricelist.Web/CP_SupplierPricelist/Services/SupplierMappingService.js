
app.service('CP_SupplierPricelist_SupplierMappingService', ['VRModalService',
    function ( VRModalService) {

        function addSupplierMapping(onSupplierAdded) {
            var settings = {
            
            };
            var parameters = {
            };

            settings.onScopeReady = function(modalScope) {
                //modalScope.onCustomerAdded = onCustomerAdded;
            };
            VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/SupplierMapping/SupplierMappingEditor.html', parameters, settings);
        }

        function editSupplierMapping(customerId, onCustomerUpdated) {
                var modalSettings = {
                };
                var parameters = {
                   // customerId: customerId
                };

                modalSettings.onScopeReady = function (modalScope) {
                  //  modalScope.onCustomerUpdated = onCustomerUpdated;
                };
                VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/SupplierMapping/SupplierMappingEditor.html', parameters, modalSettings);
            }
        return ({
            addSupplierMapping: addSupplierMapping,
            editSupplierMapping: editSupplierMapping
        });
    }]);

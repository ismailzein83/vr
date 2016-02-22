
app.service('CP_SupplierPricelist_SupplierService', ['VRModalService',
    function ( VRModalService) {

        function addSupplier(onSupplierAdded) {
            var settings = {
            
            };
            var parameters = {
            };

            settings.onScopeReady = function(modalScope) {
                //modalScope.onCustomerAdded = onCustomerAdded;
            };
            VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/Supplier/SupplierEditor.html', parameters, settings);
        }

        function editSupplier(customerId, onCustomerUpdated) {
                var modalSettings = {
                };
                var parameters = {
                   // customerId: customerId
                };

                modalSettings.onScopeReady = function (modalScope) {
                  //  modalScope.onCustomerUpdated = onCustomerUpdated;
                };
                VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/Supplier/SupplierEditor.html', parameters, modalSettings);
            }
        return ({
            addSupplier: addSupplier,
            editSupplier: editSupplier
        });
    }]);

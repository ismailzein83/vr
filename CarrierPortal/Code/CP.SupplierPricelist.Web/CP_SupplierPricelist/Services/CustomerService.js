
app.service('CP_SupplierPricelist_CustomerService', ['VRModalService',
    function (VRModalService) {
        var drillDownDefinitions = [];

        function addCustomer(onCustomerAdded) {
            var settings = {

            };
            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCustomerAdded = onCustomerAdded;
            };
            VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/Customer/CustomerEditor.html', parameters, settings);
        }
        function editcustomer(customerId, onCustomerUpdated) {
            var modalSettings = {
            };
            var parameters = {
                customerId: customerId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCustomerUpdated = onCustomerUpdated;
            };
            VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/Customer/CustomerEditor.html', parameters, modalSettings);
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return ({
            addCustomer: addCustomer,
            editcustomer: editcustomer,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        });
    }]);

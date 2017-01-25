
(function (appControllers) {

    "use strict";

    CustomerRouteService.$inject = ['VRModalService'];

    function CustomerRouteService(NPModalService) {

        function editCustomerRoute(destination, customerId, onCustomerRouteUpdated) {
            var settings = {};
            var parameters = {
                Destination: destination,
                CustomerId: customerId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCustomerRouteUpdated = onCustomerRouteUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/CustomerRoute/CustomerRouteEditor.html', parameters, settings);
        }

        return {
            editCustomerRoute: editCustomerRoute
        };
    }
    appControllers.service('NP_IVSwitch_CustomerRouteService', CustomerRouteService);

})(appControllers);
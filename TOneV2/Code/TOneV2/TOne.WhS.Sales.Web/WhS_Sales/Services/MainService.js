app.service('WhS_Sales_MainService', ["VRModalService", function (VRModalService) {

    return ({
        sellCustomerZones: sellCustomerZones,
    });

    function sellNewZones(customerId, onCustomerZonesSold) {

        var modalSettings = {
        };

        var parameters = {
            CustomerId: customerId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onCustomerZonesSold = onCustomerZonesSold;
        };

        VRModalService.showModal('/Client/Modules/WhS_Sales/Views/SellNewZones.html', parameters, modalSettings);
    }
}]);

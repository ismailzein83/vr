app.service("WhS_Sales_MainService", ["VRModalService", function (VRModalService) {

    return ({
        sellNewZones: sellNewZones,
        viewRPRouteOptionSupplier: viewRPRouteOptionSupplier
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

        VRModalService.showModal("/Client/Modules/WhS_Sales/Views/SellNewZones.html", parameters, modalSettings);
    }

    function viewRPRouteOptionSupplier(routingProductId, saleZoneId, supplierId) {
        var parameters = {
            RoutingProductId: routingProductId,
            SaleZoneId: saleZoneId,
            SupplierId: supplierId
        };

        VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RPRouteOptionSupplier.html", parameters, null);
    }
}]);

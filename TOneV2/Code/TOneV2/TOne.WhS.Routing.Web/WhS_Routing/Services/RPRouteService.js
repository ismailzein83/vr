(function (appControllers) {

    'use strict';

    RPRouteService.$inject = ['VRModalService'];

    function RPRouteService(VRModalService) {

        function viewRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId, currencyId, saleRate) {
            var parameters = {
                RoutingDatabaseId: routingDatabaseId,
                RoutingProductId: routingProductId,
                SaleZoneId: saleZoneId,
                SupplierId: supplierId,
                CurrencyId: currencyId,
                SaleRate: saleRate
            };
            
            VRModalService.showModal("/Client/Modules/WhS_Routing/Views/RPRoute/RPRouteOptionSupplier.html", parameters, null);
        }

        return ({
            viewRPRouteOptionSupplier: viewRPRouteOptionSupplier
        });
    }

    appControllers.service('WhS_Routing_RPRouteService', RPRouteService);

})(appControllers);

(function (appControllers) {

    'use strict';

    RPRouteService.$inject = ['VRModalService'];

    function RPRouteService(VRModalService) {

        function viewRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId, currencyId, saleRate, moduleName) {
            var parameters = {
                RoutingDatabaseId: routingDatabaseId,
                RoutingProductId: routingProductId,
                SaleZoneId: saleZoneId,
                SupplierId: supplierId,
                CurrencyId: currencyId,
                SaleRate: saleRate,
                ModuleName: moduleName
            };
            
            VRModalService.showModal("/Client/Modules/WhS_Routing/Views/RPRoute/RPRouteOptionSupplier.html", parameters, null);
        }

        return ({
            viewRPRouteOptionSupplier: viewRPRouteOptionSupplier
        });
    }

    appControllers.service('WhS_Routing_RPRouteService', RPRouteService);

})(appControllers);

﻿(function (appControllers) {

    'use strict';

    RPRouteService.$inject = ['VRModalService'];

    function RPRouteService(VRModalService) {
        return ({
            viewRPRouteOptionSupplier: viewRPRouteOptionSupplier
        });

        function viewRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId) {
            var parameters = {
                RoutingDatabaseId: routingDatabaseId,
                RoutingProductId: routingProductId,
                SaleZoneId: saleZoneId,
                SupplierId: supplierId
            };

            VRModalService.showModal("/Client/Modules/WhS_Routing/Views/RPRoute/RPRouteOptionSupplier.html", parameters, null);
        }
    }

    appControllers.service('WhS_Routing_RPRouteService', RPRouteService);

})(appControllers);

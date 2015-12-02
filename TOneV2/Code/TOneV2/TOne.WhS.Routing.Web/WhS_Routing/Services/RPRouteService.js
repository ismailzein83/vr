
app.service('WhS_Routing_RPRouteService', ['VRModalService', 'VRNotificationService',
    function (VRModalService, VRNotificationService) {

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
    }]);

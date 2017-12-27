(function (appControllers) {

    'use strict';

    SaleEntityZoneRoutingProductService.$inject = ['VRModalService'];

    function SaleEntityZoneRoutingProductService(VRModalService) {

        function editSaleEntityZoneRouting(customerId, sellingNumberPlanId, zoneId, zoneName, routingProductId, onZoneRoutingProductUpdated) {

            var parameters = {
                CustomerId: customerId,
                SellingNumberPlanId: sellingNumberPlanId,
                ZoneId: zoneId,
                ZoneName: zoneName,
                CurrentRoutingProductId: routingProductId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onZoneRoutingProductUpdated = onZoneRoutingProductUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneRoutingProduct/ZoneRoutingProductEditor.html', parameters, modalSettings);
        }

        return ({
            editSaleEntityZoneRouting: editSaleEntityZoneRouting
        });
    }

    appControllers.service('WhS_BE_SaleEntityZoneRoutingProductService', SaleEntityZoneRoutingProductService);

})(appControllers);
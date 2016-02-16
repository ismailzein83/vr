(function (appControllers) {

    'use stict';

    SaleZoneService.$inject = ['VRModalService'];

    function SaleZoneService(VRModalService) {
        return ({
            addSaleZone: addSaleZone,
            editSaleZone: editSaleZone//,
        });

        function addSaleZone(onSaleZoneAdded, sellingNumberPlanId) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSaleZoneAdded = onSaleZoneAdded;
            };
            var parameters = {};
            if (sellingNumberPlanId != undefined) {
                parameters.sellingNumberPlanId = sellingNumberPlanId;
            }

            VRModalService.showModal('/Client/Modules/Common/Views/SaleZone/SaleZoneEditor.html', parameters, settings);
        }

        function editSaleZone(saleZoneId, onSaleZoneUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSaleZoneUpdated = onSaleZoneUpdated;
            };
            var parameters = {
                SaleZoneId: saleZoneId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/SaleZone/SaleZoneEditor.html', parameters, settings);
        }

    }

    appControllers.service('WhS_BE_SaleZoneService', SaleZoneService);

})(appControllers);

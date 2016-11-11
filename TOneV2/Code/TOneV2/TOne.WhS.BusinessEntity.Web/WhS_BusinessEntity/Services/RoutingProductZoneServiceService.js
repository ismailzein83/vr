(function (appControllers) {

    'use strict';

    RoutingProductZoneServiceService.$inject = ['VRModalService', 'VRNotificationService'];

    function RoutingProductZoneServiceService(VRModalService, VRNotificationService) {

        function addRoutingProductZoneService(selectedSellingNumberPlanId, availableZoneIds, excludedZoneIds, onRoutingProductZoneServiceAdded) {

            var parameters = {
                selectedSellingNumberPlanId: selectedSellingNumberPlanId,
                availableZoneIds: availableZoneIds,
                excludedZoneIds: excludedZoneIds
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRoutingProductZoneServiceAdded = onRoutingProductZoneServiceAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductZoneServiceEditor.html', parameters, modalSettings);
        }

        function editRoutingProductZoneService(zoneService, selectedSellingNumberPlanId, availableZoneIds, excludedZoneIds, onRoutingProductZoneServiceUpdated) {

            var parameters = {
                zoneService: zoneService,
                selectedSellingNumberPlanId: selectedSellingNumberPlanId,
                availableZoneIds: availableZoneIds,
                excludedZoneIds: excludedZoneIds
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRoutingProductZoneServiceUpdated = onRoutingProductZoneServiceUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductZoneServiceEditor.html', parameters, modalSettings);
        }


        return ({
            addRoutingProductZoneService: addRoutingProductZoneService,
            editRoutingProductZoneService: editRoutingProductZoneService,
        });
    }

    appControllers.service('WhS_BE_RoutingProductZoneServiceService', RoutingProductZoneServiceService);

})(appControllers);
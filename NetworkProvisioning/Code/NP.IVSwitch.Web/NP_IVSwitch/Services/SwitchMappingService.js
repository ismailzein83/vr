
(function (appControllers) {

    "use strict";

    SwitchMappingService.$inject = ['VRModalService', 'WhS_BE_CarrierAccountService','UtilsService'];

    function SwitchMappingService(VRModalService, WhS_BE_CarrierAccountService, UtilsService) {

        function assignEndPoint(CarrierAccountId, onAssignEndPoint) {
            var settings = {};

            var parameters = {
                CarrierAccountId: CarrierAccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignEndPoint = onAssignEndPoint
            };
            VRModalService.showModal('/Client/Modules/NP_IVSwitch/Views/SwitchMapping/AssignEndPointEditor.html', parameters, settings);
        };

        function assignRoute(CarrierAccountId, onAssignRoute) {
            var settings = {};

            var parameters = {
                CarrierAccountId: CarrierAccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignRoute = onAssignRoute
            };
            VRModalService.showModal('/Client/Modules/NP_IVSwitch/Views/SwitchMapping/AssignRouteEditor.html', parameters, settings);
        };
       
        return {
            assignEndPoint: assignEndPoint,
            assignRoute: assignRoute
        };
    }

    appControllers.service('NP_IVSwitch_SwitchMappingService', SwitchMappingService);

})(appControllers);
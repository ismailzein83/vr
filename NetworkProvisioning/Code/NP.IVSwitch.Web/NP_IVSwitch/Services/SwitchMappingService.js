
(function (appControllers) {

    "use strict";

    SwitchMappingService.$inject = ['VRModalService', 'WhS_BE_CarrierAccountService','UtilsService'];

    function SwitchMappingService(VRModalService, WhS_BE_CarrierAccountService, UtilsService) {

        function linkEndPoints(CarrierAccountId, onEndPointLinked) {
            var settings = {};

            var parameters = {
                CarrierAccountId: CarrierAccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onEndPointLinked = onEndPointLinked
            };
            VRModalService.showModal('/Client/Modules/NP_IVSwitch/Views/SwitchMapping/LinkEndPointsEditor.html', parameters, settings);
        };

        function linkRoutes(CarrierAccountId, onRouteLinked) {
            var settings = {};

            var parameters = {
                CarrierAccountId: CarrierAccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteLinked = onRouteLinked
            };
            VRModalService.showModal('/Client/Modules/NP_IVSwitch/Views/SwitchMapping/LinkRoutesEditor.html', parameters, settings);
        };
       
        return {
            linkEndPoints: linkEndPoints,
            linkRoutes: linkRoutes
        };
    }

    appControllers.service('NP_IVSwitch_SwitchMappingService', SwitchMappingService);

})(appControllers);
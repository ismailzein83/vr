
(function (appControllers) {

    "use strict";

    RouteService.$inject = ['VRModalService'];

    function RouteService(NPModalService) {

        function addRoute(AccountId, onRouteAdded) {
            var settings = {};

            var parameters = {
                AccountId: AccountId,
            };

             settings.onScopeReady = function (modalScope) {
                modalScope.onRouteAdded = onRouteAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Route/RouteEditor.html', parameters, settings);
        };
        function editRoute(RouteId, onRouteUpdated) {
            var settings = {};

            var parameters = {
                RouteId: RouteId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteUpdated = onRouteUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Route/RouteEditor.html', parameters, settings);
        }


        return {
            addRoute: addRoute,
            editRoute: editRoute
        };
    }

    appControllers.service('NP_IVSwitch_RouteService', RouteService);

})(appControllers);
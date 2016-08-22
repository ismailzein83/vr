
(function (appControllers) {

    "use strict";

    RouteSyncDefinitionService.$inject = ['VRModalService'];

    function RouteSyncDefinitionService(VRModalService) {

        function addRouteSyncDefinition(onRouteSyncDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteSyncDefinitionAdded = onRouteSyncDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/WhS_RouteSync/Views/RouteSyncDefinition/RouteSyncDefinitionEditor.html', null, settings);
        };

        function editRouteSyncDefinition(routeSyncDefinitionId, onRouteSyncDefinitionUpdated) {
            var settings = {};

            var parameters = {
                routeSyncDefinitionId: routeSyncDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteSyncDefinitionUpdated = onRouteSyncDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_RouteSync/Views/RouteSyncDefinition/RouteSyncDefinitionEditor.html', parameters, settings);
        }


        return {
            addRouteSyncDefinition: addRouteSyncDefinition,
            editRouteSyncDefinition: editRouteSyncDefinition
        };
    }

    appControllers.service('WhS_RouteSync_RouteSyncDefinitionService', RouteSyncDefinitionService);

})(appControllers);

(function (appControllers) {

    'use stict';

    function beReceiveDefinitionService(vrModalService) {

        function addReceiveDefinition(onReceiveDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onReceiveDefinitionAdded = onReceiveDefinitionAdded;
            };
            vrModalService.showModal('/Client/Modules/VR_BEBridge/Views/BEReceiveDefinition/BEReceiveDefinitionEditor.html', null, settings);
        };
        function editReceiveDefinition(beReceiveDefinitionId, onReceiveDefinitionUpdated) {
            var settings = {};

            var parameters = {
                BEReceiveDefinitionId: beReceiveDefinitionId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onReceiveDefinitionUpdated = onReceiveDefinitionUpdated;
            };
            vrModalService.showModal('/Client/Modules/VR_BEBridge/Views/BEReceiveDefinition/BEReceiveDefinitionEditor.html', parameters, settings);
        }
        return {
            addReceiveDefinition: addReceiveDefinition,
            editReceiveDefinition: editReceiveDefinition
        };
    }

    beReceiveDefinitionService.$inject = ['VRModalService'];
    appControllers.service('VR_BEBridge_BEReceiveDefinitionService', beReceiveDefinitionService);

})(appControllers);
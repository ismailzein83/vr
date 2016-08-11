
(function (appControllers) {

    "use strict";

    VRObjectTypeDefinitionService.$inject = ['VRModalService'];

    function VRObjectTypeDefinitionService(VRModalService) {

        function addVRObjectTypeDefinition(onVRObjectTypeDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRObjectTypeDefinitionAdded = onVRObjectTypeDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectTypeDefinition/VRObjectTypeDefinitionEditor.html', null, settings);
        };

        function editVRObjectTypeDefinition(vrObjectTypeDefinitionId, onVRObjectTypeDefinitionUpdated) {
            var settings = {};

            var parameters = {
                vrObjectTypeDefinitionId: vrObjectTypeDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRObjectTypeDefinitionUpdated = onVRObjectTypeDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectTypeDefinition/VRObjectTypeDefinitionEditor.html', parameters, settings);
        }


        return {
            addVRObjectTypeDefinition: addVRObjectTypeDefinition,
            editVRObjectTypeDefinition: editVRObjectTypeDefinition
        };
    }

    appControllers.service('VRCommon_VRObjectTypeDefinitionService', VRObjectTypeDefinitionService);

})(appControllers);
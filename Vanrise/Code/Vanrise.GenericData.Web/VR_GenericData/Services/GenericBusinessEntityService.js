(function (appControllers) {

    'use strict';

    GenericBusinessEntityService.$inject = ['VRModalService'];

    function GenericBusinessEntityService(VRModalService) {
        return ({
            addGenericBusinessEntity: addGenericBusinessEntity,
            updateGenericBusinessEntity: updateGenericBusinessEntity
        });

        function addGenericBusinessEntity(businessEntityDefinitionId, onGenericBusinessEntityAdded) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBusinessEntityAdded = onGenericBusinessEntityAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        }

        function updateGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId, onGenericBusinessEntityUpdated) {
            var parameters = {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBusinessEntityUpdated = onGenericBusinessEntityUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        }
    };

    appControllers.service('VR_GenericData_GenericBusinessEntityService', GenericBusinessEntityService);

})(appControllers);
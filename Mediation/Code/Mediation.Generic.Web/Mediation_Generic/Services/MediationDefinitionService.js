(function (appControllers) {

    'use strict';

    MediationDefinitionService.$inject = ['VRModalService'];

    function MediationDefinitionService(VRModalService) {
        return ({
            addMediationDefinition: addMediationDefinition,
            editMediationDefinition: editMediationDefinition,
        });

        function addMediationDefinition(onMediationDefinitionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onMediationDefinitionAdded = onMediationDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Mediation_Generic/Views/MediationDefinition/MediationDefinitionEditor.html', null, modalSettings);
        }

        function editMediationDefinition(mediationDefinitionId, onMediationDefinitionUpdated) {
            var modalParameters = {
                MediationDefinitionId: mediationDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onMediationDefinitionUpdated = onMediationDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Mediation_Generic/Views/MediationDefinition/MediationDefinitionEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('Mediation_Generic_MediationDefinitionService', MediationDefinitionService);

})(appControllers);

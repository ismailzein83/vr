(function (appControllers) {

    'use strict';

    SummaryTransformationDefinitionService.$inject = ['VRModalService'];

    function SummaryTransformationDefinitionService(VRModalService) {
        return ({
            addSummaryTransformationDefinition: addSummaryTransformationDefinition,
            editSummaryTransformationDefinition: editSummaryTransformationDefinition,
        });

        function addSummaryTransformationDefinition(onSummaryTransformationDefinitionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSummaryTransformationDefinitionAdded = onSummaryTransformationDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/SummaryTransformationDefinition/SummaryTransformationDefinitionEditor.html', null, modalSettings);
        }

        function editSummaryTransformationDefinition(summaryTransformationDefinitionId, onSummaryTransformationDefinitionUpdated) {
            var modalParameters = {
                SummaryTransformationDefinitionId: summaryTransformationDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSummaryTransformationDefinitionUpdated = onSummaryTransformationDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/SummaryTransformationDefinition/SummaryTransformationDefinitionEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_GenericData_SummaryTransformationDefinitionService', SummaryTransformationDefinitionService);

})(appControllers);


(function (appControllers) {

    "use strict";

    dataAnalysisItemDefinitionService.$inject = ['VRModalService'];

    function dataAnalysisItemDefinitionService(VRModalService) {

        function addDataAnalysisItemDefinition(dataAnalysisDefinitionId, itemDefinitionTypeId, onDataAnalysisItemDefinitionAdded) {
            var settings = {};

            var parameters = {
                dataAnalysisDefinitionId: dataAnalysisDefinitionId,
                itemDefinitionTypeId: itemDefinitionTypeId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataAnalysisItemDefinitionAdded = onDataAnalysisItemDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/DataAnalysisItemDefinition/DataAnalysisItemDefinitionEditor.html', parameters, settings);
        };

        function editDataAnalysisItemDefinition(dataAnalysisItemDefinitionId, dataAnalysisDefinitionId, itemDefinitionTypeId, onDataAnalysisItemDefinitionUpdated) {
            var settings = {};

            var parameters = {
                dataAnalysisItemDefinitionId: dataAnalysisItemDefinitionId,
                dataAnalysisDefinitionId: dataAnalysisDefinitionId,
                itemDefinitionTypeId: itemDefinitionTypeId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataAnalysisItemDefinitionUpdated = onDataAnalysisItemDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/DataAnalysisItemDefinition/DataAnalysisItemDefinitionEditor.html', parameters, settings);
        }


        return {
            addDataAnalysisItemDefinition: addDataAnalysisItemDefinition,
            editDataAnalysisItemDefinition: editDataAnalysisItemDefinition
        };
    }

    appControllers.service('VR_Analytic_DataAnalysisItemDefinitionService', dataAnalysisItemDefinitionService);

})(appControllers);
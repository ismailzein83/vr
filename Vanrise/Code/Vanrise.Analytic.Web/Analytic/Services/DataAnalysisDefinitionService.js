
(function (appControllers) {

    "use strict";

    DataAnalysisDefinitionService.$inject = ['VRModalService'];

    function DataAnalysisDefinitionService(VRModalService) {

        function addDataAnalysisDefinition(onDataAnalysisDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataAnalysisDefinitionAdded = onDataAnalysisDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/DataAnalysisDefinition/DataAnalysisDefinitionEditor.html', null, settings);
        };

        function editDataAnalysisDefinition(dataAnalysisDefinitionId, onDataAnalysisDefinitionUpdated) {
            var settings = {};

            var parameters = {
                dataAnalysisDefinitionId: dataAnalysisDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataAnalysisDefinitionUpdated = onDataAnalysisDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/DataAnalysisDefinition/DataAnalysisDefinitionEditor.html', parameters, settings);
        }


        return {
            addDataAnalysisDefinition: addDataAnalysisDefinition,
            editDataAnalysisDefinition: editDataAnalysisDefinition
        };
    }

    appControllers.service('VR_Analytic_DataAnalysisDefinitionService', DataAnalysisDefinitionService);

})(appControllers);

(function (appControllers) {

    "use strict";

    dataAnalysisItemDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function dataAnalysisItemDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addDataAnalysisItemDefinition(dataAnalysisDefinitionId, itemDefinitionTypeId, onDataAnalysisItemDefinitionAdded) {
            var settings = {};

            var parameters = {
                dataAnalysisDefinitionId: dataAnalysisDefinitionId,
                itemDefinitionTypeId: itemDefinitionTypeId,
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
                itemDefinitionTypeId: itemDefinitionTypeId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataAnalysisItemDefinitionUpdated = onDataAnalysisItemDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/DataAnalysisItemDefinition/DataAnalysisItemDefinitionEditor.html', parameters, settings);
        }

        function getEntityUniqueName() {
            return "VR_Analytic_DataAnalysisItemDefinition";
        }

        function registerObjectTrackingDrillDownToDataAnalysisItemDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, dataAnalysisItemDefinitionItem) {
                dataAnalysisItemDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: dataAnalysisItemDefinitionItem.Entity.DataAnalysisItemDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return dataAnalysisItemDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addDataAnalysisItemDefinition: addDataAnalysisItemDefinition,
            editDataAnalysisItemDefinition: editDataAnalysisItemDefinition,
            registerObjectTrackingDrillDownToDataAnalysisItemDefinition: registerObjectTrackingDrillDownToDataAnalysisItemDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VR_Analytic_DataAnalysisItemDefinitionService', dataAnalysisItemDefinitionService);

})(appControllers);
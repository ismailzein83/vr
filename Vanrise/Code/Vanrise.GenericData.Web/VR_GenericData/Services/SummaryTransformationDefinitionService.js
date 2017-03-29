(function (appControllers) {

    'use strict';

    SummaryTransformationDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function SummaryTransformationDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addSummaryTransformationDefinition: addSummaryTransformationDefinition,
            editSummaryTransformationDefinition: editSummaryTransformationDefinition,
            registerObjectTrackingDrillDownToSummaryTransformationDefinition: registerObjectTrackingDrillDownToSummaryTransformationDefinition,
            getDrillDownDefinition: getDrillDownDefinition

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

        function getEntityUniqueName() {
            return "VR_GenericData_SummaryTransformationDefinition";
        }

        function registerObjectTrackingDrillDownToSummaryTransformationDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, summaryTransformationDefinitionItem) {
                summaryTransformationDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: summaryTransformationDefinitionItem.Entity.SummaryTransformationDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return summaryTransformationDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    };

    appControllers.service('VR_GenericData_SummaryTransformationDefinitionService', SummaryTransformationDefinitionService);

})(appControllers);

(function (appControllers) {

    "use strict";

    BusinessProcess_BPDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    var drillDownDefinitions = [];

    function BusinessProcess_BPDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {

        function addBusinessProcessDefinition(onBPDefenitionAdded) {

            var modalParameters = {
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBPDefenitionAdded = onBPDefenitionAdded;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPDefinition/BPTechnicalDefinitionEditor.html', modalParameters, modalSettings);
        }
        function editBusinessProcessDefinition(businessProcessDefinitionId, onBPDefenitionUpdated) {
            var modalParameters = {
                businessProcessDefinitionId: businessProcessDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBPDefenitionUpdated = onBPDefenitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPDefinition/BPTechnicalDefinitionEditor.html', modalParameters, modalSettings);
        }

        function getEntityUniqueName() {
            return "BusinessProcess_BP_BPDefinition";
        }

        function registerObjectTrackingDrillDownToBPDefinition() {
            var drillDownDefinition = {};
            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, bpDefinitionItem) {
                bpDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: bpDefinitionItem.Entity.BPDefinitionID,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return bpDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);
        }

        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function openCompletionView(completionViewURL, processInstanceId, hideSelectedColumn, onCompletionViewClosed) {
            var parameters = {
                processInstanceId: processInstanceId,
                HideSelectedColumn: hideSelectedColumn
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCompletionViewClosed = onCompletionViewClosed;
            };

            VRModalService.showModal(completionViewURL, parameters, settings);
        }

        return ({
            addBusinessProcessDefinition: addBusinessProcessDefinition,
            editBusinessProcessDefinition: editBusinessProcessDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToBPDefinition: registerObjectTrackingDrillDownToBPDefinition,
            openCompletionView: openCompletionView
        });
    }

    appControllers.service('BusinessProcess_BPDefinitionService', BusinessProcess_BPDefinitionService);
})(appControllers);
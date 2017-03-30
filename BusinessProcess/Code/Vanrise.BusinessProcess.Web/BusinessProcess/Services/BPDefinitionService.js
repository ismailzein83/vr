﻿(function (appControllers) {

    "use strict";
    BusinessProcess_BPDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];
    var drillDownDefinitions = [];
    function BusinessProcess_BPDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {

        return ({
            editBusinessProcessDefinition: editBusinessProcessDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToBPDefinition: registerObjectTrackingDrillDownToBPDefinition
        });

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
                console.log(bpDefinitionItem);
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
    }
    appControllers.service('BusinessProcess_BPDefinitionService', BusinessProcess_BPDefinitionService);

})(appControllers);
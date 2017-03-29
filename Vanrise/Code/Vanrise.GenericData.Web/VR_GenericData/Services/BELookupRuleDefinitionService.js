(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionService.$inject = ['VR_GenericData_BELookupRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function BELookupRuleDefinitionService(VR_GenericData_BELookupRuleDefinitionAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService)
    {
        var drillDownDefinitions = [];
        var beLookupRuleDefinitionEditorUrl = '/Client/Modules/VR_GenericData/Views/BELookupRuleDefinition/BELookupRuleDefinitionEditor.html';
        var beLookupRuleDefinitionCriteriaFieldEditorUrl = '/Client/Modules/VR_GenericData/Views/BELookupRuleDefinition/BELookupRuleDefinitionCriteriaFieldEditor.html';

        function addBELookupRuleDefinition(onBELookupRuleDefinitionAdded)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionAdded = onBELookupRuleDefinitionAdded;
            };

            VRModalService.showModal(beLookupRuleDefinitionEditorUrl, null, settings);
        }

        function editBELookupRuleDefinition(beLookupRuleDefinitionId, onBELookupRuleDefinitionUpdated)
        {
            var parameters = {
                beLookupRuleDefinitionId: beLookupRuleDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionUpdated = onBELookupRuleDefinitionUpdated;
            };

            VRModalService.showModal(beLookupRuleDefinitionEditorUrl, parameters, settings);
        }

        function addBELookupRuleDefinitionCriteriaField(criteriaFields, beDefinitionId, onBELookupRuleDefinitionCriteriaFieldAdded)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionCriteriaFieldAdded = onBELookupRuleDefinitionCriteriaFieldAdded;
            };

            var parameters = {
                criteriaFields: criteriaFields,
                beDefinitionId: beDefinitionId
            };

            VRModalService.showModal(beLookupRuleDefinitionCriteriaFieldEditorUrl, parameters, settings);
        }

        function editBELookupRuleDefinitionCriteriaField(criteriaFieldTitle, criteriaFields, beDefinitionId, onBELookupRuleDefinitionCriteriaFieldUpdated)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionCriteriaFieldUpdated = onBELookupRuleDefinitionCriteriaFieldUpdated;
            };

            var parameters = {
                criteriaFieldTitle: criteriaFieldTitle,
                criteriaFields: criteriaFields,
                beDefinitionId: beDefinitionId
            };

            VRModalService.showModal(beLookupRuleDefinitionCriteriaFieldEditorUrl, parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_GenericData_BELookupRuleDefinition";
        }

        function registerObjectTrackingDrillDownToBELookupRuleDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, beLookupRuleDefinitionItem) {
                beLookupRuleDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: beLookupRuleDefinitionItem.Entity.BELookupRuleDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return beLookupRuleDefinitionItem.objectTrackingGridAPI.load(query);
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
            addBELookupRuleDefinition: addBELookupRuleDefinition,
            editBELookupRuleDefinition: editBELookupRuleDefinition,
            addBELookupRuleDefinitionCriteriaField: addBELookupRuleDefinitionCriteriaField,
            editBELookupRuleDefinitionCriteriaField: editBELookupRuleDefinitionCriteriaField,
            registerObjectTrackingDrillDownToBELookupRuleDefinition: registerObjectTrackingDrillDownToBELookupRuleDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VR_GenericData_BELookupRuleDefinitionService', BELookupRuleDefinitionService);

})(appControllers);
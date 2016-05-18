(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionService.$inject = ['VRModalService'];

    function BELookupRuleDefinitionService(VRModalService)
    {
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

        function editBusinessEntityDefinition(beLookupRuleDefinitionId, onBELookupRuleDefinitionUpdated)
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

        return {
            addBELookupRuleDefinition: addBELookupRuleDefinition,
            editBusinessEntityDefinition: editBusinessEntityDefinition,
            addBELookupRuleDefinitionCriteriaField: addBELookupRuleDefinitionCriteriaField,
            editBELookupRuleDefinitionCriteriaField: editBELookupRuleDefinitionCriteriaField
        };
    }

    appControllers.service('VR_GenericData_BELookupRuleDefinitionService', BELookupRuleDefinitionService);

})(appControllers);
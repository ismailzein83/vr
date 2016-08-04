(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldService.$inject = ['VRModalService'];

    function GenericRuleDefinitionCriteriaFieldService(VRModalService) {

        return {
            addGenericRuleDefinitionCriteriaField: addGenericRuleDefinitionCriteriaField,
            editGenericRuleDefinitionCriteriaField: editGenericRuleDefinitionCriteriaField
        };

        function addGenericRuleDefinitionCriteriaField(genericRuleDefinitionCriteriaFields, context, onGenericRuleDefinitionCriteriaFieldAdded) {
            var modalParameters = {
                Context: context,
                GenericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionCriteriaFieldAdded = onGenericRuleDefinitionCriteriaFieldAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionCriteriaFieldEditor.html', modalParameters, modalSettings);
        }

        function editGenericRuleDefinitionCriteriaField(genericRuleDefinitionCriteriaFieldName, genericRuleDefinitionCriteriaFields, onGenericRuleDefinitionCriteriaFieldUpdated) {
            var modalParameters = {
                GenericRuleDefinitionCriteriaFieldName: genericRuleDefinitionCriteriaFieldName,
                GenericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionCriteriaFieldUpdated = onGenericRuleDefinitionCriteriaFieldUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionCriteriaFieldEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionCriteriaFieldService', GenericRuleDefinitionCriteriaFieldService);

})(appControllers);
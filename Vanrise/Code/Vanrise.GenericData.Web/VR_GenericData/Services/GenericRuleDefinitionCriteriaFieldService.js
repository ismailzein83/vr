(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldService.$inject = ['VRModalService'];

    function GenericRuleDefinitionCriteriaFieldService(VRModalService) {

        return {
            addGenericRuleDefinitionCriteriaField: addGenericRuleDefinitionCriteriaField,
            editGenericRuleDefinitionCriteriaField: editGenericRuleDefinitionCriteriaField
        };

        function addGenericRuleDefinitionCriteriaField(genericRuleDefinitionCriteriaFields, onGenericRuleDefinitionCriteriaFieldAdded) {
            var modalParameters = {
                genericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                onGenericRuleDefinitionCriteriaFieldAdded: onGenericRuleDefinitionCriteriaFieldAdded
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionCriteriaFieldEditor.html', modalParameters, modalSettings);
        }

        function editGenericRuleDefinitionCriteriaField(genericRuleDefinitionCriteriaFieldName, genericRuleDefinitionCriteriaFields, onGenericRuleDefinitionCriteriaFieldUpdated) {
            var modalParameters = {
                genericRuleDefinitionCriteriaFieldName: genericRuleDefinitionCriteriaFieldName,
                genericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                onGenericRuleDefinitionCriteriaFieldUpdated: onGenericRuleDefinitionCriteriaFieldUpdated
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionCriteriaFieldEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionCriteriaFieldService', GenericRuleDefinitionCriteriaFieldService);

})(appControllers);
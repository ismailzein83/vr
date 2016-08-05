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
                GenericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionCriteriaFieldAdded = onGenericRuleDefinitionCriteriaFieldAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionCriteriaFieldEditor.html', modalParameters, modalSettings);
        }

        function editGenericRuleDefinitionCriteriaField(genericRuleDefinitionCriteriaFieldName, genericRuleDefinitionCriteriaFields, context, onGenericRuleDefinitionCriteriaFieldUpdated) {
            var modalParameters = {
                GenericRuleDefinitionCriteriaFieldName: genericRuleDefinitionCriteriaFieldName,
                GenericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields,
                context: context
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
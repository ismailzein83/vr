(function (appControllers) {

    'use strict';

    ObjectPropertyVariableService.$inject = ['VRModalService'];

    function ObjectPropertyVariableService(VRModalService) {

        function addObjectPropertyVariable(context, onObjectPropertyVariableAdded) {
            var modalParameters = {
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectPropertyVariableAdded = onObjectPropertyVariableAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRObjectPropertyVariableEditor.html', modalParameters, modalSettings);
        }

        function editObjectPropertyVariable(genericRuleDefinitionCriteriaFieldName, genericRuleDefinitionCriteriaFields, context, onGenericRuleDefinitionCriteriaFieldUpdated) {
            var modalParameters = {
                GenericRuleDefinitionCriteriaFieldName: genericRuleDefinitionCriteriaFieldName,
                GenericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectPropertyVariableUpdated = onObjectPropertyVariableUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRObjectPropertyVariableEditor.html', modalParameters, modalSettings);
        }


        return {
            addObjectPropertyVariable: addObjectPropertyVariable,
            editObjectPropertyVariable: editObjectPropertyVariable
        };
    }

    appControllers.service('VRCommon_ObjectPropertyVariableService', ObjectPropertyVariableService);

})(appControllers);
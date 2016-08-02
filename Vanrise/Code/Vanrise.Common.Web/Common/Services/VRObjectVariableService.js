(function (appControllers) {

    'use strict';

    VRObjectVariableService.$inject = ['VRModalService'];

    function VRObjectVariableService(VRModalService) {

        function addVRObjectVariable(objectVariables ,onObjectVariableAdded) {

            var modalParameters = {
                objectVariables: objectVariables
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectVariableAdded = onObjectVariableAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectVariable/VRObjectVariableEditor.html', modalParameters, modalSettings);
        }

        function editVRObjectVariable(objectVariable, objectVariables, onGenericRuleDefinitionCriteriaFieldUpdated) {
            var modalParameters = {
                objectVariable: objectVariable,
                objectVariables: objectVariables
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectVariableUpdated = onGenericRuleDefinitionCriteriaFieldUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectVariable/VRObjectVariableEditor.html', modalParameters, modalSettings);
        }


        return {
            addVRObjectVariable: addVRObjectVariable,
            editVRObjectVariable: editVRObjectVariable
        };
    }

    appControllers.service('VRCommon_ObjectVariableService', VRObjectVariableService);

})(appControllers);
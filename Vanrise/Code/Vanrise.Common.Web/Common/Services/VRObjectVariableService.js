﻿(function (appControllers) {

    'use strict';

    VRObjectVariableService.$inject = ['VRModalService'];

    function VRObjectVariableService(VRModalService) {

        function addVRObjectVariable(onObjectVariableAdded) {

            var modalParameters;
            //var modalParameters = {
            //    GenericRuleDefinitionCriteriaFields: genericRuleDefinitionCriteriaFields
            //};

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectVariableAdded = onObjectVariableAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/VRObjectVariable/templates/VRObjectVariableEditor.html', modalParameters, modalSettings);
        }

        function editVRObjectVariable(objectVariable, onGenericRuleDefinitionCriteriaFieldUpdated) {
            var modalParameters = {
                objectVariable: objectVariable,
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectVariableUpdated = onGenericRuleDefinitionCriteriaFieldUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/VRObjectVariable/templates/VRObjectVariableEditor.html', modalParameters, modalSettings);
        }


        return {
            addVRObjectVariable: addVRObjectVariable,
            editVRObjectVariable: editVRObjectVariable
        };
    }

    appControllers.service('VRCommon_ObjectVariableService', VRObjectVariableService);

})(appControllers);
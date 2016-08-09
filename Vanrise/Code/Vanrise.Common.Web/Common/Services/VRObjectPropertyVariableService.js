(function (appControllers) {

    'use strict';

    ObjectPropertyVariableService.$inject = ['VRModalService'];

    function ObjectPropertyVariableService(VRModalService) {

        function addObjectPropertyVariable(variables, context, onObjectPropertyVariableAdded) {
            var modalParameters = {
                variables: variables,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectPropertyVariableAdded = onObjectPropertyVariableAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectPropertyVariable/VRObjectPropertyVariableEditor.html', modalParameters, modalSettings);
        }

        function editObjectPropertyVariable(variableName, variables, context, onObjectPropertyVariableUpdated) {
            var modalParameters = {
                variableName: variableName,
                variables: variables,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectPropertyVariableUpdated = onObjectPropertyVariableUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectPropertyVariable/VRObjectPropertyVariableEditor.html', modalParameters, modalSettings);
        }


        return {
            addObjectPropertyVariable: addObjectPropertyVariable,
            editObjectPropertyVariable: editObjectPropertyVariable
        };
    }

    appControllers.service('VRCommon_VRObjectPropertyVariableService', ObjectPropertyVariableService);

})(appControllers);
(function (appControllers) {

    'use strict';

    ExecutionFlowDefinitionService.$inject = ['VRModalService'];

    function ExecutionFlowDefinitionService(VRModalService) {
        return ({
            addExecutionFlowDefinition: addExecutionFlowDefinition,
            editExecutionFlowDefiniton: editExecutionFlowDefiniton,
        });

        function addExecutionFlowDefinition(onExecutionFlowDefinitionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowDefinitionAdded = onExecutionFlowDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionEditor.html', null, modalSettings);
        }

        function editExecutionFlowDefiniton(ID, onExecutionFlowDefinitionUpdated) {
            var modalParameters = {
                ID: ID
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowDefinitionUpdated = onExecutionFlowDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionEditor.html', modalParameters, modalSettings);
        }
 
    };

    appControllers.service('VR_Queueing_ExecutionFlowDefinitionService', ExecutionFlowDefinitionService);

})(appControllers);

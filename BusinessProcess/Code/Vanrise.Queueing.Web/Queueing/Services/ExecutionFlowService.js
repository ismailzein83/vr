(function (appControllers) {

    'use strict';

    ExecutionFlowService.$inject = ['VRModalService'];

    function ExecutionFlowService(VRModalService) {
        return ({
            addExecutionFlow: addExecutionFlow,
            editExecutionFlow: editExecutionFlow
        });

        function addExecutionFlow(onExecutionFlowAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowAdded = onExecutionFlowAdded;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlow/ExecutionFlowEditor.html', null, modalSettings);
        }

        function editExecutionFlow(userId, onUserUpdated) {
            var modalParameters = {
                userId: userId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserUpdated = onUserUpdated;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlow/ExecutionFlowEditor.html', modalParameters, modalSettings);
        }
 
    };

    appControllers.service('ExecutionFlowService', ExecutionFlowService);

})(appControllers);

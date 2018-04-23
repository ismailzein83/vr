(function (appControllers) {
    'use stict';
    RuntimeNodeService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRRuntime_RuntimeNodeAPIService'];
    function RuntimeNodeService(VRModalService, VRNotificationService, UtilsService, VRRuntime_RuntimeNodeAPIService) {

        function addRuntimeNode(onRuntimeNodeAdded) {
            var settings = {};
            settings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeAdded = onRuntimeNodeAdded;
            };
            var parameters = {};


            VRModalService.showModal('/Client/Modules/Runtime/Views/Runtime/RuntimeNodeEditor.html', parameters, settings);
        }

        function editRuntimeNode(runtimeNodeId, onRuntimeNodeUpdated) {
            var settings = {};

            settings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeUpdated = onRuntimeNodeUpdated;
            };
            var parameters = {
                RuntimeNodeId: runtimeNodeId
            };

            VRModalService.showModal('/Client/Modules/Runtime/Views/Runtime/RuntimeNodeEditor.html', parameters, settings);
        }

        return {
           addRuntimeNode: addRuntimeNode,
           editRuntimeNode: editRuntimeNode,
        };
    }

    appControllers.service('VRRuntime_RuntimeNodeService', RuntimeNodeService);

})(appControllers);

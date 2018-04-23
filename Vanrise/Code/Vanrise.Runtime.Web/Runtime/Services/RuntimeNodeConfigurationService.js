(function (appControllers) {
    'use stict';
    RuntimeNodeConfigurationService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];
    function RuntimeNodeConfigurationService(VRModalService, VRNotificationService, UtilsService) {


        function addRuntimeNodeConfiguration(onRuntimeNodeConfigurationAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeConfigurationAdded = onRuntimeNodeConfigurationAdded;
            };
            var parameters = {};


            VRModalService.showModal('/Client/Modules/Runtime/Views/Runtime/RuntimeNodeConfigurationEditor.html', parameters, settings);
        }

        function editRuntimeNodeConfiguration(runtimeNodeConfigurationId, onRuntimeNodeConfigurationUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeConfigurationUpdated = onRuntimeNodeConfigurationUpdated;
            };
            var parameters = {
                RuntimeNodeConfigurationId: runtimeNodeConfigurationId
            };

            VRModalService.showModal('/Client/Modules/Runtime/Views/Runtime/RuntimeNodeConfigurationEditor.html', parameters, settings);
        }

        function addRuntimeNodeConfigurationProcesses(onRuntimeNodeConfigurationAdded, runtimeNodeConfigurationId) {
            var modelParameters = {
                runtimeNodeConfigurationId: runtimeNodeConfigurationId
            };
            var modelSettings = {};

            modelSettings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeConfigurationAdded = onRuntimeNodeConfigurationAdded;
            };

            VRModalService.showModal('/Client/Modules/Runtime/Directives/RuntimeNodeConfiguration/Templates/RuntimeNodeConfigurationProcessesEditor.html', modelParameters, modelSettings);
        }

        function editRuntimeNodeConfigurationProcesses(runtimeNodeConfigurationEntity, onRuntimeNodeConfigurationsUpdated) {
            var modelParameters = {
                runtimeNodeConfigurationEntity: runtimeNodeConfigurationEntity
            };
            var modelSettings = {};

            modelSettings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeConfigurationsUpdated = onRuntimeNodeConfigurationsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Runtime/Directives/RuntimeNodeConfiguration/Templates/RuntimeNodeConfigurationProcessesEditor.html', modelParameters, modelSettings);
        }

        function addRuntimeNodeConfigurationServices(onRuntimeNodeConfigurationAdded, runtimeNodeConfigurationId) {
            var modelParameters = {
                runtimeNodeConfigurationId: runtimeNodeConfigurationId
            };
            var modelSettings = {};

            modelSettings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeConfigurationAdded = onRuntimeNodeConfigurationAdded;
            };

            VRModalService.showModal('/Client/Modules/Runtime/Directives/RuntimeNodeConfiguration/Templates/RuntimeNodeConfigurationServicesEditor.html', modelParameters, modelSettings);
        }
        function editRuntimeNodeConfigurationServices(runtimeNodeConfigurationEntity, onRuntimeNodeConfigurationsUpdated) {
            var modelParameters = {
                runtimeNodeConfigurationEntity: runtimeNodeConfigurationEntity
            };
            var modelSettings = {};

            modelSettings.onScopeReady = function (modelScope) {
                modelScope.onRuntimeNodeConfigurationsUpdated = onRuntimeNodeConfigurationsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Runtime/Directives/RuntimeNodeConfiguration/Templates/RuntimeNodeConfigurationServicesEditor.html', modelParameters, modelSettings);
        }


        return {
           addRuntimeNodeConfiguration: addRuntimeNodeConfiguration,
           editRuntimeNodeConfiguration: editRuntimeNodeConfiguration,
           addRuntimeNodeConfigurationProcesses: addRuntimeNodeConfigurationProcesses,
           editRuntimeNodeConfigurationProcesses: editRuntimeNodeConfigurationProcesses,
           addRuntimeNodeConfigurationServices: addRuntimeNodeConfigurationServices,
           editRuntimeNodeConfigurationServices: editRuntimeNodeConfigurationServices
        };
    }

    appControllers.service('VRRuntime_RuntimeNodeConfigurationService', RuntimeNodeConfigurationService);

})(appControllers);

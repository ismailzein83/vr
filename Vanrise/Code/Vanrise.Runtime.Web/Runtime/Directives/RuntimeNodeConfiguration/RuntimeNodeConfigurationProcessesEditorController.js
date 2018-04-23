(function (appControllers) {

    "use strict";

    runtimeNodeConfigurationProcessesEditorController.$inject = ['$scope', 'VRRuntime_RuntimeNodeConfigurationAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function runtimeNodeConfigurationProcessesEditorController($scope, VRRuntime_RuntimeNodeConfigurationAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var runtimeNodeConfigurationEntity;

        var servicesEditorAPI;
        var servicesEditorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                runtimeNodeConfigurationEntity = parameters.runtimeNodeConfigurationEntity;
            }
            isEditMode = (runtimeNodeConfigurationEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onServicesEditorReady = function (api) {
                servicesEditorAPI = api;
                servicesEditorReadyDeferred.resolve();

            };

            $scope.saveRuntimeNodeConfiguration = function () {
                if (isEditMode)
                    return updateRuntimeNodeConfigurations();
                else
                    return insertRuntimeNodeConfigurations();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls()
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServicesDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && runtimeNodeConfigurationEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(runtimeNodeConfigurationEntity.Name, "Runtime Node Configuration Processes");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Runtime Node Configuration Processes");
        }

        function loadStaticData() {

            if (runtimeNodeConfigurationEntity == undefined)
                return;
            $scope.scopeModel.Name = runtimeNodeConfigurationEntity.Name;
            if (runtimeNodeConfigurationEntity.Settings != undefined)
            {
                $scope.scopeModel.IsEnabled = runtimeNodeConfigurationEntity.Settings.IsEnabled;
                $scope.scopeModel.NbOfInstances = runtimeNodeConfigurationEntity.Settings.NbOfInstances;
            }
 
        }

        function loadServicesDirective() {
            var servicesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            servicesEditorReadyDeferred.promise
                .then(function () {
                    var payload;
                    if (runtimeNodeConfigurationEntity != undefined && runtimeNodeConfigurationEntity.Settings != undefined) {
                        payload = {
                            services: runtimeNodeConfigurationEntity.Settings.Services
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(servicesEditorAPI, payload, servicesLoadPromiseDeferred);
                });
            return servicesLoadPromiseDeferred.promise;
        }


        function buildRuntimeNodeConfigurationsObjFromScope() {
            var obj = {
                RuntimeProcessConfigurationId:runtimeNodeConfigurationEntity != undefined? runtimeNodeConfigurationEntity.RuntimeProcessConfigurationId:UtilsService.guid(),
                Name: $scope.scopeModel.Name,
                Settings: {
                    IsEnabled: $scope.scopeModel.IsEnabled,
                    NbOfInstances: $scope.scopeModel.NbOfInstances,
                    Services: servicesEditorAPI.getData()
                }

            };
            return obj;
        }

        function insertRuntimeNodeConfigurations() {
            var runtimeNodeConfigurationsObject = buildRuntimeNodeConfigurationsObjFromScope();
            if ($scope.onRuntimeNodeConfigurationAdded != undefined)
                $scope.onRuntimeNodeConfigurationAdded(runtimeNodeConfigurationsObject);
            $scope.modalContext.closeModal();

        }
        function updateRuntimeNodeConfigurations() {
            var runtimeNodeConfigurationsObject = buildRuntimeNodeConfigurationsObjFromScope();
            if ($scope.onRuntimeNodeConfigurationsUpdated != undefined)
                $scope.onRuntimeNodeConfigurationsUpdated(runtimeNodeConfigurationsObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VRRuntime_RuntimeNodeConfigurationProcessesEditorController', runtimeNodeConfigurationProcessesEditorController);
})(appControllers);

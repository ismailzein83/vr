(function (appControllers) {
    'use strict';

    RuntimeNodeConfigurationEditorController.$inject = ['$scope', 'VRRuntime_RuntimeNodeConfigurationService', 'VRRuntime_RuntimeNodeConfigurationAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function RuntimeNodeConfigurationEditorController($scope, VRRuntime_RuntimeNodeConfigurationService, VRRuntime_RuntimeNodeConfigurationAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {


        var runtimeNodeConfigurationId;
        var editMode;
        var runtimeNodeConfigurationEntity;
        var processesEditorAPI;
        var processesEditorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                runtimeNodeConfigurationId = parameters.RuntimeNodeConfigurationId;
            }
            editMode = (runtimeNodeConfigurationId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onProcessesEditorReady = function (api) {
                 processesEditorAPI = api;
                 processesEditorReadyDeferred.resolve();

             };

            $scope.saveRuntimeNodeConfiguration = function () {
                if (editMode)
                    return updateRuntimeNodeConfiguration();
                else
                    return insertRuntimeNodeConfiguration();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (editMode) {
                getRuntimeNodeConfiguration().then(function () {
                    loadAllControls().finally(function () {
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });
                });
            }
            else {
                loadAllControls().catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
        }

        function getRuntimeNodeConfiguration() {
            return VRRuntime_RuntimeNodeConfigurationAPIService.GetRuntimeNodeConfiguration(runtimeNodeConfigurationId).then(function (runtimeNodeConfiguration) {
                runtimeNodeConfigurationEntity = runtimeNodeConfiguration;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadProcessesDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (editMode && runtimeNodeConfigurationEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(runtimeNodeConfigurationEntity.Name, "Runtime Node Configuration");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Runtime Node Configuration");
        }

        function loadStaticData() {

            if (runtimeNodeConfigurationEntity == undefined)
                return;

            $scope.scopeModel.name = runtimeNodeConfigurationEntity.Name;
        }

        function loadProcessesDirective() {
            var processesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            processesEditorReadyDeferred.promise
                .then(function () {
                    var payload;
                    if (runtimeNodeConfigurationEntity != undefined && runtimeNodeConfigurationEntity.Settings != undefined) {
                        payload = {
                            processes: runtimeNodeConfigurationEntity.Settings.Processes
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(processesEditorAPI, payload, processesLoadPromiseDeferred);
                });
            return processesLoadPromiseDeferred.promise;
        }

        function buildRuntimeNodeConfigurationObjFromScope() {
            var obj = {
                RuntimeNodeConfigurationId: runtimeNodeConfigurationId,
                Name: $scope.scopeModel.name,
                Settings: {
                    Processes: processesEditorAPI.getData()
                }
            };
            return obj;
        }

        function insertRuntimeNodeConfiguration() {
            $scope.isLoading = true;

            var runtimeNodeConfigurationObject = buildRuntimeNodeConfigurationObjFromScope();
            return VRRuntime_RuntimeNodeConfigurationAPIService.AddRuntimeNodeConfiguration(runtimeNodeConfigurationObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Runtime Node Configuration", response, "Name")) {
                    if ($scope.onRuntimeNodeConfigurationAdded != undefined)
                        $scope.onRuntimeNodeConfigurationAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updateRuntimeNodeConfiguration() {
            $scope.isLoading = true;

            var runtimeNodeConfigurationObject = buildRuntimeNodeConfigurationObjFromScope();
            VRRuntime_RuntimeNodeConfigurationAPIService.UpdateRuntimeNodeConfiguration(runtimeNodeConfigurationObject).then(function (response) { 
                if (VRNotificationService.notifyOnItemUpdated("Runtime Node Configuration", response, "Name")) {
                    if ($scope.onRuntimeNodeConfigurationUpdated != undefined)
                        $scope.onRuntimeNodeConfigurationUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

    }

    appControllers.controller('VRRuntime_RuntimeNodeConfigurationEditorController', RuntimeNodeConfigurationEditorController);

})(appControllers);

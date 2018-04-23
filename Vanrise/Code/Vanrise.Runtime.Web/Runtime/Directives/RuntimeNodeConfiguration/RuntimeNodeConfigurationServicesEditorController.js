(function (appControllers) {

    "use strict";

    runtimeNodeConfigurationServicesEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function runtimeNodeConfigurationServicesEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var runtimeNodeConfigurationEntity;

        var runtimeServiceDirectiveAPI;
        var runtimeServiceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.saveRuntimeNodeConfiguration = function () {
                if (isEditMode)
                    return updateRuntimeNodeConfigurations();
                else
                    return insertRuntimeNodeConfigurations();
            };

            $scope.scopeModel.onRuntimeServiceReady = function (api) {
                runtimeServiceDirectiveAPI = api;
                runtimeServiceReadyPromiseDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRuntimeService])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && runtimeNodeConfigurationEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(runtimeNodeConfigurationEntity.Name, "Runtime Node Configuration Services");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Runtime Node Configuration Services");
        }
        function loadStaticData() {

            if (runtimeNodeConfigurationEntity == undefined)
                return;
            $scope.scopeModel.Name = runtimeNodeConfigurationEntity.Name;
        }

        function loadRuntimeService() {
            var runtimeServiceLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            runtimeServiceReadyPromiseDeferred.promise.then(function () {

                var directivePayload = {
                    runtimeService: runtimeNodeConfigurationEntity != undefined && runtimeNodeConfigurationEntity.Settings != undefined ? runtimeNodeConfigurationEntity.Settings.RuntimeService : undefined
                }

                VRUIUtilsService.callDirectiveLoad(runtimeServiceDirectiveAPI, directivePayload, runtimeServiceLoadPromiseDeferred);

            });
            return runtimeServiceLoadPromiseDeferred.promise;
        }

        function buildRuntimeNodeConfigurationsObjFromScope() { 
            var obj = {
                RuntimeServiceConfigurationId: runtimeNodeConfigurationEntity != undefined ? runtimeNodeConfigurationEntity.RuntimeServiceConfigurationId : UtilsService.guid(),
                Name: $scope.scopeModel.Name,
                Settings:{
                    RuntimeService: runtimeServiceDirectiveAPI.getData()
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
    appControllers.controller('VRRuntime_RuntimeNodeConfigurationServicesEditorController', runtimeNodeConfigurationServicesEditorController);
    })(appControllers);
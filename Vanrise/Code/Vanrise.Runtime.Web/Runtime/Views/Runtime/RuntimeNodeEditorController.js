(function (appControllers) {
    'use strict';

    RuntimeNodeEditorController.$inject = ['$scope', 'VRRuntime_RuntimeNodeService', 'VRRuntime_RuntimeNodeAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRRuntime_RuntimeNodeConfigurationAPIService'];

    function RuntimeNodeEditorController($scope, VRRuntime_RuntimeNodeService, VRRuntime_RuntimeNodeAPIService, VRNotificationService, VRNavigationService, UtilsService,VRUIUtilsService, VRRuntime_RuntimeNodeConfigurationAPIService) {

        var nodeId;
        var runtimeNodeConfigurationId;
        var runtimeNodeEntity;
        var editMode;

        var runtimeNodeConfigurationSelectorAPI;
        var runtimeNodeConfigurationSelectoReadyDeferred = UtilsService.createPromiseDeferred();
        var onSMSMessageTypeSelectionChangedDeferred;

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                nodeId = parameters.RuntimeNodeId;
            }
            editMode = (nodeId != undefined);
        }


        function defineScope() {
            $scope.scopeModel = {};
            $scope.runtimeNodesConfigurations = [];

            $scope.scopeModel.onRuntimeNodeConfigurationSelectorReady = function (api) {
                
               runtimeNodeConfigurationSelectorAPI = api;
               runtimeNodeConfigurationSelectoReadyDeferred.resolve();
            };

            $scope.saveRuntimeNode = function () {
                if (editMode)
                    return updateRuntimeNode();
                else
                    return insertRuntimeNode();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (editMode) {
                getRuntimeNode().then(function () {
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

        function getRuntimeNode() {
            return VRRuntime_RuntimeNodeAPIService.GetRuntimeNode(nodeId).then(function (runtimeNode) {
                runtimeNodeEntity = runtimeNode;
            });
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRuntimeNodeConfigurationSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
           $scope.title = UtilsService.buildTitleForAddEditor("Runtime Node");
        }

        function loadStaticData() {
            if (runtimeNodeEntity == undefined)
                return;
            $scope.scopeModel.name = runtimeNodeEntity.Name;
        }

        function loadRuntimeNodeConfigurationSelector() {
            var runtimeNodeConfigurationSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            runtimeNodeConfigurationSelectoReadyDeferred.promise.then(function () {
                var runtimeNodeConfigurationSelectorPayload =  {
                    runtimeNodeConfiguration: runtimeNodeEntity != undefined ? runtimeNodeEntity : undefined
                }

                VRUIUtilsService.callDirectiveLoad(runtimeNodeConfigurationSelectorAPI, runtimeNodeConfigurationSelectorPayload, runtimeNodeConfigurationSelectorLoadDeferred);
            });
            return runtimeNodeConfigurationSelectorLoadDeferred.promise;
        }

        function buildRuntimeNodeObjFromScope() {
            var obj = {
                RuntimeNodeId: nodeId,
                RuntimeNodeConfigurationId: runtimeNodeConfigurationSelectorAPI.getSelectedId(),
                Name: $scope.scopeModel.name,
                Settings: {
                }

            };
            return obj;
        }

        function insertRuntimeNode() {
            $scope.isLoading = true;

            var runtimeNodeObject = buildRuntimeNodeObjFromScope();
            return VRRuntime_RuntimeNodeAPIService.AddRuntimeNode(runtimeNodeObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Runtime Node", response, "Name")) {
                    if ($scope.onRuntimeNodeAdded != undefined)
                        $scope.onRuntimeNodeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updateRuntimeNode() {
            $scope.isLoading = true;

            var runtimeNodeObject = buildRuntimeNodeObjFromScope();
            VRRuntime_RuntimeNodeAPIService.UpdateRuntimeNode(runtimeNodeObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Runtime Node", response, "Name")) {
                    if ($scope.onRuntimeNodeUpdated != undefined)
                        $scope.onRuntimeNodeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

    }

    appControllers.controller('VRRuntime_RuntimeNodeEditorController', RuntimeNodeEditorController);

})(appControllers);

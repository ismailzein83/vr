(function (appControllers) {

    "use strict";

    vrConnectionEditorController.$inject = ['$scope', 'VRCommon_VRConnectionAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vrConnectionEditorController($scope, VRCommon_VRConnectionAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var vrConnectionId;
        var connectionTypeIds;
        var vrConnectionEntity;

        var connectionTypeAPI;
        var connectionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var connectionEditorAPI;
        var connectionEditorAPIReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrConnectionId = parameters.vrConnectionId;
                var connectionTypeUpperIds= parameters.connectionTypeIds;

                if (connectionTypeUpperIds != undefined && connectionTypeUpperIds.length > 0) {
                    connectionTypeIds = [];
                    for (var i = 0; i < connectionTypeUpperIds.length; i++)
                        connectionTypeIds.push(connectionTypeUpperIds[i].toLowerCase());
                }
            }

            isEditMode = (vrConnectionId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.onConnectionTypeConfigReady = function (api) {
                connectionTypeAPI = api;
                connectionTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onConnectionEditorReady = function (api) {
                connectionEditorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isConnectionDirectiveDirectiveloading = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, connectionEditorAPI, undefined, setLoader, connectionEditorAPIReadyDeferred);
            };
            
            $scope.saveVRConnection = function () {
                if (isEditMode) {
                    return updateVRConnection();
                }
                else {
                    return insertVRConnection();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                connectionEditorAPIReadyDeferred = UtilsService.createPromiseDeferred();
                getVRConnection().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRConnection() {
            return VRCommon_VRConnectionAPIService.GetVRConnection(vrConnectionId).then(function (entity) {
                vrConnectionEntity = entity;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRConnectionType, loadVRConnectionEditor])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
                .finally(function () {
                    connectionEditorAPIReadyDeferred = undefined;
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadVRConnectionType() {
            
            var loadConnectionConfigTypePromiseDeferred = UtilsService.createPromiseDeferred();
            connectionTypeSelectorReadyDeferred.promise.then(function () {
                var payloadDirective = {
                    selectedIds: vrConnectionEntity && vrConnectionEntity.Settings && vrConnectionEntity.Settings.ConfigId || undefined,
                    filter: { connectionTypeIds: connectionTypeIds }
                };
                VRUIUtilsService.callDirectiveLoad(connectionTypeAPI, payloadDirective, loadConnectionConfigTypePromiseDeferred);
            });
            return loadConnectionConfigTypePromiseDeferred.promise;
        }

        function loadVRConnectionEditor() {
            if (isEditMode) {
            var loadConnectionEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                connectionEditorAPIReadyDeferred.promise.then(function () {
                var payloadDirective = {
                    data: vrConnectionEntity && vrConnectionEntity.Settings || undefined
                    };
                VRUIUtilsService.callDirectiveLoad(connectionEditorAPI, payloadDirective, loadConnectionEditorPromiseDeferred);
            });
            return loadConnectionEditorPromiseDeferred.promise;
        }
    }
        function setTitle() {
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor(vrConnectionEntity.Name, "Connection");
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Connection');
            }
        }

        function loadStaticData() {
            if (vrConnectionEntity != undefined) {
                $scope.scopeModel.name = vrConnectionEntity.Name;
            }
        }

        function buildVRConnectionObjFromScope() {
            return {
                VRConnectionId: vrConnectionId,
                Name: $scope.scopeModel.name,
                Settings: connectionEditorAPI.getData()
            };
        }

        function updateVRConnection() {
            $scope.scopeModel.isLoading = true;

            var connectionObject = buildVRConnectionObjFromScope();
            VRCommon_VRConnectionAPIService.UpdateVRConnection(connectionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Connection", response, "Name")) {
                    if ($scope.onVRConnectionUpdated != undefined)
                        $scope.onVRConnectionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertVRConnection() {
            $scope.scopeModel.isLoading = true;

            var connectionObject = buildVRConnectionObjFromScope();

            VRCommon_VRConnectionAPIService.AddVRConnection(connectionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Connection", response, "Name")) {
                    if ($scope.onVRConnectionAdded != undefined)
                        $scope.onVRConnectionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_VRConnectionEditorController', vrConnectionEditorController);
})(appControllers);

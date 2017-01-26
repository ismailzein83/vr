(function (appControllers) {

    "use strict";

    vrConnectionEditorController.$inject = ['$scope', 'VRCommon_VRConnectionAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vrConnectionEditorController($scope, VRCommon_VRConnectionAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var vrConnectionId;
        var vrConnectionEntity;

        var connectionTypeAPI;
        var connectionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrConnectionId = parameters.vrConnectionId;
            }
            isEditMode = (vrConnectionId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.onConnectionTypeConfigReady = function (api) {
                connectionTypeAPI = api;
                connectionTypeSelectorReadyDeferred.resolve();
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
            
        }

      

        function getVRConnection() {
            return VRCommon_VRConnectionAPIService.GetVRConnection(vrConnectionId).then(function (entity) {
                vrConnectionEntity = entity;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRConnectionEditor])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadVRConnectionEditor() {
            
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
                $scope.name = vrConnectionEntity.Name;
            }
        }

        function buildVRConnectionObjFromScope() {
            return {

            }
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

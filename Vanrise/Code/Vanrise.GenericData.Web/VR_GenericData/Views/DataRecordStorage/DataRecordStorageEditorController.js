(function (appControllers) {

    'use strict';

    DataRecordStorageEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService','VR_GenericData_DataRecordStorageAPIService'];

    function DataRecordStorageEditorController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordStorageAPIService) {

        var isEditMode;
        var dataRecordStorageEntity;
        var dataRecordStorageId;
        var editorDirectiveAPI;
        var eitorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                dataRecordStorageId = parameters.DataRecordStorageId;
            }
            isEditMode = (dataRecordStorageId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onEditorDirectiveReady = function (api) {
                editorDirectiveAPI = api;
                eitorDirectiveReadyDeferred.resolve();
            };

            //#region Action
            $scope.scopeModel.saveDataRecordStorage = function () {
                if (isEditMode)
                    return updateDataRecordStorage();
                else
                    return insertDataRecordStorage();
            };
            $scope.scopeModel.hasSaveDataRecordStorage = function () {
                if (isEditMode) {
                    return VR_GenericData_DataRecordStorageAPIService.HasUpdateDataRecordStorage();
                }
                else {
                    return VR_GenericData_DataRecordStorageAPIService.HasAddDataRecordStorage();
                }
            };
            $scope.scopeModel.closeDataRecordStorage = function () {
                $scope.modalContext.closeModal();
            };

            //#endregion

        }
        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                getDataRecordStorage().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            setTitle();
            var rootPromiseNode = {
                promises: [loadEditorDirective()],
            };
            return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                $scope.isLoading = false;
            });
        }

        function getDataRecordStorage() {
            return VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(dataRecordStorageId).then(function (response) {
                dataRecordStorageEntity = response;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((dataRecordStorageEntity != undefined) ? dataRecordStorageEntity.Name : null, 'Data Record Storage') :
                UtilsService.buildTitleForAddEditor('Data Record Storage');
        }

        function loadEditorDirective() {
            var loadEditorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
               eitorDirectiveReadyDeferred.promise.then(function () {
                   var payload = {
                       dataRecordStorageEntity: dataRecordStorageEntity,
                   };
                VRUIUtilsService.callDirectiveLoad(editorDirectiveAPI, payload, loadEditorDirectivePromiseDeferred);
            });
            return loadEditorDirectivePromiseDeferred.promise;
        }

        function insertDataRecordStorage() {
            $scope.isLoading = true;

            return VR_GenericData_DataRecordStorageAPIService.AddDataRecordStorage(buildDataRecordStorageObjectFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Data Record Storage', response, 'Name')) {
                    if ($scope.onDataRecordStorageAdded != undefined && typeof ($scope.onDataRecordStorageAdded) == 'function') {
                        $scope.onDataRecordStorageAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function updateDataRecordStorage() {
            $scope.isLoading = true;

            return VR_GenericData_DataRecordStorageAPIService.UpdateDataRecordStorage(buildDataRecordStorageObjectFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Data Record Storage', response, 'Name')) {
                    if ($scope.onDataRecordStorageUpdated != undefined && typeof ($scope.onDataRecordStorageUpdated) == 'function') {
                        $scope.onDataRecordStorageUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildDataRecordStorageObjectFromScope() {
            return editorDirectiveAPI.getData();
        }


    }

    appControllers.controller('VR_GenericData_DataRecordStorageEditorController', DataRecordStorageEditorController);

})(appControllers);
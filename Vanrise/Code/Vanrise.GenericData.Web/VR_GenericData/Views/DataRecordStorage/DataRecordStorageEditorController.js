(function (appControllers) {

    'use strict';

    DataRecordStorageEditorController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataStoreConfigAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function DataRecordStorageEditorController($scope, VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataStoreConfigAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var isEditMode;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataStoreSelectorAPI;
        var dataStoreSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred;

        var dataRecordStorageId;
        var dataRecordStorageEntity;

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
            $scope.settingsEditor;

            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.onDataStoreSelectorReady = function (api) {
                dataStoreSelectorAPI = api;
                dataStoreSelectorReadyDeferred.resolve();
            };
            $scope.onDataStoreSelectionChanged = function () {
                if ($scope.selectedDataStore == undefined) {
                    return;
                }
                settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                loadSettingsDirective();
            };
            $scope.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.saveDataRecordStorage = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getDataRecordStorage().then(function () {
                    loadAllControls().finally(function () {
                        dataRecordStorageEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }
        function getDataRecordStorage() {
            return VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(dataRecordStorageId).then(function (response) {
                dataRecordStorageEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadDataStoreSelector]).then(function () {
                if (dataRecordStorageEntity != undefined) {
                    settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                    loadSettingsDirective();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((dataRecordStorageEntity != undefined) ? dataRecordStorageEntity.Name : null, 'Data Record Storage') :
                    UtilsService.buildTitleForAddEditor('Data Record Storage');
            }
            function loadStaticData() {
                if (dataRecordStorageEntity == undefined) {
                    return;
                }
                $scope.name = dataRecordStorageEntity.Name;
            }
            function loadDataRecordTypeSelector() {
                var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                    var payload;

                    if (dataRecordStorageEntity != undefined) {
                        payload = {
                            selectedIds: dataRecordStorageEntity.DataRecordTypeId
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
                });

                return dataRecordTypeSelectorLoadDeferred.promise;
            }
            function loadDataStoreSelector() {
                var dataStoreSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                
                dataStoreSelectorReadyDeferred.promise.then(function () {
                    var payload;

                    if (dataRecordStorageEntity != undefined) {
                        payload = {
                            selectedIds: dataRecordStorageEntity.DataStoreId
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(dataStoreSelectorAPI, payload, dataStoreSelectorLoadDeferred);
                });

                return dataStoreSelectorLoadDeferred.promise;
            }
        }
        function loadSettingsDirective() {
            var promises = [];

            var getDataStoreConfigPromise = getDataStoreConfig();
            promises.push(getDataStoreConfigPromise);

            var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(settingsDirectiveLoadDeferred.promise);

            getDataStoreConfigPromise.then(function () {
                settingsDirectiveReadyDeferred.promise.then(function () {
                    var payload = {
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds()
                    };

                    if (dataRecordStorageEntity != undefined) {
                        payload = dataRecordStorageEntity.Settings;
                        payload.DataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                    }

                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, settingsDirectiveLoadDeferred);
                });
            });

            settingsDirectiveLoadDeferred.promise.finally(function () {
                settingsDirectiveReadyDeferred = undefined;
            });

            return UtilsService.waitMultiplePromises(promises);

            function getDataStoreConfig() {
                return VR_GenericData_DataStoreConfigAPIService.GetDataStoreConfig($scope.selectedDataStore.DataStoreId).then(function (response) {
                    if (response != null) {
                        $scope.settingsEditor = response.DataRecordSettingsEditor
                    }
                });
            }
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
            return {
                DataRecordDefinitionId: dataRecordDefinitionId,
                Name: $scope.name,
                DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                DataStoreId: null,
                Settings: null
            };
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageEditorController', DataRecordStorageEditorController);

})(appControllers);
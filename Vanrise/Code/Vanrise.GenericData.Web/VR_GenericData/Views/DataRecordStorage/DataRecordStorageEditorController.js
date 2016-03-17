﻿(function (appControllers) {

    'use strict';

    DataRecordStorageEditorController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataStoreConfigAPIService', 'VR_GenericData_DataStoreAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function DataRecordStorageEditorController($scope, VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataStoreConfigAPIService, VR_GenericData_DataStoreAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var isEditMode;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var typeSelectorChangeCount = 0;

        var dataStoreSelectorAPI;
        var dataStoreSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var storeSelectorChangeCount = 0;

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
            $scope.onDataRecordTypeSelectionChanged = function (option) {
                $scope.selectedDataRecordType = option;
                typeSelectorChangeCount++;
                if ((isEditMode && typeSelectorChangeCount <= 2) || (!isEditMode && typeSelectorChangeCount <= 1)) {
                    return;
                }
                loadSettingsDirectiveFromScope();
            };
            $scope.onDataStoreSelectorReady = function (api) {
                dataStoreSelectorAPI = api;
                dataStoreSelectorReadyDeferred.resolve();
            };
            $scope.onDataStoreSelectionChanged = function (option) {
                $scope.selectedDataStore = option;
                storeSelectorChangeCount++;
                if ((isEditMode && storeSelectorChangeCount <= 4) || (!isEditMode && storeSelectorChangeCount <= 2)) {
                    return;
                }
                loadSettingsEditor();
            };
            $scope.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                loadSettingsDirectiveFromScope();
            };

            $scope.saveDataRecordStorage = function () {
                if (isEditMode)
                    return updateDataRecordStorage();
                else
                    return insertDataRecordStorage();
            };
            $scope.hasSaveDataRecordStorage = function () {
                if (isEditMode) {
                    return VR_GenericData_DataRecordStorageAPIService.HasUpdateDataRecordStorage();
                }
                else {
                    return VR_GenericData_DataRecordStorageAPIService.HasAddDataRecordStorage();
                }
            }
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadDataStoreSelector, loadSettingsDirectiveOnPageLoad]).catch(function (error) {
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
                            selectedIds: dataRecordStorageEntity.DataStoreId,
                            showaddbutton: true
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(dataStoreSelectorAPI, payload, dataStoreSelectorLoadDeferred);
                });

                return dataStoreSelectorLoadDeferred.promise;
            }
            function loadSettingsDirectiveOnPageLoad() {
                var promises = [];

                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(settingsDirectiveLoadDeferred.promise);

                if (dataRecordStorageEntity != undefined) {
                    var loadSettingsEditorPromise = loadSettingsEditor();
                    promises.push(loadSettingsEditorPromise);

                    loadSettingsEditorPromise.then(function () {
                        settingsDirectiveReadyDeferred.promise.then(function () {
                            settingsDirectiveReadyDeferred = undefined;
                            var payload = dataRecordStorageEntity.Settings; // The Settings columns doesn't allow NULL values
                            payload.DataRecordTypeId = dataRecordStorageEntity.DataRecordTypeId;
                            VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, settingsDirectiveLoadDeferred);
                        });
                    });
                }
                else {
                    settingsDirectiveLoadDeferred.resolve();
                }

                return UtilsService.waitMultiplePromises(promises);
            }
        }
        function loadSettingsEditor() {
            var promises = [];
            var dataStoreEntity;
            settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var getDataStorePromise = getDataStore();
            promises.push(getDataStorePromise);

            var getDataStoreConfigDeferred = UtilsService.createPromiseDeferred();
            promises.push(getDataStoreConfigDeferred.promise);

            getDataStore().then(function () {
                VR_GenericData_DataStoreConfigAPIService.GetDataStoreConfig(dataStoreEntity.Settings.ConfigId).then(function (response) {
                    if (response != null) {
                        $scope.settingsEditor = response.DataRecordSettingsEditor
                    }
                    getDataStoreConfigDeferred.resolve();
                }).catch(function (error) {
                    getDataStoreConfigDeferred.reject(error);
                });
            });

            return UtilsService.waitMultiplePromises(promises);

            function getDataStore() {
                var dataStoreId = (dataRecordStorageEntity != undefined) ? dataRecordStorageEntity.DataStoreId : dataStoreSelectorAPI.getSelectedIds();
                return VR_GenericData_DataStoreAPIService.GetDataStore(dataStoreId).then(function (response) {
                    dataStoreEntity = response;
                });
            }
        }
        function loadSettingsDirectiveFromScope() {
            var selectedTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
            var selectedStoreId = dataStoreSelectorAPI.getSelectedIds();

            if (dataRecordStorageEntity != undefined) {
                settingsDirectiveReadyDeferred.resolve();
                return;
            }
            else {
                settingsDirectiveReadyDeferred = undefined;
                if (selectedTypeId == undefined || selectedStoreId == undefined) {
                    return;
                }
            }

            var payload = {
                DataRecordTypeId: selectedTypeId
            };
            var setLoader = function (value) { $scope.isLoading = value; };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, settingsDirectiveAPI, payload, setLoader, settingsDirectiveReadyDeferred);
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
                DataRecordStorageId: dataRecordStorageId,
                Name: $scope.name,
                DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                DataStoreId: dataStoreSelectorAPI.getSelectedIds(),
                Settings: settingsDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageEditorController', DataRecordStorageEditorController);

})(appControllers);
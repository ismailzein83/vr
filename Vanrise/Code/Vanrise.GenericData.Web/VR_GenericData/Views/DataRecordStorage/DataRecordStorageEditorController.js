(function (appControllers) {

    'use strict';

    DataRecordStorageEditorController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataStoreAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService'];

    function DataRecordStorageEditorController($scope, VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataStoreAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService) {

        var isEditMode;

        var dataRecordStorageId;
        var dataRecordStorageEntity;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var typeSelectorChangeCount = 0;
        var selectedDataRecordTypeDeferred = UtilsService.createPromiseDeferred();

        var dataStoreSelectorAPI;
        var dataStoreSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var storeSelectorChangeCount = 0;

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred;

        var requiredPermissionAPI;
        var requiredPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var modifiedByFieldSelectorAPI;
        var modifiedByFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var createdByFieldSelectorAPI;
        var createdByFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var modifiedTimeFieldSelectorAPI;
        var modifiedTimeFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var createdTimeFieldSelectorAPI;
        var createdTimeFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.dataRecordTypeFields = [];
            $scope.scopeModel.datasources = [];
            $scope.scopeModel.settingsEditor;

            $scope.scopeModel.createdByFieldSelectorReady = function (api) {
                createdByFieldSelectorAPI = api;
                createdByFieldSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.modifiedTimeFieldSelectorReady = function (api) {
                modifiedTimeFieldSelectorAPI = api;
                modifiedTimeFieldSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.createdTimeFieldSelectorReady = function (api) {
                createdTimeFieldSelectorAPI = api;
                createdTimeFieldSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.modifiedByFieldSelectorReady = function (api) {
                modifiedByFieldSelectorAPI = api;
                modifiedByFieldSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDataRecordTypeSelectionChanged = function (option) {
                $scope.scopeModel.selectedDataRecordType = option;
                typeSelectorChangeCount++;
                loadDataRecordFields();
                if (option != undefined) {
                    if (selectedDataRecordTypeDeferred != undefined)
                        selectedDataRecordTypeDeferred.resolve();
                    else {
                        var modifiedByFieldPayload = {
                            dataRecordTypeId: option.DataRecordTypeId,
                        };
                        var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, modifiedByFieldSelectorAPI, modifiedByFieldPayload, modifiedByFieldSetLoader);

                        var createdByFieldPayload = {
                            dataRecordTypeId: option.DataRecordTypeId,
                        };
                        var createdByFieldSetLoader = function (value) { $scope.scopeModel.isCreatedBySelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, createdByFieldSelectorAPI, createdByFieldPayload, createdByFieldSetLoader);

                        var modifiedTimeFieldPayload = {
                            dataRecordTypeId: option.DataRecordTypeId,
                        };
                        var modifiedTimeFieldSetLoader = function (value) { $scope.scopeModel.isModifiedTimeSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, modifiedTimeFieldSelectorAPI, modifiedTimeFieldPayload, modifiedTimeFieldSetLoader);

                        var createdTimeFieldPayload = {
                            dataRecordTypeId: option.DataRecordTypeId,
                        };
                        var createdTimeFieldSetLoader = function (value) { $scope.scopeModel.isCreatedTimeSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, createdTimeFieldSelectorAPI, createdTimeFieldPayload, createdTimeFieldSetLoader);
                    }
                }
             
                if ((isEditMode && typeSelectorChangeCount <= 2) || (!isEditMode && typeSelectorChangeCount <= 1)) {
                    return;
                }
                loadSettingsDirectiveFromScope();
            };

            $scope.scopeModel.onDataStoreSelectorReady = function (api) {
                dataStoreSelectorAPI = api;
                dataStoreSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDataStoreSelectionChanged = function (option) {
                $scope.scopeModel.selectedDataStore = option;
                storeSelectorChangeCount++;
                if ((isEditMode && storeSelectorChangeCount <= 4) || (!isEditMode && storeSelectorChangeCount <= 2)) {
                    return;
                }
                loadSettingsEditor();
            };

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                loadSettingsDirectiveFromScope();
            };

            $scope.scopeModel.onRequiredPermissionReady = function (api) {
                requiredPermissionAPI = api;
                requiredPermissionReadyDeferred.resolve();
            };

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

        function getDataRecordStorage() {
            return VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(dataRecordStorageId).then(function (response) {
                dataRecordStorageEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData,loadCreatedTimeFieldSelector,loadModifiedTimeFieldSelector, loadCreatedByFieldSelector,loadDataRecordTypeSelector, loadDataStoreSelector, loadDataRecordFields, loadRequiredPermission, loadDataStoreConfigs, loadModifiedByFieldSelector]).then(function () {
                selectedDataRecordTypeDeferred = undefined;
                loadSettingsDirectiveOnPageLoad().then(function () {
                    dataRecordStorageEntity = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }
        function loadModifiedByFieldSelector() {
            if (isEditMode) {
                var modifiedByFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitMultiplePromises([modifiedByFieldSelectorReadyDeferred.promise, selectedDataRecordTypeDeferred.promise]).then(function () {
                    var modifiedByFieldSelectorPayload = {
                    };
                    if (dataRecordStorageEntity != undefined) {
                        modifiedByFieldSelectorPayload.dataRecordTypeId = dataRecordStorageEntity.DataRecordTypeId
                    }
                    VRUIUtilsService.callDirectiveLoad(modifiedByFieldSelectorAPI, modifiedByFieldSelectorPayload, modifiedByFieldSelectorLoadDeferred);
                });
                return modifiedByFieldSelectorLoadDeferred.promise;
            }
        }

        function loadCreatedByFieldSelector() {
            if (isEditMode) {
                var createdByFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitMultiplePromises([createdByFieldSelectorReadyDeferred.promise, selectedDataRecordTypeDeferred.promise]).then(function () {
                    var createdByFieldSelectorPayload = {
                    };
                    if (dataRecordStorageEntity != undefined) {
                        createdByFieldSelectorPayload.dataRecordTypeId = dataRecordStorageEntity.DataRecordTypeId
                    }
                    VRUIUtilsService.callDirectiveLoad(createdByFieldSelectorAPI, createdByFieldSelectorPayload, createdByFieldSelectorLoadDeferred);
                });
                return createdByFieldSelectorLoadDeferred.promise;
            }
        }
        function loadModifiedTimeFieldSelector() {
            if (isEditMode) {
                var modifiedTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitMultiplePromises([modifiedTimeFieldSelectorReadyDeferred.promise, selectedDataRecordTypeDeferred.promise]).then(function () {
                    var modifiedTimeSelectorPayload = {
                    };
                    if (dataRecordStorageEntity != undefined) {
                        modifiedTimeSelectorPayload.dataRecordTypeId = dataRecordStorageEntity.DataRecordTypeId
                    }
                    VRUIUtilsService.callDirectiveLoad(modifiedTimeFieldSelectorAPI, modifiedTimeSelectorPayload, modifiedTimeSelectorLoadDeferred);
                });
                return modifiedTimeSelectorLoadDeferred.promise;
            }
        }
        function loadCreatedTimeFieldSelector() {
            if (isEditMode) {
                var createdTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitMultiplePromises([createdTimeFieldSelectorReadyDeferred.promise, selectedDataRecordTypeDeferred.promise]).then(function () {
                    var createdTimeSelectorPayload = {
                    };
                    if (dataRecordStorageEntity != undefined) {
                        createdTimeSelectorPayload.dataRecordTypeId = dataRecordStorageEntity.DataRecordTypeId
                    }
                    VRUIUtilsService.callDirectiveLoad(createdTimeFieldSelectorAPI, createdTimeSelectorPayload, createdTimeSelectorLoadDeferred);
                });
                return createdTimeSelectorLoadDeferred.promise;
            }
        }

        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((dataRecordStorageEntity != undefined) ? dataRecordStorageEntity.Name : null, 'Data Record Storage') :
                UtilsService.buildTitleForAddEditor('Data Record Storage');
        }
        function loadStaticData() {
            if (dataRecordStorageEntity == undefined) {
                return;
            }
            $scope.scopeModel.name = dataRecordStorageEntity.Name;
            if (dataRecordStorageEntity.Settings != undefined)
                 $scope.scopeModel.enableUseCaching = dataRecordStorageEntity.Settings.EnableUseCaching;
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
        function loadDataRecordFields() {
            $scope.scopeModel.selectedDataRecordTypeField = undefined;
            $scope.scopeModel.dataRecordTypeFields.length = 0;

            var currentDataRecordTypeId;
            if (dataRecordStorageEntity != undefined) {
                currentDataRecordTypeId = dataRecordStorageEntity.DataRecordTypeId;
            }
            else if ($scope.scopeModel.selectedDataRecordType) {
                currentDataRecordTypeId = $scope.scopeModel.selectedDataRecordType.DataRecordTypeId;
            }

            if (currentDataRecordTypeId != undefined) {

                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(currentDataRecordTypeId).then(function (response) {
                    if (response != undefined) {
                        angular.forEach(response, function (item) {
                            $scope.scopeModel.dataRecordTypeFields.push(item);
                        });

                        if (dataRecordStorageEntity != undefined) {
                            $scope.scopeModel.selectedDataRecordTypeField = UtilsService.getItemByVal($scope.scopeModel.dataRecordTypeFields, dataRecordStorageEntity.Settings.DateTimeField, 'Entity.Name');
                        }
                    }
                });
            }
        }
        function loadRequiredPermission() {
            var requiredPermissionLoadDeferred = UtilsService.createPromiseDeferred();

            requiredPermissionReadyDeferred.promise.then(function () {
                var payload;

                if (dataRecordStorageEntity != undefined && dataRecordStorageEntity.Settings != undefined && dataRecordStorageEntity.Settings.RequiredPermission != null) {
                    payload = {
                        data: dataRecordStorageEntity.Settings.RequiredPermission
                    };
                }

                VRUIUtilsService.callDirectiveLoad(requiredPermissionAPI, payload, requiredPermissionLoadDeferred);
            });

            return requiredPermissionLoadDeferred.promise;
        }
        function loadDataStoreConfigs() {
            VR_GenericData_DataStoreAPIService.GetDataStoreConfigs().then(function (response) {
                if (response) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.datasources.push(response[i]);
                    }
                }
            });
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
                        payload.DataStoreId = dataRecordStorageEntity.DataStoreId;
                        VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, settingsDirectiveLoadDeferred);
                    });
                });
            }
            else {
                settingsDirectiveLoadDeferred.resolve();
            }

            return UtilsService.waitMultiplePromises(promises);
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
                DataRecordTypeId: selectedTypeId,
                DataStoreId: selectedStoreId
            };
            var setLoader = function (value) { $scope.scopeModel.isLoading = value; };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, settingsDirectiveAPI, payload, setLoader, settingsDirectiveReadyDeferred);
        }
        function loadSettingsEditor() {
            var promises = [];
            var dataStoreEntity;
            settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var getDataStorePromise = getDataStore();
            promises.push(getDataStorePromise);

            getDataStore().then(function () {

                var item = UtilsService.getItemByVal($scope.scopeModel.datasources, dataStoreEntity.Settings.ConfigId, "ExtensionConfigurationId");
                if (item != undefined) {
                    $scope.scopeModel.settingsEditor = item.DataRecordSettingsEditor;
                }
            });
            return UtilsService.waitMultiplePromises(promises);

            function getDataStore() {
                var dataStoreId = (dataRecordStorageEntity != undefined) ? dataRecordStorageEntity.DataStoreId : dataStoreSelectorAPI.getSelectedIds();
                return VR_GenericData_DataStoreAPIService.GetDataStore(dataStoreId).then(function (response) {
                    dataStoreEntity = response;
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
            var obj = {
                DataRecordStorageId: dataRecordStorageId,
                Name: $scope.scopeModel.name,
                DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                DataStoreId: dataStoreSelectorAPI.getSelectedIds(),
                Settings: settingsDirectiveAPI.getData()
            };
            obj.Settings.DateTimeField = $scope.scopeModel.selectedDataRecordTypeField.Entity.Name;
            obj.Settings.RequiredPermission = requiredPermissionAPI.getData();
            obj.Settings.EnableUseCaching = $scope.scopeModel.enableUseCaching;
            return obj;
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageEditorController', DataRecordStorageEditorController);

})(appControllers);
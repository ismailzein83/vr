(function (appControllers) {

    "use strict";
    DataStoreEditorController.$inject = ["$scope", "VR_GenericData_DataStoreAPIService", "VRNavigationService", "VRNotificationService", "UtilsService", "VRUIUtilsService"];

    function DataStoreEditorController($scope, VR_GenericData_DataStoreAPIService , VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var dataStoreId;
        var dataStoreEntity;
        var isEditMode;
        var dataStoreSelectorAPI;
        var dataStoreReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var dataStoreDirectiveAPI;
        var dataStoreConfigReadyPromiseDeferred; // = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                dataStoreId = parameters.DataStoreId;

            isEditMode = (dataStoreId != undefined);
        }

        function defineScope() {

            $scope.onReadyDataStoreConfigSelector = function (api) {
                dataStoreSelectorAPI = api;
                dataStoreReadyPromiseDeferred.resolve();
            };
            $scope.onReadyDataStoreConfigDirective = function (api) {
                dataStoreDirectiveAPI = api;

                var setLoader = function (value) { $scope.isLoadingConfigDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataStoreDirectiveAPI, undefined, setLoader, dataStoreConfigReadyPromiseDeferred);
            };
            $scope.saveDataStore = function () {
                $scope.isLoading = true;
                if (isEditMode)
                    return updateDataStore();
                else
                    return insertDataStore();
            };
            $scope.hasSaveDataStore = function () { 
                if (isEditMode) {
                    return VR_GenericData_DataStoreAPIService.HasUpdateDataStore();
                }
                else {
                    return VR_GenericData_DataStoreAPIService.HasUpdateDataStore();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                GetDataStore().then(function () {
                    loadAllControls().finally(function () {
                        dataStoreEntity = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });
                });
            }
            else {
                loadAllControls();
            }

        }

        function GetDataStore() {
            return VR_GenericData_DataStoreAPIService.GetDataStore(dataStoreId).then(function (response) {
                dataStoreEntity = response;
             });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataStoreConfigSelector, loadDataStoreConfigDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && dataStoreEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataStoreEntity.Name, "Data Store");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Data Store");
        }

        function loadStaticData() {
            if (dataStoreEntity == undefined)
                return;
            $scope.name = dataStoreEntity.Name;
        }

        function loadDataStoreConfigSelector() {
            var loadDataStorePromiseDeferred = UtilsService.createPromiseDeferred();
            dataStoreReadyPromiseDeferred.promise.then(function () {
                if (dataStoreEntity != undefined && dataStoreEntity.Settings != undefined)
                    var payLoad = {
                        selectedIds: (dataStoreEntity != undefined && dataStoreEntity.Settings != undefined && dataStoreEntity.Settings.ConfigId != undefined) ? dataStoreEntity.Settings.ConfigId : undefined
                    };
                VRUIUtilsService.callDirectiveLoad(dataStoreSelectorAPI, payLoad, loadDataStorePromiseDeferred);
            });
            return loadDataStorePromiseDeferred.promise;
        }
        function loadDataStoreConfigDirective() {
            if (dataStoreEntity != undefined && dataStoreEntity.Settings != undefined) {
                dataStoreConfigReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                var loadDataStoreConfigPromiseDeferred = UtilsService.createPromiseDeferred();
                dataStoreConfigReadyPromiseDeferred.promise.then(function () {
                    dataStoreConfigReadyPromiseDeferred = undefined;
                    var directivePayLoad = {
                        data: dataStoreEntity.Settings
                    };
                    VRUIUtilsService.callDirectiveLoad(dataStoreDirectiveAPI, directivePayLoad, loadDataStoreConfigPromiseDeferred);
                });
                return loadDataStoreConfigPromiseDeferred.promise;
            }

         
        }


        function updateDataStore() {
            var dataStoreObj = buildDataSToreObjFromScope();
            return VR_GenericData_DataStoreAPIService.UpdateDataStore(dataStoreObj)
                .then(function (response) {
                    $scope.isLoading = false;
                    if (VRNotificationService.notifyOnItemUpdated("Data Store", response, "Name")) {

                        if ($scope.onDataStoreUpdated != undefined)
                            $scope.onDataStoreUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });
        }

        function insertDataStore() {
            var dataStoreObj = buildDataSToreObjFromScope();
            return VR_GenericData_DataStoreAPIService.AddDataStore(dataStoreObj)
                .then(function (response) {
                    $scope.isLoading = false;
                    if (VRNotificationService.notifyOnItemAdded("Data Store", response, "Name")) {
                        if ($scope.onDataStoreAdded != undefined)
                            $scope.onDataStoreAdded(response.InsertedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });
        }

        function buildDataSToreObjFromScope() {
            var settings = dataStoreDirectiveAPI.getData();
            settings.ConfigId = dataStoreSelectorAPI.getSelectedIds();
            return {
                DataStoreId: dataStoreId,
                Name: $scope.name,
                Settings: settings
            };
        }
    }

    appControllers.controller("VR_GenericData_DataStoreEditorController", DataStoreEditorController);

})(appControllers);
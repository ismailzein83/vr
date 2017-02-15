(function (app) {

    'use strict';

    DataRecordStorageSettingsRestAPIDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataStoreAPIService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_DataRecordStorageAPIService'];

    function DataRecordStorageSettingsRestAPIDirective(UtilsService, VRUIUtilsService, VR_GenericData_DataStoreAPIService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_DataRecordStorageAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RestAPIDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/Templates/DataRecordStorageSettingsRestAPITemplate.html'
        };

        function RestAPIDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataStoreId;
            var dataStoreEntity;
            var connectionId;
            var remoteDataRecordTypeId;
            var remoteDataRecordStorageIds;
            var vrRestAPIRecordQueryInterceptor;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectionChangedDeferred;

            var dataRecordStorageSelectorAPI;
            var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var vrRestAPIRecordQueryInterceptorSelectiveAPI;
            var vrRestAPIRecordQueryInterceptorSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataRecordTypes = [];
                $scope.scopeModel.dataRecordStorages = [];
                $scope.scopeModel.selectedDataRecordStorages = [];
                $scope.scopeModel.showDataRecordStorageSelector = false;

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyDeferred.resolve();
                }
                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageSelectorAPI = api;
                    dataRecordStorageSelectorReadyDeferred.resolve();
                }
                $scope.scopeModel.onVRRestAPIRecordQueryInterceptorSelectiveReady = function (api) {
                    vrRestAPIRecordQueryInterceptorSelectiveAPI = api;
                    vrRestAPIRecordQueryInterceptorSelectiveReadyDeferred.resolve();
                }

                $scope.scopeModel.onDataRecordTypeChanged = function (selectedDataRecordType) {

                    if (selectedDataRecordType != undefined) {
                        remoteDataRecordTypeId = selectedDataRecordType.DataRecordTypeId;

                        if (dataRecordTypeSelectionChangedDeferred != undefined) {
                            dataRecordTypeSelectionChangedDeferred.resolve();
                        }
                        else {
                            loadDataRecordStoragesSelector();

                            function loadDataRecordStoragesSelector() {
                                $scope.scopeModel.showDataRecordStorageSelector = true;
                                $scope.scopeModel.isDataRecordStorageSelectorLoading = true;

                                var filter = {
                                    DataRecordTypeId: remoteDataRecordTypeId
                                };

                                VR_GenericData_DataRecordStorageAPIService.GetRemoteDataRecordsStorageInfo(connectionId, UtilsService.serializetoJson(filter)).then(function (response) {
                                    var dataRecordStoragesInfos = response;

                                    if (dataRecordStoragesInfos != undefined) {
                                        dataRecordStorageSelectorAPI.clearDataSource();

                                        for (var i = 0; i < dataRecordStoragesInfos.length; i++) {
                                            var cunrrentDataRecordsStorage = dataRecordStoragesInfos[i];
                                            $scope.scopeModel.dataRecordStorages.push({
                                                DataRecordStorageId: cunrrentDataRecordsStorage.DataRecordStorageId,
                                                Name: cunrrentDataRecordsStorage.Name
                                            });
                                        }

                                        $scope.scopeModel.isDataRecordStorageSelectorLoading = false;
                                    }
                                });
                            }
                        }
                    }
                }

                UtilsService.waitMultiplePromises([dataRecordTypeSelectorReadyDeferred.promise, dataRecordStorageSelectorReadyDeferred.promise,
                    vrRestAPIRecordQueryInterceptorSelectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        dataStoreId = payload.DataStoreId;
                        remoteDataRecordTypeId = payload.RemoteDataRecordTypeId;
                        remoteDataRecordStorageIds = payload.RemoteDataRecordStorageIds;
                        vrRestAPIRecordQueryInterceptor = payload.VRRestAPIRecordQueryInterceptor;
                    }

                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                    if (dataStoreId != undefined) {
                        getDataStoreEntityPromise().then(function () {
                            getDataRecordTypeSelectorLoadPromise().then(function () {
                                UtilsService.waitMultipleAsyncOperations([getDataRecordStoragesSelectorLoadPromise, getVRRestAPIRecordQueryInterceptorSelectiveLoadPromise]).then(function () {
                                    loadPromiseDeferred.resolve();
                                }).catch(function () {
                                    loadPromiseDeferred.reject();
                                });
                            }).catch(function () {
                                loadPromiseDeferred.reject();
                            });
                        }).catch(function () {
                            loadPromiseDeferred.reject();
                        });
                    }

                    function getDataStoreEntityPromise() {
                        return VR_GenericData_DataStoreAPIService.GetDataStore(dataStoreId).then(function (response) {
                            dataStoreEntity = response;
                            connectionId = dataStoreEntity && dataStoreEntity.Settings ? dataStoreEntity.Settings.ConnectionId : undefined;
                        });
                    }
                    function getDataRecordTypeSelectorLoadPromise() {
                        if (remoteDataRecordTypeId != undefined && dataRecordTypeSelectionChangedDeferred == undefined)
                            dataRecordTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        return VR_GenericData_DataRecordTypeAPIService.GetRemoteDataRecordTypeInfo(connectionId).then(function (response) {
                            var dataRecordTypeInfos = response;

                            if (dataRecordTypeInfos != undefined) {
                                for (var i = 0; i < dataRecordTypeInfos.length; i++) {
                                    var cunrrentDataRecordTypeInfo = dataRecordTypeInfos[i];
                                    $scope.scopeModel.dataRecordTypes.push({
                                        DataRecordTypeId: cunrrentDataRecordTypeInfo.DataRecordTypeId,
                                        Name: cunrrentDataRecordTypeInfo.Name
                                    });
                                }
                            }
                            if (remoteDataRecordTypeId != undefined) {
                                $scope.scopeModel.selectedDataRecordType = UtilsService.getItemByVal($scope.scopeModel.dataRecordTypes, remoteDataRecordTypeId, 'DataRecordTypeId');
                            }
                        });
                    }
                    function getDataRecordStoragesSelectorLoadPromise() {
                        if (remoteDataRecordTypeId == undefined)
                            return;

                        var loadDataRecordStoragesSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectionChangedDeferred.promise.then(function () {
                            var filter = {
                                DataRecordTypeId: remoteDataRecordTypeId
                            };

                            VR_GenericData_DataRecordStorageAPIService.GetRemoteDataRecordsStorageInfo(connectionId, UtilsService.serializetoJson(filter)).then(function (response) {
                                dataRecordTypeSelectionChangedDeferred = undefined;
                                var dataRecordStoragesInfos = response;

                                if (dataRecordStoragesInfos != undefined) {
                                    for (var i = 0; i < dataRecordStoragesInfos.length; i++) {
                                        var cunrrentDataRecordsStorage = dataRecordStoragesInfos[i];
                                        $scope.scopeModel.dataRecordStorages.push({
                                            DataRecordStorageId: cunrrentDataRecordsStorage.DataRecordStorageId,
                                            Name: cunrrentDataRecordsStorage.Name
                                        });
                                    }
                                }
                                if (remoteDataRecordStorageIds != undefined) {
                                    for (var j = 0; j < remoteDataRecordStorageIds.length; j++) {
                                        var currentDataRecordStorage = UtilsService.getItemByVal($scope.scopeModel.dataRecordStorages, remoteDataRecordStorageIds[j], 'DataRecordStorageId');
                                        $scope.scopeModel.selectedDataRecordStorages.push(currentDataRecordStorage);
                                    }
                                }

                                loadDataRecordStoragesSelectorPromiseDeferred.resolve();
                                $scope.scopeModel.showDataRecordStorageSelector = true;
                            });
                        });

                        return loadDataRecordStoragesSelectorPromiseDeferred.promise;
                    }
                    function getVRRestAPIRecordQueryInterceptorSelectiveLoadPromise() {

                        var loadVRRestAPIRecordQueryInterceptorSelectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        vrRestAPIRecordQueryInterceptorSelectiveReadyDeferred.promise.then(function () {
                            var vrRestAPIRecordQueryInterceptorSelectivePayload;
                            if (vrRestAPIRecordQueryInterceptor != undefined) {
                                vrRestAPIRecordQueryInterceptorSelectivePayload = {
                                    vrRestAPIRecordQueryInterceptor: vrRestAPIRecordQueryInterceptor
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(vrRestAPIRecordQueryInterceptorSelectiveAPI, vrRestAPIRecordQueryInterceptorSelectivePayload, loadVRRestAPIRecordQueryInterceptorSelectivePromiseDeferred);
                        });

                        return loadVRRestAPIRecordQueryInterceptorSelectivePromiseDeferred.promise;
                    }

                    return loadPromiseDeferred.promise;
                };

                api.getData = function () {

                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.DataStorages.DataRecordStorage.VRRestAPIDataRecordStorageSettings, Vanrise.GenericData.MainExtensions',
                        RemoteDataRecordTypeId: $scope.scopeModel.selectedDataRecordType != undefined ? $scope.scopeModel.selectedDataRecordType.DataRecordTypeId : undefined,
                        RemoteDataRecordStorageIds: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedDataRecordStorages, 'DataRecordStorageId'),
                        VRRestAPIRecordQueryInterceptor: vrRestAPIRecordQueryInterceptorSelectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataDatarecordstoragesettingsRestapi', DataRecordStorageSettingsRestAPIDirective);

})(app);
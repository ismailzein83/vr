(function (app) {

    'use strict';

    DataRecordStorageSettingsRestAPIDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataStoreAPIService', 'VR_GenericData_DataRecordStorageAPIService'];

    function DataRecordStorageSettingsRestAPIDirective(UtilsService, VRUIUtilsService, VR_GenericData_DataStoreAPIService, VR_GenericData_DataRecordStorageAPIService) {
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
                };
                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageSelectorAPI = api;
                    dataRecordStorageSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onVRRestAPIRecordQueryInterceptorSelectiveReady = function (api) {
                    vrRestAPIRecordQueryInterceptorSelectiveAPI = api;
                    vrRestAPIRecordQueryInterceptorSelectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedDataRecordType) {

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

                                var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                                var payload = {
                                    filter: {
                                        DataRecordTypeId: remoteDataRecordTypeId
                                    },
                                    connectionId: connectionId
                                };
                                VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, payload, dataRecordStorageSelectorLoadDeferred);


                                return dataRecordStorageSelectorLoadDeferred.promise.then(function () {
                                    $scope.scopeModel.isDataRecordStorageSelectorLoading = false;
                                });
                            }
                        }
                    }
                };

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
                        if (remoteDataRecordTypeId != undefined)
                            dataRecordTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var payload = {
                            connectionId: connectionId
                        };
                        if (remoteDataRecordTypeId != undefined) {
                            payload.selectedIds = remoteDataRecordTypeId;
                        }
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);

                        return dataRecordTypeSelectorLoadDeferred.promise;
                    }
                    function getDataRecordStoragesSelectorLoadPromise() {
                        if (remoteDataRecordTypeId == undefined)
                            return;

                        var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectionChangedDeferred.promise.then(function () {
                            dataRecordTypeSelectionChangedDeferred = undefined;

                            var payload = {
                                filter: {
                                    DataRecordTypeId: remoteDataRecordTypeId
                                },
                                connectionId: connectionId
                            };
                            if (remoteDataRecordStorageIds != undefined) {
                                payload.selectedIds = remoteDataRecordStorageIds;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, payload, dataRecordStorageSelectorLoadDeferred);
                        });

                        return dataRecordStorageSelectorLoadDeferred.promise.then(function () {
                            $scope.scopeModel.showDataRecordStorageSelector = true;
                        });
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
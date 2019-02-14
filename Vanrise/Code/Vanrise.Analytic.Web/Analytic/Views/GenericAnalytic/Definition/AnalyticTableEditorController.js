(function (appControllers) {

    "use strict";

    AnalyticTableEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticTableAPIService'];

    function AnalyticTableEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticTableAPIService) {

        var isEditMode;
        var tableEntity;
        var tableId;
        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var requiredPermissionAPI;
        var requiredPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var statusDefinitionAPI;
        var statusDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        var analyticDataProviderSettingsDirectiveAPI;
        var analyticDataProviderSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var beDefinitionSelectorApi;
        var beDefinitionSelectorPromiseReadyDeferred = UtilsService.createPromiseDeferred();
        var beDefinitionSelectedPromiseDeferred;

        var connectionStringApi;
        var connectionStringPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                tableId = parameters.tableId;
            }
            isEditMode = (tableId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
         
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorApi = api;
                beDefinitionSelectorPromiseReadyDeferred.resolve();
            };
            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionAPI = api;
                statusDefinitionReadyDeferred.resolve();
            };
            $scope.scopeModel.businessEntitySelectionChanged = function () {
                var selectedBeDefinitionId = beDefinitionSelectorApi.getSelectedIds();
                if (selectedBeDefinitionId != undefined) {
                    var setLoader = function (value) { $scope.isLoadingStatusDefinitions = value; };
                    var payload = {
                        filter: { BusinessEntityDefinitionId: selectedBeDefinitionId }
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, statusDefinitionAPI, payload, setLoader, beDefinitionSelectedPromiseDeferred);
                }
                else if (statusDefinitionAPI != undefined) {
                    statusDefinitionAPI.clearDataSource();
                    $scope.scopeModel.selectedStatusDefinition = undefined;
                }
            };
            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRequiredPermissionReady = function (api) {
                requiredPermissionAPI = api;
                requiredPermissionReadyDeferred.resolve();
            };
            $scope.onAnalyticDataProviderSettingsDirectiveReady = function (api) {
                analyticDataProviderSettingsDirectiveAPI = api;
                analyticDataProviderSettingsDirectiveReadyDeferred.resolve();
            };
            $scope.onConnectionStringReady = function (api) {
                connectionStringApi = api;
                connectionStringPromiseReadyDeferred.resolve();
            };

            $scope.scopeModel.saveTable = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getTable().then(function () {
                    loadAllControls().finally(function () {
                        tableEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadRequiredPermission, loadAnalyticDataProviderSettingsDirective, loadBeDefinitionStatusDefinitionSection, loadConnectionStringDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && tableEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(tableEntity.Name, 'Analytic Table Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Analytic Table Editor');
                }

                function loadStaticData() {
                    if (tableEntity != undefined) {
                        $scope.scopeModel.name = tableEntity.Name;
                        $scope.scopeModel.tableName = tableEntity.Settings.TableName;
                        $scope.scopeModel.timeColumnName = tableEntity.Settings.TimeColumnName;
                    }
                }

            }

        }
        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (tableEntity != undefined && tableEntity.Settings != undefined) {
                    payload = {
                        selectedIds: tableEntity.Settings.DataRecordTypeIds
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
            });

            return dataRecordTypeSelectorLoadDeferred.promise;
        }

        function loadRequiredPermission() {
            var requiredPermissionLoadDeferred = UtilsService.createPromiseDeferred();

            requiredPermissionReadyDeferred.promise.then(function () {
                var payload;

                if (tableEntity != undefined && tableEntity.Settings != undefined && tableEntity.Settings.RequiredPermission != null) {
                    payload = {
                        data: tableEntity.Settings.RequiredPermission
                    };
                }

                VRUIUtilsService.callDirectiveLoad(requiredPermissionAPI, payload, requiredPermissionLoadDeferred);
            });

            return requiredPermissionLoadDeferred.promise;
        }

        function loadConnectionStringDirective() {
            var connectionStringPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            connectionStringPromiseReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (tableEntity != undefined && tableEntity.Settings != undefined) {
                     payloadDirective = {
                        ConnectionString: tableEntity.Settings.ConnectionString,
                        ConnectionStringName: tableEntity.Settings.ConnectionStringName,
                        ConnectionStringAppSettingName: tableEntity.Settings.ConnectionStringAppSettingName
                    };
                }
                VRUIUtilsService.callDirectiveLoad(connectionStringApi, payloadDirective, connectionStringPromiseLoadDeferred);
            });
            return connectionStringPromiseLoadDeferred.promise;
        }

        function getTable() {
            return VR_Analytic_AnalyticTableAPIService.GetTableById(tableId).then(function (response) {
                tableEntity = response;
            });
        }
        function loadBeDefinitionStatusDefinitionSection() {
            var loadBeDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadBeDefinitionPromiseDeferred.promise);

            var payload = {
                filter: {
                    Filters: [{
                        $type: "Vanrise.Common.Business.StatusDefinitionBEFilter, Vanrise.Common.Business"
                    }]
                }
            };
            if (tableEntity != undefined && tableEntity.Settings.StatusDefinitionBEId != undefined) {
                payload.selectedIds = tableEntity.Settings.StatusDefinitionBEId;
                beDefinitionSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            beDefinitionSelectorPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payload, loadBeDefinitionPromiseDeferred);
            });



            if (tableEntity != undefined && tableEntity.Settings.StatusDefinitionBEId != undefined) {
                var loadStatusDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadStatusDefinitionPromiseDeferred.promise);
                UtilsService.waitMultiplePromises([statusDefinitionReadyDeferred.promise, beDefinitionSelectedPromiseDeferred.promise]).then(function () {
                    var statusDefinitionPayload = {
                        filter: { BusinessEntityDefinitionId: tableEntity.Settings.StatusDefinitionBEId },
                        selectedIds: tableEntity.Settings.RecommendedStatusDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoad(statusDefinitionAPI, statusDefinitionPayload, loadStatusDefinitionPromiseDeferred);
                    beDefinitionSelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function buildTableObjectFromScope() {
            var connectionString = connectionStringApi.getData();
            var table = {
                AnalyticTableId: tableId,
                Name: $scope.scopeModel.name,
                Settings: {
                    TableName: $scope.scopeModel.tableName,
                    TimeColumnName: $scope.scopeModel.timeColumnName,
                    ConnectionString: connectionString.ConnectionString,
                    ConnectionStringName: connectionString.ConnectionStringName,
                    ConnectionStringAppSettingName: connectionString.ConnectionStringAppSettingName,
                    DataRecordTypeIds: dataRecordTypeSelectorAPI.getSelectedIds(),
                    RequiredPermission: requiredPermissionAPI.getData(),
                    DataProvider: analyticDataProviderSettingsDirectiveAPI.getData(),
                    StatusDefinitionBEId: beDefinitionSelectorApi.getSelectedIds(),
                    RecommendedStatusDefinitionId: statusDefinitionAPI.getSelectedIds()

                }
            };
            return table;
        }

        function insert() {

            $scope.scopeModel.isLoading = true;

            var tableObj = buildTableObjectFromScope();

            return VR_Analytic_AnalyticTableAPIService.AddAnalyticTable(tableObj)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded('Analytic Table', response, 'Name')) {
                        if ($scope.onAnalyticTableAdded != undefined)
                            $scope.onAnalyticTableAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function update() {

            $scope.scopeModel.isLoading = true;

            var tableObj = buildTableObjectFromScope();

            return VR_Analytic_AnalyticTableAPIService.UpdateAnalyticTable(tableObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Analytic Table', response, 'Name')) {
                    if ($scope.onAnalyticTableUpdated != undefined)
                        $scope.onAnalyticTableUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function loadAnalyticDataProviderSettingsDirective() {
            var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            analyticDataProviderSettingsDirectiveReadyDeferred.promise.then(function () {
                var settingsDirectivePayload;
                if (tableEntity != undefined && tableEntity.Settings != undefined) {
                    settingsDirectivePayload = { AnalyticDataProvider: tableEntity.Settings.DataProvider };
                }
                VRUIUtilsService.callDirectiveLoad(analyticDataProviderSettingsDirectiveAPI, settingsDirectivePayload, analyticDataProviderSettingsDirectiveReadyDeferred);
            });

            return analyticDataProviderSettingsDirectiveReadyDeferred.promise;
        }
    }

    appControllers.controller('VR_Analytic_AnalyticTableEditorController', AnalyticTableEditorController);
})(appControllers);

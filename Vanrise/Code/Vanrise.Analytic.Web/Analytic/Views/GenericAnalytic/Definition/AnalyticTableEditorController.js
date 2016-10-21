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

        var analyticDataProviderSettingsDirectiveAPI;
        var analyticDataProviderSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var connectionStringType;
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
            connectionStringType = {
                ConnectionString: { value: 0, description: "Connection String" },
                ConnectionStringName: { value: 1, description: "Connection String Name" },
            }

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            }
            $scope.scopeModel.onRequiredPermissionReady = function (api) {
                requiredPermissionAPI = api;
                requiredPermissionReadyDeferred.resolve();
            }
            $scope.onAnalyticDataProviderSettingsDirectiveReady = function (api) {
                analyticDataProviderSettingsDirectiveAPI = api;
                analyticDataProviderSettingsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.connectionStringType = UtilsService.getArrayEnum(connectionStringType);
            $scope.scopeModel.selectedConnectionStringType = connectionStringType.ConnectionString;
            $scope.scopeModel.showConnectionString = true;
            $scope.scopeModel.showConnectionStringName = false;
            $scope.scopeModel.onConnectionStringTypeSelectionChanged = function () {
                if ($scope.scopeModel.selectedConnectionStringType != undefined) {

                    switch ($scope.scopeModel.selectedConnectionStringType.value) {
                        case connectionStringType.ConnectionString.value: $scope.scopeModel.showConnectionString = true; $scope.scopeModel.showConnectionStringName = false; break;
                        case connectionStringType.ConnectionStringName.value: $scope.scopeModel.showConnectionStringName = true; $scope.scopeModel.showConnectionString = false; break;
                    }

                }
            }


            $scope.scopeModel.saveTable = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
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
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadRequiredPermission, loadAnalyticDataProviderSettingsDirective]).then(function () {

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
                        $scope.scopeModel.connectionString = tableEntity.Settings.ConnectionString;
                        $scope.scopeModel.tableName = tableEntity.Settings.TableName;
                        $scope.scopeModel.timeColumnName = tableEntity.Settings.TimeColumnName;


                        $scope.scopeModel.connectionStringName = tableEntity.Settings.ConnectionStringName;
                        $scope.scopeModel.connectionString = tableEntity.Settings.ConnectionString;

                        if ($scope.scopeModel.connectionStringName != undefined) {
                            $scope.scopeModel.selectedConnectionStringType = connectionStringType.ConnectionStringName;
                        } else if ($scope.scopeModel.connectionString != undefined) {
                            $scope.scopeModel.selectedConnectionStringType = connectionStringType.ConnectionString;
                        }


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

        function getTable() {
            return VR_Analytic_AnalyticTableAPIService.GetTableById(tableId).then(function (response) {
                tableEntity = response;
            });
        }

        function buildTableObjectFromScope() {
            var table = {
                AnalyticTableId: tableId,
                Name: $scope.scopeModel.name,
                Settings: {
                    TableName: $scope.scopeModel.tableName,
                    TimeColumnName: $scope.scopeModel.timeColumnName,
                    ConnectionStringName: $scope.scopeModel.showConnectionStringName ? $scope.scopeModel.connectionStringName : undefined,
                    ConnectionString: $scope.scopeModel.showConnectionString ? $scope.scopeModel.connectionString : undefined,
                    DataRecordTypeIds: dataRecordTypeSelectorAPI.getSelectedIds(),
                    RequiredPermission: requiredPermissionAPI.getData(),
                    DataProvider: analyticDataProviderSettingsDirectiveAPI.getData()
                }
            }
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

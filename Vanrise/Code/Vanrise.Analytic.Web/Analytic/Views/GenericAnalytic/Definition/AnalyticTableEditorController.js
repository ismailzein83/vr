(function (appControllers) {

    "use strict";

    AnalyticTableEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_Analytic_AnalyticTableAPIService','VR_Analytic_ConnectionStringTypeEnum'];

    function AnalyticTableEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticTableAPIService, VR_Analytic_ConnectionStringTypeEnum) {

        var isEditMode;
        var tableEntity;
        var tableId;
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

            $scope.scopeModel.connectionStringType = UtilsService.getArrayEnum(VR_Analytic_ConnectionStringTypeEnum);
            $scope.scopeModel.selectedConnectionStringType = VR_Analytic_ConnectionStringTypeEnum.ConnectionString;
            $scope.scopeModel.showConnectionString = true;
            $scope.scopeModel.showConnectionStringName = false;
            $scope.scopeModel.onConnectionStringTypeSelectionChanged = function () {
                if ($scope.scopeModel.selectedConnectionStringType != undefined) {

                    switch ($scope.scopeModel.selectedConnectionStringType.value) {
                        case VR_Analytic_ConnectionStringTypeEnum.ConnectionString.value: $scope.scopeModel.showConnectionString = true; $scope.scopeModel.showConnectionStringName = false; break;
                        case VR_Analytic_ConnectionStringTypeEnum.ConnectionStringName.value: $scope.scopeModel.showConnectionStringName = true; $scope.scopeModel.showConnectionString = false; break;
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
                return UtilsService.waitMultipleAsyncOperations([setTitle,loadStaticData]).then(function () {

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

                function loadStaticData()
                {
                    if (tableEntity != undefined)
                    {
                        $scope.scopeModel.name = tableEntity.Name;
                        $scope.scopeModel.connectionString = tableEntity.Settings.ConnectionString;
                        $scope.scopeModel.tableName = tableEntity.Settings.TableName;
                        $scope.scopeModel.timeColumnName = tableEntity.Settings.TimeColumnName;


                        $scope.scopeModel.connectionStringName = tableEntity.Settings.ConnectionStringName;
                        $scope.scopeModel.connectionString = tableEntity.Settings.ConnectionString;

                        if ($scope.scopeModel.connectionStringName != undefined) {
                            $scope.scopeModel.selectedConnectionStringType = VR_Analytic_ConnectionStringTypeEnum.ConnectionStringName;
                        } else if ($scope.scopeModel.connectionString != undefined) {
                            $scope.scopeModel.selectedConnectionStringType = VR_Analytic_ConnectionStringTypeEnum.ConnectionString;
                        }


                    }
                }

            }

        }

        function getTable() {
            return VR_Analytic_AnalyticTableAPIService.GetTableById(tableId).then(function (response) {
                tableEntity = response;
            });
        }

        function buildTableObjectFromScope() {
            var table = {
                AnalyticTableId:tableId,
                Name:  $scope.scopeModel.name ,
                Settings:{
                    TableName: $scope.scopeModel.tableName,
                    TimeColumnName: $scope.scopeModel.timeColumnName,
                    ConnectionStringName: $scope.scopeModel.showConnectionStringName? $scope.scopeModel.connectionStringName:undefined,
                    ConnectionString:$scope.scopeModel.showConnectionString? $scope.scopeModel.connectionString:undefined
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

    }

    appControllers.controller('VR_Analytic_AnalyticTableEditorController', AnalyticTableEditorController);
})(appControllers);

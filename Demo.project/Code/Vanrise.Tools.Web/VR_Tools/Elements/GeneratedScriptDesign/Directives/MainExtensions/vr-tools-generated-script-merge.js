"use strict";

appControllers.directive("vrToolsGeneratedScriptMerge", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Tools_ColumnsAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService,VR_Tools_ColumnsAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Merge($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Directives/MainExtensions/Templates/GeneratedScriptMergeTemplate.html"
        };

        function Merge($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};
            var entity = {};
            var directivesReadyPromises = [];
            var isEditMode;

            var selectedTableDataGridApi;
            var selectTableDataGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var tableDataGridApi;
            var tableDataGridPromiseDeferred = UtilsService.createPromiseDeferred();
            var insertColumnsDirectiveApi;
            var insertColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var updateColumnsDirectiveApi;
            var updateColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var identifierColumnsDirectiveApi;
            var identifierColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var identifierColumnsChangedPromiseDeferred;

            var queryTypeDirectiveApi;
            var queryTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInsertColumnsDirectiveReady = function (api) {
                    insertColumnsDirectiveApi = api;
                    insertColumnsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onUpdateColumnsDirectiveReady = function (api) {
                    updateColumnsDirectiveApi = api;
                    updateColumnsReadyPromiseDeferred.resolve();

                };

                $scope.scopeModel.onIdentifierColumnsDirectiveReady = function (api) {
                    identifierColumnsDirectiveApi = api;
                    identifierColumnsReadyPromiseDeferred.resolve();
                };
                 
                $scope.scopeModel.onQueryTypeDirectiveReady = function (api) {
                    queryTypeDirectiveApi = api;
                    queryTypeReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectedTableDataGridReady = function (api) {
                    selectedTableDataGridApi = api;
                    selectTableDataGridReadyPromiseDeferred.resolve();

                };

                $scope.scopeModel.generateSelectedTableDataGrid = function () {

                    selectedTableDataGridApi.load({ Query: tableDataGridApi.getData().gridQuery, ColumnNames: tableDataGridApi.getData().columnNames });
                };

                $scope.scopeModel.onIdentifierColumnsChanged = function () {

                    identifierColumnsChangedPromiseDeferred=UtilsService.createPromiseDeferred();
                };

                $scope.scopeModel.onTableDataGridReady = function (api) {
                    tableDataGridApi = api;
                    tableDataGridPromiseDeferred.resolve();

                };

                $scope.scopeModel.onSelectedTableDataGridReady = function (api) {
                    selectedTableDataGridApi = api;
                    selectTableDataGridReadyPromiseDeferred.resolve();

                };

                $scope.scopeModel.executeQuery = function () {

                    tableDataGridPromiseDeferred.promise.then(function (response) {
                        tableDataGridApi.load(getFilter()).then(function () {
                            if (identifierColumnsChangedPromiseDeferred) {
                                selectTableDataGridReadyPromiseDeferred.promise.then(function (response) {
                                    selectedTableDataGridApi.load({ Query: tableDataGridApi.getData().gridQuery, ColumnNames: tableDataGridApi.getData().columnNames });
                                });
                                identifierColumnsChangedPromiseDeferred = undefined;
                            }
                        });
                    });
                };

                $scope.scopeModel.validateTableSelection = function () {
                    if (!isEditMode) {
                        if (selectedTableDataGridApi == undefined || selectedTableDataGridApi.getData().tableRows.length == 0)
                            return 'You Should At Least Select One Row ';
                    }
                    return null;
                };

                $scope.scopeModel.disableExecuteQuery = function () {
                    if (identifierColumnsDirectiveApi != undefined && identifierColumnsDirectiveApi.getSelectedIds()!=undefined && identifierColumnsDirectiveApi.getSelectedIds().length != 0)
                        return false;
                    return true;
                };

                $scope.scopeModel.disablegenerateSelectedTableDataGrid = function () {

                    if (tableDataGridApi == undefined || tableDataGridApi.getData() == undefined || tableDataGridApi.getData().gridQuery == undefined || tableDataGridApi.getData().gridQuery.BulkActionFinalState == undefined || tableDataGridApi.getData().gridQuery.BulkActionFinalState.TargetItems == undefined || (tableDataGridApi.getData().gridQuery.BulkActionFinalState.TargetItems.length == 0 && tableDataGridApi.getData().gridQuery.allSelected == false))
                        return true;
                    return false;
                };
             
                function getFilter() {
                    var selectedColumns = identifierColumnsDirectiveApi.getSelectedIds();
                    var selectedColumnsNames = [];
                    if (selectedColumns != undefined) {
                        for (var i = 0; i < selectedColumns.length; i++) {
                            selectedColumnsNames.push({ ColumnName: selectedColumns[i] });
                        }
                    }
                    return {
                        
                        context :{
                            triggerRetrieveData: function () {
                                gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                                gridApi.retrieveData(gridQuery);
                            },
                            hasItems: function () {
                                return $scope.scopeModel.tableData.length > 0;
                            },
                            setActionsEnablity : function (enablity) {
                                $scope.scopeModel.setActionsEnablity = !enablity;
                            }
                        },
                        query: {
                            ConnectionId:entity.filter.ConnectionId,
                            SchemaName: entity.filter.SchemaName,
                            TableName: entity.filter.TableName,
                            WhereCondition: $scope.scopeModel.sqlFilter,
                            IdentifierColumns: selectedColumnsNames
                        }
                    };
                }

                UtilsService.waitMultiplePromises([insertColumnsReadyPromiseDeferred.promise, updateColumnsReadyPromiseDeferred.promise, identifierColumnsReadyPromiseDeferred.promise, queryTypeReadyPromiseDeferred.promise,
                selectTableDataGridReadyPromiseDeferred.promise, tableDataGridPromiseDeferred.promise, selectTableDataGridReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};
                var settingsPayload;
                api.load = function (payload) {

                    entity = payload.generatedScriptSettingsEntity;
                    isEditMode = entity.isEditMode;
                    var filter = entity.filter;
                    var promises = [];

                    function loadInsertColumnsDirective(insertColumnsPayload) {
                        var promises = [];
                        var insertColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        promises.push(insertColumnsReadyPromiseDeferred.promise);
                        UtilsService.waitMultiplePromises(promises).then(function (response) {

                            
                            VRUIUtilsService.callDirectiveLoad(insertColumnsDirectiveApi, insertColumnsPayload, insertColumnsDirectiveLoadDeferred);
                        });

                        return insertColumnsDirectiveLoadDeferred.promise;
                    }
                    function loadUpdateColumnsDirective(updateColumnsPayload) {
                        var promises = [];
                        var updateColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        promises.push(updateColumnsReadyPromiseDeferred.promise);
                        UtilsService.waitMultiplePromises(promises).then(function (response) {
                           

                            VRUIUtilsService.callDirectiveLoad(updateColumnsDirectiveApi, updateColumnsPayload, updateColumnsDirectiveLoadDeferred);
                        });

                        return updateColumnsDirectiveLoadDeferred.promise;
                    }
                    function loadIdentifierColumnsDirective(identifierColumnsPayload) {
                        var promises = [];
                        var identifierColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        promises.push(identifierColumnsReadyPromiseDeferred.promise);
                        UtilsService.waitMultiplePromises(promises).then(function (response) {
                           
                            VRUIUtilsService.callDirectiveLoad(identifierColumnsDirectiveApi, identifierColumnsPayload, identifierColumnsDirectiveLoadDeferred);
                        });

                        return identifierColumnsDirectiveLoadDeferred.promise;
                    }
                    function loadQueryTypeDirective(queryTypePayload) {
                        var promises = [];
                        var queryTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        promises.push(queryTypeReadyPromiseDeferred.promise);
                        UtilsService.waitMultiplePromises(promises).then(function (response) {

                            VRUIUtilsService.callDirectiveLoad(queryTypeDirectiveApi, queryTypePayload, queryTypeDirectiveLoadDeferred);
                        });

                        return queryTypeDirectiveLoadDeferred.promise;
                    }

                    function loadSelectedDataRows() {
                        var columnNames = [];
                        var selectTableDataGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        VR_Tools_ColumnsAPIService.GetColumnsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    columnNames.push(response[i]);
                                }
                                selectTableDataGridReadyPromiseDeferred.promise.then(function () {
                                    selectedTableDataGridApi.load({ DataRows: entity.Settings.DataRows, ColumnNames: columnNames });
                                    selectTableDataGridLoadPromiseDeferred.resolve();
                                });
                            }

                        });
                        return selectTableDataGridLoadPromiseDeferred.promise;
                    }
                    if (isEditMode) {

                        $scope.scopeModel.IsIdentity = entity.Settings.IsIdentity;

                        var insertColumnsNames = [];
                        var updateColumnsNames = [];
                        var identifierColumnsNames = [];

                            for (var i = 0; i < entity.Settings.InsertColumns.length; i++) {
                                insertColumnsNames.push(entity.Settings.InsertColumns[i].ColumnName);
                            }
                        var insertColumnsPayload = {
                            filter: entity.filter,
                            selectedIds: insertColumnsNames
                        };
                        promises.push(loadInsertColumnsDirective(insertColumnsPayload));

                            for (var j = 0; j < entity.Settings.UpdateColumns.length; j++) {
                                updateColumnsNames.push(entity.Settings.UpdateColumns[j].ColumnName);
                            }
                        var updateColumnsPayload = {
                            filter: entity.filter,
                            selectedIds: updateColumnsNames
                        };
                        promises.push(loadUpdateColumnsDirective(updateColumnsPayload));

                            for (var k = 0; k < entity.Settings.IdentifierColumns.length; k++) {
                                identifierColumnsNames.push(entity.Settings.IdentifierColumns[k].ColumnName);
                            }
                        var identifierColumnsPayload = {
                            filter: entity.filter,
                            selectedIds: identifierColumnsNames
                        };
                        promises.push(loadIdentifierColumnsDirective(identifierColumnsPayload));

                        promises.push(loadQueryTypeDirective({ selectedIds: entity.Settings.QueryType }));

                        promises.push(loadSelectedDataRows());

                        return UtilsService.waitMultiplePromises(promises);
                    }

                    else {
                        var columnsPayload = {
                            filter: entity.filter
                        };

                        promises.push(loadInsertColumnsDirective(columnsPayload));
                        promises.push(loadUpdateColumnsDirective(columnsPayload));
                        promises.push(loadIdentifierColumnsDirective(columnsPayload));
                        promises.push(loadQueryTypeDirective());
                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.getData = function () {
               
                    var insertColumnsData = insertColumnsDirectiveApi.getSelectedIds();
                    var updateColumnsData = updateColumnsDirectiveApi.getSelectedIds();
                    var identifierColumnsData = identifierColumnsDirectiveApi.getSelectedIds();

                    var insertColumnsToJson = [];
                    for (var j = 0; j < insertColumnsData.length; j++) {
                        insertColumnsToJson.push({ ColumnName: insertColumnsData[j] });

                    }
                    var updateColumnsToJson = [];
                    for (var k = 0; k < updateColumnsData.length; k++) {
                        updateColumnsToJson.push({ ColumnName: updateColumnsData[k] });

                    }
                    var identifierColumnsToJson = [];
                    for (var l = 0; l < identifierColumnsData.length; l++) {
                        identifierColumnsToJson.push({ ColumnName: identifierColumnsData[l] });

                    }

                    var dataRows;
                    if (selectedTableDataGridApi != undefined) {
                        dataRows = selectedTableDataGridApi.getData().tableRows;
                    }

                    return {
                        $type: "Vanrise.Common.MainExtensions.MergeGeneratedScriptItem, Vanrise.Common.MainExtensions",
                        IdentifierColumns: identifierColumnsToJson,
                        InsertColumns: insertColumnsToJson,
                        UpdateColumns: updateColumnsToJson,
                        DataRows: dataRows,
                        QueryType: queryTypeDirectiveApi.getSelectedIds(),
                        IsIdentity: $scope.scopeModel.IsIdentity
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);
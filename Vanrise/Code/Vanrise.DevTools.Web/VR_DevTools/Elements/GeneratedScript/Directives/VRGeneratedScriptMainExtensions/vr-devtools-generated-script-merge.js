"use strict";

appControllers.directive("vrDevtoolsGeneratedScriptMerge", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Devtools_ColumnsAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Devtools_ColumnsAPIService) {

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
            templateUrl: "/Client/Modules/VR_DevTools/Elements/GeneratedScript/Directives/VRGeneratedScriptMainExtensions/Templates/VRGeneratedScriptMergeTemplate.html"
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

            var gridLoadedPromiseDeferred=UtilsService.createPromiseDeferred();;
            var queryTypeDirectiveApi;
            var queryTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var variablesDataGridApi;
            var variablesGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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
                $scope.scopeModel.onQueryTypeChanged = function (value) {
                    if (queryTypeDirectiveApi != undefined) {
                        $scope.scopeModel.insertColumnsRequired = true;
                        $scope.scopeModel.updateColumnsRequired = true;
                        if (value) {
                            if (value.value == 1) { 
                                $scope.scopeModel.insertColumnsRequired = true;
                            $scope.scopeModel.updateColumnsRequired = false;

                            }
                            else {
                                $scope.scopeModel.insertColumnsRequired = false;
                                $scope.scopeModel.updateColumnsRequired = true;
                            }
                        }
                    }

                };
                $scope.scopeModel.onInsertColumnsChanged = function (value) {
                    if (insertColumnsDirectiveApi != undefined) {
                        if (value != undefined) {
                            if (gridLoadedPromiseDeferred != undefined) {
                                gridLoadedPromiseDeferred.resolve();
                            }
                            else {
                                var columnNames = updateColumnsDirectiveApi.getSelectedIds();
                                if (columnNames == undefined)
                                    columnNames = insertColumnsDirectiveApi.getSelectedIds();
                                else {
                                    var insertColumns = insertColumnsDirectiveApi.getSelectedIds();
                                    if (insertColumns != undefined) {
                                        for (var i = 0; i < insertColumns.length; ++i) {
                                            var insertColumn = insertColumns[i];
                                            if (!columnNames.includes(insertColumn))
                                                columnNames.push(insertColumn);
                                        }
                                    }
                                }
                                selectedTableDataGridApi.load({
                                    Query: tableDataGridApi.getData().gridQuery,
                                    ColumnNames: columnNames,
                                    getVariables: function () {

                                        return variablesDataGridApi.getData();
                                    }
                                });

                            }
                        }
                    }

                };

                $scope.scopeModel.onUpdateColumnsChanged = function (value) {
                    if (updateColumnsDirectiveApi != undefined) {
                        if (value != undefined) {
                            if (gridLoadedPromiseDeferred != undefined) {
                                gridLoadedPromiseDeferred.resolve();
                            }
                            else {
                                var columnNames = updateColumnsDirectiveApi.getSelectedIds();
                                if (columnNames == undefined)
                                    columnNames = insertColumnsDirectiveApi.getSelectedIds();
                                else {
                                    var insertColumns = insertColumnsDirectiveApi.getSelectedIds();
                                    if (insertColumns != undefined) {
                                        for (var i = 0; i < insertColumns.length; ++i) {
                                            var insertColumn = insertColumns[i];
                                            if (!columnNames.includes(insertColumn))
                                                columnNames.push(insertColumn);
                                        }
                                    }
                                }
                                selectedTableDataGridApi.load({
                                    Query: tableDataGridApi.getData().gridQuery,
                                    ColumnNames: columnNames,
                                    getVariables: function () {

                                        return variablesDataGridApi.getData();
                                    }
                                });
                            }
                        }
                    }
                };
                $scope.scopeModel.generateSelectedTableDataGrid = function () {

               
                    var columnNames = updateColumnsDirectiveApi.getSelectedIds();
                    if (columnNames == undefined)
                        columnNames = insertColumnsDirectiveApi.getSelectedIds();
                    else {
                        var insertColumns = insertColumnsDirectiveApi.getSelectedIds();
                        if (insertColumns != undefined) {
                            for (var i = 0; i < insertColumns.length; ++i) {
                                var insertColumn = insertColumns[i];
                                if (!columnNames.includes(insertColumn))
                                    columnNames.push(insertColumn);
                            }
                        }
                    }
                    selectedTableDataGridApi.load({
                        Query: tableDataGridApi.getData().gridQuery,
                        ColumnNames: columnNames,
                        getVariables: function () {

                            return variablesDataGridApi.getData();
                        }
                    });
                };

                $scope.scopeModel.onIdentifierColumnsChanged = function () {

                    identifierColumnsChangedPromiseDeferred = UtilsService.createPromiseDeferred();
                };

                $scope.scopeModel.onTableDataGridReady = function (api) {
                    tableDataGridApi = api;
                    tableDataGridPromiseDeferred.resolve();

                };
                $scope.scopeModel.onVariablesGridReady = function (api) {
                    variablesDataGridApi = api;
                    variablesGridReadyPromiseDeferred.resolve();

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
                                    var columnNames = updateColumnsDirectiveApi.getSelectedIds();
                                    if (columnNames == undefined)
                                        columnNames = insertColumnsDirectiveApi.getSelectedIds();
                                    else {
                                        var insertColumns = insertColumnsDirectiveApi.getSelectedIds();
                                        if (insertColumns != undefined) {
                                            for (var i = 0; i < insertColumns.length; ++i) {
                                                var insertColumn = insertColumns[i];
                                                if (!columnNames.includes(insertColumn))
                                                    columnNames.push(insertColumn);
                                            }
                                        }
                                    }
                                    selectedTableDataGridApi.load({
                                        Query: tableDataGridApi.getData().gridQuery,
                                        ColumnNames: columnNames,
                                        getVariables: function () {

                                            return variablesDataGridApi.getData();
                                        }
                                    });
                                });
                                identifierColumnsChangedPromiseDeferred = undefined;
                                gridLoadedPromiseDeferred = undefined;
                            }
                        });
                    });
                };

                $scope.scopeModel.disableExecuteQuery = function () {
                    if (identifierColumnsDirectiveApi != undefined && identifierColumnsDirectiveApi.getSelectedIds() != undefined && identifierColumnsDirectiveApi.getSelectedIds().length != 0)
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

                        context: {
                            triggerRetrieveData: function () {
                                gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                                gridApi.retrieveData(gridQuery);
                            },
                            hasItems: function () {
                                return $scope.scopeModel.tableData.length > 0;
                            },
                            setActionsEnablity: function (enablity) {
                                $scope.scopeModel.setActionsEnablity = !enablity;
                            }
                        },
                        query: {
                            ConnectionId: entity.filter.ConnectionId,
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
                    var variables = entity.Settings.Variables;
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

                    function loadSelectedDataRows(selectTableDataPayload) {
                        var selectTableDataGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                       
                        selectTableDataGridReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(selectedTableDataGridApi, selectTableDataPayload, selectTableDataGridLoadPromiseDeferred);
                                });
                        return selectTableDataGridLoadPromiseDeferred.promise;
                    }

                    function loadVariablesGridDirective(variablesPayload) {
                        var variablesGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        variablesGridReadyPromiseDeferred.promise.then(function () {
                            var variablesPayload = {
                                connectionStringId: entity.filter.ConnectionId,
                                variables: entity.isEditMode?entity.Settings.Variables:undefined
                            }; 

                            VRUIUtilsService.callDirectiveLoad(variablesDataGridApi, variablesPayload, variablesGridLoadPromiseDeferred);
                        });
                        return variablesGridLoadPromiseDeferred.promise;
                    }
                  
                    promises.push(loadVariablesGridDirective());
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
                        
                        var selectTableDataColumns = [];
                        for (var j = 0; j < updateColumnsNames.length; j++) {
                            selectTableDataColumns.push(updateColumnsNames[j]);
                        }
                        if (selectTableDataColumns == undefined)
                            selectTableDataColumns = insertColumnsNames;
                        else {
                            var insertColumns = insertColumnsNames;
                            if (insertColumns != undefined) {
                                for (var i = 0; i < insertColumns.length; ++i) {
                                    var insertColumn = insertColumns[i];
                                    if (!selectTableDataColumns.includes(insertColumn))
                                        selectTableDataColumns.push(insertColumn);
                                }
                            }
                        }

                        var selectTableDataPayload = {
                            DataRows: entity.Settings.DataRows,
                            ColumnNames: selectTableDataColumns,
                            getVariables: function () {
                                if (variablesDataGridApi != undefined && variablesDataGridApi.getData()!=undefined)
                                    variables = variablesDataGridApi.getData();
                                return variables;
                            }
                        };
                        promises.push(loadIdentifierColumnsDirective(identifierColumnsPayload));

                        promises.push(loadQueryTypeDirective({ selectedIds: entity.Settings.QueryType }));

                        promises.push(loadSelectedDataRows(selectTableDataPayload));

                       

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
                        $type: "Vanrise.DevTools.MainExtensions.MergeGeneratedScriptItem, Vanrise.DevTools.MainExtensions",
                        IdentifierColumns: identifierColumnsToJson,
                        InsertColumns: insertColumnsToJson,
                        UpdateColumns: updateColumnsToJson,
                        DataRows: dataRows,
                        QueryType: queryTypeDirectiveApi.getSelectedIds(),
                        IsIdentity: $scope.scopeModel.IsIdentity,
                        Variables: variablesDataGridApi.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);
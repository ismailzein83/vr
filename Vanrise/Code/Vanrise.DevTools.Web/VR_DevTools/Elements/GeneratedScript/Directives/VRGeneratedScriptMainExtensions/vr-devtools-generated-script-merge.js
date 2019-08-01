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
            var identifierColumnExists = false;

            var isEditMode;

            var selectedTableDataGridApi;
            var selectTableDataGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var tableDataGridApi;
            var tableDataGridPromiseDeferred = UtilsService.createPromiseDeferred();
            var identifierColumnNames;
            var selectedColumnsNames;

            var columnsDirectiveApi;
            var columnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var columnsChangedPromiseDeferred;

            var gridLoadedPromiseDeferred=UtilsService.createPromiseDeferred();;

            var variablesDataGridApi;
            var variablesGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var bulkActionDraftInstance;
            var variables;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedColumns = [];
                ctrl.datasource = [];


                $scope.scopeModel.onColumnsDirectiveReady = function (api) {
                    columnsDirectiveApi = api;
                    columnsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onColumnSelected = function (item) {
                    var columnName = UtilsService.getItemByVal(ctrl.datasource, item.Name, "data.ColumnName");
                    if (!columnName) {
                        ctrl.datasource.push({
                            data: {
                                ColumnName: item.Name,
                                IncludeInInsert: true,
                                IncludeInUpdate: true
                            }
                        });
                    }
            
                };

                $scope.scopeModel.clearAllColumns = function () {
                    ctrl.datasource.length=0;
                };
                $scope.scopeModel.onDataTabSelected = function () {

                    if (columnsDirectiveApi != undefined) {
                        //if (gridLoadedPromiseDeferred != undefined) {
                        //    gridLoadedPromiseDeferred.resolve();
                        //}
                        //else {
                            $scope.scopeModel.isLoading = true;

                            setColumns();
                            var columnNames = columnsDirectiveApi.getSelectedIds();
                      
                        var gridQuery;
                        if (tableDataGridApi.getData().gridQuery) {
                            gridQuery= tableDataGridApi.getData().gridQuery;
                            gridQuery.IdentifierColumns = identifierColumnNames;
                        }

                            selectedTableDataGridApi.load({
                                Query: gridQuery,
                                ColumnNames: columnNames,
                                context: getContext(),
                                tableData: tableDataGridApi.getData().tableData,
                                moveItems: false

                            }).finally(function () {

                                $scope.scopeModel.isLoading = false;

                                });
                        //}
                    }
                };

                $scope.scopeModel.onColumnDeselected = function (item) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, item.Name, "data.ColumnName");
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.onDeleteColumn = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    var selectedColumnIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedColumns, dataItem.data.ColumnName, "Name");
                    $scope.scopeModel.selectedColumns.splice(selectedColumnIndex, 1);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.onSelectedTableDataGridReady = function (api) {
                    selectedTableDataGridApi = api;
                    selectTableDataGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.validateColumnsSelection = function () {
                    var insertOrUpdateColumnExists = false;
                    identifierColumnExists = false;
                    if (ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var item = ctrl.datasource[i].data;
                            if (item.IsIdentifier) {
                                identifierColumnExists = true;
                                if (!item.IncludeInInsert)
                                    return 'Identifier Column Must Be Included In Insert Columns ';
                            }

                            //if (item.IncludeInInsert || item.IncludeInUpdate)
                            //    insertOrUpdateColumnExists = true; && !insertOrUpdateColumnExists
                        }
                    }
                    //if (!identifierColumnExists)
                    //    return 'You Should At Least Select One Identifier Column And One Insert Or Update Column ';
                    if (!identifierColumnExists)
                        return 'You Should At Least Select One Identifier Column';
                    //else if (!insertOrUpdateColumnExists)
                    //    return 'You Should At Least Select One Insert Or Update Column'
                    return null;
                };

                $scope.scopeModel.onColumnsChanged = function () {

                    columnsChangedPromiseDeferred = UtilsService.createPromiseDeferred();
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
                            if (columnsChangedPromiseDeferred) {
                                selectTableDataGridReadyPromiseDeferred.promise.then(function (response) {

                                    var gridQuery = tableDataGridApi.getData().gridQuery;
                                    gridQuery.IdentifierColumns = identifierColumnNames;
                                    selectedTableDataGridApi.load({
                                        Query: tableDataGridApi.getData().gridQuery,
                                        ColumnNames: columnsDirectiveApi.getSelectedIds(),
                                        context: getContext(),
                                        moveItems: false,
                                        moveItems: false,
                                        executeQuery: true,
                                        tableData: tableDataGridApi.getData().tableData,
                                    }).then(function () {
                                        compareTables();
                                        });
                                });
                                columnsChangedPromiseDeferred = undefined;
                                gridLoadedPromiseDeferred = undefined;
                            }
                        });
                    });
                };

                $scope.scopeModel.disableExecuteQuery = function () {
                    if (identifierColumnExists)
                        return false;
                    return true;
                };

                UtilsService.waitMultiplePromises([columnsReadyPromiseDeferred.promise,
                selectTableDataGridReadyPromiseDeferred.promise, tableDataGridPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function getFilter() {

                setColumns();
                var columns = [];
                if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        columns.push(ctrl.datasource[i].data.ColumnName);
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
                        },
                        selectedTableDataContext: getContext(),
                        nonIdentifierColumnNames:getNonIdentifierColumnNames
                    },
                    query: {
                        ConnectionId: entity.filter.ConnectionId,
                        SchemaName: entity.filter.SchemaName,
                        TableName: entity.filter.TableName,
                        WhereCondition: $scope.scopeModel.sqlFilter,
                        JoinStatement: $scope.scopeModel.sqlJoinStatement,
                        IdentifierColumns: identifierColumnNames
                    },
                    columnNames: selectedColumnsNames,
                    generateSelectedTableDataGrid: function (payload) {
                        payload.ColumnNames = columns;
                        return selectedTableDataGridApi.load(payload);
                    }

                };
            }

            function setColumns() {

                selectedColumnsNames = [];
                identifierColumnNames = [];
                if (ctrl.datasource.length > 0) {
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        var item = ctrl.datasource[i].data;
                        selectedColumnsNames.push({ Name: item.ColumnName });
                        if (item.IsIdentifier)
                            identifierColumnNames.push({ ColumnName: item.ColumnName });
                    }
                }

            }

            function getNonIdentifierColumnNames() {
                var selectedColumns=[];
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var item = ctrl.datasource[i].data;
                    if (!item.IsIdentifier)
                        selectedColumns.push({ Name: item.ColumnName });
                }
                return selectedColumns;
            }


            function compareTables(rowIndex, dataItem, column) {
                if (tableDataGridApi && selectedTableDataGridApi) {

                    var tableData = tableDataGridApi.getData().tableData;
                    var selectedTableData = selectedTableDataGridApi.getData().selectedTableData;

                    if (selectedTableData != undefined && selectedTableData.length > 0 && tableData != undefined && tableData.length > 0) {
                        setColumns();

                        if (rowIndex != undefined) {
                            var dataItemdentifierKey = "";
                            for (var l = 0; l < identifierColumnNames.length; l++) {
                                var idKey = identifierColumnNames[l].ColumnName;
                                dataItemdentifierKey += dataItem.Entity.FieldValues[idKey] + "_";
                            }
                            for (var j = 0; j < tableData.length; j++) {
                                var row = tableData[j];
                                var identifierKey = "";
                                for (var l = 0; l < identifierColumnNames.length; l++) {
                                    var identifierColumnName = identifierColumnNames[l].ColumnName;
                                    identifierKey += row.FieldValues[identifierColumnName] + "_";
                                }
                                if (dataItemdentifierKey == identifierKey) {
                                    if (column != undefined) {
                                        if (row.FieldValues[column.name] != dataItem.DescriptionEntity[column.name].value) {
                                            selectedTableData[rowIndex].DescriptionEntity[column.name].differentValue = true;
                                            row.DescriptionEntity[column.name] = { differentValue: true };
                                        }
                                        else {
                                            selectedTableData[rowIndex].DescriptionEntity[column.name].differentValue = false;
                                            row.DescriptionEntity[column.name] = { differentValue: false };

                                        }
                                    }
                                    else {
                                        row.rowExists = false;
                                        row.DescriptionEntity = {};
                                    }
                                    break;
                                }
                            }
                        }
                        else {
                            for (var k = 0; k < selectedTableData.length; k++) {
                                var selectedIdentifierKey = "";
                                var selectedRow = selectedTableData[k];

                                for (var l = 0; l < identifierColumnNames.length; l++) {
                                    var idKey = identifierColumnNames[l].ColumnName;
                                    selectedIdentifierKey += selectedRow.Entity.FieldValues[idKey] + "_";
                                }


                                for (var j = 0; j < tableData.length; j++) {
                                    var row = tableData[j];
                                    var identifierKey = "";
                                    for (var l = 0; l < identifierColumnNames.length; l++) {
                                        var identifierColumnName = identifierColumnNames[l].ColumnName;
                                        if (row.FieldValues[identifierColumnName] != undefined && row.FieldValues[identifierColumnName] != null && row.FieldValues[identifierColumnName] != "")
                                        identifierKey += row.FieldValues[identifierColumnName] + "_";
                                    } 
                                    if (identifierKey == selectedIdentifierKey) {
                                        var columnNames = columnsDirectiveApi.getSelectedIds();

                                        for (var l = 0; l < columnNames.length; l++) {
                                            var columnName = columnNames[l];
                                            if (selectedRow.DescriptionEntity[columnName] != undefined && selectedRow.DescriptionEntity[columnName].value != undefined && selectedRow.DescriptionEntity[columnName].value != "*") {
                                                if (row.FieldValues[columnName] != selectedRow.DescriptionEntity[columnName].value) {
                                                    selectedRow.DescriptionEntity[columnName].differentValue = true;
                                                    row.DescriptionEntity[columnName] = { differentValue: true };
                                                }
                                                else {
                                                    selectedRow.DescriptionEntity[columnName].differentValue = false;
                                                    row.DescriptionEntity[columnName] = { differentValue: false };
                                                }
                                            }
                                            else {
                                                if (row.FieldValues[columnName] != undefined) {
                                                    selectedRow.DescriptionEntity[columnName] = { value: "*", differentValue: true };
                                                    row.DescriptionEntity[columnName] = { differentValue: true };
                                                }
                                                else
                                                {
                                                    selectedRow.DescriptionEntity[columnName] = { value: "*", differentValue: false };
                                                    row.DescriptionEntity[columnName] = { differentValue: false };
                                                }
                                            }
                                        } 
                                      
                                            selectedRow.rowExists = true;
                                        row.rowExists = true;
                                        break;
                                    }
                                    else {
                                        selectedRow.rowExists = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            function getContext() {
                return {
                    compareTables: compareTables,
                    getVariables: function () {
                        return variablesDataGridApi.getData();
                    }
                };
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    entity = payload.generatedScriptSettingsEntity;
                    variables = entity.Settings != undefined ? entity.Settings.Variables : undefined;
                    isEditMode = entity.isEditMode;
                    var promises = [];

                    function loadColumnsDirective(payload) {

                        var promises = [];
                        var columnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        promises.push(columnsReadyPromiseDeferred.promise);
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            VRUIUtilsService.callDirectiveLoad(columnsDirectiveApi, payload, columnsDirectiveLoadDeferred);
                        });

                        return columnsDirectiveLoadDeferred.promise;
                    }

                    function loadSelectedDataRows(selectTableDataPayload) {
                        var promises = [];
                        promises.push(selectTableDataGridReadyPromiseDeferred.promise);
                        promises.push(variablesGridReadyPromiseDeferred.promise);
                        var selectTableDataGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                       
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            VRUIUtilsService.callDirectiveLoad(selectedTableDataGridApi, selectTableDataPayload, selectTableDataGridLoadPromiseDeferred);
                        });
                        return selectTableDataGridLoadPromiseDeferred.promise;
                    }

                    function loadVariablesGridDirective() {
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
                        $scope.scopeModel.sqlFilter = entity.Settings.LastWhereCondition;
                        $scope.scopeModel.sqlJoinStatement = entity.Settings.LastJoinStatement;
                        var columnsNames = [];

                        for (var k = 0; k < entity.Settings.Columns.length; k++)
                        {
                            var data = entity.Settings.Columns[k];
                            ctrl.datasource.push({ data: data });
                            columnsNames.push(data.ColumnName);
                        }

                        var columnsPayload = {
                            filter: entity.filter,
                            selectedIds: columnsNames
                        };
                        

                        var selectTableDataPayload = {
                            selectedDataRows: entity.Settings.DataRows,
                            ColumnNames: columnsNames,
                            context: getContext()
                        };
                        promises.push(loadColumnsDirective(columnsPayload));


                        promises.push(loadSelectedDataRows(selectTableDataPayload));

                        return UtilsService.waitMultiplePromises(promises);
                    }

                    else {
                        var columnsPayload = {
                            filter: entity.filter
                        };

                        promises.push(loadColumnsDirective(columnsPayload));
                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.getData = function () {
                
                    var columns=[];
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            columns.push(ctrl.datasource[i].data);
                        }
                    }

                    var dataRows;

                    if (selectedTableDataGridApi != undefined) {
                        dataRows = selectedTableDataGridApi.getData().tableRows; 
                    } 

                    return {
                        $type: "Vanrise.DevTools.MainExtensions.MergeGeneratedScriptItem, Vanrise.DevTools.MainExtensions",
                        DataRows: dataRows,
                        Columns: columns,
                        IsIdentity: $scope.scopeModel.IsIdentity,
                        Variables: variablesDataGridApi.getData(),
                        LastWhereCondition: $scope.scopeModel.sqlFilter,
                        LastJoinStatement: $scope.scopeModel.sqlJoinStatement
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);
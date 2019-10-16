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


            var variablesDataGridApi;
            var variablesGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var bulkActionDraftInstance;
            var tabsContainerApi;
            var variables;
            function initializeController() {
                $scope.scopeModel = {};
              
                $scope.scopeModel.isIncludeAllInInsert = true;
                $scope.scopeModel.isIncludeAllInUpdate = true;

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
                                "$type": "Vanrise.DevTools.Business.MergeGeneratedScriptItemColumn, Vanrise.DevTools.Business",
                                ColumnName: item.Name,
                                IncludeInInsert: true,
                                IncludeInUpdate: true
                            }
                        });
                    }

                };

                $scope.scopeModel.clearAllColumns = function () {
                    ctrl.datasource.length = 0;
                };

                $scope.scopeModel.onTabsContainerReady = function (api) {
                    tabsContainerApi = api;
                };

                $scope.scopeModel.onDataTabSelected = function () {

                    if (columnsDirectiveApi != undefined) {
                  
                        $scope.scopeModel.isLoading = true;

                        setColumns();
                        var columnNames = columnsDirectiveApi.getSelectedIds();

                        var gridQuery;
                        if (tableDataGridApi.getData().gridQuery) {
                            gridQuery = tableDataGridApi.getData().gridQuery;
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

                $scope.scopeModel.includeAllInInsert = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            ctrl.datasource[i].data.IncludeInInsert = $scope.scopeModel.isIncludeAllInInsert;
                        }
                    }
                };
                $scope.scopeModel.includeAllInUpdate = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            ctrl.datasource[i].data.IncludeInUpdate = $scope.scopeModel.isIncludeAllInUpdate;
                        }
                    }
                };
                $scope.scopeModel.validateColumnsSelection = function () {
                    identifierColumnExists = false;
          
                    if (ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var item = ctrl.datasource[i].data;
                            if (item.IsIdentifier) {
                                identifierColumnExists = true;
                                if (!item.IncludeInInsert)
                                    return 'Identifier Column Must Be Included In Insert Columns ';
                            }
                        }
                    }
                    if (!identifierColumnExists)
                        return 'You Should At Least Select One Identifier Column';
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
                        nonIdentifierColumnNames: getNonIdentifierColumnNames
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
                var selectedColumns = [];
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var item = ctrl.datasource[i].data;
                    if (!item.IsIdentifier)
                        selectedColumns.push({ Name: item.ColumnName });
                }
                return selectedColumns;
            }
            function setRowAttributes(row, isDifferent, isVariable, isOverridden) {

                row.isDifferent = isDifferent;
                row.isVariable = isVariable;
                row.isOverridden = isOverridden;
                row.includeOverriddenValues = $scope.scopeModel.includeOverriddenValues;
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
                                            if (dataItem.DescriptionEntity[column.name].$type != undefined && dataItem.DescriptionEntity[column.name].$type.includes("GeneratedScriptOverriddenData")) {
                                                setRowAttributes(selectedTableData[rowIndex].DescriptionEntity[column.name], false, false, true);
                                                row.DescriptionEntity[column.name] = { isDifferent: $scope.scopeModel.includeOverriddenValues };
                                            }
                                            else if (dataItem.DescriptionEntity[column.name].$type != undefined && dataItem.DescriptionEntity[column.name].$type.includes("GeneratedScriptVariableData")) {
                                                setRowAttributes(selectedTableData[rowIndex].DescriptionEntity[column.name], false, true, false);
                                                row.DescriptionEntity[column.name] = { isDifferent: false };
                                            }
                                            else {
                                                if (selectedTableData[rowIndex].DescriptionEntity[column.name].value == "*") {
                                                    setRowAttributes(selectedTableData[rowIndex].DescriptionEntity[column.name], false, false, false);
                                                    row.DescriptionEntity[column.name] = { isDifferent: false };
                                                }
                                                else {
                                                    setRowAttributes(selectedTableData[rowIndex].DescriptionEntity[column.name], true, false, false);
                                                    row.DescriptionEntity[column.name] = { isDifferent: true };
                                                }
                                            }
                                        }
                                        else {
                                            setRowAttributes(selectedTableData[rowIndex].DescriptionEntity[column.name], false, false, false);
                                            row.DescriptionEntity[column.name] = { isDifferent: false };
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
                                                    if (selectedRow.DescriptionEntity[columnName].$type != undefined && selectedRow.DescriptionEntity[columnName].$type.includes("GeneratedScriptOverriddenData")) {
                                                        setRowAttributes(selectedRow.DescriptionEntity[columnName], false, false, true);
                                                        row.DescriptionEntity[columnName] = { isDifferent: $scope.scopeModel.includeOverriddenValues };
                                                    }
                                                    else if (selectedRow.DescriptionEntity[columnName].$type != undefined && selectedRow.DescriptionEntity[columnName].$type.includes("GeneratedScriptVariableData")) {
                                                        setRowAttributes(selectedRow.DescriptionEntity[columnName], false, true, false);
                                                        row.DescriptionEntity[columnName] = { isDifferent: false };
                                                    }
                                                    else {
                                                        setRowAttributes(selectedRow.DescriptionEntity[columnName], true, false, false);
                                                        row.DescriptionEntity[columnName] = { isDifferent: true };
                                                    }
                                                }
                                                else {
                                                    setRowAttributes(selectedRow.DescriptionEntity[columnName], false, false, false);
                                                    row.DescriptionEntity[columnName] = { isDifferent: false };
                                                }
                                            }
                                            else {
                                                if (row.FieldValues[columnName] != undefined) {
                                                    selectedRow.DescriptionEntity[columnName] = { value: "*", isDifferent: false, isOverridden: true, isVariable: false };
                                                    row.DescriptionEntity[columnName] = { isDifferent: false };
                                                }
                                                else {
                                                    selectedRow.DescriptionEntity[columnName] = { value: "*", isDifferent: false, isOverridden: false, isVariable: false };
                                                    row.DescriptionEntity[columnName] = { isDifferent: false };
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
                                variables: entity.isEditMode ? entity.Settings.Variables : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(variablesDataGridApi, variablesPayload, variablesGridLoadPromiseDeferred);
                        });
                        return variablesGridLoadPromiseDeferred.promise;
                    }

                    promises.push(loadVariablesGridDirective());
                    if (isEditMode) {
                        $scope.scopeModel.isIncludeAllInInsert = true;
                        $scope.scopeModel.isIncludeAllInUpdate = true;

                        $scope.scopeModel.IsIdentity = entity.Settings.IsIdentity;
                        $scope.scopeModel.sqlFilter = entity.Settings.LastWhereCondition;
                        $scope.scopeModel.sqlJoinStatement = entity.Settings.LastJoinStatement;
                        var columnsNames = [];
                        var allColumnsIncludedInInsert = true;
                        var allColumnsNotIncludedInInsert = true;
                        var allColumnsIncludedInUpdate = true;
                        var allColumnsNotIncludedInUpdate = true;

                        for (var k = 0; k < entity.Settings.Columns.length; k++) {
                            var data = entity.Settings.Columns[k];

                            if (!data.IncludeInInsert)
                                allColumnsIncludedInInsert = false;
                            else
                                allColumnsNotIncludedInInsert = false;

                            if (!data.IncludeInUpdate)
                                allColumnsIncludedInUpdate = false;
                            else
                                allColumnsNotIncludedInUpdate = false;

                            ctrl.datasource.push({ data: data });
                            columnsNames.push(data.ColumnName);
                        }

                        if (allColumnsIncludedInInsert)
                            $scope.scopeModel.isIncludeAllInInsert = true;

                        else if (allColumnsNotIncludedInInsert)
                            $scope.scopeModel.isIncludeAllInInsert = false;

                        if (allColumnsIncludedInUpdate)
                            $scope.scopeModel.isIncludeAllInUpdate = true;

                        else if (allColumnsNotIncludedInUpdate)
                            $scope.scopeModel.isIncludeAllInUpdate = false;

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

                        return UtilsService.waitPromiseNode({
                            promises: promises,
                            getChildNode: function () {
                                tabsContainerApi.setTabSelected(1);
                                return { promises: [] };
                            }
                        });
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

                    var columns = [];
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            columns.push(ctrl.datasource[i].data);
                        }
                    }

                    var dataRows;

                    if (selectedTableDataGridApi != undefined) {
                        dataRows = selectedTableDataGridApi.getData().tableRows;
                    }

                    var variablesData = variablesDataGridApi.getData();

                    return {
                        $type: "Vanrise.DevTools.Business.MergeGeneratedScriptItem, Vanrise.DevTools.Business",
                        Columns: columns,
                        DataRows: dataRows,
                        Variables: (variablesData == undefined || variablesData.length == 0) ? null : variablesData,
                        LastWhereCondition: $scope.scopeModel.sqlFilter,
                        LastJoinStatement: $scope.scopeModel.sqlJoinStatement,
                        IsIdentity: $scope.scopeModel.IsIdentity,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);
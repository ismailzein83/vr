appControllers.directive("vrDevtoolsSelectedTableDataGrid", ["UtilsService", "VRNotificationService", "VR_Devtools_TableDataAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "VR_Devtools_GeneratedScriptService","LabelColorsEnum",
    function (UtilsService, VRNotificationService, VR_Devtools_TableDataAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService, VR_Devtools_GeneratedScriptService,LabelColorsEnum) {
        "use strict";

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selectedTableDataGrid = new SelectedTableDataGrid($scope, ctrl, $attrs);
                selectedTableDataGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_DevTools/Elements/GeneratedScript/Directives/VRGeneratedScript/Templates/VRSelectedTableDataGridTemplate.html"
        };

        function SelectedTableDataGrid($scope, ctrl) {

            var gridApi;
            var getVariables;
            var compareTables;
            var columnNames;
            var identifierColumns;
            var originalTableData = []; 
            var executeQuery; 
            var columns;
            var includeOverriddenValues;
            var includeVariables;

            $scope.scopeModel = {};
            $scope.scopeModel.selectedTableData = [];
            $scope.scopeModel.selectedTableDataGridDS = [];
            $scope.scopeModel.columnNames = [];

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                };

                $scope.scopeModel.getRowStyle = function (row) {
                    var rowStyle; 
                    if (row.rowExists == undefined || row.rowExists == false)
                        rowStyle = { CssClass: 'alert-danger'  };
                    return rowStyle;
                };
                $scope.scopeModel.validateTableSelection = function () {
                    if ($scope.scopeModel.selectedTableData.length == 0)
                            return 'You Should At Least Select One Row ';
                    return null;
                };

                $scope.getStatusColor = function (dataItem, column) {

                    if (dataItem.DescriptionEntity[column.name] != undefined) {
                        if (dataItem.DescriptionEntity[column.name].isDifferent)
                            return LabelColorsEnum.Processing.color;
                        if (dataItem.DescriptionEntity[column.name].isVariable)
                            return LabelColorsEnum.Error.color;
                        if (dataItem.DescriptionEntity[column.name].isOverridden)
                            return LabelColorsEnum.New.color;
                    }
                };

                $scope.scopeModel.loadMoreData = function () {
                    return loadMoreSelectedRows();
                };

                $scope.scopeModel.totalRows = function () {
                    return $scope.scopeModel.selectedTableData.length;
                };

                $scope.scopeModel.rowsHavingDifferences = 0;

                $scope.scopeModel.newRows = function () {
                    var newRowsCount = 0;
                    $scope.scopeModel.rowsHavingDifferences = 0;
                    for (var i = 0; i < $scope.scopeModel.selectedTableData.length; i++) {
                        var row = $scope.scopeModel.selectedTableData[i];
                        if (!row.rowExists)
                            newRowsCount++;
                        else {
                            for (var j = 0; j < columns.length; j++) {

                                var column = columns[j];
                                if (row.DescriptionEntity[column] != undefined && (row.DescriptionEntity[column].isDifferent || (row.DescriptionEntity[column].includeOverriddenValues && row.DescriptionEntity[column].isOverridden))) {
                                    $scope.scopeModel.rowsHavingDifferences++;
                                    break;
                                }
                            }
                        }
                    }
                    return newRowsCount;
                };

                function generateTargetRow(sourceRow, selectedTableRow) {
                    if (selectedTableRow == undefined)
                        selectedTableRow = {
                            Entity: {
                                FieldValues: {}
                            },
                            DescriptionEntity: {}
                        };

                    for (var j = 0; j < columns.length; j++) {
                        var columnName = columns[j];
                        if ((selectedTableRow.Entity.FieldValues[columnName] == undefined || selectedTableRow.Entity.FieldValues[columnName].$type == undefined) ||
                            (selectedTableRow.Entity.FieldValues[columnName] != undefined && selectedTableRow.Entity.FieldValues[columnName].$type != undefined && selectedTableRow.Entity.FieldValues[columnName].$type.includes("GeneratedScriptOverriddenData") && includeOverriddenValues) ||
                            (selectedTableRow.Entity.FieldValues[columnName] != undefined && selectedTableRow.Entity.FieldValues[columnName].$type != undefined && selectedTableRow.Entity.FieldValues[columnName].$type.includes("GeneratedScriptVariableData") && includeVariables)) {

                            selectedTableRow.Entity.FieldValues[columnName] = sourceRow.FieldValues[columnName];
                            selectedTableRow.DescriptionEntity[columnName] = {
                                value: sourceRow.FieldValues[columnName]
                            };
                        }
                    }
                    return selectedTableRow;
                }
              
                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                        getVariables = payload.context.getVariables;
                        compareTables = payload.context.compareTables;
                        originalTableData = payload.tableData;
                        columnNames = payload.ColumnNames;
                        executeQuery = payload.executeQuery;
                        var variables = getVariables();
                        var filteredColumnNames = payload.filteredColumnNames;
                        includeOverriddenValues = payload.includeOverriddenValues;
                        includeVariables = payload.includeVariables;
                        var moveItems = payload.moveItems;

                        $scope.scopeModel.columnNames = [];
                        if (columnNames != undefined)
                            for (var j = 0; j < columnNames.length; j++) {
                                $scope.scopeModel.columnNames.push(columnNames[j]);
                            }
                        columns = filteredColumnNames == undefined ? $scope.scopeModel.columnNames : filteredColumnNames;

                        if (payload.selectedDataRows != undefined) {
                            for (var i = 0; i < payload.selectedDataRows.length; i++) {
                                var dataRow = payload.selectedDataRows[i];
                                var dataRowDescription = {};
                                for (var j = 0; j < $scope.scopeModel.columnNames.length; j++) {
                                    var columnName = $scope.scopeModel.columnNames[j];
                                    dataRowDescription[columnName] = { value: dataRow.FieldValues[columnName] };
                                    if (dataRow.FieldValues[columnName] != undefined && typeof (dataRow.FieldValues[columnName]) == "object" && dataRow.FieldValues[columnName].IsVariable)
                                        dataRowDescription[columnName] = {
                                            value: '@' + UtilsService.getItemByVal(variables, dataRowDescription[columnName].value.VariableId, 'Id').Name,
                                            $type: dataRowDescription[columnName].value.$type
                                        };
                                    else if (dataRow.FieldValues[columnName] != undefined && typeof (dataRow.FieldValues[columnName]) == "object" && dataRow.FieldValues[columnName].IsOverridden)
                                        dataRowDescription[columnName] = {
                                            $type: dataRow.FieldValues[columnName].$type,
                                            value: dataRow.FieldValues[columnName].Value
                                        };
                                }

                                $scope.scopeModel.selectedTableData.push({
                                    Entity: payload.selectedDataRows[i],
                                    DescriptionEntity: dataRowDescription
                                });
                            }
                        }
                        if (payload.Query) {
                            var filter = payload.Query;
                            identifierColumns = filter.IdentifierColumns;

                            if (identifierColumns != undefined) {
                                for (var i = 0; i < identifierColumns.length; i++) {
                                    var identifierColumn = identifierColumns[i];
                                    if (!columns.includes(identifierColumn.ColumnName))
                                        columns.unshift(identifierColumn.ColumnName);
                                }
                            }
                            if (!executeQuery) {
                                if (moveItems) {
                                    if (filter.BulkActionFinalState.IsAllSelected) {
                                        $scope.scopeModel.selectedTableDataGridDS.length=0;
                                        for (var i = 0; i < originalTableData.length; i++) {
                                            var dataRow = originalTableData[i];
                                            var exists = false;
                                            for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                                                var selectedTableRow = $scope.scopeModel.selectedTableData[k];
                                                if (compareIdentifierKeys(dataRow, selectedTableRow)) {
                                                    $scope.scopeModel.selectedTableData[k] = generateTargetRow(dataRow, selectedTableRow);
                                                    exists = true; 
                                                    break;
                                                }
                                            }
                                            if (!exists) {
                                                $scope.scopeModel.selectedTableData.push(generateTargetRow(dataRow));
                                            }
                                        }
                                        loadMoreSelectedRows();
                                        loadPromiseDeferred.resolve();
                                    }

                                    else if (filter.BulkActionFinalState.TargetItems.length != 0) {
                                        return VR_Devtools_TableDataAPIService.GetSelectedTableData(filter).then(function (response) {
                                            if (response) {

                                                for (var i = 0; i < response.length; i++) {
                                                    var dataRow = response[i];
                                                    var exists = false;
                                                    for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                                                        var selectedTableRow = $scope.scopeModel.selectedTableData[k];
                                                        if (compareIdentifierKeys(dataRow, selectedTableRow)) {
                                                            exists = true; 
                                                            $scope.scopeModel.selectedTableData[k] = generateTargetRow(dataRow, selectedTableRow); 
                                                            break;
                                                        }
                                                    }
                                                    if (!exists) {
                                                        $scope.scopeModel.selectedTableData.push(generateTargetRow(dataRow));
                                                    }
                                                }
                                            }
                                            $scope.scopeModel.selectedTableDataGridDS.length = 0;
                                            loadMoreSelectedRows();
                                            loadPromiseDeferred.resolve();
                                        });
                                    }
                                }
                                else {
                                    $scope.scopeModel.selectedTableDataGridDS.length = 0;

                                    if ($scope.scopeModel.selectedTableData != undefined && $scope.scopeModel.selectedTableData.length > 0) {
                                        for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                                            var selectedTableRow = $scope.scopeModel.selectedTableData[k];
                                            var fieldValues = {};
                                            var fieldValuesDescription = {};

                                            for (var j = 0; j < columns.length; j++) {
                                                var columnName = columns[j];
                                                fieldValues[columnName] = selectedTableRow.Entity.FieldValues[columnName];
                                                fieldValuesDescription[columnName] = selectedTableRow.DescriptionEntity[columnName];
                                                if (fieldValues[columnName] == undefined) {
                                                    fieldValues[columnName] = null;
                                                    fieldValuesDescription[columnName] = {value:"*"};
                                                }
                                            }
                                            selectedTableRow.Entity.FieldValues = fieldValues;
                                            selectedTableRow.DescriptionEntity = fieldValuesDescription;

                                        }
                                    }
                                    loadMoreSelectedRows();
                                    loadPromiseDeferred.resolve();
                                };
                            }

                            else {
                                loadMoreSelectedRows();
                                loadPromiseDeferred.resolve()
                            };
                        }
                        else {
                            if ($scope.scopeModel.selectedTableData != undefined && $scope.scopeModel.selectedTableData.length > 0) {
                                for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                                    var selectedTableRow = $scope.scopeModel.selectedTableData[k];
                                    var fieldValues = {};
                                    var fieldValuesDescription = {};

                                    for (var j = 0; j < columns.length; j++) {
                                        var columnName = columns[j];
                                        fieldValues[columnName] = selectedTableRow.Entity.FieldValues[columnName];
                                        fieldValuesDescription[columnName] = selectedTableRow.DescriptionEntity[columnName];
                                        if (fieldValues[columnName] == undefined) {
                                            fieldValues[columnName] = null;
                                            fieldValuesDescription[columnName] = { value: "*" };
                                        }
                                    }
                                    selectedTableRow.Entity.FieldValues = fieldValues;
                                    selectedTableRow.DescriptionEntity = fieldValuesDescription;

                                }
                            }
                            loadMoreSelectedRows();
                            loadPromiseDeferred.resolve()
                        };
                        return loadPromiseDeferred.promise;
                    };

                    directiveApi.getData = function () {
                        var tableRows = [];
                        for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                            tableRows.push($scope.scopeModel.selectedTableData[k].Entity);
                        }

                        return {
                            tableRows: tableRows,
                            selectedTableData: $scope.scopeModel.selectedTableData,
                        };

                    };
                    return directiveApi;
                }

                function compareIdentifierKeys(sourceRow, targetRow) {
                    var identifierKey = "";
                    for (var k = 0; k < identifierColumns.length; k++) {
                        var idKey = identifierColumns[k].ColumnName;
                        identifierKey += targetRow.Entity.FieldValues[idKey] + "_";
                    }

                    var originalIdentifierKey = "";
                    for (var k = 0; k < identifierColumns.length; k++) {
                        var idKey = identifierColumns[k].ColumnName;
                        originalIdentifierKey += sourceRow.FieldValues[idKey] + "_";
                    } 

                    return (identifierKey == originalIdentifierKey);

                }

                function getOriginalCellValue(dataItem, column) {
                    if (originalTableData != undefined && originalTableData.length > 0) {
                        for (var j = 0; j < originalTableData.length; j++) {
                            var originalRow = originalTableData[j];

                            if (compareIdentifierKeys(originalRow, dataItem)) {
                                return originalRow.FieldValues[column.name];
                            }
                        }
                    }
                }

                $scope.scopeModel.deleteTableDataRow = function (row) {
                    var index = $scope.scopeModel.selectedTableData.indexOf(row);
                    if (index > -1) {
                        compareTables(index,row);
                        $scope.scopeModel.selectedTableDataGridDS.splice(index, 1); 
                        $scope.scopeModel.selectedTableData.splice(index, 1); 

                    }
                };

                $scope.scopeModel.editCell = function (dataItem, column) {
                    var variables = getVariables();
                    var index = $scope.scopeModel.selectedTableData.indexOf(dataItem);
                    var cellValue = $scope.scopeModel.selectedTableData[dataItem.rowIndex].Entity.FieldValues[column.name];

                    var modifySelectedTableData = function (newCellValue) {
                        $scope.scopeModel.selectedTableData[dataItem.rowIndex].Entity.FieldValues[column.name] = (newCellValue != undefined && newCellValue != null) ? newCellValue : "*";
                        if (newCellValue != undefined && typeof (newCellValue) == "object" && newCellValue.IsVariable) {
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].Entity.FieldValues[column.name] = newCellValue;
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].DescriptionEntity[column.name].$type = newCellValue.$type;
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].DescriptionEntity[column.name].value = '@' + UtilsService.getItemByVal(variables, newCellValue.VariableId, 'Id').Name;
                        }
                        else if (newCellValue != undefined && typeof (newCellValue) == "object" && newCellValue.IsOverridden) {
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].Entity.FieldValues[column.name] = newCellValue;
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].DescriptionEntity[column.name].$type = newCellValue.$type;
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].DescriptionEntity[column.name].value = (newCellValue != undefined && newCellValue.Value != undefined) ? newCellValue.Value : "*";

                        }
                        else {
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].DescriptionEntity[column.name].value = (newCellValue != undefined && newCellValue != null) ? newCellValue : "*";
                        }
                        compareTables(index, $scope.scopeModel.selectedTableData[dataItem.rowIndex],column);
                    };
                    VR_Devtools_GeneratedScriptService.editTableCell(modifySelectedTableData, cellValue, getVariables, getOriginalCellValue(dataItem, column), dataItem, column);
                };
            }

            function loadMoreSelectedRows() {

                var pageInfo = gridApi.getPageInfo();
                var itemsLength = pageInfo.toRow;
                if (pageInfo.toRow > $scope.scopeModel.selectedTableData.length) {
                    if (pageInfo.fromRow <= $scope.scopeModel.selectedTableData.length) { itemsLength = $scope.scopeModel.selectedTableData.length; }
                    else
                        return;
                }
                var items = [];
                for (var i = pageInfo.fromRow - 1; i < itemsLength; i++) {
                    items.push($scope.scopeModel.selectedTableData[i]);
                } 
                gridApi.addItemsToSource(items);
            }
        }

        return directiveDefinitionObject;

    }]);


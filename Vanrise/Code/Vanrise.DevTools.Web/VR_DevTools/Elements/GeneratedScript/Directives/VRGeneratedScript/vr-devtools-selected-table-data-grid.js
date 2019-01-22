appControllers.directive("vrDevtoolsSelectedTableDataGrid", ["UtilsService", "VRNotificationService", "VR_Devtools_TableDataAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "VR_Devtools_GeneratedScriptService",
    function (UtilsService, VRNotificationService, VR_Devtools_TableDataAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService, VR_Devtools_GeneratedScriptService) {
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
            $scope.scopeModel = {};
            $scope.scopeModel.selectedTableData = [];
            $scope.scopeModel.columnNames = [];
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                };
              
                
                $scope.scopeModel.validateTableSelection = function () {
                    if ($scope.scopeModel.selectedTableData.length == 0)
                            return 'You Should At Least Select One Row ';
                    return null;
                };
            
                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        if (payload != undefined) {
            
                            getVariables = payload.getVariables;
                            var variables = getVariables();
                            var columnNames = payload.ColumnNames;
                            $scope.scopeModel.columnNames = [];
                            if (columnNames!=undefined)
                            for (var j = 0; j < columnNames.length; j++) {
                                $scope.scopeModel.columnNames.push(columnNames[j]);
                                }

                            if (payload.DataRows != undefined) {
                                for (var i = 0; i < payload.DataRows.length; i++) {
                                    var dataRow = payload.DataRows[i];
                                    var dataRowDescription = {};
                                    for (var j = 0; j < $scope.scopeModel.columnNames.length; j++) {
                                        var columnName = $scope.scopeModel.columnNames[j];
                                        dataRowDescription[columnName] = dataRow.FieldValues[columnName];

                                        if (dataRow.FieldValues[columnName] !=undefined && typeof (dataRow.FieldValues[columnName]) == "object" && dataRow.FieldValues[columnName].IsVariable)
                                            dataRowDescription[columnName] = '@' + UtilsService.getItemByVal(variables, dataRowDescription[columnName].VariableId, 'Id').Name;

                                    }
                              
                                    $scope.scopeModel.selectedTableData.push({
                                        Entity: payload.DataRows[i],
                                        DescriptionEntity: dataRowDescription
                                    });
                                }
                            } 

                            if (payload.Query != undefined) {
                                var filter = payload.Query;
                                if (filter.BulkActionFinalState.TargetItems.length != 0 || filter.BulkActionFinalState.IsAllSelected == true) {
                                    return VR_Devtools_TableDataAPIService.GetSelectedTableData(filter).then(function (response) {
                                        if (response) {

                                            for (var i = 0; i < response.length; i++) {

                                                var exists = false;
                                                var responseIdentifierKey = "";
                                                for (var j = 0; j < filter.IdentifierColumns.length; j++) {
                                                    var key = filter.IdentifierColumns[j].ColumnName;
                                                    responseIdentifierKey += response[i].FieldValues[key] + "_";
                                                }
                                                var selectedDataRow = {};
                                                for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                                                    var identifierKey = "";
                                                    selectedDataRow = $scope.scopeModel.selectedTableData[k];
                                                    for (var l = 0; l < filter.IdentifierColumns.length; l++) {
                                                        var idKey = filter.IdentifierColumns[l].ColumnName;
                                                        identifierKey += $scope.scopeModel.selectedTableData[k].Entity.FieldValues[idKey] + "_";
                                                    }

                                                    if (responseIdentifierKey == identifierKey) {
                                                        exists = true; break;
                                                    }

                                                }
                                                if(!exists) {
                                                    var dataRow = response[i];
                                                    var dataRowDescription = {};
                                                    for (var j = 0; j < $scope.scopeModel.columnNames.length; j++) {
                                                        var columnName = $scope.scopeModel.columnNames[j];
                                                        dataRowDescription[columnName] = dataRow.FieldValues[columnName];

                                                        if (dataRow.FieldValues[columnName]!=undefined && typeof (dataRow.FieldValues[columnName]) == "object" && dataRow.FieldValues[columnName].IsVariable)
                                                            dataRowDescription[columnName] = '@' + UtilsService.getItemByVal(variables, dataRowDescription[columnName].VariableId, 'Id').Name;
                                                    }
                                                    $scope.scopeModel.selectedTableData.push({
                                                        Entity: response[i],
                                                        DescriptionEntity: dataRowDescription
                                                    });
                                                }

                                            }
                                            for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                                                if (exists) {
                                                    var dataRow = $scope.scopeModel.selectedTableData[k].Entity;
                                                    var dataRowDescription = $scope.scopeModel.selectedTableData[k].DescriptionEntity;
                                                    for (var j = 0; j < $scope.scopeModel.columnNames.length; j++) {
                                                        var columnName = $scope.scopeModel.columnNames[j];
                                                        if (dataRowDescription[columnName] == undefined) {
                                                            dataRowDescription[columnName] = dataRow.FieldValues[columnName];

                                                            if (dataRowDescription[columnName] !=undefined && typeof (dataRowDescription[columnName]) == "object" && dataRowDescription[columnName].IsVariable)
                                                                dataRowDescription[columnName] = '@' + UtilsService.getItemByVal(variables, dataRowDescription[columnName].VariableId, 'Id').Name;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    });
                                }
                            }
                        }
                        else {
                            $scope.scopeModel.selectedTableData = [];
                        }
                    };

                    directiveApi.getData = function () {
                        var tableRows = [];
                        for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                            tableRows.push($scope.scopeModel.selectedTableData[k].Entity);
                        } 
                        return { tableRows: tableRows };

                    };
                    return directiveApi;
                }

                $scope.scopeModel.deleteTableDataRow = function (row) {
                    var index = $scope.scopeModel.selectedTableData.indexOf(row);
                    if (index > -1) {
                        $scope.scopeModel.selectedTableData.splice(index, 1);
                    }
                };

                $scope.scopeModel.editCell = function (dataItem, column) {
                    var variables = getVariables();
                    var cellValue = $scope.scopeModel.selectedTableData[dataItem.rowIndex].Entity.FieldValues[column.name];

                    var modifySelectedTableData = function (newCellValue) {
                        $scope.scopeModel.selectedTableData[dataItem.rowIndex].Entity.FieldValues[column.name] = newCellValue;
                        if (newCellValue !=undefined && typeof (newCellValue) == "object" && newCellValue.IsVariable)
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].DescriptionEntity[column.name] = '@' + UtilsService.getItemByVal(variables, newCellValue.VariableId, 'Id').Name;
                        else
                            $scope.scopeModel.selectedTableData[dataItem.rowIndex].DescriptionEntity[column.name] = newCellValue;
                    }; 
                    VR_Devtools_GeneratedScriptService.editTableCell(modifySelectedTableData, cellValue, getVariables);

                };
            }
        }

        return directiveDefinitionObject;

    }]);


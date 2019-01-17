appControllers.directive("vanriseToolsSelectedTableDataGrid", ["UtilsService", "VRNotificationService", "VR_Tools_TableDataAPIService", "VR_Tools_ColumnsAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService","VR_Tools_GeneratedScriptService",
function (UtilsService, VRNotificationService, VR_Tools_TableDataAPIService, VR_Tools_ColumnsAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService, VR_Tools_GeneratedScriptService) {
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
        templateUrl: "/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Directives/GeneratedScript/Design/Templates/SelectedTableDataGridTemplate.html"
    };

    function SelectedTableDataGrid($scope, ctrl) {
    
        var gridApi;
        $scope.scopeModel = {};
        $scope.scopeModel.selectedTableData = [];
        $scope.scopeModel.columnNames = [];
        var name;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }


            };

            function getDirectiveApi() {
                var directiveApi = {};

                directiveApi.load = function (payload) {
                    if (payload != undefined) {

                        if (payload.DataRows != undefined) {
                            for (var i = 0; i < payload.DataRows.length; i++) {
                                $scope.scopeModel.selectedTableData.push({ Entity: payload.DataRows[i] });
                            }
                        }

                        var columnNames = payload.ColumnNames;
                        $scope.scopeModel.columnNames = [];

                        for (var j = 0; j < columnNames.length; j++) {
                            $scope.scopeModel.columnNames.push(columnNames[j]);
                        }

                        if (payload.Query != undefined) {
                            var filter = payload.Query;
                            if (filter.BulkActionFinalState.TargetItems.length != 0 || filter.BulkActionFinalState.IsAllSelected == true) {
                                return VR_Tools_TableDataAPIService.GetSelectedTableData(filter).then(function (response) {
                                    if (response) {

                                        for (var i = 0; i < response.length; i++) {

                                            var exists = false;
                                            var responseIdentifierKey = "";
                                            for (var j = 0; j < filter.IdentifierColumns.length; j++) {
                                                var key = filter.IdentifierColumns[j].ColumnName;
                                                responseIdentifierKey += response[i].FieldValues[key] + "_";
                                            }

                                            for (var k = 0; k < $scope.scopeModel.selectedTableData.length; k++) {
                                                var identifierKey = "";

                                                for (var l = 0; l < filter.IdentifierColumns.length; l++) {
                                                    var idKey = filter.IdentifierColumns[l].ColumnName;
                                                    identifierKey += $scope.scopeModel.selectedTableData[k].Entity.FieldValues[idKey] + "_";
                                                }

                                                if (responseIdentifierKey == identifierKey) {
                                                    exists = true; break;
                                                }

                                            }
                                            if (exists == false) {
                                                $scope.scopeModel.selectedTableData.push({ Entity: response[i] });
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
                VR_Tools_GeneratedScriptService.editTableCell(dataItem.rowIndex, column.name, $scope.scopeModel.selectedTableData);

            };
        }
    }

    return directiveDefinitionObject;

}]);


"use strict";
app.directive("vrAnalyticAdvancedexcelFilegenerator", ["UtilsService", "VRAnalytic_AdvancedExcelFileGeneratorService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRAnalytic_AdvancedExcelFileGeneratorService, VRNotificationService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var advancedExcel = new AdvancedExcel($scope, ctrl, $attrs);
            advancedExcel.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorTemplate.html"
    };


    function AdvancedExcel($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.columns = [];

                $scope.scopeModel.removeColumn = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.id, 'id');
                    if (index > -1) {
                        $scope.scopeModel.columns.splice(index, 1);
                    }
                };

                $scope.scopeModel.addTableDefinition = addTableDefinition;

                function addTableDefinition() {
                    var onTableDefinitionAdded = function (obj) {
                        $scope.scopeModel.columns.push(obj);
                    };
                    VRAnalytic_AdvancedExcelFileGeneratorService.addTableDefinition(onTableDefinitionAdded);
                }

                $scope.scopeModel.validateColumns = function () {
                    if ($scope.scopeModel.columns.length == 0) {
                        return 'Please, one record must be added at least.';
                    }
                    var columnNames = [];
                    for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                        if ($scope.scopeModel.columns[i].QueryName != undefined) {
                            columnNames.push($scope.scopeModel.columns[i].QueryName.toUpperCase());
                        }
                    }
                    while (columnNames.length > 0) {
                        var nameToValidate = columnNames[0];
                        columnNames.splice(0, 1);
                        if (!validateName(nameToValidate, columnNames)) {
                            return 'Two or more columns have the same Name';
                        }
                    }
                    return null;
                    function validateName(name, array) {
                        for (var j = 0; j < array.length; j++) {
                            if (array[j] == name)
                                return false;
                        }
                        return true;
                    }
                };

                defineMenuActions();
                defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                  var tableDefinitions = payload.TableDefinitions;

                  if (tableDefinitions != undefined) {
                      for (var i = 0; i < tableDefinitions.length; i++) {
                            var gridItem = {
                                id: i,
                                QueryName: tableDefinitions[i].QueryName,
                                ListName: tableDefinitions[i].ListName,
                                SheetName: tableDefinitions[i].SheetName,
                                RowIndex: tableDefinitions[i].RowIndex,
                            };
                            $scope.scopeModel.columns.push(gridItem);
                        }
                    }
                }
            };

            api.getData = function () {

                return {
                   $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGenerator,Vanrise.Analytic.MainExtensions",
                   TableDefinitions: $scope.scopeModel.columns.length > 0 ? getColumns() : null,
                };

                function getColumns() {
                    var columns = [];
                    for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                        var column = $scope.scopeModel.columns[i];
                        columns.push({
                            QueryName: column.QueryName,
                            ListName: column.ListName,
                            SheetName: column.SheetName,
                            RowIndex: column.RowIndex
                            //list
                        });
                    }
                    return columns;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editTableDefinition,
            }];
        }

        function editTableDefinition(object) {

            var onTableDefinitionUpdated = function (obj) {
                var index = $scope.scopeModel.columns.indexOf(object);
                $scope.scopeModel.columns[index] = obj;
            };
            VRAnalytic_AdvancedExcelFileGeneratorService.editTableDefinition(object, onTableDefinitionUpdated);
        }
    }

    return directiveDefinitionObject;
}
]);
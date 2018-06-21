"use strict";

app.directive("vrAnalyticAdvancedexcelFilegeneratorMappedtableGrid", ["VRUIUtilsService", "UtilsService", "VRNotificationService", 
function (VRUIUtilsService, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: { 
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new AdvancedExcelMappedTableGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorMappedTableGridTemplate.html'

    };

    function AdvancedExcelMappedTableGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var context;

        var fieldSelectorAPI;

        var selectedQueryId;

        var contextPromiseDeferred = UtilsService.createPromiseDeferred();


        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {
        };

            $scope.scopeModel.mappedCols =[];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {
                };
                    directiveAPI.load = function (query) {
                        contextPromiseDeferred.resolve();
                        context = query.context;
                        return loadMappedColumns(query.mappedSheet);
                };

                    directiveAPI.getData = function () {
                        return getMappedTable();
                };


                    directiveAPI.addMappedCol = function () {
                        var mappedCol = getMappedCol();
                        $scope.scopeModel.mappedCols.push(mappedCol);
                };


                    return directiveAPI;
            }

                contextPromiseDeferred.promise.then(function () {
                    if (context.getSelectedQuery != undefined && typeof (context.getSelectedQuery) == "function") {
                        selectedQueryId = context.getSelectedQuery().value;
                }
            });

        };

            $scope.scopeModel.removeMappedCol = function (dataItem) {
                var index = $scope.scopeModel.mappedCols.indexOf(dataItem);
                $scope.scopeModel.mappedCols.splice(index, 1);
        };

            $scope.scopeModel.addAllFields = function () {
                if (context != undefined && context.getQueryFields != undefined && typeof (context.getQueryFields) == "function") {
                    var fieldsPromise = context.getQueryFields(selectedQueryId);
                    var fieldsArray =[];
                    fieldsPromise.then(function (fields) {
                        if (fields != undefined) {
                            for (var fieldName in fields) {
                                fieldsArray.push({
                                        description: fields[fieldName],
                                    value: fieldName
                            });
                        }
                            for (var i = 0; i < fieldsArray.length; i++) {
                                var mappedCol = getMappedCol(undefined, undefined, undefined, fieldsArray[i]);
                                $scope.scopeModel.mappedCols.push(mappedCol);
                        }
                    }
                });
            }


        };

            $scope.scopeModel.validateGrid = function () {
                if ($scope.scopeModel.mappedCols.length == 0) {
                    return 'At least one record must be added.';
            }
                var titles =[];
                var firstRowDirectives =[];
                for (var i = 0; i < $scope.scopeModel.mappedCols.length; i++) {
                    if ($scope.scopeModel.mappedCols[i].directiveAPI != undefined && $scope.scopeModel.mappedCols[i].directiveAPI.getData() != undefined) {
                        firstRowDirectives.push($scope.scopeModel.mappedCols[i].directiveAPI.getData().CellIndex);
                }
            }
                while (firstRowDirectives.length > 0) {
                    var indexToValidate = firstRowDirectives[0];
                    firstRowDirectives.splice(0, 1);
                    if (!validateIndex(indexToValidate, firstRowDirectives)) {
                        return 'Two or more columns have the same column index.';
                }

            }

                return null;
                function validateIndex(index, array) {
                    for (var j = 0; j < array.length; j++) {
                        if (array[j]== index)
                            return false;
                }
                    return true;
            }
        };

        }

        function loadMappedColumns(mappedSheet) {
            var promises = [];
            if (mappedSheet != undefined && mappedSheet.ColumnDefinitions != undefined) {
                for (var i = 0; i < mappedSheet.ColumnDefinitions.length; i++) {
                    var mappedCol = getMappedCol(mappedSheet.ColumnDefinitions[i], mappedSheet.SheetIndex, mappedSheet.RowIndex);
                    promises.push(mappedCol.directiveLoadDeferred.promise);
                    $scope.scopeModel.mappedCols.push(mappedCol);
                }
            }
            return UtilsService.waitMultiplePromises(promises);
        }



        function getMappedCol(mappedColumn, sheetIndex, firstRowIndex, selectedField) {
            if (mappedColumn != undefined) {
                $scope.scopeModel.isLoadingMappedCol = true;
            }

            var mappedCol = {};

            mappedCol.directiveLoadDeferred = UtilsService.createPromiseDeferred();

            mappedCol.onDirectiveReady = function (api) {
                mappedCol.directiveAPI = api;
                var directivePayload = {
                    context: getCellFieldMappingContext(),
                    showEditButton: false
                };
                if (mappedColumn != undefined) {
                    directivePayload.fieldMapping = {
                        SheetIndex: sheetIndex,
                        RowIndex: firstRowIndex,
                        CellIndex: mappedColumn.ColumnIndex
                    };
                }
                VRUIUtilsService.callDirectiveLoad(mappedCol.directiveAPI, directivePayload, mappedCol.directiveLoadDeferred);
            };

            mappedCol.fieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            mappedCol.onFieldSelectorReady = function (api) {
                mappedCol.fields = [];
                mappedCol.fieldSelectorAPI = api;
                    var fieldSelectorReadyPromiseDeferred = context.getQueryFields(selectedQueryId);
                    fieldSelectorReadyPromiseDeferred.then(function (fields) {
                        if (fields != undefined) {
                            for (var fieldName in fields) {
                                mappedCol.fields.push({
                                    description: fields[fieldName],
                                    value: fieldName
                                });
                            }
                            mappedCol.fieldSelectorLoadPromiseDeferred.resolve();
                        }
                        if (mappedColumn != undefined) {
                            mappedCol.selectedField = UtilsService.getItemByVal(mappedCol.fields, mappedColumn.FieldName, "value");
                        }
                        if (selectedField != undefined) {
                            mappedCol.selectedField = selectedField;
                        }
                    });
            };

            
            mappedCol.editedTitle = mappedColumn != undefined ? mappedColumn.FieldTitle : undefined;

            mappedCol.onFieldSelectionChanged = function (value) {
                if (value != undefined) {
                    mappedCol.editedTitle = value.description;
                }
            };

            UtilsService.waitMultiplePromises([mappedCol.fieldSelectorLoadPromiseDeferred.promise]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoadingMappedCol = false;
            });

            return mappedCol;
        };


        function getCellFieldMappingContext() {

            function selectCellAtSheet(rowIndex, columnIndex, sheetIndex) {
                var rowIndexAsInt = parseInt(rowIndex);
                var columnIndexAsInt = parseInt(columnIndex);
                if (context.getSelectedSheetApi() != undefined)
                    context.selectCellAtSheet(rowIndex, columnIndex, sheetIndex);
            }
            function getSelectedCell() {
                var selectedSheetAPI = context.getSelectedSheetApi();
                if (selectedSheetAPI != undefined)
                    return selectedSheetAPI.getSelected();
            }
            function getSelectedSheet() {
                return context.getSelectedSheet();
            }
            function getFirstRowIndex() {

                var firstRowDirectiveData = context.getFirstRowData();
                if (firstRowDirectiveData != undefined) {
                    return {
                        sheet: firstRowDirectiveData.SheetIndex,
                        row: firstRowDirectiveData.RowIndex
                    };
                }
            }

            return {
                setSelectedCell: selectCellAtSheet,
                getSelectedCell: getSelectedCell,
                getSelectedSheet: getSelectedSheet,
                getFirstRowIndex: getFirstRowIndex
            };
        }


        function getMappedTable() {

            if ($scope.scopeModel.mappedCols.length == 0)
                return null;

            var mappedColumns = [];

            for (var i = 0; i < $scope.scopeModel.mappedCols.length; i++) {

                var mappedCol = $scope.scopeModel.mappedCols[i];
                var mappedColumn = {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorTableColumnDefinition, Vanrise.Analytic.MainExtensions"
                };

                var directiveData = mappedCol.directiveAPI.getData();
                if (directiveData != undefined) {
                    mappedColumn.ColumnIndex = directiveData.CellIndex;
                }
                mappedColumn.FieldTitle = mappedCol.editedTitle;
                mappedColumn.FieldName = mappedCol.selectedField.value;

                mappedColumns.push(mappedColumn);
            }

            return mappedColumns;
        }
    }

    return directiveDefinitionObject;

}]);

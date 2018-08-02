"use strict";

app.directive("vrAnalyticAdvancedexcelFilegeneratorMappedtableGrid", ["VRUIUtilsService", "UtilsService", "VRNotificationService", "VR_Analytic_AutomatedReportQuerySourceEnum",
function (VRUIUtilsService, UtilsService, VRNotificationService, VR_Analytic_AutomatedReportQuerySourceEnum) {

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

        var selectedQueryId;

        var contextPromiseDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {
            };

            $scope.scopeModel.mappedCols = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    
                    directiveAPI.load = function (query) {
                        contextPromiseDeferred.resolve(query.mappedSheet);
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

                contextPromiseDeferred.promise.then(function (mappedSheet) {
                    if (context.getSelectedQueryId != undefined && typeof (context.getSelectedQueryId) == "function" ) {
                        selectedQueryId = mappedSheet!=undefined ? mappedSheet.VRAutomatedReportQueryId : context.getSelectedQueryId();
                    }
                });

            };

            $scope.scopeModel.removeMappedCol = function (dataItem) {
                var index = $scope.scopeModel.mappedCols.indexOf(dataItem);
                $scope.scopeModel.mappedCols.splice(index, 1);
            };

            $scope.scopeModel.addAllFields = function () {
                if (context != undefined && context.getQueryFields != undefined && typeof (context.getQueryFields) == "function") {
                    var fieldsPromise = getAllFields(selectedQueryId);
                    fieldsPromise.then(function (fields) {
                        if (fields != undefined) {
                            for (var i = 0; i < fields.length; i++) {
                                var mappedCol = getMappedCol(undefined, undefined, undefined, fields[i]);
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
                var titles = [];
                var firstRowDirectives = [];
                for (var i = 0; i < $scope.scopeModel.mappedCols.length; i++) {
                    var mappedCol = $scope.scopeModel.mappedCols[i];
                    if (mappedCol.directiveAPI != undefined && mappedCol.directiveAPI.getData() != undefined) {
                        firstRowDirectives.push(mappedCol.directiveAPI.getData().CellIndex);
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
                        if (array[j] == index)
                            return false;
                    }
                    return true;
                }
            };

        }

        function loadMappedColumns(mappedSheet) {
            var promises = [];
            if (mappedSheet != undefined) {
                if (mappedSheet.ColumnDefinitions != undefined) {
                    for (var i = 0; i < mappedSheet.ColumnDefinitions.length; i++) {
                        var mappedCol = getMappedCol(mappedSheet.ColumnDefinitions[i], mappedSheet.SheetIndex, mappedSheet.RowIndex);
                        promises.push(mappedCol.directiveLoadDeferred.promise);
                        promises.push(mappedCol.fieldSelectorLoadPromiseDeferred.promise);
                        $scope.scopeModel.mappedCols.push(mappedCol);
                    }
                }
                if (mappedSheet.SubTableDefinitions != undefined) {
                    for (var i = 0; i < mappedSheet.SubTableDefinitions.length; i++) {
                        var mappedSubTable = getMappedCol(mappedSheet.SubTableDefinitions[i], mappedSheet.SheetIndex, mappedSheet.RowIndex);
                        promises.push(mappedSubTable.directiveLoadDeferred.promise);
                        promises.push(mappedSubTable.fieldSelectorLoadPromiseDeferred.promise);
                        $scope.scopeModel.mappedCols.push(mappedSubTable);
                    }
                }
            }
            return UtilsService.waitMultiplePromises(promises);
        }



        function getMappedCol(mappedColumn, sheetIndex, firstRowIndex, selectedField) {
            var allFields;

            var mappedCol = {};

            mappedCol.onFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            mappedCol.onFirstRowDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            mappedCol.onSubTableFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            mappedCol.fieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            mappedCol.directiveLoadDeferred = UtilsService.createPromiseDeferred();

            mappedCol.fields = [];
            mappedCol.onFieldSelectorReady = function (api) {
                mappedCol.fieldSelectorAPI = api;
                mappedCol.onFieldSelectorReadyDeferred.resolve();
                
                mappedCol.onFieldSelectorReadyDeferred.promise.then(function () {
                    var fieldSelectorReadyPromiseDeferred = getAllFields(selectedQueryId);
                    fieldSelectorReadyPromiseDeferred.then(function (fields) {
                        if (fields != undefined) {
                            allFields = fields;
                            mappedCol.fields = fields;
                            mappedCol.fieldSelectorLoadPromiseDeferred.resolve();
                        }
                        if (mappedColumn != undefined) {
                            if (mappedColumn.SubTableId != undefined) {
                                mappedCol.selectedField = UtilsService.getItemByVal(mappedCol.fields, mappedColumn.SubTableId, "value");
                            }
                            else if (mappedColumn.FieldName != undefined) {
                                mappedCol.selectedField = UtilsService.getItemByVal(mappedCol.fields, mappedColumn.FieldName, "value");
                            }
                        }
                        if (selectedField != undefined) {
                            mappedCol.selectedField = selectedField;
                        }
                        mappedCol.fieldSelectorAPI.selectIfSingleItem();
                    });
                });
            };

            mappedCol.subTableFields = [];
            mappedCol.onSubTableFieldSelectorReady = function (api) {
               mappedCol.subTableFieldSelectorAPI = api;
               mappedCol.onSubTableFieldSelectorReadyDeferred.resolve();
               if (mappedColumn != undefined && mappedColumn.SubTableFields != undefined) {
                    for (var i = 0; i < mappedColumn.SubTableFields.length; i++) {
                        var field = mappedColumn.SubTableFields[i];
                        mappedCol.selectedSubTableFields = UtilsService.getItemByVal(mappedCol.subTableFields, field.FieldName, "value");
                    }
                }
            };

            mappedCol.onDirectiveReady = function (api) {
                mappedCol.directiveAPI = api;
                mappedCol.onFirstRowDirectiveReadyDeferred.resolve();
                mappedCol.onFirstRowDirectiveReadyDeferred.promise.then(function () {
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
                });
            };

            

            mappedCol.onFieldSelectionChanged = function (value) {
                if(value!=undefined)
                {
                    if (value.source == VR_Analytic_AutomatedReportQuerySourceEnum.MainTable)
                    {
                        if (mappedColumn != undefined) {
                            mappedCol.editedTitle = mappedColumn.FieldTitle;
                            mappedColumn = undefined;
                        }
                        else {
                            mappedCol.editedTitle = value.description;
                        }
                    }
                    else
                    {
                        loadSubTableSelector();
                        mappedColumn = undefined;
                    }
                }

            };

           mappedCol.subTableTitle = mappedColumn != undefined ? mappedColumn.SubTableTitle : undefined;

            function loadSubTableSelector() {
                mappedCol.onSubTableFieldSelectorReadyDeferred.promise.then(function () {
                    if (mappedCol.selectedField != undefined && mappedCol.selectedField.source == VR_Analytic_AutomatedReportQuerySourceEnum.SubTable) {
                        mappedCol.subTableFields.length = 0;
                        if(allFields!=undefined){
                               for (var i = 0; i < allFields.length; i++) {
                            var field = allFields[i];
                            if (field.subTableFields != undefined && field.value == mappedCol.selectedField.value) {
                                for (var subTableFieldName in field.subTableFields) {
                                    if (subTableFieldName != "$type") {
                                        var matchingField = field.subTableFields[subTableFieldName].Field;
                                        mappedCol.subTableFields.push({
                                                description: matchingField.Title,
                                                value: matchingField.Name,
                                                source: VR_Analytic_AutomatedReportQuerySourceEnum.SubTable
                                    });
                                    }
                                }
                               }
                            }
                        }
                        mappedCol.subTableFieldSelectorAPI.selectIfSingleItem();
                    }
                });
            }

            return mappedCol;
        }

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

            var mappedTable = {};
            mappedTable.mappedColumns = [];
            mappedTable.mappedSubTables = [];

            for (var i = 0; i < $scope.scopeModel.mappedCols.length; i++) {

                var mappedCol = $scope.scopeModel.mappedCols[i];
                if (mappedCol.editedTitle != undefined) {
                    var mappedColumn = {
                        $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorTableColumnDefinition, Vanrise.Analytic.MainExtensions"
                    };
                    var directiveData = mappedCol.directiveAPI.getData();
                    if (directiveData != undefined) {
                        mappedColumn.ColumnIndex = directiveData.CellIndex;
                    }
                    mappedColumn.FieldName = mappedCol.selectedField.value;
                    mappedColumn.FieldTitle = mappedCol.editedTitle;
                    mappedTable.mappedColumns.push(mappedColumn);
                    continue;
                }
                else {
                    var mappedSubTable = {
                        $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorSubTableDefinition, Vanrise.Analytic.MainExtensions"
                    };
                    var directiveData = mappedCol.directiveAPI.getData();
                    if (directiveData != undefined) {
                        mappedSubTable.ColumnIndex = directiveData.CellIndex;
                    }
                    if (mappedCol.selectedSubTableFields != undefined) {
                        mappedSubTable.SubTableFields = [];
                        mappedSubTable.SubTableFields.push({
                            $type : "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorSubTableColumnDefinition, Vanrise.Analytic.MainExtensions",
                            FieldName: mappedCol.selectedSubTableFields.value,
                            FileTitle: mappedCol.selectedSubTableFields.description
                        });
                    }
                    mappedSubTable.SubTableName = mappedCol.selectedField.description;
                    mappedSubTable.SubTableTitle = mappedCol.subTableTitle;
                    mappedSubTable.SubTableId = mappedCol.selectedField.value;
                    mappedTable.mappedSubTables.push(mappedSubTable);
                }
            }

            return mappedTable;
        }

        function getAllFields(selectedQueryId) {
            var fieldsPromise = context.getQueryFields(selectedQueryId);
            var fieldsArrayPromise = UtilsService.createPromiseDeferred();
            var fieldsArray = [];
            fieldsPromise.then(function (fields) {
                if (fields != undefined) {
                    for (var i = 0; i < fields.length; i++) {
                        var field = fields[i];
                        if (field.Source == VR_Analytic_AutomatedReportQuerySourceEnum.MainTable) {
                            fieldsArray.push({
                                description: field.FieldTitle,
                                value: field.FieldName,
                                source: field.Source
                            });
                        }
                        else {
                            fieldsArray.push({
                                description: field.FieldName,
                                value: field.SubTableId,
                                source: field.Source,
                                subTableFields: field.SubTableFields
                            });
                        }
                    }
                    fieldsArrayPromise.resolve(fieldsArray);
                }
            });
            return fieldsArrayPromise.promise;
        }
    }

    return directiveDefinitionObject;

}]);

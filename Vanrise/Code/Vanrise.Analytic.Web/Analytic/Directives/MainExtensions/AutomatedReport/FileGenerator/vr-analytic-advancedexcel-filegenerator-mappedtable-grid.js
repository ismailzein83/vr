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

        var fieldSelectorAPI;

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
                                        value: field.FieldName,
                                        source: field.Source,
                                        subTableId: field.SubTableId
                                    });
                                }
                            }
                            for (var i = 0; i < fieldsArray.length; i++) {
                                var field = fieldsArray[i];
                                if (field.source == VR_Analytic_AutomatedReportQuerySourceEnum.MainTable) {
                                    var mappedCol = getMappedCol(undefined, undefined, undefined, fieldsArray[i]);
                                    $scope.scopeModel.mappedCols.push(mappedCol);
                                }
                                else {
                                    var mappedTable = getMappedTable(undefined, undefined, undefined, fieldsArray[i]);
                                    $scope.scopeModel.mappedCols.push(mappedTable);
                                }
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
                        promises.push(mappedSubTable.subTableFieldSelectorLoadPromiseDeferred.promise);
                        $scope.scopeModel.mappedCols.push(mappedSubTable);
                    }
                }
            }
            return UtilsService.waitMultiplePromises(promises);
        }



        function getMappedCol(mappedColumn, sheetIndex, firstRowIndex, selectedField) {
            var allFields;
            if (mappedColumn != undefined) {
                $scope.scopeModel.isLoadingMappedCol = true;
            }

            var mappedCol = {};
            function loadFirstRowDirective() {
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
                return mappedCol.directiveLoadDeferred.promise;
            }
        
            function loadFieldSelector() {
                mappedCol.fieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                mappedCol.onFieldSelectorReady = function (api) {
                    mappedCol.fields = [];
                    mappedCol.fieldSelectorAPI = api;
                    var fieldSelectorReadyPromiseDeferred = context.getQueryFields(selectedQueryId);
                    fieldSelectorReadyPromiseDeferred.then(function (fields) {
                        if (fields != undefined) {
                            allFields = fields;
                            for (var i = 0; i < fields.length; i++) {
                                var field = fields[i];
                                if (field.Source == VR_Analytic_AutomatedReportQuerySourceEnum.MainTable) {
                                    mappedCol.fields.push({
                                        description: field.FieldTitle,
                                        value: field.FieldName,
                                        source: field.Source
                                    });
                                }
                                else {
                                    mappedCol.fields.push({
                                        description: field.FieldName,
                                        value: field.FieldName,
                                        source: field.Source,
                                        subTableId: field.SubTableId
                                    });
                                }
                            }
                            mappedCol.fieldSelectorLoadPromiseDeferred.resolve();
                        }
                        if (mappedColumn != undefined) {
                            if(mappedColumn.SubTableName!=undefined){
                                mappedCol.selectedField = UtilsService.getItemByVal(mappedCol.fields, mappedColumn.SubTableName, "value");
                            }
                            else if (mappedColumn.FieldName != undefined) {
                                mappedCol.selectedField = UtilsService.getItemByVal(mappedCol.fields, mappedColumn.FieldName, "value");
                            }
                        }
                        if (selectedField != undefined) {
                            mappedCol.selectedField = selectedField;
                        }
                    });
                };
                return mappedCol.fieldSelectorLoadPromiseDeferred.promise;
            }

            function loadSubTableFields() {
                mappedCol.subTableFields = [];
                mappedCol.selectedSubTableFields = [];

                mappedCol.subTableFieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                mappedCol.onSubTableFieldSelectorReady = function (api) {
                    mappedCol.subTableFieldSelectorAPI = api;
                    if (mappedCol.selectedField != undefined && mappedCol.selectedField.source == VR_Analytic_AutomatedReportQuerySourceEnum.SubTable) {
                        for (var i = 0; i < allFields.length; i++) {
                            var field = allFields[i];
                            if (field.FieldName == mappedCol.selectedField.value) {
                                var subTableFields = field.SubTableFields;
                                for (var subTableFieldName in subTableFields) {
                                    if (subTableFieldName != "$type") {
                                        var field = subTableFields[subTableFieldName].Field;
                                        mappedCol.subTableFields.push({
                                            description: field.Title,
                                            value: field.Name,
                                            source: VR_Analytic_AutomatedReportQuerySourceEnum.SubTable
                                        });
                                    }
                                }
                            }
                        }
                        if (mappedColumn != undefined && mappedColumn.SubTableFields != undefined) {
                            for (var i = 0; i < mappedColumn.SubTableFields.length; i++) {
                                var field = mappedColumn.SubTableFields[i];
                                var fieldInfo = UtilsService.getItemByVal(mappedCol.subTableFields, field.FieldName, "value");
                                if (fieldInfo != null) {
                                    mappedCol.selectedSubTableFields.push({
                                        description: fieldInfo.description,
                                        value: fieldInfo.value,
                                        source: fieldInfo.source
                                    });
                                }
                            }
                        }
                    }
                };
                return mappedCol.subTableFieldSelectorLoadPromiseDeferred.promise;
            }


            mappedCol.editedTitle = mappedColumn != undefined ? mappedColumn.FieldTitle : undefined;

            mappedCol.onFieldSelectionChanged = function (value) {
                if (value != undefined && value.source == VR_Analytic_AutomatedReportQuerySourceEnum.MainTable) {
                    mappedCol.editedTitle = value.description;
                }
            };


            UtilsService.waitMultiplePromises([loadFieldSelector(), loadFirstRowDirective(), loadSubTableFields()]).finally(function () {
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

            var mappedTable = {};
            mappedTable.mappedColumns = [];
     
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
                    mappedTable.mappedSubTables = [];
                    var mappedSubTable = {
                        $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorSubTableDefinition, Vanrise.Analytic.MainExtensions"
                    };
                    var directiveData = mappedCol.directiveAPI.getData();
                    if (directiveData != undefined) {
                        mappedSubTable.ColumnIndex = directiveData.CellIndex;
                    }
                    if (mappedCol.selectedSubTableFields != undefined && mappedCol.selectedSubTableFields.length > 0) {
                        mappedSubTable.SubTableFields = [];
                        for (var i = 0; i < mappedCol.selectedSubTableFields.length; i++) {
                            var subTableField = mappedCol.selectedSubTableFields[i];
                            mappedSubTable.SubTableFields.push({
                                $type : "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorSubTableColumnDefinition, Vanrise.Analytic.MainExtensions",
                                FieldName: subTableField.value,
                                FileTitle: subTableField.description
                            });
                        }
                    }
                    mappedSubTable.SubTableName = mappedCol.selectedField.value;
                    mappedSubTable.SubTableId = mappedCol.selectedField.subTableId;
                    mappedTable.mappedSubTables.push(mappedSubTable);
                }
            }

            return mappedTable;
        }
    }

    return directiveDefinitionObject;

}]);

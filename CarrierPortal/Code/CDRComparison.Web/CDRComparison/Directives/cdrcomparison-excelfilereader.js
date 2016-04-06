(function (app) {

    'use strict';

    ExcelFileReaderDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ExcelFileReaderDirective(CDRComparison_CDRComparisonAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var excelFileReader = new ExcelFileReader($scope, ctrl, $attrs);
                excelFileReader.initializeController();
            },
            controllerAs: "excelFileCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/CDRComparison/Directives/Templates/ExcelFileReaderTemplate.html"
        };

        function ExcelFileReader($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var cdrSourceContext;
            var fieldMappings;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.dateTimeFormat = 'yyyy-MM-dd HH:mm:ss.fff';
                $scope.scopeModel.gridColumns = [];
                $scope.scopeModel.sampleData = [];
                $scope.scopeModel.headerGridSource = [];
                $scope.scopeModel.firstRowHeader = false;

                $scope.scopeModel.readSample = function () {
                    return readSample();
                };

                $scope.scopeModel.validateFieldMappings = function () {
                    if ($scope.scopeModel.headerGridSource.length == 0)
                        return null;
                    var cells = $scope.scopeModel.headerGridSource[0].cells;
                    var fieldNames = [];
                    var filledCols = 0;
                    for (var i = 0; i < cells.length; i++) {
                        if (cells[i].selectedField != undefined) {
                            filledCols++;

                            if (UtilsService.contains(fieldNames, cells[i].selectedField.description))
                                return 'Each column should be mapped to a unique field';
                            else
                                fieldNames.push(cells[i].selectedField.description);
                        }
                    }
                    if (filledCols < cells[0].fields.length)
                        return 'All columns should be mapped.';
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var excelFileId;

                    if (payload != undefined) {
                        cdrSourceContext = payload.cdrSourceContext;
                        if (payload.DateTimeFormat != undefined)
                            $scope.scopeModel.dateTimeFormat = payload.DateTimeFormat;
                        if (payload.FirstRowIndex != undefined)
                            $scope.scopeModel.firstRowHeader = payload.FirstRowIndex != 0

                        excelFileId = payload.fileId;
                        fieldMappings = payload.FieldMappings;
                    }

                    if (excelFileId != undefined) {
                        var readSamplePromise = readSample();
                        promises.push(readSamplePromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.MainExtensions.ExcelFileReader, CDRComparison.MainExtensions',
                        FieldMappings: buildFieldMappings(),
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat,
                        FirstRowIndex: $scope.scopeModel.firstRowHeader ? 1 : 0
                    };

                    function buildFieldMappings() {
                        var fieldMappings;
                        var dataItem = ($scope.scopeModel.headerGridSource.length > 0) ? $scope.scopeModel.headerGridSource[0] : undefined;

                        if (dataItem != undefined && dataItem.cells != undefined) {
                            fieldMappings = [];
                            for (var i = 0; i < dataItem.cells.length; i++) {
                                if (dataItem.cells[i].selectedField != undefined) {
                                    fieldMappings.push({
                                        FieldIndex: i,
                                        FieldName: (dataItem.cells[i].selectedField != undefined) ? dataItem.cells[i].selectedField.description : undefined
                                    });
                                }

                            }
                        }

                        return fieldMappings;
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function readSample() {
                $scope.scopeModel.gridColumns.length = 0;
                $scope.scopeModel.headerGridSource.length = 0;
                $scope.scopeModel.sampleData.length = 0;

                $scope.scopeModel.isLoadingSampleGrid = true;

                return cdrSourceContext.readSample().then(function (response) {
                    if (response != null) {
                        defineGridColumns(response.ColumnCount);
                        for (var j = 0; j < response.Rows.length; j++) {
                            $scope.scopeModel.sampleData.push(response.Rows[j]);
                        }
                        defineHeaderGridDataItem(response.ColumnCount);
                    }
                }).finally(function () {
                    $scope.scopeModel.isLoadingSampleGrid = false;
                });

                function defineGridColumns(columnCount) {
                    for (var i = 0; i < columnCount; i++) {
                        $scope.scopeModel.gridColumns.push({
                            id: i + 1,
                            name: 'Column ' + (i + 1)
                        });
                    }
                }

                function defineHeaderGridDataItem(columnCount) {
                    var dataItem = {};
                    dataItem.cells = [];
                    for (var i = 0; i < columnCount; i++) {
                        var cell = {};
                        cell.fields = getFields();

                        cell.selectedField = getSelectedField(cell.fields, i);
                        dataItem.cells.push(cell);
                    }
                    $scope.scopeModel.headerGridSource.push(dataItem);

                    function getFields() {
                        return [
                            { value: 1, description: 'CDPN' },
                            { value: 2, description: 'CGPN' },
                            { value: 3, description: 'Time' },
                            { value: 4, description: 'DurationInSec' }
                        ];
                    }
                    function getSelectedField(cellFields, cellIndex) {
                        var selectedField;
                        if (fieldMappings != undefined) {

                            var fieldMapping = UtilsService.getItemByVal(fieldMappings, cellIndex, 'FieldIndex');
                            if (fieldMapping != undefined)
                                selectedField = UtilsService.getItemByVal(cellFields, fieldMapping.FieldName, 'description');
                        }
                        return selectedField;
                    }
                }
            }
        }
    }

    app.directive('cdrcomparisonExcelfilereader', ExcelFileReaderDirective);

})(app);
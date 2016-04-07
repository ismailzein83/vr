(function (app) {

    'use strict';

    FlatFileReaderDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'UtilsService', 'VRUIUtilsService'];

    function FlatFileReaderDirective(CDRComparison_CDRComparisonAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var flatFileReader = new FlatFileReader($scope, ctrl, $attrs);
                flatFileReader.initializeController();
            },
            controllerAs: "flatFileCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/CDRComparison/Directives/Templates/FlatFileReaderTemplate.html"
        };

        function FlatFileReader($scope, ctrl, $attrs) {
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
                $scope.scopeModel.cdrFields = getFields();

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
                        return 'missing fields mapping';
                    return null;
                };

                defineAPI();
            }

            function getFields() {
                return [
                    { value: 1, description: 'CDPN' },
                    { value: 2, description: 'CGPN' },
                    { value: 3, description: 'Time' },
                    { value: 4, description: 'DurationInSec' }
                ];
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var flatFileId;

                    if (payload != undefined) {
                        cdrSourceContext = payload.cdrSourceContext;
                        $scope.scopeModel.delimiter = payload.Delimiter;
                        if (payload.DateTimeFormat != undefined)
                            $scope.scopeModel.dateTimeFormat = payload.DateTimeFormat;
                        if (payload.FirstRowIndex !=undefined)
                        $scope.scopeModel.firstRowHeader = payload.FirstRowIndex != 0
                      
                        flatFileId = payload.fileId;
                        fieldMappings = payload.FieldMappings;
                    }

                    if (flatFileId != undefined) {
                        var readSamplePromise = readSample();
                        promises.push(readSamplePromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.MainExtensions.CDRFileReaders.FlatFileReader, CDRComparison.MainExtensions',
                        Delimiter: $scope.scopeModel.delimiter,
                        FieldMappings: buildFieldMappings(),
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat,
                        FirstRowIndex:$scope.scopeModel.firstRowHeader? 1:0 
                    };

                    function buildFieldMappings() {
                        var returnValue;
                        var dataItem = ($scope.scopeModel.headerGridSource.length > 0) ? $scope.scopeModel.headerGridSource[0] : undefined;

                        if (dataItem != undefined && dataItem.cells != undefined) {
                            returnValue = [];
                            for (var i = 0; i < dataItem.cells.length; i++) {
                                if (dataItem.cells[i].selectedField != undefined)
                                {
                                    returnValue.push({
                                        FieldIndex: i,
                                        FieldName: (dataItem.cells[i].selectedField != undefined) ? dataItem.cells[i].selectedField.description : undefined
                                    });
                                }
                               
                            }
                        }
                        else {
                            returnValue = fieldMappings; // Set the field mappings to the config's field mappings if exists
                        }

                        return returnValue;
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

                    
                    function getSelectedField(cellFields, cellIndex) {
                        var selectedField;
                        if (fieldMappings != undefined) {
                            
                            var fieldMapping = UtilsService.getItemByVal(fieldMappings, cellIndex, 'FieldIndex');
                            if (fieldMapping !=undefined)
                            selectedField = UtilsService.getItemByVal(cellFields, fieldMapping.FieldName, 'description');
                        }
                        return selectedField;
                    }
                }
            }
        }
    }

    app.directive('cdrcomparisonFlatfilereader', FlatFileReaderDirective);

})(app);
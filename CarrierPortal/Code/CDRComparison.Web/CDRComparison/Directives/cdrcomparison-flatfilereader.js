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

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.gridColumns = [];
                $scope.scopeModel.sampleData = [];
                $scope.scopeModel.headerGridSource = [];

                $scope.scopeModel.readSample = function () {
                    return readSample();
                };

                $scope.scopeModel.validateFieldMappings = function () {
                    if ($scope.scopeModel.headerGridSource.length == 0)
                        return null;
                    var cells = $scope.scopeModel.headerGridSource[0].cells;
                    var fieldNames = [];
                    for (var i = 0; i < cells.length; i++) {
                        if (cells[i].selectedField != undefined) {
                            if (UtilsService.contains(fieldNames, cells[i].selectedField.description))
                                return 'Each column should be mapped to a unique field';
                            else
                                fieldNames.push(cells[i].selectedField.description);
                        }
                    }
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var flatFileId;

                    if (payload != undefined) {
                        cdrSourceContext = payload.cdrSourceContext;
                        $scope.scopeModel.delimiter = payload.Delimiter;
                        $scope.scopeModel.dateTimeFormat = payload.DateTimeFormat;
                        flatFileId = payload.fileId;
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
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat
                    };

                    function buildFieldMappings() {
                        var fieldMappings;
                        var dataItem = ($scope.scopeModel.headerGridSource.length > 0) ? $scope.scopeModel.headerGridSource[0] : undefined;

                        if (dataItem != undefined && dataItem.cells != undefined) {
                            fieldMappings = [];
                            for (var i = 0; i < dataItem.cells.length; i++) {
                                fieldMappings.push({
                                    FieldIndex: i,
                                    FieldName: (dataItem.cells[i].selectedField != undefined) ? dataItem.cells[i].selectedField.description : undefined
                                });
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
                        dataItem.cells.push(cell);
                    }
                    $scope.scopeModel.headerGridSource.push(dataItem);

                    function getFields() {
                        return [
                            { value: 3, description: 'CGPN' },
                            { value: 4, description: 'CDPN' },
                            { value: 1, description: 'Time' },
                            { value: 2, description: 'DurationInSec' }
                        ];
                    }
                }
            }
        }
    }

    app.directive('cdrcomparisonFlatfilereader', FlatFileReaderDirective);

})(app);
(function (app) {

    'use strict';

    FlatFileReaderDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function FlatFileReaderDirective(CDRComparison_CDRComparisonAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
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
            var fileCDRSourceContext;
            var fieldMappings;

            var cdrFields = [
                { value: 1, description: 'CDPN', title: 'CDPN' },
                { value: 2, description: 'CGPN', title: 'CGPN' },
                { value: 3, description: 'Time', title: 'Time' },
                { value: 4, description: 'DurationInSec', title: 'Duration' }
            ];

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.dateTimeFormat = 'yyyy-MM-dd HH:mm:ss.fff';
                $scope.scopeModel.gridColumns = [];
                $scope.scopeModel.sampleData = [];
                $scope.scopeModel.headerGridSource = [];
                $scope.scopeModel.firstRowHeader = false;

                $scope.scopeModel.availableCDRFields = [];
                for (var i = 0; i < cdrFields.length; i++) {
                    $scope.scopeModel.availableCDRFields.push(cdrFields[i]);
                }

                $scope.scopeModel.onFieldSelectionChanged = function (selectedField) {
                    if (selectedField != undefined) {
                        var index = UtilsService.getItemIndexByVal($scope.scopeModel.availableCDRFields, selectedField.value, 'value');
                        $scope.scopeModel.availableCDRFields.splice(index, 1);
                    }

                    // Add deselected fields to available fields

                    $scope.scopeModel.availableCDRFields.length = 0;
                    var selectedFieldValues = getSelectedFieldValues();

                    for (var i = 0; i < cdrFields.length; i++) {
                        if (!UtilsService.contains(selectedFieldValues, cdrFields[i].value))
                            $scope.scopeModel.availableCDRFields.push(cdrFields[i]);
                    }
                };

                $scope.scopeModel.disableReadSampleButton = function () {
                    return (fileCDRSourceContext != undefined) ? fileCDRSourceContext.disableReadSampleButton() : true;
                };

                $scope.scopeModel.readSample = function () {
                    return readSample();
                };
                
                $scope.scopeModel.validateSection = function () {
                    if ($scope.scopeModel.headerGridSource.length > 0)
                        return null;
                    if (fieldMappings != null)
                        return null;
                    return 'Fields are not mapped';
                };

                $scope.scopeModel.validateFieldMappings = function () {
                    if ($scope.scopeModel.headerGridSource.length == 0) {
                        return null;
                    }
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
                    if (filledCols < cdrFields.length)
                        return 'Missing fields mapping';
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
                        fileCDRSourceContext = payload.fileCDRSourceContext;

                        $scope.scopeModel.isTabDelimited = payload.IsTabDelimited;
                        $scope.scopeModel.delimiter = payload.Delimiter;

                        if (payload.DateTimeFormat != undefined)
                            $scope.scopeModel.dateTimeFormat = payload.DateTimeFormat;
                        if (payload.FirstRowIndex != undefined)
                            $scope.scopeModel.firstRowHeader = payload.FirstRowIndex != 0;
                      
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
                        $type: 'CDRComparison.MainExtensions.FlatFileReader, CDRComparison.MainExtensions',
                        IsTabDelimited: $scope.scopeModel.isTabDelimited,
                        Delimiter: ($scope.scopeModel.isTabDelimited) ? null : $scope.scopeModel.delimiter,
                        FieldMappings: buildFieldMappings(),
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat,
                        FirstRowIndex:$scope.scopeModel.firstRowHeader ? 1 : 0 
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
                        if (response.ErrorMessage != null) {
                            VRNotificationService.showError(response.ErrorMessage);
                        }
                        else {
                            defineGridColumns(response.ColumnCount);
                            for (var j = 0; j < response.Rows.length; j++) {
                                $scope.scopeModel.sampleData.push(response.Rows[j]);
                            }
                            defineHeaderGridDataItem(response.ColumnCount);
                        }
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
                    resetAvailableCDRFields();

                    var dataItem = {};
                    dataItem.cells = [];
                    for (var i = 0; i < columnCount; i++) {
                        var cell = {};
                        cell.selectedField = getSelectedField(i);
                        dataItem.cells.push(cell);
                    }
                    $scope.scopeModel.headerGridSource.push(dataItem);
                    
                    function resetAvailableCDRFields() {
                        $scope.scopeModel.availableCDRFields.length = 0;
                        for (var i = 0; i < cdrFields.length; i++) {
                            $scope.scopeModel.availableCDRFields.push(cdrFields[i]);
                        }
                    }

                    function getSelectedField(cellIndex) {
                        var selectedField;
                        if (fieldMappings != undefined) {
                            var fieldMapping = UtilsService.getItemByVal(fieldMappings, cellIndex, 'FieldIndex');
                            if (fieldMapping != undefined)
                                selectedField = UtilsService.getItemByVal($scope.scopeModel.availableCDRFields, fieldMapping.FieldName, 'description');
                        }
                        return selectedField;
                    }
                }
            }

            function getSelectedFieldValues() {
                var values;
                if ($scope.scopeModel.headerGridSource.length > 0) {
                    values = [];
                    var cells = $scope.scopeModel.headerGridSource[0].cells;
                    for (var i = 0; i < cells.length; i++) {
                        if (cells[i].selectedField != undefined)
                            values.push(cells[i].selectedField.value);
                    }
                }
                return values;
            }
        }
    }

    app.directive('cdrcomparisonFlatfilereader', FlatFileReaderDirective);

})(app);
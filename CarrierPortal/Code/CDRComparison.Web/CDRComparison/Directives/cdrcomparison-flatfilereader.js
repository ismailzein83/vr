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
                $scope.scopeModel.sampleGridColumns = [];
                $scope.scopeModel.sampleData = [];
                $scope.scopeModel.mappingData = [{}];

                $scope.scopeModel.fields = [
                    { value: 3, description: 'CGPN' },
                    { value: 4, description: 'CDPN' },
                    { value: 1, description: 'Time' },
                    { value: 2, description: 'DurationInSec' }
                ];

                

                $scope.scopeModel.readSample = function () {
                    $scope.scopeModel.isLoadingSampleGrid = true;

                    cdrSourceContext.readSample().then(function (response) {
                        if (response != null) {
                            defineSampleGridColumns(response.ColumnCount);
                            for (var j = 0; j < response.Rows.length; j++) {
                                $scope.scopeModel.sampleData.push(response.Rows[j]);
                            }
                            defineFieldMappings(response.ColumnCount);
                        }
                    }).finally(function () {
                        $scope.scopeModel.isLoadingSampleGrid = false;
                    });

                    function defineSampleGridColumns(columnCount) {
                        for (var i = 0; i < columnCount; i++) {
                            $scope.scopeModel.sampleGridColumns.push({
                                id: i + 1,
                                name: 'Column ' + (i + 1)
                            });
                        }
                    }

                    function defineFieldMappings(columnCount) {

                        function getFieldArray() {
                            return 
                        }
                    }
                };

                $scope.scopeModel.validateFieldMappings = function () {
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var flatFileId;
                    var sampleCDRs;

                    if (payload != undefined) {
                        cdrSourceContext = payload.cdrSourceContext;
                        $scope.scopeModel.delimiter = payload.Delimiter;
                        $scope.scopeModel.dateTimeFormat = payload.DateTimeFormat;
                        flatFileId = payload.fileId;
                    }

                    if (flatFileId != undefined) {
                        var getSampleCDRsPromise = getSampleCDRs();
                        promises.push(getSampleCDRsPromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);

                    function getSampleCDRs() {
                        return requestSampleCDRsFromServer(flatFileId).then(function (response) {
                            if (response != null) {
                                sampleCDRs = [];
                                for (var i = 0; i < response.length; i++) {
                                    sampleCDRs.push(response[i]);
                                }
                            }
                        });
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.MainExtensions.CDRFileReaders.FlatFileReader, CDRComparison.MainExtensions',
                        Delimiter: $scope.scopeModel.delimiter,
                        FieldMappings: buildFieldMappings(),
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat
                    };

                    function buildFieldMappings() {
                        var returnValue = [];
                        for (var i = 0; i < $scope.scopeModel.mappingData.length; i++) {
                            returnValue.push({
                                FieldIndex: i + 1//,
                             //   FieldName: $scope.scopeModel.mappingData[i].selectedField.description
                            });
                        }
                        return returnValue;
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('cdrcomparisonFlatfilereader', FlatFileReaderDirective);

})(app);
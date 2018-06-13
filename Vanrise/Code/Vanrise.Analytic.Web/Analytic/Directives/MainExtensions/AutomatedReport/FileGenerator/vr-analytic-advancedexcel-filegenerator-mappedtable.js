'use strict';

app.directive('vrAnalyticAdvancedexcelFilegeneratorMappedtable', ['UtilsService', 'VRUIUtilsService',  function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var advancedExcelMappedTableTemplate = new AdvancedExcelMappedTableTemplate($scope, ctrl, $attrs);
            advancedExcelMappedTableTemplate.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorMappedTableTemplate.html'
    };

    function AdvancedExcelMappedTableTemplate($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var firstRowDirectiveAPI;
        var firstRowDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedTitleAPI;
        var mappedTitleReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedColumnsAPI;
        var mappedColumnsReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedTable;
        var context;

        function initializeController() {

            $scope.onFirstRowMappingReady = function (api) {
                firstRowDirectiveAPI = api;
                firstRowDirectiveReadyDeferred.resolve();
            };

            $scope.onMappedTitleGridReady = function (api) {
                mappedTitleAPI = api;
                mappedTitleReadyDeferred.resolve();
            };

            $scope.onMappedColumnsGridReady = function (api) {
                mappedColumnsAPI = api;
                mappedColumnsReadyDeferred.resolve();
            };


            $scope.addMappedCol = function () {
                mappedColumnsAPI.addMappedCol();
            };
            $scope.addAllFields = function () {
                mappedColumnsAPI.addAllFields();

            };

            $scope.disableAddMappedCol = function () {
                if (context!=undefined && context.getQueryInfo()!=undefined && context.getQueryInfo().length == 0) {
                    return true;
                }
            };

            UtilsService.waitMultiplePromises([firstRowDirectiveReadyDeferred.promise, mappedTitleReadyDeferred.promise, mappedColumnsReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};


            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    mappedTable = payload.mappedTable;
                    context = payload.context;
                }
                if (mappedTable != undefined) {
                    $scope.includeHeaders = mappedTable.IncludeHeaders;
                    $scope.includeTitle = mappedTable.IncludeTitle;
                }

                var loadFirstRowDirectivePromise = loadFirstRowDirective();
                promises.push(loadFirstRowDirectivePromise);

                var loadMappedColumnsGridPromise = loadMappedColumnsGrid();
                promises.push(loadMappedColumnsGridPromise);

                var loadMappedTitleGridPromise = loadMappedTitleGrid();
                promises.push(loadMappedTitleGridPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function getData() {
                var firstRowDirectiveData = firstRowDirectiveAPI.getData();
                if (firstRowDirectiveData == undefined)
                    return null;

                var mappedTable = {
                    $type: 'Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorTableDefinition, Vanrise.Analytic.MainExtensions',
                    SheetIndex: firstRowDirectiveData.SheetIndex,
                    RowIndex: firstRowDirectiveData.RowIndex,
                    IncludeTitle:$scope.includeTitle,
                    IncludeHeaders: $scope.includeHeaders,
                    TitleDefinition: mappedTitleAPI.getData(),
                    ColumnDefinitions: mappedColumnsAPI.getData()
                };
                return mappedTable;
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadFirstRowDirective() {

            var firstRowDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            firstRowDirectiveReadyDeferred.promise.then(function () {
                var firstRowDirectivePayload = {
                    context: getCellFieldMappingContext(),
                    showEditButton: false
                };

                if (mappedTable != undefined) {
                    firstRowDirectivePayload.fieldMapping = {
                        SheetIndex: mappedTable.SheetIndex,
                        RowIndex: mappedTable.RowIndex,
                        CellIndex: 0
                    };
                }

                VRUIUtilsService.callDirectiveLoad(firstRowDirectiveAPI, firstRowDirectivePayload, firstRowDirectiveLoadDeferred);
            });

            return firstRowDirectiveLoadDeferred.promise;
        }

        function loadMappedTitleGrid(){
            var mappedTitleLoadDeferred = UtilsService.createPromiseDeferred();

            mappedTitleReadyDeferred.promise.then(function () {
                var mappedTitlePayload = {
                    context: getContext(),
                    mappedSheet: mappedTable,
                };
                VRUIUtilsService.callDirectiveLoad(mappedTitleAPI, mappedTitlePayload, mappedTitleLoadDeferred);
            });
            return mappedTitleLoadDeferred.promise;
        }

        function loadMappedColumnsGrid() {
            var mappedColumnsLoadDeferred = UtilsService.createPromiseDeferred();
            mappedColumnsReadyDeferred.promise.then(function () {
                var mappedColumnsPayload = {
                    context: getContext(),
                    mappedSheet: mappedTable,
                };
                VRUIUtilsService.callDirectiveLoad(mappedColumnsAPI, mappedColumnsPayload, mappedColumnsLoadDeferred);
            });
            return mappedColumnsLoadDeferred.promise;
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }
            currentContext.getFirstRowData = function () {
                return firstRowDirectiveAPI.getData();
            };

            return currentContext;
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
                if (selectedSheetAPI != undefined) {
                    return selectedSheetAPI.getSelected();
                }
            }
            function getSelectedSheet() {
                return context.getSelectedSheet();
            }
            function getFirstRowIndex() {
                var firstRowDirectiveData = firstRowDirectiveAPI.getData();
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

    }
}]);
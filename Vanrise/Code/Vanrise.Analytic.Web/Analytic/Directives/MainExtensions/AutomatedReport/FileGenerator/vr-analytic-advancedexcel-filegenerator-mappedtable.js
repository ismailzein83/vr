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
        controllerAs: "ctrlTable",
        bindToController: true,
        templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorMappedTableTemplate.html'
    };

    function AdvancedExcelMappedTableTemplate($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var firstRowDirectiveAPI;
        var firstRowDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedColumnsAPI;
        var mappedColumnsReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedTablePayload;
        var context;


        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.includeHeaders = true;

            $scope.scopeModel.onFirstRowMappingReady = function (api) {
                firstRowDirectiveAPI = api;
                firstRowDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onMappedColumnsGridReady = function (api) {
                mappedColumnsAPI = api;
                mappedColumnsReadyDeferred.resolve();
            };


            $scope.scopeModel.addMappedCol = function () {
                mappedColumnsAPI.addMappedCol();
            };
            $scope.scopeModel.addAllFields = function () {
                mappedColumnsAPI.addAllFields();
            };

            $scope.scopeModel.disableAddMappedCol = function () {
                if (context!=undefined && context.getQueryInfo()!=undefined && context.getQueryInfo().length == 0) {
                    return true;
                }
            };

            UtilsService.waitMultiplePromises([firstRowDirectiveReadyDeferred.promise, mappedColumnsReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            var vrAutomatedReportQueryId;
            var listName;

            api.load = function (payload) {

                var promises = [];
                if (payload != undefined) {
                    mappedTablePayload = payload.mappedTable;
                    context = payload.context;
                    vrAutomatedReportQueryId = payload.VRAutomatedReportQueryId;
                    listName = payload.ListName;
                }
                if (mappedTablePayload != undefined) {
                    $scope.scopeModel.includeHeaders = mappedTablePayload.IncludeHeaders;
                    $scope.scopeModel.includeTitle = mappedTablePayload.IncludeTitle;
                    $scope.scopeModel.tableTitle = mappedTablePayload.Title;
                }

                var loadFirstRowDirectivePromise = loadFirstRowDirective();
                promises.push(loadFirstRowDirectivePromise);

                var loadMappedColumnsGridPromise = loadMappedColumnsGrid();
                promises.push(loadMappedColumnsGridPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function getData() {
                var firstRowDirectiveData = firstRowDirectiveAPI.getData();
                if (firstRowDirectiveData == undefined)
                    return null;

                var mappedTableObject = mappedColumnsAPI.getData();
                var mappedTable = {
                    $type: 'Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorTableDefinition, Vanrise.Analytic.MainExtensions',
                    SheetIndex: firstRowDirectiveData.SheetIndex,
                    RowIndex: firstRowDirectiveData.RowIndex,
                    IncludeTitle: $scope.scopeModel.includeTitle,
                    VRAutomatedReportQueryId: vrAutomatedReportQueryId,
                    ListName: listName,
                    IncludeHeaders: $scope.scopeModel.includeHeaders,
                    Title: $scope.scopeModel.includeTitle ? $scope.scopeModel.tableTitle : undefined,
                    ColumnDefinitions: mappedTableObject != undefined ? mappedTableObject.mappedColumns : undefined,
                    SubTableDefinitions: (mappedTableObject != undefined && mappedTableObject.mappedSubTables!=undefined) ? mappedTableObject.mappedSubTables: null,
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

                if (mappedTablePayload != undefined) {
                    firstRowDirectivePayload.fieldMapping = {
                        SheetIndex: mappedTablePayload.SheetIndex,
                        RowIndex: mappedTablePayload.RowIndex,
                        CellIndex: 0
                    };
                }

                VRUIUtilsService.callDirectiveLoad(firstRowDirectiveAPI, firstRowDirectivePayload, firstRowDirectiveLoadDeferred);
            });

            return firstRowDirectiveLoadDeferred.promise;
        }


        function loadMappedColumnsGrid() {
            var mappedColumnsLoadDeferred = UtilsService.createPromiseDeferred();
            mappedColumnsReadyDeferred.promise.then(function () {
                var mappedColumnsPayload = {
                    context: getContext(),
                    mappedSheet: mappedTablePayload,
                };
                VRUIUtilsService.callDirectiveLoad(mappedColumnsAPI, mappedColumnsPayload, mappedColumnsLoadDeferred);
            });
            return mappedColumnsLoadDeferred.promise;
        }

        function getContext() {
            var currentContext = UtilsService.cloneObject(context);
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
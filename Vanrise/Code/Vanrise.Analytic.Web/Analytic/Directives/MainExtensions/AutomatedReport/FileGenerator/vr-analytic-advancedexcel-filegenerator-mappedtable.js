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


        var mappedTableTitlesGridAPI;
        var mappedTableTitlesGridReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedTablePayload;
        var context;

        var vrAutomatedReportQueryId;
        var listName;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.includeHeaders = true;
            $scope.scopeModel.includeSummary = false;
            $scope.scopeModel.disableAddMappedCol = false;

            $scope.scopeModel.onFirstRowMappingReady = function (api) {
                firstRowDirectiveAPI = api;
                firstRowDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onMappedColumnsGridReady = function (api) {
                mappedColumnsAPI = api;
                mappedColumnsReadyDeferred.resolve();
            };

            $scope.scopeModel.onTitlesGridDirectiveReady = function (api) {
                mappedTableTitlesGridAPI = api;
                mappedTableTitlesGridReadyDeferred.resolve();
            };

            $scope.scopeModel.addMappedCol = function () {
                mappedColumnsAPI.addMappedCol();
            };
            $scope.scopeModel.addAllFields = function () {
                mappedColumnsAPI.addAllFields();
            };

            UtilsService.waitMultiplePromises([firstRowDirectiveReadyDeferred.promise, mappedColumnsReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            vrAutomatedReportQueryId = undefined;
            listName = undefined;

            api.load = function (payload) {

                var promises = [];
                if (payload != undefined) {
                    mappedTablePayload = payload.mappedTable;
                    context = payload.context;
                    vrAutomatedReportQueryId = payload.VRAutomatedReportQueryId;
                    listName = payload.ListName;
                }
                if (context != undefined && context.getQueryInfo != undefined && typeof(context.getQueryInfo)=="function"){
                    var queries = context.getQueryInfo();
                    if((queries==undefined)||(queries!=undefined && queries.length == 0)) {
                        $scope.scopeModel.disableAddMappedCol = true;
                    }
                    if (queries != undefined && queries.length>0 && vrAutomatedReportQueryId != undefined) {
                        for (var i = 0; i < queries.length; i++) {
                            var query = queries[i];
                            if (query.VRAutomatedReportQueryId == vrAutomatedReportQueryId) {
                                if (query.Settings != undefined && query.Settings.WithSummary != undefined) {
                                    $scope.scopeModel.showSummarySwitch = query.Settings.WithSummary;
                                    if ($scope.scopeModel.showSummarySwitch) {
                                        $scope.scopeModel.includeSummary = true;
                                    }
                                }
                                else {
                                    $scope.scopeModel.showSummarySwitch = false;
                                }
                            }
                            else {
                                $scope.scopeModel.showSummarySwitch = false;
                            }
                        }
                    }
                    else
                    {
                        $scope.scopeModel.showSummarySwitch = false;
                    }
                }
                if (mappedTablePayload != undefined) {
                    $scope.scopeModel.includeHeaders = mappedTablePayload.IncludeHeaders;
                    $scope.scopeModel.includeTitle = mappedTablePayload.IncludeTitle;
                    if ($scope.scopeModel.showSummarySwitch) {
                        $scope.scopeModel.includeSummary = mappedTablePayload.IncludeSummary;
                    }
                }

                var loadFirstRowDirectivePromise = loadFirstRowDirective();
                promises.push(loadFirstRowDirectivePromise);

                var loadMappedColumnsGridPromise = loadMappedColumnsGrid();
                promises.push(loadMappedColumnsGridPromise);

                if ($scope.scopeModel.includeTitle) {
                    var loadMappedTableTitlesGridPromise = loadMappedTableTitlesGrid();
                    promises.push(loadMappedTableTitlesGridPromise);
                }
              

                return UtilsService.waitMultiplePromises(promises);
            };

            api.reload = function (vrAutomatedReportQueryId) {
                if (context != undefined && context.getQueryInfo != undefined && typeof (context.getQueryInfo) == "function") {
                    var queries = context.getQueryInfo();
                    var query = UtilsService.getItemByVal(queries, vrAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                    if (query!=null) {
                        if (query.Settings != undefined && query.Settings.WithSummary != undefined) {
                            $scope.scopeModel.showSummarySwitch = query.Settings.WithSummary;
                            if ($scope.scopeModel.showSummarySwitch) {
                                $scope.scopeModel.includeSummary = true;
                            }
                        }
                        else {
                            $scope.scopeModel.showSummarySwitch = false;
                            $scope.scopeModel.includeSummary = false;
                        }
                    }
                    else {
                        $scope.scopeModel.showSummarySwitch = false;
                        $scope.scopeModel.includeSummary = false;
                    }
                }
                if (mappedColumnsAPI != undefined && mappedColumnsAPI.reload != undefined && typeof (mappedColumnsAPI.reload) == 'function') {
                    mappedColumnsAPI.reload(vrAutomatedReportQueryId);
                }
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
                    IncludeSummary: $scope.scopeModel.showSummarySwitch ? $scope.scopeModel.includeSummary : false,
                    Titles: $scope.scopeModel.includeTitle? mappedTableTitlesGridAPI.getData() : undefined,
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

        function loadMappedTableTitlesGrid() {
            var mappedTableTitlesGridLoadDeferred = UtilsService.createPromiseDeferred();
            mappedTableTitlesGridReadyDeferred.promise.then(function () {
                var payload = {
                    titles: mappedTablePayload!=undefined ? mappedTablePayload.Titles : undefined
                };
                VRUIUtilsService.callDirectiveLoad(mappedTableTitlesGridAPI, payload, mappedTableTitlesGridLoadDeferred);
            });
            return mappedTableTitlesGridLoadDeferred.promise;
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
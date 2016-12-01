'use strict';

app.directive('vrWhsBePricelisttemplateCodesbyzoneMappedtable', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var codeOnEachRowMappedTableTemplate = new CodeOnEachRowMappedTableTemplate($scope, ctrl, $attrs);
            codeOnEachRowMappedTableTemplate.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/CodesByZoneMappedTable/Templates/CodesByZoneMappedTableTemplate.html'
    };

    function CodeOnEachRowMappedTableTemplate($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var firstRowDirectiveAPI;
        var firstRowDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedColumnsAPI;
        var mappedColumnsReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedTable;
        var priceListType;
        var context;

        function initializeController() {

            $scope.onFirstRowMappingReady = function (api) {
                firstRowDirectiveAPI = api;
                firstRowDirectiveReadyDeferred.resolve();
            };

            $scope.onMappedColumnsGridReady = function (api) {
                mappedColumnsAPI = api;
                mappedColumnsReadyDeferred.resolve();
            };

            $scope.addMappedCol = function () {
                mappedColumnsAPI.addMappedCol();
            };

            UtilsService.waitMultiplePromises([firstRowDirectiveReadyDeferred.promise, mappedColumnsReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};


            api.load = function (payload) {

                var promises = [];



                if (payload != undefined) {
                    mappedTable = payload.mappedTable;
                    priceListType = payload.priceListType;
                    context = payload.context
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

                var codesByZone = {
                    $type: 'TOne.WhS.BusinessEntity.MainExtensions.CodesByZoneMappedTable, TOne.WhS.BusinessEntity.MainExtensions',
                    SheetIndex: firstRowDirectiveData.SheetIndex,
                    FirstRowIndex: firstRowDirectiveData.RowIndex,
                    MappedColumns: mappedColumnsAPI.getData(),
                    Delimiter: $scope.delimiterValue
                };

                return codesByZone;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadFirstRowDirective() {

            var firstRowDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            firstRowDirectiveReadyDeferred.promise.then(function () {
                var firstRowDirectivePayload = {
                    context: getCellFieldMappingContext()
                };

                if (mappedTable != undefined) {
                    firstRowDirectivePayload.fieldMapping = {
                        SheetIndex: mappedTable.SheetIndex,
                        RowIndex: mappedTable.FirstRowIndex,
                        CellIndex: 0
                    };
                }
                VRUIUtilsService.callDirectiveLoad(firstRowDirectiveAPI, firstRowDirectivePayload, firstRowDirectiveLoadDeferred);
            });

            return firstRowDirectiveLoadDeferred.promise;
        }

        function getMappedColumns() {

            if ($scope.mappedCols.length == 0)
                return null;

            var mappedColumns = [];

            for (var i = 0; i < $scope.mappedCols.length; i++) {

                var mappedCol = $scope.mappedCols[i];
                var mappedColumn = {};

                var directiveData = mappedCol.directiveAPI.getData();
                if (directiveData != undefined)
                    mappedColumn.ColumnIndex = directiveData.CellIndex;

                mappedColumn.MappedValue = mappedCol.mappedValueSelectiveAPI.getData();

                mappedColumns.push(mappedColumn);
            }

            return mappedColumns;
        }

        function loadMappedColumnsGrid() {
            var mappedColumnsLoadDeferred = UtilsService.createPromiseDeferred();
            mappedColumnsReadyDeferred.promise.then(function () {
                var mappedColumnsPayload = {
                    context: getContext(),
                    mappedSheet: mappedTable,
                    priceListType: priceListType
                };
                if (mappedTable != undefined)
                    $scope.delimiterValue = mappedTable.Delimiter;

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
                if (selectedSheetAPI != undefined)
                    return selectedSheetAPI.getSelected();
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
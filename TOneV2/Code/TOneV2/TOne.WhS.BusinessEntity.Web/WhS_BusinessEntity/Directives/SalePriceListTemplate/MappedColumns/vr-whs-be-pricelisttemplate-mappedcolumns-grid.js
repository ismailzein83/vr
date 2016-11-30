"use strict";

app.directive("vrWhsBePricelisttemplateMappedcolumnsGrid", ["VRUIUtilsService", "UtilsService", "VRNotificationService", "WhS_BE_SalePriceListTemplateAPIService",
function (VRUIUtilsService, UtilsService, VRNotificationService, WhS_BE_SalePriceListTemplateAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SalePriceListGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/MappedColumns/Templates/MappedColumnsGridTemplate.html"

    };

    function SalePriceListGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var context;
        var priceListType;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.mappedCols = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        context = query.context;
                        priceListType = query.priceListType;
                        return loadMappedColumns(query.mappedSheet);
                    };


                    directiveAPI.getData = function () {
                        return getMappedTable();
                    }


                    directiveAPI.addMappedCol = function () {
                        var mappedCol = getMappedCol();
                        $scope.mappedCols.push(mappedCol);
                    }

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SalePriceListTemplateAPIService.GetFilteredSalePriceLists(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            
 
           
        }

        function loadMappedColumns(mappedSheet) {

            var promises = [];

            if (mappedSheet != undefined && mappedSheet.MappedColumns != null) {
                for (var i = 0; i < mappedSheet.MappedColumns.length; i++) {
                    var mappedCol = getMappedCol(mappedSheet.MappedColumns[i], mappedSheet.SheetIndex, mappedSheet.FirstRowIndex);
                    promises.push(mappedCol.directiveLoadDeferred.promise);
                    $scope.mappedCols.push(mappedCol);
                }
            }

            return UtilsService.waitMultiplePromises(promises);
        }


        

        function getMappedCol(mappedColumn, sheetIndex, firstRowIndex) {

            if (mappedColumn != undefined)
                $scope.isLoadingMappedCol = true;

            var mappedCol = {};

            mappedCol.directiveLoadDeferred = UtilsService.createPromiseDeferred();

            mappedCol.onDirectiveReady = function (api) {
                mappedCol.directiveAPI = api;
                var directivePayload = {
                    context: getCellFieldMappingContext()
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

            mappedCol.mappedValueSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            mappedCol.onMappedValueSelectiveReady = function (api) {
                mappedCol.mappedValueSelectiveAPI = api;
                var mappedValueSelectivePayload = {
                    priceListType: priceListType
                };
                if (mappedColumn != undefined)
                    mappedValueSelectivePayload.mappedValue = mappedColumn.MappedValue;
                
                VRUIUtilsService.callDirectiveLoad(mappedCol.mappedValueSelectiveAPI, mappedValueSelectivePayload, mappedCol.mappedValueSelectiveLoadDeferred);
            };

            UtilsService.waitMultiplePromises([mappedCol.mappedValueSelectiveLoadDeferred.promise]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingMappedCol = false;
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
    }

    return directiveDefinitionObject;

}]);

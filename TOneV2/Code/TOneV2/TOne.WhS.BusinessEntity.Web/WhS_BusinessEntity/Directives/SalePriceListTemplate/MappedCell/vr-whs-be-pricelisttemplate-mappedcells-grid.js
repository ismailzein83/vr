"use strict";

app.directive("vrWhsBePricelisttemplateMappedcellsGrid", ["VRUIUtilsService", "UtilsService", "VRNotificationService", "WhS_BE_SalePriceListTemplateAPIService",
function (VRUIUtilsService, UtilsService, VRNotificationService, WhS_BE_SalePriceListTemplateAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new MappedCellsGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/MappedCell/Templates/MappedCellsGridTemplate.html"

    };

    function MappedCellsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var context;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.mappedCells = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        context = query.context;
                        return loadMappedCells(query.mappedCells);
                    };


                    directiveAPI.getData = function () {
                        return getMappedCells();
                    };


                    directiveAPI.addMappedCell = function () {
                        var mappedCell = addMappedCellToGrid();
                        var promises = [];
                        promises.push(mappedCell.directiveLoadDeferred.promise);
                        promises.push(mappedCell.mappedCellSelectiveLoadDeferred.promise);
                        $scope.mappedCells.push(mappedCell);
                        return UtilsService.waitMultiplePromises(promises);
                    };

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            };


            $scope.removeMappedCell = function (dataItem) {
                var index = $scope.mappedCells.indexOf(dataItem);
                $scope.mappedCells.splice(index, 1);
            };

        }

        function loadMappedCells(mappedCells) {

            var promises = [];

            if (mappedCells != null) {
                for (var i = 0; i < mappedCells.length; i++) {
                    var mappedCell = addMappedCellToGrid(mappedCells[i]);
                    promises.push(mappedCell.directiveLoadDeferred.promise);
                    promises.push(mappedCell.mappedCellSelectiveLoadDeferred.promise);
                    $scope.mappedCells.push(mappedCell);
                }
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function addMappedCellToGrid(mappedCell) {


            var result = {
            };

            result.directiveLoadDeferred = UtilsService.createPromiseDeferred();

            result.onDirectiveReady = function (api) {
                result.directiveAPI = api;
                var directivePayload = {
                    context: getCellFieldMappingContext(),
                    showEditButton: false
                };
                if (mappedCell != undefined) {
                    directivePayload.fieldMapping = {
                        SheetIndex: mappedCell.SheetIndex,
                        RowIndex: mappedCell.RowIndex,
                        CellIndex: mappedCell.CellIndex
                    };
                }

                VRUIUtilsService.callDirectiveLoad(result.directiveAPI, directivePayload, result.directiveLoadDeferred);
            };

            result.mappedCellSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            result.onMappedCellSelectiveReady = function (api) {
                result.mappedCellSelectiveAPI = api;
                var mappedCellSelectivePayload = {
                };
                if (mappedCell != undefined)
                    mappedCellSelectivePayload.mappedCell = mappedCell;

                VRUIUtilsService.callDirectiveLoad(result.mappedCellSelectiveAPI, mappedCellSelectivePayload, result.mappedCellSelectiveLoadDeferred);
            };

            return result;
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
                return undefined;
            }

            return {
                setSelectedCell: selectCellAtSheet,
                getSelectedCell: getSelectedCell,
                getSelectedSheet: getSelectedSheet,
                getFirstRowIndex: getFirstRowIndex
            };
        }

        function getMappedCells() {

            if ($scope.mappedCells.length == 0)
                return null;

            var result = [];

            for (var i = 0; i < $scope.mappedCells.length; i++) {

                var mappedCell = $scope.mappedCells[i];

                var mappedCellResult = mappedCell.mappedCellSelectiveAPI.getData();

                var directiveData = mappedCell.directiveAPI.getData();

                if (directiveData != undefined) {
                    mappedCellResult.CellIndex = directiveData.CellIndex;
                    mappedCellResult.RowIndex = directiveData.RowIndex;
                    mappedCellResult.SheetIndex = directiveData.SheetIndex;
                }

                result.push(mappedCellResult);
            }

            return result;
        }


    }

    return directiveDefinitionObject;

}]);

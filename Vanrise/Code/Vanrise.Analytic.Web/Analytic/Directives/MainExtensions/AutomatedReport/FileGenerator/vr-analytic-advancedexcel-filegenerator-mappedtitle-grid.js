"use strict";

app.directive("vrAnalyticAdvancedexcelFilegeneratorMappedtitleGrid", ["VRUIUtilsService", "UtilsService", "VRNotificationService",
function (VRUIUtilsService, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new AdvancedExcelMappedTableGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorMappedTitleGridTemplate.html'

    };

    function AdvancedExcelMappedTableGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var context;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.titleMapping = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        context = query.context;
                        return loadTitleDefinition(query.mappedSheet);
                    };

                    directiveAPI.getData = function () {
                        return getMappedTable();
                    };

                    return directiveAPI;
                }
            };
        }

        function loadTitleDefinition(mappedSheet) {
            var promises = [];
            if (mappedSheet != undefined && mappedSheet.TitleDefinition != undefined) {
                var titleDefinition = mappedSheet.TitleDefinition;
                var def = getTitleDefinition(titleDefinition, mappedSheet.SheetIndex, mappedSheet.RowIndex);
                promises.push(def.directiveLoadDeferred.promise);
                $scope.titleMapping.push(def);
            }
            else {
                $scope.titleMapping.push(getTitleDefinition(null, null, null));
            }
            return UtilsService.waitMultiplePromises(promises);
        }



        function getTitleDefinition(titleDefinition, sheetIndex, firstRowIndex) {
            if (titleDefinition != undefined) {
                $scope.isLoadingMappedTitle = true;
            }

            var definition = {};

            definition.directiveLoadDeferred = UtilsService.createPromiseDeferred();

            definition.onDirectiveReady = function (api) {
                definition.directiveAPI = api;
                var directivePayload = {
                    context: getCellFieldMappingContext(),
                    showEditButton: false
                };
                if (titleDefinition != undefined) {
                    directivePayload.fieldMapping = {
                        SheetIndex: sheetIndex,
                        RowIndex: firstRowIndex,
                        CellIndex: titleDefinition.ColumnIndex
                    };
                }
                VRUIUtilsService.callDirectiveLoad(definition.directiveAPI, directivePayload, definition.directiveLoadDeferred);
            };

            definition.tableTitle = titleDefinition != undefined ? titleDefinition.Title : undefined;

            UtilsService.waitMultiplePromises([definition.directiveLoadDeferred.promise]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingMappedTitle = false;
            });

            return definition;
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

            if ($scope.titleMapping.length == 0)
                return null;

            var titleDefinition = {};

            for (var i = 0; i < $scope.titleMapping.length; i++) {

                var mappedDefinition = $scope.titleMapping[i];
                var titleDefinition = {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGeneratorTableTitleDefinition, Vanrise.Analytic.MainExtensions"
                };

                var directiveData = mappedDefinition.directiveAPI.getData();
                if (directiveData != undefined) {
                    titleDefinition.ColumnIndex = directiveData.CellIndex;
                }
                titleDefinition.Title = mappedDefinition.tableTitle;

            }

            return titleDefinition;
        }
    }

    return directiveDefinitionObject;

}]);

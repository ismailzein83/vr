'use strict';

app.directive('vrWhsBePricelisttemplateMappedcells', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var mappedCellsTemplate = new MappedCellsTemplate($scope, ctrl, $attrs);
            mappedCellsTemplate.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/MappedCell/Templates/MappedCellsTemplate.html'
    };

    function MappedCellsTemplate($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var mappedCellsGridAPI;
        var mappedCellsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedCells;
        var context;

        function initializeController() {

            $scope.onMappedCellsGridReady = function (api) {
                mappedCellsGridAPI = api;
                mappedCellsGridReadyDeferred.resolve();
            };

            $scope.addMappedCell = function () {
                $scope.isLoadingMappedCellsGrid = true;
                mappedCellsGridAPI.addMappedCell().then(function (response) {
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                }).finally(function() {
                    $scope.isLoadingMappedCellsGrid = false;
            });
            };

            UtilsService.waitMultiplePromises([mappedCellsGridReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};
            api.load = function (payload) {

                var promises = [];
                if (payload != undefined) {
                    mappedCells = payload.mappedCells;
                    context = payload.context;
                }

                var loadMappedCellsGridPromise = loadMappedCellsGrid();
                promises.push(loadMappedCellsGridPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function getData() {
                return mappedCellsGridAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadMappedCellsGrid() {
            var mappedCellsGridLoadDeferred = UtilsService.createPromiseDeferred();
            $scope.isLoadingMappedCellsGrid = true;
            mappedCellsGridReadyDeferred.promise.then(function () {
                var mappedCellsGridPayload = {
                    context: getContext(),
                    mappedCells: mappedCells
                };

                VRUIUtilsService.callDirectiveLoad(mappedCellsGridAPI, mappedCellsGridPayload, mappedCellsGridLoadDeferred);
            });

            return mappedCellsGridLoadDeferred.promise.then(function (response) {
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            }).finally(function (response) {
                $scope.isLoadingMappedCellsGrid = false;
            });
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }
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

            return {
                setSelectedCell: selectCellAtSheet,
                getSelectedCell: getSelectedCell,
                getSelectedSheet: getSelectedSheet,
            };
        }

    }
}]);
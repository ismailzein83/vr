"use strict";

app.directive("vrIntegrationImportedbatchSearch", ['VR_Integration_MappingResultEnum', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService','VRValidationService',
function (VR_Integration_MappingResultEnum, UtilsService, VRNotificationService,VRUIUtilsService , VRValidationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new LogSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Integration/Directives/DataSource/Templates/DataSourceImportedBatchSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {


        var gridApi;
        var dataSourceDirectiveAPI;
        var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();


        this.initializeController = initializeController;
        function initializeController() {
            
            defineScope();

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());
            function getDirectiveAPI() {

                var directiveAPI = {};
                directiveAPI.load = function () {
                    return load();
                };
                return directiveAPI;
            }

        }

        function defineScope() {
            $scope.mappingResults = [];
            $scope.selectedMappingResults = [];
            $scope.importedBatches = [];
            var fromDate = new Date();
            fromDate.setHours(0, 0, 0, 0);
            $scope.selectedFromDateTime = fromDate;
            $scope.showGrid = false;

            $scope.gridReady = function (api) {
                gridApi = api;
            };

            $scope.searchClicked = function () {
                $scope.showGrid = true;
                return gridApi.loadGrid(getGridQuery());
            };

            $scope.mappingResults = UtilsService.getArrayEnum(VR_Integration_MappingResultEnum);

            $scope.onDataSourceSelectorReady = function (api) {
                dataSourceDirectiveAPI = api;
                dataSourceReadyPromiseDeferred.resolve();
            };

            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.selectedFromDateTime, $scope.selectedToDateTime);
            };

        }

        function load() {
            $scope.isLoading = true;
            return loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDatasourceSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function getGridQuery() {
            var query = {
                DataSourceId: dataSourceDirectiveAPI.getSelectedIds(),
                BatchName: ($scope.batchName != undefined && $scope.batchName != "") ? $scope.batchName : null,
                MappingResults: getMappedMappingResults(),
                From: $scope.selectedFromDateTime,
                To: $scope.selectedToDateTime
            };
            return query;

        }

        function loadDatasourceSelector() {
            var dataSourceLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            dataSourceReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, undefined, dataSourceLoadPromiseDeferred);
                });
            return dataSourceLoadPromiseDeferred.promise;
        }

        function getMappedMappingResults() {

            if ($scope.selectedMappingResults.length == 0) {
                // select all
                $scope.selectedMappingResults = UtilsService.getArrayEnum(VR_Integration_MappingResultEnum);
            }

            var mappedMappingResults = [];

            for (var i = 0; i < $scope.selectedMappingResults.length; i++) {
                mappedMappingResults.push($scope.selectedMappingResults[i].value);
            }

            return mappedMappingResults;
        }

    }

    return directiveDefinitionObject;

}]);

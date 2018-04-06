"use strict";

app.directive("vrIntegrationImportedbatchSearch", ['VR_Integration_MappingResultEnum', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService', 'VRDateTimeService', '$filter','UISettingsService',
function (VR_Integration_MappingResultEnum, UtilsService, VRNotificationService, VRUIUtilsService, VRValidationService, VRDateTimeService, $filter, UISettingsService) {

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

        var dataSourceId;
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
                directiveAPI.load = function (payload) {
                    if (payload != undefined) {
                        dataSourceId = payload.dataSourceId;
                        $scope.usedInDrillDown = dataSourceId != undefined;
                    }
                    return load();
                };
                return directiveAPI;
            }

        }

        function defineScope() {
            $scope.usedInDrillDown = false;
            $scope.mappingResults = [];
            $scope.selectedMappingResults = [];
            $scope.importedBatches = [];
            var fromDate = VRDateTimeService.getNowDateTime();
            fromDate.setHours(0, 0, 0, 0);
            $scope.selectedFromDateTime = fromDate;
            $scope.top = 100;
            $scope.maxNumberOfRecords = UISettingsService.getMaxSearchRecordCount();
            $scope.showGrid = false;

            $scope.gridReady = function (api) {
                gridApi = api;
                if (dataSourceId != undefined)
                  return $scope.searchClicked();
            };

            $scope.searchClicked = function () {
                $scope.showGrid = true;
                return gridApi.loadGrid(getGridQuery());
            };

            $scope.mappingResults = $filter('orderBy')(UtilsService.getArrayEnum(VR_Integration_MappingResultEnum), 'description');

            $scope.onDataSourceSelectorReady = function (api) {
                dataSourceDirectiveAPI = api;
                dataSourceReadyPromiseDeferred.resolve();
            };

            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.selectedFromDateTime, $scope.selectedToDateTime);
            };
            $scope.checkMaxNumberResords = function () {
                if ($scope.top <= $scope.maxNumberOfRecords || $scope.maxNumberOfRecords == undefined) {
                    return null;
                }
                else {
                    return "Max top value can be entered is: " + $scope.maxNumberOfRecords;
                }
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
                To: $scope.selectedToDateTime,
                Top: $scope.top
            };
            return query;

        }

        function loadDatasourceSelector() {
            var dataSourceLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            dataSourceReadyPromiseDeferred.promise
                .then(function () {
                    var selectorPayload;
                    if (dataSourceId != undefined)
                        selectorPayload = { selectedIds: dataSourceId };
                    VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, selectorPayload, dataSourceLoadPromiseDeferred);
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

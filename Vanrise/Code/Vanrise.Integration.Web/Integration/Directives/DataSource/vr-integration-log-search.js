"use strict";

app.directive("vrIntegrationLogSearch", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRValidationService", 'VRDateTimeService', '$filter', 'UISettingsService',
function (UtilsService, VRNotificationService, VRUIUtilsService, VRValidationService, VRDateTimeService, $filter, UISettingsService) {

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
        templateUrl: "/Client/Modules/Integration/Directives/DataSource/Templates/DataSourceLogSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {


        var gridApi;
        var dataSourceDirectiveAPI;
        var dataSourceId;
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
            $scope.showGrid = false;
            $scope.usedInDrillDown = false;
            $scope.dataSources = [];
            $scope.severities = [];
            $scope.selectedSeverities = [];
            var fromDate = VRDateTimeService.getNowDateTime();
            fromDate.setHours(0, 0, 0, 0);
            $scope.selectedFromDateTime = fromDate;
            $scope.top = 100;
            $scope.maxNumberOfRecords = UISettingsService.getMaxSearchRecordCount();

            $scope.gridReady = function (api) {
                gridApi = api;
                if (dataSourceId != undefined)
                    return $scope.searchClicked();
            };
            $scope.searchClicked = function () {
                $scope.showGrid = true;
                return gridApi.loadGrid(getQueryGrid());
            };
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
            return UtilsService.waitMultipleAsyncOperations([loadSeverities, loadDatasourceSelector]).then(function () {

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            }).finally(function () {
                $scope.isLoading = false;
            });
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

        function getQueryGrid() {
            var query = {
                DataSourceId: dataSourceDirectiveAPI.getSelectedIds(),
                Severities: getMappedSelectedSeverities(),
                From: ($scope.selectedFromDateTime != undefined) ? $scope.selectedFromDateTime : null,
                To: ($scope.selectedToDateTime != undefined) ? $scope.selectedToDateTime : null,
                Top: $scope.top
            };
            return query
        }

        function loadSeverities() {

            var data = $filter('orderBy')(UtilsService.getLogEntryType(), 'description');
            $scope.severities = data;
        }

        function getMappedSelectedSeverities() {

            if ($scope.selectedSeverities.length == 0) {
                var logEntryType = UtilsService.getLogEntryType();
                logEntryType.splice(3, 1); // remove Verbose

                $scope.selectedSeverities = logEntryType; // select Error, Warning and Information only
            }

            var mappedSelectedSeverities = [];

            for (var i = 0; i < $scope.selectedSeverities.length; i++) {
                mappedSelectedSeverities.push($scope.selectedSeverities[i].value);
            }

            return mappedSelectedSeverities;
        }

    }

    return directiveDefinitionObject;

}]);

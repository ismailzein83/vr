(function (appControllers) {

    "use strict";

    whSAnalyticsTrafficMonitorReportController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];
    function whSAnalyticsTrafficMonitorReportController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {

        var chartApi;
        var gridApi;
        var filterDirectiveAPI;
        var filterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var measures = [];
        var periods = [];
        var dimensions = [];
        var tableId = 2;
        defineScope();
        load();


        function defineScope() {

            $scope.onGridReady = function (api) {
                gridApi = api;
            };

            $scope.onFilterDirectivectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                return UtilsService.waitMultipleAsyncOperations([loadGrid]).finally(function () {

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }

        function loadGrid() {
            return gridApi.load(getQuery());
        }

        function getQuery() {
            var selectedObject = filterDirectiveAPI.getData();
            var query = {
                Settings: GetAnalyticWidgetSettings(),
                DimensionFilters: selectedObject.selectedfilters,
                GroupingDimensions: getGroupingDimensions(),
                Measures: measures,
                FromTime: selectedObject.fromdate,
                ToTime: selectedObject.todate,
                TableId: tableId
            };
            return query;
        }

        function load() {
            loadDimensions();
            loadMeasures();
            $scope.isLoadingFilter = true;
            return UtilsService.waitMultipleAsyncOperations([loadFilterSection]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                $scope.isLoadingFilter = false;
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }

        function loadFilterSection() {
            var loadFilterPromiseDeferred = UtilsService.createPromiseDeferred();
            filterReadyPromiseDeferred.promise.then(function () {
                var payload = loadPayload();
                VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, payload, loadFilterPromiseDeferred);
            });
            return loadFilterPromiseDeferred.promise;
        }

        function loadPayload() {
            var payload = {};
            payload.dimensions = dimensions;
            payload.measures = measures;
            payload.filters = [];
            payload.measureThresholds = [];

            return payload;
        }

        function loadDimensions() {
            dimensions.length = 0;
            dimensions.push(GetMockDimension('SaleZone', false, 'Sale Zone', false, null));
            dimensions.push(GetMockDimension('SaleCode', false, 'Sale Code', false, 'SaleZone'));
            dimensions.push(GetMockDimension('SupplierZone', false, 'Supplier Zone', true, 'Supplier'));
            dimensions.push(GetMockDimension('Customer', false, 'Customer', false, null));
            dimensions.push(GetMockDimension('Supplier', false, 'Supplier', false, null));
        }

        function loadMeasures() {
            measures.length = 0;
            measures.push(GetMockMeasure('DeliveredAttempts', 'Delivered Attempts'));
            measures.push(GetMockMeasure('NumberOfCalls', 'Number Of Calls'));
            measures.push(GetMockMeasure('SuccessfulAttempts', 'Successful Attempts'));
            measures.push(GetMockMeasure('DeliveredNumberOfCalls', 'AVG Delivered Calls Number'));
        }

        function getGroupingDimensions() {
            return filterDirectiveAPI.getData().selecteddimensions;
            //var groupingDimensions = [];
            //groupingDimensions.push(GetMockDimension('SaleZone', false, 'Sale Zone'));
            //groupingDimensions.push(GetMockDimension('SupplierZone', false, 'Supplier Zone'));
            //return groupingDimensions;
        }

        function GetMockDimension(name, isRoot, title, isRequired, parentDimension) {
            return {
                DimensionName: name,
                IsRootDimension: isRoot,
                Title: title,
                ParentDimension: parentDimension,
                IsRequiredFromParent: isRequired
            };
        }
        function GetMockMeasure(name, title) {
            return {
                MeasureName: name,
                Title: title
            };
        }

        function GetAnalyticWidgetSettings() {
            return {
                RootDimensionsFromSearchSection: false,
                Dimensions: dimensions,
                Measures: measures
            };
        }
    }
    appControllers.controller('WhS_Analytics_TrafficMonitorReportController', whSAnalyticsTrafficMonitorReportController);

})(appControllers);
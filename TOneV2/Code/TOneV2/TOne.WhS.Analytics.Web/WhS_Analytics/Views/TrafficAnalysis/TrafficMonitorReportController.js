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

        defineScope();
        load();


        function defineScope() {


            $scope.onGridReady = function (api) {
                gridApi = api;
            }

            $scope.onFilterDirectivectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterReadyPromiseDeferred.resolve();
            }

            $scope.searchClicked = function () {
                return UtilsService.waitMultipleAsyncOperations([loadGrid]).finally(function () {

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }

        function loadGrid() {
            return gridApi.loadGrid(getQuery());
        }

        function getQuery() {
            var selectedObject = filterDirectiveAPI.getData();
            var query = {
                Filters: selectedObject.selectedfilters,
                DimensionFields: selectedObject.selecteddimensions,
                FixedDimensionFields: selectedObject.selectedperiod,
                MeasureFields: measures,
                Dimensions: dimensions,
                FromTime: selectedObject.fromdate,
                ToTime: selectedObject.todate,
                Currency: selectedObject.currency,
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
            dimensions.push({
                value: 'SaleZone',
                description: 'Sale Zone'
            });
            //dimensions.push('SupplierZone');
        }

        function loadMeasures() {
            measures.length = 0;
            measures.push('DeliveredAttempts');
        }

    }
    appControllers.controller('WhS_Analytics_TrafficMonitorReportController', whSAnalyticsTrafficMonitorReportController);

})(appControllers);
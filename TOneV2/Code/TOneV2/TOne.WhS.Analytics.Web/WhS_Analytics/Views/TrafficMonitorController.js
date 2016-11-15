(function (appControllers) {

    "use strict";

    trafficMonitorController.$inject = ['$scope', 'WhS_Analytics_GenericAnalyticDimensionEnum', 'WhS_Analytics_GenericAnalyticMeasureEnum', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];
    function trafficMonitorController($scope, WhS_Analytics_GenericAnalyticDimensionEnum, WhS_Analytics_GenericAnalyticMeasureEnum, UtilsService, VRNotificationService, VRUIUtilsService) {

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
            $scope.showCharts = false;
            $scope.onReadyGenericChart = function (api) {
                chartApi = api;
            };

            $scope.onReadyGenericGrid = function (api) {
                gridApi = api;
            };

            $scope.onFilterDirectivectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                $scope.showCharts = true;
                return UtilsService.waitMultipleAsyncOperations([loadGrid, loadCharts]).finally(function () {

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

        function loadCharts() {
            var payload = getQuery();
            payload.measures = loadMeasuresChart();
            return chartApi.loadChart(payload);
        }

        function load() {
            $scope.isLoadingFilter = true;
            return UtilsService.waitMultipleAsyncOperations([loadFilterSection, loadMeasures]).catch(function (error) {
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

        function loadDimensions() {

            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Customer.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Country.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Switch.value);
            // dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.GateWayIn.value);
            // dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.GateWayOut.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.PortIn.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.PortOut.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.CodeSales.value);
            dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.CodeBuy.value);
            return dimensions;
        }

        function loadPayload() {
            var payload = {};
            payload.dimensions = loadDimensions();

            payload.filters = [];
            payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Customer.value);
            payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value);
            payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value);
            payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Country.value);
            payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Switch.value);
            payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.PortIn.value);
            payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.PortOut.value);

            payload.measureThresholds = [];
            payload.measureThresholds.push(WhS_Analytics_GenericAnalyticMeasureEnum.ASR.value);
            payload.measureThresholds.push(WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value);
            payload.measureThresholds.push(WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.value);
            return payload;
        }

        function loadMeasures() {
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.DeliveredAttempts.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.SuccessfulAttempts.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.DurationsInSeconds.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.FailedAttempts.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.CeiledDuration.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.ASR.value);
          //   measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.NER.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value);
             measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.ABR.value);
            // 
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.PDDInSeconds.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.PGAD.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.MaxDurationInSeconds.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.LastCDRAttempt.value);
        }

        function loadMeasuresChart() {
            var measures = [];
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.value);
            measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.DurationsInSeconds.value);
         //   measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value);
            // measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.NER.value);

            return measures;
        }
    }
    appControllers.controller('WhS_Analytics_TrafficMonitorController', trafficMonitorController);

})(appControllers);
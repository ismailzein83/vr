(function (appControllers) {

    "use strict";

    GHourlyReportController.$inject = ['$scope', 'GenericAnalyticDimensionEnum', 'GenericAnalyticMeasureEnum', 'UtilsService', 'VRNotificationService'];
    function GHourlyReportController($scope, GenericAnalyticDimensionEnum, GenericAnalyticMeasureEnum, UtilsService, VRNotificationService) {

        load();
        var chartApi;
        var gridApi;
        function defineScope() {
            $scope.onReadyGeneralChart = function (api) {
                chartApi = api;
            }

            $scope.onReadyGeneralGrid = function (api) {
                gridApi = api;
            }

            $scope.subViewConnector = {};

            $scope.dimensions = [];
            $scope.periods = [];            
            $scope.filters = [];
            $scope.measures = [];
            $scope.measureChart = [];
            
            $scope.selectedobject = {};

            loadDimensions();
            loadPeriods();
            loadFilters();
            loadMeasures();
            loadMeasuresChart();

            $scope.searchClicked = function () {

                return UtilsService.waitMultipleAsyncOperations([retrieveData, retrieveDataChart]).finally(function () {

                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
        }

        function retrieveData() {
            
            gridApi.LoadGrid();

            //var query = {
            //    Filters: $scope.selectedobject.selectedfilters,
            //    DimensionFields: $scope.selectedobject.selecteddimensions,
            //    FixedDimensionFields: $scope.selectedobject.selectedperiod,
            //    MeasureFields: $scope.measures,
            //    FromTime: $scope.selectedobject.fromdate,
            //    ToTime: $scope.selectedobject.todate,
            //    Currency: $scope.selectedobject.currency,
            //    WithSummary: true
            //};
            //return $scope.subViewConnector.retrieveData(query);
        }

        function retrieveDataChart() {
            chartApi.LoadChart();
        }

        function load() {
            defineScope();
        }

        function loadDimensions() {
            $scope.dimensions.push(GenericAnalyticDimensionEnum.Customer);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.Zone);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.CodeGroup);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.Switch);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.GateWayIn);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.GateWayOut);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.PortIn);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.PortOut);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.CodeSales);
            $scope.dimensions.push(GenericAnalyticDimensionEnum.CodeBuy);
        }

        function loadPeriods() {
            $scope.periods.push(GenericAnalyticDimensionEnum.Hour);
            $scope.periods.push(GenericAnalyticDimensionEnum.Day);
            //$scope.periods.push(GenericAnalyticDimensionEnum.Week);
            //$scope.periods.push(GenericAnalyticDimensionEnum.Month);
        }

        function loadFilters() {
            $scope.filters.push(GenericAnalyticDimensionEnum.Customer);
            $scope.filters.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.filters.push(GenericAnalyticDimensionEnum.Zone);
            $scope.filters.push(GenericAnalyticDimensionEnum.CodeGroup);
            $scope.filters.push(GenericAnalyticDimensionEnum.Switch);
            $scope.filters.push(GenericAnalyticDimensionEnum.PortIn);
            $scope.filters.push(GenericAnalyticDimensionEnum.PortOut);
            $scope.filters.push(GenericAnalyticDimensionEnum.ASR);
            $scope.filters.push(GenericAnalyticDimensionEnum.ACD);
            $scope.filters.push(GenericAnalyticDimensionEnum.Attempts);
        }

        function loadMeasures() {
            $scope.measures.push(GenericAnalyticMeasureEnum.DeliveredAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.SuccessfulAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.ASR);
            $scope.measures.push(GenericAnalyticMeasureEnum.NER);
            $scope.measures.push(GenericAnalyticMeasureEnum.ACD);
            $scope.measures.push(GenericAnalyticMeasureEnum.GrayArea);
            $scope.measures.push(GenericAnalyticMeasureEnum.GreenArea);
            $scope.measures.push(GenericAnalyticMeasureEnum.CapacityUsageDetails);
            $scope.measures.push(GenericAnalyticMeasureEnum.FailedAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.DurationsInSeconds);
            $scope.measures.push(GenericAnalyticMeasureEnum.LastCDRAttempt);
        }

        function loadMeasuresChart() {
            $scope.measureChart.push({
                measure: (GenericAnalyticMeasureEnum.SuccessfulAttempts)
            });

            $scope.measureChart.push({
                measure: (GenericAnalyticMeasureEnum.ASR)
            });

            $scope.measureChart.push({
                measure: (GenericAnalyticMeasureEnum.ACD)
            });

            $scope.measureChart.push({
                measure: (GenericAnalyticMeasureEnum.NER)
            });
        }
    }
    appControllers.controller('Analytics_GHourlyReportController', GHourlyReportController);

})(appControllers);
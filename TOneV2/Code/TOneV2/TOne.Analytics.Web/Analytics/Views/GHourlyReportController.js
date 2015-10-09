(function (appControllers) {

    "use strict";

    GHourlyReportController.$inject = ['$scope', 'GenericAnalyticDimensionEnum', 'GenericAnalyticMeasureEnum', 'UtilsService', 'VRNotificationService'];
    function GHourlyReportController($scope, GenericAnalyticDimensionEnum, GenericAnalyticMeasureEnum, UtilsService, VRNotificationService) {

        load();
        var chartApi;
        function defineScope() {
            $scope.onReadyGeneralChart = function (api) {
                chartApi = api;
            }
            $scope.subViewConnector = {};

            $scope.groupKeys = [];
            $scope.periods = [];            
            $scope.filterKeys = [];
            $scope.measures = [];
            $scope.measureChart = [];
            
            $scope.selectedobject = {};

            loadGroupKeys();
            loadFixedGroupKeys();
            loadFilterKeys();
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
            var query = {
                Filters: $scope.selectedobject.selectedfilters,
                DimensionFields: $scope.selectedobject.selecteddimensions,
                FixedDimensionFields: $scope.selectedobject.selectedperiod,
                MeasureFields: $scope.measures,
                FromTime: $scope.selectedobject.fromdate,
                ToTime: $scope.selectedobject.todate,
                Currency: $scope.selectedobject.currency
            };
            return $scope.subViewConnector.retrieveData(query);
        }

        function retrieveDataChart() {
            chartApi.LoadChart();
        }

        function load() {
            defineScope();
        }

        function loadGroupKeys() {
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Customer);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Zone);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.CodeGroup);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Switch);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.GateWayIn);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.GateWayOut);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.PortIn);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.PortOut);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.CodeSales);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.CodeBuy);
        }

        function loadFixedGroupKeys() {
            $scope.periods.push(GenericAnalyticDimensionEnum.Hour);
            $scope.periods.push(GenericAnalyticDimensionEnum.Day);
            //$scope.periods.push(GenericAnalyticDimensionEnum.Week);
            //$scope.periods.push(GenericAnalyticDimensionEnum.Month);
        }

        function loadFilterKeys() {
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Customer);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Zone);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.CodeGroup);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Switch);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.PortIn);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.PortOut);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.ASR);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.ACD);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Attempts);
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
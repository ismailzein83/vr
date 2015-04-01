appControllers.controller('ZoneMonitorController',
    function ZoneMonitorController($scope, AnalyticsAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();
        var chartZonesAPI;
        function defineScopeObjects() {
            $scope.testModel = 'ZoneMonitorController';

            $scope.optionsTopCount = {
                datasource: [
                { description: "5", value: "5" },
                { description: "10", value: "10" },
                { description: "15", value: "15" },
                { description: "20", value: "25" }
                ]
            };
            $scope.optionsTopCount.lastselectedvalue = $scope.optionsTopCount.datasource[1];

            $scope.optionsMeasureTypes = {
                datasource: []
            };
        }
        
        function defineScopeMethods() {

            $scope.chartZonesReady = function (api) {
                chartZonesAPI = api;
            };

            $scope.getData = function () {
                getData();
            };
        }

        function load() {
            $scope.fromDate = '2014-08-01';
            $scope.toDate = '2012-08-02';
            loadMeasureTypes();
        }

        function getData() {
            if (!chartZonesAPI)
                return;
            chartZonesAPI.showLoader();
            var count = $scope.optionsTopCount.lastselectedvalue.value;
            var measure = $scope.optionsMeasureTypes.lastselectedvalue;
            AnalyticsAPIService.GetTrafficStatisticSummary('', [4], $scope.fromDate, $scope.toDate, 1, count, measure.value, true).then(function (response) {
                var chartData = response;
                var chartDefinition = {
                    type: "pie",
                    title: "TOP DESTINATIONS",
                    yAxisTitle: "Value",
                    showLegendsWithValues: $scope.showValuesOnLegends
                };

                var seriesDefinitions = [{
                    title: measure.description,
                    titlePath: "GroupKeyValues[0]",
                    valuePath: measure.description
                }];

                chartZonesAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
            })
                .finally(function () {
                    chartZonesAPI.hideLoader();
                });
        }

        function loadMeasureTypes() {
            AnalyticsAPIService.GetTrafficStatisticMeasureList().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.optionsMeasureTypes.datasource.push({
                        value: itm.Value,
                        description: itm.Description
                    });
                    $scope.optionsMeasureTypes.lastselectedvalue = $scope.optionsMeasureTypes.datasource[0];
                });
            });
        }
        
    });
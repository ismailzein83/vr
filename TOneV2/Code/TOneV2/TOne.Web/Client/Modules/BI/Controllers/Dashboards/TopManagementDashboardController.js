﻿appControllers.controller('TopManagementDashboardController',
    function TopManagementDashboardController($scope, BIAPIService, BIUtilitiesService, BITimeDimensionTypeEnum) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {


            $scope.testModel = 'TopManagementDashboardController';
           
            $scope.timeDimensionTypesOption = {
                datasource: []
            };

            for (prop in BITimeDimensionTypeEnum) {
                var obj = {
                    name: BITimeDimensionTypeEnum[prop].description,
                    value: BITimeDimensionTypeEnum[prop].value
                };
                $scope.timeDimensionTypesOption.datasource.push(obj);
                if (obj.value == BITimeDimensionTypeEnum.Daily.value)
                    $scope.timeDimensionTypesOption.lastselectedvalue = obj;
            }
            

            $scope.fromDate = '2012-01-02';
            $scope.toDate = '2012-04-28';

            $scope.profit = [];

        }

        function defineScopeMethods() {

            $scope.chartSaleCostProfitReady = function (api) {
                $scope.chartSaleCostProfitAPI = api;
                updateProfitChart();
            };

            $scope.chartProfitReady = function (api) {
                $scope.chartProfitAPI = api;
                updateProfitChart();
            };

            $scope.updateChart = function () {
                updateProfitChart();
            };
        }

        function load() {
        }

        function updateProfitChart() {

            if ($scope.chartSaleCostProfitAPI == undefined)
                return;

            if ($scope.chartProfitAPI == undefined)
                return;
            
            $scope.profit.length = 0;
            $scope.chartSaleCostProfitAPI.showLoader();
            $scope.chartProfitAPI.showLoader();
            var selectedTimeDimension = $scope.timeDimensionTypesOption.lastselectedvalue;
            var fromDate = $scope.fromDate;
            var toDate = $scope.toDate;
            BIAPIService.GetMeasureValues(selectedTimeDimension.value, fromDate, toDate, [1, 2, 3])
                .then(function (response) {
                    BIUtilitiesService.fillDateTimeProperties(response, selectedTimeDimension.value, fromDate, toDate);
                    angular.forEach(response, function (item) {
                        $scope.profit.push(item);
                    });

                    var chartData = $scope.profit;
                    var chartDefinition = {
                        type: "column",
                        title: "Cost/Sale/Profit",
                        yAxisTitle: "Value"
                    };
                    var xAxisDefinition = { titlePath: "dateTimeValue", groupNamePath: "dateTimeGroupValue" };
                    var seriesDefinitions = [ {
                        title: "PROFIT",
                        valuePath: "Values[2]",
                        type: "spline"
                    },{
                        title: "SALE",
                        valuePath: "Values[0]"
                    }, {
                        title: "COST",
                        valuePath: "Values[1]"
                    }
                    ];

                    $scope.chartSaleCostProfitAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);

                    var chartDefinition2 = {
                        type: "column",
                        title: "Profit",
                        yAxisTitle: "Value"
                    };
                    var xAxisDefinition2 = { titlePath: "dateTimeValue", groupNamePath: "dateTimeGroupValue" };
                    var seriesDefinitions2 = [{
                        title: "PROFIT",
                        valuePath: "Values[2]"
                    }
                    ];
                    $scope.chartProfitAPI.renderChart(chartData, chartDefinition2, seriesDefinitions2, xAxisDefinition2);
                })
                .finally(function () {
                    $scope.chartSaleCostProfitAPI.hideLoader();
                    $scope.chartProfitAPI.hideLoader();
                });
        }

    });
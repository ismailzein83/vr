appControllers.controller('TopManagementDashboardController',
    function TopManagementDashboardController($scope, BIAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {


            $scope.testModel = 'TopManagementDashboardController';
            $scope.timeDimensionTypes = [{ name: "Daily", value: 0, fromDate: '2012-1-2', toDate: '2012-2-28' },
            { name: "Weekly", value: 1, fromDate: '2012-1-2', toDate: '2012-04-28' },
            { name: "Monthly", value: 2, fromDate: '2012-1-2', toDate: '2013-12-31' },
            { name: "Yearly", value: 3, fromDate: '2012-1-2', toDate: '2014-1-1' }, ];

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
            $scope.selectedTimeDimensionType = $scope.timeDimensionTypes[0];
            
        }

        function updateProfitChart() {

            if ($scope.chartSaleCostProfitAPI == undefined)
                return;

            if ($scope.chartProfitAPI == undefined)
                return;
            
            $scope.profit.length = 0;
            $scope.chartSaleCostProfitAPI.showLoader();
            $scope.chartProfitAPI.showLoader();

            BIAPIService.GetProfit($scope.selectedTimeDimensionType.value, $scope.selectedTimeDimensionType.fromDate, $scope.selectedTimeDimensionType.toDate)
                .then(function (response) {

                    angular.forEach(response, function (item) {
                        $scope.profit.push(item);
                    });

                    var chartData = $scope.profit;
                    var chartDefinition = {
                        type: "column",
                        title: "Cost/Sale/Profit",
                        yAxisTitle: "Value"
                    };
                    var xAxisDefinition = { titleFieldName: "TimeValue", groupFieldName: "TimeGroupName" };
                    var seriesDefinitions = [{
                        title: "COST",
                        valueFieldName: "Cost"
                    }, {
                        title: "SALE",
                        valueFieldName: "Sale"
                    }, {
                        title: "PROFIT",
                        valueFieldName: "Profit",
                        type: "spline"
                    }
                    ];

                    $scope.chartSaleCostProfitAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);

                    var chartDefinition2 = {
                        type: "column",
                        title: "Profit",
                        yAxisTitle: "Value"
                    };
                    var xAxisDefinition2 = { titleFieldName: "TimeValue", groupFieldName: "TimeGroupName" };
                    var seriesDefinitions2 = [{
                        title: "PROFIT",
                        valueFieldName: "Profit"
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
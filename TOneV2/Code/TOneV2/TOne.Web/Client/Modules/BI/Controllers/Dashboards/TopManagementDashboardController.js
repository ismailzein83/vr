appControllers.controller('TopManagementDashboardController',
    function TopManagementDashboardController($scope, BIAPIService, BIUtilitiesService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {


            $scope.testModel = 'TopManagementDashboardController';           
            $scope.timeDimensionTypesOption = {
                datasource: [{ name: "Daily", value: 3, fromDate: '2012-1-2', toDate: '2012-2-28' },
            { name: "Weekly", value: 2, fromDate: '2012-1-2', toDate: '2012-04-28' },
            { name: "Monthly", value: 1, fromDate: '2012-1-2', toDate: '2013-12-31' },
            { name: "Yearly", value: 0, fromDate: '2012-1-2', toDate: '2014-1-1' }, ]
            };

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
            $scope.timeDimensionTypesOption.lastselectedvalue = $scope.timeDimensionTypesOption.datasource[0];
            
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
            BIAPIService.GetMeasureValues(selectedTimeDimension.value, selectedTimeDimension.fromDate, selectedTimeDimension.toDate, [1, 2, 3])
                .then(function (response) {
                    BIUtilitiesService.fillDateTimeProperties(response, selectedTimeDimension.value, selectedTimeDimension.fromDate, selectedTimeDimension.toDate);
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
                    var xAxisDefinition2 = { titlePath: "TimeValue", groupNamePath: "TimeGroupName" };
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
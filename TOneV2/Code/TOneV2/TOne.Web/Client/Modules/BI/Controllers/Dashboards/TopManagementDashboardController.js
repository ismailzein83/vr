appControllers.controller('TopManagementDashboardController',
    function TopManagementDashboardController($scope, BIAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {
            $scope.chartProfitAPI = {};
            $scope.chartSaleCostProfitAPI = {};
            $scope.testModel = 'TopManagementDashboardController';
            $scope.timeDimensionTypes = [{ name: "Daily", value: 0, fromDate: '2012-1-2', toDate: '2012-2-28' },
            { name: "Weekly", value: 1, fromDate: '2012-1-2', toDate: '2012-04-28' },
            { name: "Monthly", value: 2, fromDate: '2012-1-2', toDate: '2013-12-31' },
            { name: "Yearly", value: 3, fromDate: '2012-1-2', toDate: '2014-1-1' }, ];

            $scope.profit = [];
           
        }

        function defineScopeMethods() {
            $scope.updateChart = function () {
                updateProfitChart();
            };
        }

        function load() {
            $scope.selectedTimeDimensionType = $scope.timeDimensionTypes[0];
            setTimeout(function () {
                updateProfitChart();
            }, 1000);
           
        }

        function updateProfitChart()
        {
           
            $scope.profit.length = 0;
            $scope.chartSaleCostProfitAPI.showLoader();
            $scope.chartProfitAPI.showLoader();
            BIAPIService.GetProfit($scope.selectedTimeDimensionType.value, $scope.selectedTimeDimensionType.fromDate, $scope.selectedTimeDimensionType.toDate).then(function (response) {
                                
                angular.forEach(response, function (item) {
                    $scope.profit.push(item);
                });

                var chartData = $scope.profit;
                var chartDefinition = {
                    type: "column",
                    title: "Cost/Sale/Profit",
                    yAxisTitle: "Value"
                };
                var xAxisDefinition = { fieldName: "TimeValue", groupFieldName: "TimeGroupName" };
                var seriesDefinitions = [{
                    title: "COST NET",
                    fieldName: "Cost"
                }, {
                    title: "SALE NET",
                    fieldName: "Sale"
                }, {
                    title: "PROFIT",
                    fieldName: "Profit",
                    type: "spline"
                }
                ];

                $scope.chartSaleCostProfitAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);

                var chartDefinition2 = {
                    type: "column",
                    title: "Profit",
                    yAxisTitle: "Value"
                };
                var xAxisDefinition2 = { fieldName: "TimeValue", groupFieldName: "TimeGroupName" };
                var seriesDefinitions2 = [{                
                    title: "PROFIT",
                    fieldName: "Profit"
                }
                ];
                $scope.chartProfitAPI.renderChart(chartData, chartDefinition2, seriesDefinitions2, xAxisDefinition2);
            }).finally(function () {
                $scope.chartSaleCostProfitAPI.hideLoader();
                $scope.chartProfitAPI.hideLoader();
            });
        }

        function renderChart(divName, chartData, chartDefinition, seriesDefinitions, xAxisDefinition) {
            var xAxis = [];
            var series = [];
            angular.forEach(seriesDefinitions, function (sDef) {
                var serie = {
                    name: sDef.title,
                    data: [],
                    type: sDef.type ? sDef.type:chartDefinition.type
                };
                series.push(serie);
            });

            angular.forEach(chartData, function (dataItem) {

                if (xAxisDefinition.groupFieldName != 'undefined' && xAxisDefinition.groupFieldName != null) {
                    var groupName = dataItem[xAxisDefinition.groupFieldName];
                    if(groupName == null)
                    {
                        xAxis.push(dataItem[xAxisDefinition.fieldName]);
                    }
                    else
                    {
                        var group = null;
                        angular.forEach(xAxis, function (grp) {
                            if(grp.name == groupName)
                                group = grp;
                        });
                        if(group == null)
                        {
                            group = {
                                name: groupName,
                                categories: []
                            };
                            xAxis.push(group);
                        }
                        group.categories.push(dataItem[xAxisDefinition.fieldName]);
                    }
                }
                else {                        
                    xAxis.push(dataItem[xAxisDefinition.fieldName]);
                }
                for (var i = 0; i < series.length; i++) {
                    series[i].data.push(Number(dataItem[seriesDefinitions[i].fieldName]));
                }
            });

            $(divName).highcharts({
                chart: {
                    options3d: {
                        enabled: false,
                        alpha: 15,
                        beta: 15,
                        depth: 50,
                        viewDistance: 25
                    }
                },
                title: {
                    text: chartDefinition.title
                },
                plotOptions: {
                    column: {
                        depth: 25
                    }
                },
                xAxis: {
                    categories: xAxis
                },
                yAxis: {
                    title: {
                        text: chartDefinition.yAxisTitle
                    }
                },
                series: series,
                tooltip: {
                    shared: true
                },
            }); 
        }

        
    });
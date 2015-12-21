(function (appControllers) {

    "use strict";

    Qm_CliTester_DashboardManagementController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService'];

    function Qm_CliTester_DashboardManagementController($scope, Qm_CliTester_TestCallAPIService) {
            defineScope();
            function defineScope() {
                $scope.isGettingData = true;
                var chartApi;
                $scope.searchClicked = function () {
                    return loadCharts();
                };

                $scope.chartSelectedEntityReady = function (api) {
                    chartApi = api;
                }

                function loadCharts() {
                    var groupKeys = [];
                    var measureChartValues = [];
                    //$scope.MeasureChart = value.MeasureChart;
                    //if (value.FixedDimensionFields == undefined)
                    //    value.FixedDimensionFields = [];

                    //value.FixedDimensionFields.forEach(function (group) {
                    //    groupKeys.push(group.value);

                    //    if (group == GenericAnalyticDimensionEnum.Hour) {
                    //        groupKeys.push(GenericAnalyticDimensionEnum.Date.value);
                    //    }
                    //});

                    //for (var i = 0, len = value.MeasureChart.length; i < len; i++) {
                    //    measureChartValues.push(value.MeasureChart[i].measure.value);
                    //}

                    return Qm_CliTester_TestCallAPIService.GetTotalCallsByUserId()
                        .then(function (response) {

                            $scope.isGettingEntityStatistics = true;

                            var chartData = new Array();
                            var chartDefinition = [];
                            var dates = [];
                            var y = 0;
                            var seriesDefinitions = [];
                            var countDates = 0;


                            //console.log(response);

                            for (var i = 0; i < response.length; i++) {

                                seriesDefinitions.push({
                                    title: response[i].CreationDate,
                                    valuePath: 'TotalCalls'
                                });



                                chartDefinition.push({
                                    type: "column",
                                    title: "CreationDate",
                                    yAxisTitle: "CreationDate"
                                });

                                var values = {};
                                values['TotalCalls'] = response[i].TotalCalls;
                                chartData.push(values);
                            }


                            var xAxisDefinition = {
                                titlePath: "CreationDate",
                                isDateTime: false
                            };

                            console.log(chartData);
                            console.log(chartDefinition);

                            console.log(seriesDefinitions);
                            console.log(xAxisDefinition);

                            chartApi.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
                            
                            $scope.isGettingEntityStatistics = false;
                        });
                };
            }
    }

    appControllers.controller('Qm_CliTester_DashboardManagementController', Qm_CliTester_DashboardManagementController);
})(appControllers);
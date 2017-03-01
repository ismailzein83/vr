(function (appControllers) {

    "use strict";

    SwitchDashboardManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService','NP_IVSwitch_SwitchDashboardAPIService','LabelColorsEnum','VRTimerService'];
    function SwitchDashboardManagementController($scope, UtilsService, vruiUtilsService, NP_IVSwitch_SwitchDashboardAPIService, LabelColorsEnum, VRTimerService) {

       var customerChartAPI;
       var customerChartReadyDeferred = UtilsService.createPromiseDeferred();

       var supplierChartAPI;
       var supplierChartReadyDeferred = UtilsService.createPromiseDeferred();

       var destinationChartAPI;
       var destinationChartReadyDeferred = UtilsService.createPromiseDeferred();

       var lastDistributionChartAPI;
       var lastDistributionChartReadyDeferred = UtilsService.createPromiseDeferred();

       var guageChartAPI;
       var guageChartReadyDeferred = UtilsService.createPromiseDeferred();

       var solidguageChartAPI;
       var solidguageChartReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.customerChartReady = function(api)
            {
                customerChartAPI = api;
                customerChartReadyDeferred.resolve();
            }
            $scope.scopeModel.supplierChartReady = function (api) {
                supplierChartAPI = api;
                supplierChartReadyDeferred.resolve();
            }
            $scope.scopeModel.destinationChartReady = function (api) {
                destinationChartAPI = api;
                destinationChartReadyDeferred.resolve();
            }
            $scope.scopeModel.getACDColor = function(dataItem)
            {
                if(dataItem.ACD > 0 && dataItem.ACD <= 5)
                {
                    return LabelColorsEnum.Error.color;
                } else if (dataItem.ACD > 5 && dataItem.ACD <= 30)
                {
                    return LabelColorsEnum.Warning.color;
                } else if (dataItem.ACD > 30 && dataItem.ACD <= 60) {
                    return LabelColorsEnum.Processed.color;
                }
            }
            $scope.scopeModel.getPDDColor = function (dataItem) {
                if (dataItem.PDDInSec > 3) {
                    return LabelColorsEnum.Error.color;
                }
            }
            $scope.scopeModel.getPercConnectedColor = function (dataItem) {
                if (dataItem.PercConnected > 0 && dataItem.PercConnected <= 50) {
                    return LabelColorsEnum.Error.color;
                } else if (dataItem.PercConnected > 50 && dataItem.PercConnected <= 80) {
                    return LabelColorsEnum.Warning.color;
                } else if (dataItem.PercConnected > 80 && dataItem.PercConnected <= 100) {
                    return LabelColorsEnum.Processed.color;
                }
            }
            $scope.scopeModel.lastDistributionChartReady = function (api) {
                lastDistributionChartAPI = api;
                lastDistributionChartReadyDeferred.resolve();
            }
            $scope.scopeModel.topCustomers = [];
            $scope.scopeModel.topSuppliers = [];

            $scope.scopeModel.guageChartReady = function(api)
            {
                guageChartAPI = api;
                guageChartReadyDeferred.resolve();
            }
            $scope.scopeModel.solidguageChartReady = function (api) {
                solidguageChartAPI = api;
                solidguageChartReadyDeferred.resolve();
            }

        }
        function load()
        {
            $scope.scopeModel.isLoading = true;
            VRTimerService.registerJob(loadDashboarResult, $scope, 10);
        }
        function loadDashboarResult()
        {
            return NP_IVSwitch_SwitchDashboardAPIService.GetSwitchDashboardManagerResult().then(function (response) {
                if (response)
                {
                    if (response.TopCustomersResult != undefined && response.TopCustomersResult.CustomerResults != undefined)
                    {
                        $scope.scopeModel.topCustomers.length = 0;
                        for(var i=0;i<response.TopCustomersResult.CustomerResults.length;i++)
                        {
                            var customerResult = response.TopCustomersResult.CustomerResults[i];
                            $scope.scopeModel.topCustomers.push({
                                CustomerName: customerResult.CustomerName,
                                PercConnected: customerResult.PercConnected,
                                ACD: customerResult.ACD,
                                PDDInSec: customerResult.PDDInSec,
                                TotalDuration: customerResult.TotalDuration,
                            });
                        }
                    }
                    if (response.TopSuppliersResult != undefined && response.TopSuppliersResult.SupplierResults != undefined) {
                        $scope.scopeModel.topSuppliers.length = 0;
                        for (var i = 0; i < response.TopSuppliersResult.SupplierResults.length; i++) {
                            var supplierResult = response.TopSuppliersResult.SupplierResults[i];
                            $scope.scopeModel.topSuppliers.push({
                                SupplierName: supplierResult.SupplierName,
                                PercConnected: supplierResult.PercConnected,
                                ACD: supplierResult.ACD,
                                PDDInSec: supplierResult.PDDInSec,
                                TotalDuration: supplierResult.TotalDuration,
                            });
                        }
                    }
                    renderCustomerCharts(response.TopCustomersResult);
                    renderSupplierCharts(response.TopSuppliersResult);
                    renderDestinationCharts(response.TopZonesResult);
                    renderLastDistributionCharts(response.LastDistributionResult);
                    loadGauge(response.LiveSummaryResult);
                    loadSolidGauge(response.LiveSummaryResult);

                }
                $scope.scopeModel.showCharts = true;
                $scope.scopeModel.isLoading = false;

            });
        }

        function renderCustomerCharts(topCustomersResult) {
            customerChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "column",
                    yAxisTitle: "Value"
                };
                var xAxisDefinition = {
                    titlePath: "CustomerName"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Attempts",
                    valuePath: "Attempts"
                });
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected"
                });

                customerChartAPI.renderChart(topCustomersResult.CustomerResults, chartDefinition, seriesDefinitions, xAxisDefinition);
            });
            
        }
        function renderSupplierCharts(topSuppliersResult) {
            supplierChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "column",
                    yAxisTitle: "Value"
                };
                var xAxisDefinition = {
                    titlePath: "SupplierName"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Attempts",
                    valuePath: "Attempts"
                });
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected"
                });

                supplierChartAPI.renderChart(topSuppliersResult.SupplierResults, chartDefinition, seriesDefinitions, xAxisDefinition);
            });

        }
        function renderDestinationCharts(topDestinationsResult) {
            destinationChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "column",
                    yAxisTitle: "Value"
                };
                var xAxisDefinition = {
                    titlePath: "ZoneName"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Attempts",
                    valuePath: "Attempts"
                });
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected"
                });

                destinationChartAPI.renderChart(topDestinationsResult.ZoneResults, chartDefinition, seriesDefinitions, xAxisDefinition);
            });

        }
        function renderLastDistributionCharts(lastDistributionResult) {
            lastDistributionChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "column",
                    yAxisTitle: "Value"
                };
                var xAxisDefinition = {
                    titlePath: "DurationRange"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected"
                });

                lastDistributionChartAPI.renderChart(lastDistributionResult.DistributionResults, chartDefinition, seriesDefinitions, xAxisDefinition);
            });

        }



        function loadGauge(liveSummaryResult)
        {

            guageChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "gauge",
                };
                var xAxisDefinition = {
                    titlePath: "DurationRange"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected",
                    tooltip: {
                        valueSuffix: ' %'
                    },
                });

                var yAxisDefinition = {
                    min: liveSummaryResult.CountConnected,
                    max: liveSummaryResult.Attempts,
                    mid: liveSummaryResult.PercConnected,
                    title: '%'
                };
                var items = [{
                    label :"Connected",
                    value:liveSummaryResult.CountConnected,
                    },{
                        label :"Attempts",
                        value:liveSummaryResult.Attempts,
                        }];
                guageChartAPI.renderChart(liveSummaryResult.PercConnected, chartDefinition, seriesDefinitions, xAxisDefinition, yAxisDefinition,items);
            });

            console.log(Highcharts);

            //Highcharts.chart('container', {

            //    // the value axis
                

            //    series: [{
            //        name: 'Speed',
            //        data: [80],
            //        tooltip: {
            //            valueSuffix: ' km/h'
            //        }
            //    }]

            //},
            //// Add some life
            //function (chart) {
            //    if (!chart.renderer.forExport) {
            //        setInterval(function () {
            //            var point = chart.series[0].points[0],
            //                newVal,
            //                inc = Math.round((Math.random() - 0.5) * 20);

            //            newVal = point.y + inc;
            //            if (newVal < 0 || newVal > 200) {
            //                newVal = point.y - inc;
            //            }

            //            point.update(newVal);

            //        }, 3000);
            //    }
            //});
        }

        function loadSolidGauge(liveSummaryResult) {
            solidguageChartReadyDeferred.promise.then(function () {

                var chartDefinition = {
                    type: "solidgauge",
                };
                var xAxisDefinition = {
                    titlePath: "DurationRange"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Connected",
                    tooltip: {
                        valueSuffix: ' %'
                    },
                });
                var yAxisDefinition = {
                    min: liveSummaryResult.CountConnected,
                    max: liveSummaryResult.Attempts,
                    title: '%'
                };
             

                solidguageChartAPI.renderChart(liveSummaryResult.PercConnected, chartDefinition, seriesDefinitions, xAxisDefinition, yAxisDefinition);
            });
        }
    }
    appControllers.controller('NP_IVSwitch_SwitchDashboardManagementController', SwitchDashboardManagementController);

})(appControllers);
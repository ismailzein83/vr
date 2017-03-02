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

       var liveChartAPI;
       var liveChartReadyDeferred = UtilsService.createPromiseDeferred();

       var acdGuageChartAPI;
       var acdGuageChartReadyDeferred = UtilsService.createPromiseDeferred();

       var acdLiveChartAPI;
       var acdLiveChartReadyDeferred = UtilsService.createPromiseDeferred();


       var liveObject;
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
            $scope.scopeModel.liveChartReady = function (api) {
                liveChartAPI = api;
                liveChartReadyDeferred.resolve();
            }

            $scope.scopeModel.acdGuageChartReady = function (api) {
                acdGuageChartAPI = api;
                acdGuageChartReadyDeferred.resolve();
            }
            $scope.scopeModel.acdLiveChartReady = function (api) {
                acdLiveChartAPI = api;
                acdLiveChartReadyDeferred.resolve();
            }

        }
        function load()
        {
            $scope.scopeModel.isLoading = true;
            VRTimerService.registerJob(loadDashboarResult, $scope, 3);
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
                    if (!$scope.scopeModel.showCharts)
                        loadLiveChart(response.LiveSummaryResult);
                    else
                        chartDataFunction(response.LiveSummaryResult);

                    if (!$scope.scopeModel.showCharts)
                    loadACDGauge(response.LiveSummaryResult);
                    else
                        guageACDLiveFunction(response.LiveSummaryResult);
                    if (!$scope.scopeModel.showCharts)
                        loadACDLiveChart(response.LiveSummaryResult);
                    else
                        chartACDLiveFunction(response.LiveSummaryResult);
                    $scope.scopeModel.totalDuration = response.LiveSummaryResult.TotalDuration;
                    $scope.scopeModel.pDDInSec = response.LiveSummaryResult.PDDInSec;

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
                    type: "pie",
                    yAxisTitle: "Value"
                };
                var xAxisDefinition = {
                    titlePath: "DurationRange"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected",
                    titlePath: "DurationRange"
                });

                lastDistributionChartAPI.renderSingleDimensionChart(lastDistributionResult.DistributionResults, chartDefinition, seriesDefinitions);
            });

        }


        function loadGauge(liveSummaryResult)
        {

            guageChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "solidgauge",
                    ranges: [
                            [0, '#DF5353'], // red/
                            [50, '#DDDF0D'], // yellow
                            [80, '#55BF3B'] //green
                    ]
                };
                var xAxisDefinition = {
                    titlePath: "DurationRange"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Perc Connected",
                    valuePath: "PercConnected",
                    tooltip: {
                        valueSuffix: ' %'
                    },
                });

                var yAxisDefinition = {
                    min: 0,
                    max: 100,
                   // mid: liveSummaryResult.PercConnected,
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
        }
        function loadLiveChart(liveSummaryResult) {
            liveChartReadyDeferred.promise.then(function () {

                var chartDefinition = {
                    type: "areaspline",
                    yAxisTitle: "Value",
                    numberOfPoints:20
                };
                var xAxisDefinition = {
                    titlePath: "ResponseDate",
                    isTime: true,
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected"
                });
                seriesDefinitions.push({
                    title: "Attempts",
                    valuePath: "Attempts"
                });
               

                liveChartAPI.renderChart([{ ResponseDate: UtilsService.createDateFromString(liveSummaryResult.ResponseDate), CountConnected: liveSummaryResult.CountConnected, Attempts: liveSummaryResult.Attempts }], chartDefinition, seriesDefinitions, xAxisDefinition);
            });
        }
        function chartDataFunction(liveSummaryResult)
        {
            liveChartAPI.addItem({ ResponseDate: UtilsService.createDateFromString(liveSummaryResult.ResponseDate), CountConnected: liveSummaryResult.CountConnected, Attempts: liveSummaryResult.Attempts });
        }

        function loadACDGauge(liveSummaryResult) {

            acdGuageChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "solidgauge",
                    ranges: [
                            [0.0, '#DF5353'],// red 
                            [0.1, '#DDDF0D'], // yellow
                            [0.4, '#55BF3B'], //green
                        ]
                };
                var xAxisDefinition = {
                    titlePath: "ACD"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "ACD",
                    valuePath: "ACD",
                    tooltip: {
                        valueSuffix: ' '
                    },
                });

                var yAxisDefinition = {
                    min: 0,
                    max: 50,
                  //  mid: liveSummaryResult.ACD,
                    title: ' '
                };
                var items = [{
                    label: "ACD",
                    value: liveSummaryResult.ACD,
                }];
                acdGuageChartAPI.renderChart( { ACD : liveSummaryResult.ACD }, chartDefinition, seriesDefinitions, xAxisDefinition, yAxisDefinition, items);
            });
        }
        function guageACDLiveFunction(liveSummaryResult) {
            acdGuageChartAPI.updateValue(liveSummaryResult.ACD);
        }

        function loadACDLiveChart(liveSummaryResult) {
            acdLiveChartReadyDeferred.promise.then(function () {

                var chartDefinition = {
                    type: "areaspline",
                    yAxisTitle: "Value",
                    numberOfPoints: 20
                };
                var xAxisDefinition = {
                    titlePath: "Date",
                    isTime: true,
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "ACD",
                    valuePath: "ACD"
                });


                acdLiveChartAPI.renderChart([{ Date: UtilsService.createDateFromString(liveSummaryResult.ResponseDate), ACD: liveSummaryResult.ACD }], chartDefinition, seriesDefinitions, xAxisDefinition);
            });
        }
        function chartACDLiveFunction(liveSummaryResult) {
            acdLiveChartAPI.addItem({ Date: UtilsService.createDateFromString(liveSummaryResult.ResponseDate), ACD: liveSummaryResult.ACD });
        }

    }
    appControllers.controller('NP_IVSwitch_SwitchDashboardManagementController', SwitchDashboardManagementController);

})(appControllers);
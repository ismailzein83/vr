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

       var lastACD;

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
            $scope.scopeModel.getACDColor = function (dataItem) {
                if (dataItem.ACD <= 5) {
                    return LabelColorsEnum.Error.color;
                } else if (dataItem.ACD <= 30) {
                    return LabelColorsEnum.Warning.color;
                } else {
                    return LabelColorsEnum.Processed.color;
                }
            }
            $scope.scopeModel.getPDDColor = function (dataItem) {
                if (dataItem.PDDInSec >= 5) {
                    return LabelColorsEnum.Error.color;
                }
                else if (dataItem.PDDInSec >= 3) {
                    return LabelColorsEnum.Warning.color;
                }
            }
            $scope.scopeModel.getPercConnectedColor = function (dataItem) {
                if (dataItem.PercConnected <= 50) {
                    return LabelColorsEnum.Error.color;
                } else if (dataItem.PercConnected <= 80) {
                    return LabelColorsEnum.Warning.color;
                } else {
                    return LabelColorsEnum.Processed.color;
                }
            }
          
            $scope.scopeModel.lastDistributionChartReady = function (api) {
                lastDistributionChartAPI = api;
                lastDistributionChartReadyDeferred.resolve();
            }
            $scope.scopeModel.topCustomers = [];
            $scope.scopeModel.topSuppliers = [];
            $scope.scopeModel.overAllResult = [];
            $scope.scopeModel.overAllDuration = [];
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
            VRTimerService.registerJob(loadDashboarResult, $scope, 2);
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
                                Attempts: customerResult.Attempts,
                                CountConnected: customerResult.CountConnected
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
                                Attempts: supplierResult.Attempts,
                                CountConnected: supplierResult.CountConnected

                            });
                        }
                    }
                 




                    if (!$scope.scopeModel.showCharts)
                    {
                        renderLastDistributionCharts(response.LastDistributionResult);
                        renderDestinationCharts(response.TopZonesResult);
                        renderSupplierCharts(response.TopSuppliersResult);
                        renderCustomerCharts(response.TopCustomersResult);
                        loadLiveChart(response.LiveSummaryResult);
                        loadACDLiveChart(response.LiveSummaryResult);
                        loadGauge(response.LiveSummaryResult);
                    }
                    else
                    {
                        updateLastDistributionChart(response.LastDistributionResult);
                        updateDestinationChart(response.TopZonesResult)
                        updateSupplierChart(response.TopSuppliersResult)
                        updateCustomerChart(response.TopCustomersResult)
                        chartDataFunction(response.LiveSummaryResult);
                        chartACDLiveFunction(response.LiveSummaryResult);
                        guageLiveFunction(response.LiveSummaryResult);
                    }
                    if (lastACD == undefined || lastACD != calculateMaxACD(response.LiveSummaryResult.ACD))
                    {
                        loadACDGauge(response.LiveSummaryResult);
                    } else
                    {
                        guageACDLiveFunction(response.LiveSummaryResult);
                    }

                    $scope.scopeModel.overAllResult.length = 0;

                    $scope.scopeModel.overAllResult.push({
                        label: "Attempts",
                        value: response.LiveSummaryResult.Attempts
                    });
                    $scope.scopeModel.overAllResult.push({
                        label: "Connected",
                        value: response.LiveSummaryResult.CountConnected,
                        color: $scope.scopeModel.getPercConnectedColor(response.LiveSummaryResult)

                    });

                    $scope.scopeModel.overAllDuration.length = 0;
                    $scope.scopeModel.overAllDuration.push({
                        label: "Total Dur. (m)",
                        value: response.LiveSummaryResult.TotalDuration
                    });
                    $scope.scopeModel.overAllDuration.push({
                        label: "PDD (s)",
                        value: response.LiveSummaryResult.PDDInSec,
                        color: $scope.scopeModel.getPDDColor(response.LiveSummaryResult)
                        });
                }
                $scope.scopeModel.showCharts = true;
                $scope.scopeModel.isLoading = false;

            });
        }

        function renderCustomerCharts(topCustomersResult) {
            customerChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "column",
                    yAxisTitle: " ",
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
                    valuePath: "CountConnected",
                    color: '#55BF3B'
                });

                customerChartAPI.renderChart(topCustomersResult.CustomerResults, chartDefinition, seriesDefinitions, xAxisDefinition);
            });
            
        }
        function updateCustomerChart(topCustomersResult) {
            customerChartAPI.updateValues(topCustomersResult.CustomerResults);
        }

        function renderSupplierCharts(topSuppliersResult) {
            supplierChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "column",
                    yAxisTitle: " ",
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
                    valuePath: "CountConnected",
                    color: '#55BF3B'
                });

                supplierChartAPI.renderChart(topSuppliersResult.SupplierResults, chartDefinition, seriesDefinitions, xAxisDefinition);
            });

        }
        function updateSupplierChart(topSuppliersResult) {
            supplierChartAPI.updateValues(topSuppliersResult.SupplierResults);
        }

        function renderDestinationCharts(topDestinationsResult) {
            destinationChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "column",
                    yAxisTitle: " "
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
                    valuePath: "CountConnected",
                    color: '#55BF3B'
                });

                destinationChartAPI.renderChart(topDestinationsResult.ZoneResults, chartDefinition, seriesDefinitions, xAxisDefinition);
            });

        }
        function updateDestinationChart(topDestinationsResult) {
            destinationChartAPI.updateValues(topDestinationsResult.ZoneResults);
        }

        function renderLastDistributionCharts(lastDistributionResult) {
            lastDistributionChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "pie",
                    yAxisTitle: " "
                };
                var xAxisDefinition = {
                    titlePath: "DurationRange"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Attempts",
                    valuePath: "Attempts",
                    titlePath: "DurationRange"
                });

                lastDistributionChartAPI.renderSingleDimensionChart(lastDistributionResult.DistributionResults, chartDefinition, seriesDefinitions);
            });

        }
        function updateLastDistributionChart(lastDistributionResult) {
            lastDistributionChartAPI.updateValues(lastDistributionResult.DistributionResults);
        }

        function loadGauge(liveSummaryResult)
        {

            guageChartReadyDeferred.promise.then(function () {
                var chartDefinition = {
                    type: "gauge",
                    title: "% Connected",
                    ranges: [{
                        from: 0,
                        to: 50,
                        color: '#DF5353',
                    }, {
                        from: 50,
                        to: 80,
                        color: '#f0ad4e',
                    }, {
                        from: 80,
                        to: 100,
                        color: '#55BF3B',
                    }
                    ]
                };
                var xAxisDefinition = {
                    titlePath: "DurationRange"
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "% Connected",
                    valuePath: "PercConnected",
                    tooltip: {
                        valueSuffix: ' '
                    },
                });

                var yAxisDefinition = {
                    min: 0,
                    max: 100,
                    interval: 50,
                    title: '%'
                };
                var items = [{
                    label :"Connected",
                    value:liveSummaryResult.CountConnected,
                    },{
                        label :"Attempts",
                        value:liveSummaryResult.Attempts,
                    }];

                guageChartAPI.renderChart({PercConnected: liveSummaryResult.PercConnected}, chartDefinition, seriesDefinitions, xAxisDefinition, yAxisDefinition,items);
            });
        }
        function guageLiveFunction(liveSummaryResult) {
            guageChartAPI.updateValue(liveSummaryResult.PercConnected);
        }
        function loadLiveChart(liveSummaryResult) {
            liveChartReadyDeferred.promise.then(function () {

                var chartDefinition = {
                    type: "spline",
                    yAxisTitle: " ",
                    numberOfPoints: 30,
                    enablePoints: false,
                    useAnimation:true
                };
                var xAxisDefinition = {
                    titlePath: "ResponseDate",
                    isTime: true,
                    hideAxes: true
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "Attempts",
                    valuePath: "Attempts"
                });
                seriesDefinitions.push({
                    title: "Connected",
                    valuePath: "CountConnected",
                    color: '#55BF3B'
                });
                var data = [];
                var i = 0;
                var date = UtilsService.createDateFromString(liveSummaryResult.ResponseDate);
                while (i < 30) {
                    i++;
                    date = new Date(date.setSeconds(date.getSeconds() - 5));
                    data.unshift({
                        ResponseDate: date,
                        CountConnected: null,
                        Attempts:null
                    });
                }
                data.push({
                    ResponseDate: UtilsService.createDateFromString(liveSummaryResult.ResponseDate),
                    CountConnected: liveSummaryResult.CountConnected,
                    Attempts: liveSummaryResult.Attempts
                });

                liveChartAPI.renderChart(data, chartDefinition, seriesDefinitions, xAxisDefinition);
            });
        }
        function chartDataFunction(liveSummaryResult)
        {
            liveChartAPI.addItem({ ResponseDate: UtilsService.createDateFromString(liveSummaryResult.ResponseDate), CountConnected: liveSummaryResult.CountConnected, Attempts: liveSummaryResult.Attempts });
        }

        function calculateMaxACD(acdValue)
        {
            return acdValue <= 50 ? 50 : (Math.round(acdValue) + Math.round(acdValue) % 2);
        }

        function loadACDGauge(liveSummaryResult) {

            acdGuageChartReadyDeferred.promise.then(function () {
                lastACD = calculateMaxACD(liveSummaryResult.ACD);
                var chartDefinition = {
                    type: "gauge",
                    title:"ACD",
                    ranges: [{
                        from: 0,
                        to: 5,
                        color: '#DF5353',
                    }, {
                        from: 5,
                        to: 20,
                        color: '#f0ad4e',
                    }, {
                        from: 20,
                        to: lastACD,
                        color: '#55BF3B',
                    }
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
                        valueSuffix: ''
                    },
                }); 
                
                var yAxisDefinition = {
                    min: 0,
                    max: lastACD,
                    title: ' ',
                    interval: Math.round(lastACD / 2)
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
                    type: "spline",
                    yAxisTitle: " ",
                    numberOfPoints: 30,
                    enablePoints: false,
                    useAnimation: true,
                    //lines: [ {
                    //    width: 3,
                    //    value: 0,
                    //    color: '#DF5353',
                    //},{
                    //    width: 3,
                    //    value: 5,
                    //    color: '#DDDF0D',
                    //}, {
                    //    width: 3,
                    //    value: 20,
                    //    color: '#55BF3B',
                    //}
                    //]

                };
                var xAxisDefinition = {
                    titlePath: "Date",
                    isTime: true,
                    hideAxes: true
                };

                var seriesDefinitions = [];
                seriesDefinitions.push({
                    title: "ACD",
                    valuePath: "ACD",
                });




                var data = [];
                var i = 0;
                var date = UtilsService.createDateFromString(liveSummaryResult.ResponseDate);
                while (i < 30)
                {
                    i++;
                    date = new Date(date.setSeconds(date.getSeconds() - 5));
                    data.unshift({
                        Date: date,
                        ACD: null
                    });
                }
                data.push({
                    Date: UtilsService.createDateFromString(liveSummaryResult.ResponseDate),
                    ACD: liveSummaryResult.ACD
                });
                acdLiveChartAPI.renderChart(data, chartDefinition, seriesDefinitions, xAxisDefinition);
            });
        }
        function chartACDLiveFunction(liveSummaryResult) {
            acdLiveChartAPI.addItem({ Date: UtilsService.createDateFromString(liveSummaryResult.ResponseDate), ACD: liveSummaryResult.ACD });
        }

    }
    appControllers.controller('NP_IVSwitch_SwitchDashboardManagementController', SwitchDashboardManagementController);

})(appControllers);
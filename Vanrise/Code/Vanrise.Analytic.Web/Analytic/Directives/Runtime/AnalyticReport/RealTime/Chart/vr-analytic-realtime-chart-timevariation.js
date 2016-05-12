"use strict";

app.directive("vrAnalyticRealtimeChartTimevariation", ['UtilsService', 'VRNotificationService', 'Analytic_AnalyticService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService',
    function (UtilsService, VRNotificationService, Analytic_AnalyticService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericChart = new AnalyticTopRecordsChart($scope, ctrl, $attrs);
                genericChart.initializeController();
            },
            controllerAs: 'analyticchartctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/RealTime/Chart/Templates/AnalyticRealTimeTimeVariationChart.html";
            }
        };

        function AnalyticTopRecordsChart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            ctrl.dimensions = [];
            ctrl.groupingDimensions = [];
            ctrl.measures = [];
            ctrl.dimensionsConfig = [];
            var directiveAPI = {};
            var chartReadyDeferred = UtilsService.createPromiseDeferred();
            ctrl.sortField = "";
            var fromTime;
            var toTime;

            function initializeController() {

                ctrl.chartReady = function (api) {
                    directiveAPI = api;
                    chartReadyDeferred.resolve();

                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());


                    function getDirectiveAPI() {

                        directiveAPI.load = function (payload) {

                            var query = getQuery(payload);
                            var dataRetrievalInput = {
                                DataRetrievalResultType: 0,
                                IsSortDescending: false,
                                ResultKey: null,
                                SortByColumnName: ctrl.sortField,
                                Query: query
                            }
                            return VR_Analytic_AnalyticAPIService.GetTimeVariationAnalyticRecords(dataRetrievalInput)
                                .then(function (response) {
                                    console.log(response);
                                    renderCharts(response, payload.Settings.ChartType);
                                    ctrl.showlaoder = false;
                                });

                            function renderCharts(response, chartType) {
                                var chartData = new Array();

                                //for (var m = 0; m < ctrl.measures.length; m++) {
                                //    var measureObject = new Object();

                                //    for (var i = 0; i < response.Data.length  ; i++) {
                                //        var dimensionName = "";
                                //        for (var d = 0; d < ctrl.groupingDimensions.length; d++) {
                                //            dimensionName += response.Data[i].DimensionValues[d].Value + ", ";
                                //        }
                                //        dimensionName = UtilsService.trim(dimensionName, ", ");
                                //        var chartRecord = UtilsService.getItemByVal(chartData, dimensionName, "DimensionValue");
                                //        if (chartRecord == undefined) {
                                //            chartRecord = {};
                                //            chartRecord["DimensionValue"] = dimensionName;
                                //            chartData.push(chartRecord);
                                //        }
                                //        chartRecord[ctrl.measures[m].MeasureName] = response.Data[i].MeasureValues[ctrl.measures[m].MeasureName];
                                //    }
                                //}
                                var chartDefinition = {
                                    type: chartType,
                                    yAxisTitle: "Value"
                                };
                                var xAxisDefinition = {
                                    titlePath: "Time",
                                    isDateTime: false
                                };

                                var seriesDefinitions = [];
                                for (var i = 0; i < ctrl.measures.length; i++) {
                                    var measure = ctrl.measures[i];
                                    seriesDefinitions.push({
                                        title: measure.Title,
                                        valuePath: measure.MeasureName
                                    });
                                }
                                directiveAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
                            }
                        };
                        return directiveAPI;
                    }

                };

            };

            function getQuery(payLoad) {
                fromTime = payLoad.FromTime;
                toTime = payLoad.ToTime;
                ctrl.measures.length = 0;

                if (payLoad.Dimensions != undefined) {
                    ctrl.dimensions = payLoad.Dimensions;
                }
                if (payLoad.DimensionsConfig != undefined) {
                    ctrl.dimensionsConfig = payLoad.DimensionsConfig;
                }

                if (payLoad.Settings != undefined) {
                    if (payLoad.Settings.Measures != undefined) {
                        for (var i = 0; i < payLoad.Settings.Measures.length; i++) {
                            ctrl.measures.push(payLoad.Settings.Measures[i]);
                        }
                    }
                }
                else {
                    for (var i = 0; i < payLoad.Measures.length; i++) {
                        ctrl.measures.push(payLoad.Measures[i]);
                    }
                }

                ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].MeasureName;
                console.log(ctrl.dimensions);
                var queryFinalized = {
                    Filters: payLoad.DimensionFilters,
                    MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                    FromTime: fromTime,
                    ToTime: toTime,
                    TableId: payLoad.TableId,
                    TimeGroupingUnit: 0,
                }
                return queryFinalized;
            }
        }

        return directiveDefinitionObject;
    }
]);
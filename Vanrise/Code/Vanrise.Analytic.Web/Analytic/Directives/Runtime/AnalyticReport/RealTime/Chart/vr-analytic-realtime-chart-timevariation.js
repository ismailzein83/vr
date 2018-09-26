"use strict";

app.directive("vrAnalyticRealtimeChartTimevariation", ['UtilsService', 'VRNotificationService', 'Analytic_AnalyticService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic_TimeGroupingUnitEnum',
    function (UtilsService, VRNotificationService, Analytic_AnalyticService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_TimeGroupingUnitEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
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

            var fromTime;
            var toTime;
            var lastHours;

            var directiveAPI = {};
            var chartReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.dimensions = [];
                ctrl.groupingDimensions = [];
                ctrl.measures = [];
                ctrl.dimensionsConfig = [];
                ctrl.sortField = "";

                ctrl.chartReady = function (api) {
                    directiveAPI = api;
                    chartReadyDeferred.resolve();

                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {

                        directiveAPI.load = function (payload) {
                            ctrl.showloader = true;
                            var query = getQuery(payload);

                            var dataRetrievalInput = {
                                DataRetrievalResultType: 0,
                                IsSortDescending: false,
                                ResultKey: null,
                                SortByColumnName: "Time",
                                Query: query
                            };

                            return VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput).then(function (response) {
                                renderCharts(response, payload.Settings.ChartType);
                                ctrl.showloader = false;
                            });

                            function renderCharts(response, chartType) {
                                var chartData = new Array();

                                for (var m = 0; m < ctrl.measures.length; m++) {
                                    var measureObject = new Object();
                                    for (var i = 0; i < response.Data.length  ; i++) {
                                        var chartRecord = {
                                            Time: response.Data[i].Time,
                                        };
                                        chartRecord[ctrl.measures[m].MeasureName] = response.Data[i].MeasureValues[ctrl.measures[m].MeasureName].Value;
                                        chartData.push(chartRecord);
                                    }
                                }
                                var chartDefinition = {
                                    type: chartType,
                                    yAxisTitle: "Value"
                                };
                                var xAxisDefinition = {
                                    titlePath: "Time",
                                };
                                if (payload != undefined) {
                                    switch (payload.TimeGroupingUnit) {
                                        case VR_Analytic_TimeGroupingUnitEnum.Day.value: xAxisDefinition.isDate = true; break;
                                        case VR_Analytic_TimeGroupingUnitEnum.Hour.value: xAxisDefinition.isDateTime = true; break;
                                    }
                                }

                                var seriesDefinitions = [];
                                for (var i = 0; i < ctrl.measures.length; i++) {
                                    var measure = ctrl.measures[i];
                                    seriesDefinitions.push({
                                        title: measure.MeasureName,
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
                lastHours = payLoad.LastHours;

                ctrl.measures.length = 0;

                if (payLoad.Dimensions != undefined) {
                    ctrl.dimensions = payLoad.Dimensions;
                }
                if (payLoad.DimensionsConfig != undefined) {
                    ctrl.dimensionsConfig = payLoad.DimensionsConfig;
                }

                var widgetRecordFilter;

                if (payLoad.Settings != undefined) {
                    if (payLoad.Settings.Measures != undefined) {
                        for (var i = 0; i < payLoad.Settings.Measures.length; i++) {
                            ctrl.measures.push(payLoad.Settings.Measures[i]);
                        }
                    }
                    if (payLoad.Settings.RecordFilter != undefined) {
                        widgetRecordFilter = payLoad.Settings.RecordFilter;
                    }
                }
                else {
                    for (var i = 0; i < payLoad.Measures.length; i++) {
                        ctrl.measures.push(payLoad.Measures[i]);
                    }
                }

                ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].MeasureName;

                var queryFinalized = {
                    Filters: payLoad.DimensionFilters,
                    MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                    FromTime: fromTime,
                    ToTime: toTime,
                    LastHours: lastHours,
                    TableId: payLoad.TableId,
                    TimeGroupingUnit: payLoad.TimeGroupingUnit,
                    FilterGroup: buildFilterGroupObj(payLoad.FilterGroup, widgetRecordFilter)
                };

                return queryFinalized;
            }
            function buildFilterGroupObj(filterObj, widgetRecordFilter) {
                if (widgetRecordFilter == undefined)
                    return filterObj;

                if (filterObj == undefined)
                    return widgetRecordFilter;

                return {
                    $type: 'Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities',
                    LogicalOperator: 0,
                    Filters: [filterObj, widgetRecordFilter]
                };
            };
        }

        return directiveDefinitionObject;
    }
]);
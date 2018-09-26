"use strict";

app.directive("vrAnalyticRealtimeChartToprecords", ['UtilsService', 'VRNotificationService', 'Analytic_AnalyticService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService',
    function (UtilsService, VRNotificationService, Analytic_AnalyticService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService) {

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
                return "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/RealTime/Chart/Templates/AnalyticRealTimeTopRecordsChart.html";
            }
        };

        function AnalyticTopRecordsChart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var fromTime;
            var toTime;
            var lastHours;

            ctrl.dimensions = [];
            ctrl.groupingDimensions = [];
            ctrl.measures = [];
            ctrl.dimensionsConfig = [];
            ctrl.sortField = "";

            var directiveAPI = {};
            var chartReadyDeferred = UtilsService.createPromiseDeferred();

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
                            };

                            return VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput).then(function (response) {
                                renderCharts(response, payload.Settings.ChartType);
                                ctrl.showlaoder = false;
                            });

                            function renderCharts(response, chartType) {
                                var chartData = new Array();

                                for (var m = 0; m < ctrl.measures.length; m++) {
                                    var measureObject = new Object();

                                    for (var i = 0; i < response.Data.length  ; i++) {
                                        var dimensionName = "";
                                        for (var d = 0; d < ctrl.groupingDimensions.length; d++) {
                                            dimensionName += response.Data[i].DimensionValues[d].Value + ", ";
                                        }
                                        dimensionName = UtilsService.trim(dimensionName, ", ");
                                        var chartRecord = UtilsService.getItemByVal(chartData, dimensionName, "DimensionValue");
                                        if (chartRecord == undefined) {
                                            chartRecord = {};
                                            chartRecord["DimensionValue"] = dimensionName;
                                            chartData.push(chartRecord);
                                        }
                                        chartRecord[ctrl.measures[m].MeasureName] = response.Data[i].MeasureValues[ctrl.measures[m].MeasureName];
                                    }
                                }
                                var chartDefinition = {
                                    type: chartType,
                                    yAxisTitle: "Value"
                                };
                                var xAxisDefinition = {
                                    titlePath: "DimensionValue",
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
                lastHours = payLoad.LastHours;
                fromTime = payLoad.FromTime;
                toTime = payLoad.ToTime;

                ctrl.groupingDimensions.length = 0;
                ctrl.measures.length = 0;

                if (payLoad.Dimensions != undefined) {
                    ctrl.dimensions = payLoad.Dimensions;
                }

                if (payLoad.DimensionsConfig != undefined) {
                    ctrl.dimensionsConfig = payLoad.DimensionsConfig;
                }

                var widgetRecordFilter;

                if (payLoad.Settings != undefined) {
                    //if (payLoad.Settings.Dimensions != undefined) {
                    //    ctrl.dimensions = payLoad.Settings.Dimensions;
                    //    for (var i = 0; i < payLoad.Settings.Dimensions.length; i++) {
                    //        var dimension = payLoad.Settings.Dimensions[i];
                    //        var groupingDimension = UtilsService.getItemByVal(payLoad.GroupingDimensions, dimension.DimensionName, 'DimensionName');
                    //        if (groupingDimension != undefined) {
                    //            ctrl.groupingDimensions.push(dimension);

                    //        }
                    //    }
                    //}
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
                    if (payLoad.GroupingDimensions != undefined) {
                        for (var i = 0; i < payLoad.GroupingDimensions.length; i++) {
                            ctrl.groupingDimensions.push(UtilsService.getItemByVal(ctrl.dimensions, payLoad.GroupingDimensions[i].DimensionName, 'DimensionName'));
                        }
                    }
                }

                if (payLoad.GroupingDimensions != undefined) {
                    for (var i = 0; i < payLoad.GroupingDimensions.length; i++) {
                        ctrl.groupingDimensions.push(UtilsService.getItemByVal(ctrl.dimensions, payLoad.GroupingDimensions[i].DimensionName, 'Name'));
                    }
                }

                if (ctrl.groupingDimensions.length > 0)
                    ctrl.sortField = 'DimensionValues[0].Name';
                else
                    ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].MeasureName;

                var queryFinalized = {
                    Filters: payLoad.DimensionFilters,
                    DimensionFields: UtilsService.getPropValuesFromArray(ctrl.groupingDimensions, 'Name'),
                    MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                    FromTime: fromTime,
                    LastHours: lastHours,
                    ToTime: toTime,
                    FilterGroup: buildFilterGroupObj(payLoad.FilterGroup, widgetRecordFilter),
                    TableId: payLoad.TableId
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
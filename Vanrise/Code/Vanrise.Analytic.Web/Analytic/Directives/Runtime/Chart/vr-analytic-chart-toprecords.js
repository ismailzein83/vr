"use strict";

app.directive("vrAnalyticChartToprecords", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService) {

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
                return "/Client/Modules/Analytic/Directives/Runtime/Chart/Templates/AnalyticTopRecordsChart.html";
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
            var currencyId;

            function initializeController() {

                ctrl.chartReady = function (api) {
                    directiveAPI = api;
                    chartReadyDeferred.resolve();

                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());


                    function getDirectiveAPI() {

                        directiveAPI.load = function (payload) {
                            var query = getQuery(payload);
                            currencyId = payload.CurrencyId;

                            var dataRetrievalInput = {
                                DataRetrievalResultType: 0,
                                IsSortDescending: false,
                                ResultKey: null,
                                SortByColumnName: ctrl.sortField,
                                Query: query
                            };
                            return VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput)
                                .then(function (response) {
                                    renderCharts(response, payload.Settings.ChartType);
                                    ctrl.showlaoder = false;
                                });

                            function renderCharts(response, chartType) {
                                var chartData = new Array();

                                var dimensionNamesList;
                                if (response.SubTables != null && response.SubTables.length > 0) {
                                    dimensionNamesList = [];
                                    var subTable = response.SubTables[0];
                                    for (var d = 0; d < subTable.DimensionValues.length; d++) {
                                        var currentDimensionValues = subTable.DimensionValues[d];

                                        var dimensionName = "";
                                        for (var i = 0; i < currentDimensionValues.length; i++) {
                                            var currentDimension = currentDimensionValues[i];
                                            dimensionName += currentDimension.Name + ", ";
                                        }
                                        dimensionName = UtilsService.trim(dimensionName, ", ");
                                        dimensionNamesList.push(dimensionName);
                                    }
                                }

                                for (var m = 0; m < ctrl.measures.length; m++) {
                                    var measureObject = new Object();

                                    for (var i = 0; i < response.Data.length; i++) {
                                        var dimensionName = "";
                                        for (var d = 0; d < ctrl.groupingDimensions.length; d++) {
                                            dimensionName += response.Data[i].DimensionValues[d].Name + ", ";
                                        }
                                        dimensionName = UtilsService.trim(dimensionName, ", ");
                                        var chartRecord = UtilsService.getItemByVal(chartData, dimensionName, "DimensionValue");
                                        if (chartRecord == undefined) {
                                            chartRecord = {};
                                            chartRecord["DimensionValue"] = dimensionName;
                                            chartData.push(chartRecord);
                                        }

                                        if (dimensionNamesList != undefined) {
                                            for (var y = 0; y < response.SubTables[0].DimensionValues.length; y++) {
                                                chartRecord[dimensionNamesList[y].replace(/[^A-Z0-9]+/ig, "")] = response.Data[i].SubTables[0].MeasureValues[y][ctrl.measures[m].MeasureName].Value;
                                            }

                                        }
                                        else {
                                            chartRecord[ctrl.measures[m].MeasureName] = response.Data[i].MeasureValues[ctrl.measures[m].MeasureName].Value;
                                        }
                                    }
                                }
                                var chartDefinition = {
                                    type: chartType,
                                    yAxisTitle: dimensionNamesList == undefined ? "Value" : ctrl.measures[0].Title
                                };
                                var xAxisDefinition = {
                                    titlePath: "DimensionValue"
                                };

                                var seriesDefinitions = [];
                                if (dimensionNamesList != undefined) {
                                    for (var i = 0; i < dimensionNamesList.length; i++) {
                                        var dim = dimensionNamesList[i];
                                        seriesDefinitions.push({
                                            title: dim,
                                            valuePath: dim.replace(/[^A-Z0-9]+/ig, "")
                                        });
                                    }
                                }
                                else {
                                    for (var i = 0; i < ctrl.measures.length; i++) {
                                        var measure = ctrl.measures[i];
                                        seriesDefinitions.push({
                                            title: measure.Title,
                                            valuePath: measure.MeasureName
                                        });
                                    }
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
                ctrl.groupingDimensions.length = 0;
                ctrl.measures.length = 0;
                var subTables = [];

                if (payLoad.Dimensions != undefined) {
                    ctrl.dimensions = payLoad.Dimensions;
                }
                if (payLoad.DimensionsConfig != undefined) {
                    ctrl.dimensionsConfig = payLoad.DimensionsConfig;
                }
                if (payLoad.Settings != undefined) {
                    if (payLoad.Settings.Dimensions != undefined && !payLoad.Settings.RootDimensionsFromSearch) {
                        ctrl.dimensions = payLoad.Settings.Dimensions;
                        for (var i = 0; i < payLoad.Settings.Dimensions.length; i++) {
                            var dimension = payLoad.Settings.Dimensions[i];
                            ctrl.groupingDimensions.push(dimension);
                        }
                    } else if (payLoad.Settings.RootDimensionsFromSearch) {
                        if (payLoad.SelectedGroupingDimensions != undefined) {
                            for (var i = 0; i < payLoad.SelectedGroupingDimensions.length; i++) {
                                var selectedDimension = payLoad.SelectedGroupingDimensions[i];
                                ctrl.groupingDimensions.push({ DimensionName: selectedDimension.DimensionName });
                            }
                        }
                    }
                    if (payLoad.Settings.Measures != undefined) {
                        for (var i = 0; i < payLoad.Settings.Measures.length; i++) {
                            ctrl.measures.push(payLoad.Settings.Measures[i]);
                        }
                    }
                    if (payLoad.Settings.SeriesDimensions != undefined) {
                        var dimensions = [];
                        for (var i = 0; i < payLoad.Settings.SeriesDimensions.length; i++) {

                            dimensions.push(payLoad.Settings.SeriesDimensions[i].DimensionName);
                        }
                        subTables.push({
                            Dimensions: dimensions,
                            Measures: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName')
                        })
                    }
                }
                else {
                    for (var i = 0; i < payLoad.Measures.length; i++) {
                        ctrl.measures.push(payLoad.Measures[i]);
                    }
                }

                if (ctrl.groupingDimensions.length > 0)
                    ctrl.sortField = 'DimensionValues[0].Name';
                else
                    ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].MeasureName + '.Value';

                var queryFinalized = {
                    Filters: payLoad.DimensionFilters,
                    DimensionFields: UtilsService.getPropValuesFromArray(ctrl.groupingDimensions, 'DimensionName'),
                    MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                    SubTables: subTables,
                    FromTime: fromTime,
                    ToTime: toTime,
                    TableId: payLoad.TableId,
                    FilterGroup: payLoad.FilterGroup,
                    TopRecords: payLoad.Settings.TopRecords,
                    OrderBy: [payLoad.Settings.TopMeasure],
                    OrderType: payLoad.Settings.OrderType,
                    AdvancedOrderOptions: payLoad.Settings.AdvancedOrderOptions,
                    CurrencyId: currencyId

                };
                return queryFinalized;
            }
        }

        return directiveDefinitionObject;
    }
]);
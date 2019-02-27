"use strict";

app.directive("vrAnalyticChartToprecords", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VRTimerService', 'VR_Analytic_AutoRefreshType','VRDateTimeService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService, VRTimerService, VR_Analytic_AutoRefreshType, VRDateTimeService) {

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
            var ranges;
            var query;

            var useSummaryValues = false;
            function initializeController() {

                ctrl.chartReady = function (api) {
                    directiveAPI = api;
                    chartReadyDeferred.resolve();

                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());


                    function getDirectiveAPI() {

                        directiveAPI.load = function (payload) {
                            var promises = [];
                            query = getQuery(payload);
                            if (payload.Settings.AutoRefreshType == VR_Analytic_AutoRefreshType.SummaryValues.value) {
                                useSummaryValues = true;
                            }
                            if (useSummaryValues) {
                                if ($scope.jobIds) {
                                    VRTimerService.unregisterJobByIds($scope.jobIds);
                                    $scope.jobIds.length = 0;
                                }

                            }
                            currencyId = payload.CurrencyId;
                            var measures = payload.Settings.Measures;
                            var input = {
                                MeasureName: measures[0].MeasureName,
                                AnalyticTableId: payload.TableId
                            };
                            var measureStyleRuleRangesPromise = VR_Analytic_AnalyticAPIService.GetMeasureStyleRulesRanges(input).then(function (response) {
                                if (response != undefined) {
                                    ranges = response;
                                }
                            });
                            promises.push(measureStyleRuleRangesPromise);
                            var getFilteredRecordsResponse;
                            var filteredRecordPromiseDeferred = getFilteredRecords(query).then(function (response) {
                                getFilteredRecordsResponse = response;
                            });

                            promises.push(filteredRecordPromiseDeferred);

                            return UtilsService.waitMultiplePromises(promises)
                                .then(function () {
                                    renderCharts(getFilteredRecordsResponse, payload.Settings.ChartType);
                                    if (useSummaryValues)
                                        registerAutoRefreshJob(payload.Settings.AutoRefreshInterval);
                                    ctrl.showlaoder = false;
                                });
                            function renderCharts(response, chartType) {
                                var chartData = new Array();

                                var dimensionNamesList;
                                if (response.SubTables != null && response.SubTables.length > 0) {
                                    dimensionNamesList = [];
                                    var subTable = response.SubTables[0];
                                    for (var dimensionIndex = 0; dimensionIndex < subTable.DimensionValues.length; dimensionIndex++) {
                                        var currentDimensionValues = subTable.DimensionValues[dimensionIndex];

                                        var subtableDimensionNames = "";
                                        for (var dimensionValueIndex = 0; dimensionValueIndex < currentDimensionValues.length; dimensionValueIndex++) {
                                            var currentDimension = currentDimensionValues[dimensionValueIndex];
                                            subtableDimensionNames += currentDimension.Name + ", ";
                                        }
                                        subtableDimensionNames = UtilsService.trim(subtableDimensionNames, ", ");
                                        dimensionNamesList.push(subtableDimensionNames);
                                    }
                                }

                                for (var m = 0; m < ctrl.measures.length; m++) {
                                    for (var i = 0; i < response.Data.length; i++) {

                                        var dimensionName = "";
                                        for (var d = 0; d < ctrl.groupingDimensions.length; d++) {
                                            dimensionName += response.Data[i].DimensionValues[d].Name + ", ";
                                        }
                                        dimensionName = UtilsService.trim(dimensionName, ", ");
                                        var chartRecord = UtilsService.getItemByVal(chartData, dimensionName, "DimensionValue");
                                        if (chartRecord == undefined) {
                                            chartRecord = {};

                                            if (useSummaryValues) {
                                                chartRecord["Date"] = VRDateTimeService.getNowDateTime();
                                            }
                                            chartRecord["DimensionValue"] = dimensionName;
                                            chartData.push(chartRecord);
                                        }

                                        if (dimensionNamesList != undefined) {
                                            //in this case m is always equal to zero because in case of subtables we have only one measure
                                            chartRecord.MeasureValues = [];
                                            for (var y = 0; y < response.SubTables[0].DimensionValues.length; y++) {
                                                chartRecord.MeasureValues[y] = [];
                                                chartRecord.MeasureValues[y][ctrl.measures[m].MeasureName] = {};
                                                chartRecord.MeasureValues[y][ctrl.measures[m].MeasureName].Value = response.Data[i].SubTables[0].MeasureValues[y][ctrl.measures[m].MeasureName].ModifiedValue;
                                            }

                                        }
                                        else {
                                            chartRecord[ctrl.measures[m].MeasureName] = response.Data[i].MeasureValues[ctrl.measures[m].MeasureName].ModifiedValue;
                                        }
                                    }

                                    if (useSummaryValues) {
                                        if (chartData.length < payload.Settings.NumberOfPoints) {
                                            for (var i = chartData.length; i <= payload.Settings.NumberOfPoints; i++) {
                                                var object = {
                                                    Date: VRDateTimeService.getNowDateTime(),
                                                };
                                                for (var m = 0; m < ctrl.measures.length;m++) {
                                                    var measure = ctrl.measures[m];
                                                    object[measure.MeasureName] = null;
                                                }
                                                chartData.unshift(object);
                                            }
                                        }
                                    }
                                }
                                var chartDefinition = {
                                    type: chartType,
                                    yAxisTitle: dimensionNamesList == undefined ? "Value" : ctrl.measures[0].Title,
                                    rangesObject: ranges
                                };
                                if (useSummaryValues) {
                                    chartDefinition.numberOfPoints = payload.Settings.NumberOfPoints;
                                    chartDefinition.enablePoints = false;
                                    chartDefinition.useAnimation = true;
                                }
                                var xAxisDefinition = {
                                    titlePath: "DimensionValue",
                                };
                                if (useSummaryValues) {
                                    xAxisDefinition.titlePath = "Date";
                                    xAxisDefinition.isTime = true;
                                    xAxisDefinition.hideAxes = true;
                                    xAxisDefinition.hideAxesTitle = true;
                                }
                                var seriesDefinitions = [];
                                if (dimensionNamesList != undefined) {
                                    //in this case we have only one measure because we have subtables; that's why ctrl.measures contains only one item ( so we can use 'ctrl.measures[0].MeasureName' while pushing items in seriesDefinitions)
                                    for (var i = 0; i < dimensionNamesList.length; i++) {
                                        var dim = dimensionNamesList[i];
                                        seriesDefinitions.push({
                                            title: dim,
                                            valuePath: 'MeasureValues[' + i + ']["' + ctrl.measures[0].MeasureName + '"].Value'
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
            function registerAutoRefreshJob(autoRefreshInterval) {
                VRTimerService.registerJob(onTimerElapsed, $scope, autoRefreshInterval);
            }

            function onTimerElapsed() {
                return getFilteredRecords(query).then(function (response) {
                    if (response != undefined && response.Data != undefined) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var dataItem = response.Data[i];
                            if (dataItem.MeasureValues != undefined) {
                                var item = {};
                                for (var p in dataItem.MeasureValues) {
                                    item[p] = dataItem.MeasureValues[p].ModifiedValue;
                                }
                                item.Date = VRDateTimeService.getNowDateTime();
                                directiveAPI.addItem(item);
                            }
                        }


                    }
                });
            }
            function getFilteredRecords(query) {
                var promiseDeferred = UtilsService.createPromiseDeferred();
                var dataRetrievalInput = {
                    DataRetrievalResultType: 0,
                    IsSortDescending: false,
                    ResultKey: null,
                    SortByColumnName: ctrl.sortField,
                    Query: query
                };
                VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput)
                    .then(function (response) {
                        promiseDeferred.resolve(response);
                    });

                return promiseDeferred.promise;
            }
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
                        });
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
                    CurrencyId: currencyId,
                    TimePeriod: payLoad.TimePeriod
                };
                return queryFinalized;
            }
        }

        return directiveDefinitionObject;
    }
]);
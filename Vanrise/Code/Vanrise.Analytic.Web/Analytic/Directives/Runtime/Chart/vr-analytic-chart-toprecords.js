"use strict";

app.directive("vrAnalyticChartToprecords", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VRTimerService', 'VR_Analytic_AutoRefreshType', 'VRDateTimeService', 'VR_Analytic_AnalyticItemActionService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService, VRTimerService, VR_Analytic_AutoRefreshType, VRDateTimeService, VR_Analytic_AnalyticItemActionService) {

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
            ctrl.sortField = "";
            ctrl.chartMenuActions = [];

            var directiveAPI = {};
            var chartReadyDeferred = UtilsService.createPromiseDeferred();
            var fromTime;
            var toTime;
            var currencyId;
            var ranges;
            var query;
            var useSummaryValues = false;
            var timeOnXAxis;
            var tableId;
            var itemActions;


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
                                timeOnXAxis = payload.Settings.TimeOnXAxis;
                            }

                            if (useSummaryValues) {
                                if ($scope.jobIds) {
                                    VRTimerService.unregisterJobByIds($scope.jobIds);
                                    $scope.jobIds.length = 0;
                                }

                            }

                            currencyId = payload.CurrencyId;
                            var measures = payload.Settings.Measures;
                            tableId = payload.TableId;
                            var input = {
                                MeasureName: measures[0].MeasureName,
                                AnalyticTableId: tableId
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

                            return UtilsService.waitMultiplePromises(promises).then(function () {
                                renderCharts(getFilteredRecordsResponse, payload);
                                if (useSummaryValues)
                                    registerAutoRefreshJob(payload.Settings.AutoRefreshInterval);
                                ctrl.showlaoder = false;
                            });

                            function renderCharts(response, payload) {
                                var chartData = new Array();
                                var chartType = payload.Settings.ChartType;
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
                                        var dataDimensionValues = {};
                                        var dimensionName = "";
                                        var dimensionValue = "";
                                        for (var d = 0; d < ctrl.groupingDimensions.length; d++) {
                                            dimensionName += response.Data[i].DimensionValues[d].Name + ", ";
                                            dimensionValue += response.Data[i].DimensionValues[d].Value + ", ";
                                        }

                                        dimensionName = UtilsService.trim(dimensionName, ", ");
                                        dimensionValue = UtilsService.trim(dimensionValue, ", ");
                                        var chartRecord = UtilsService.getItemByVal(chartData, dimensionName, "DimensionValue");
                                        if (chartRecord == undefined) {
                                            chartRecord = {};

                                            if (useSummaryValues) {
                                                chartRecord["Date"] = VRDateTimeService.getNowDateTime();
                                            }
                                            chartRecord["DimensionValue"] = dimensionName;
                                            chartRecord["DimensionRecordValue"] = dimensionValue;
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
                                                for (var m = 0; m < ctrl.measures.length; m++) {
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
                                    valuePath: "DimensionRecordValue"
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

                                var parentDimensions;
                                if (payload.Settings != undefined && payload.Settings.ItemActions != undefined && payload.Settings.ItemActions.length > 0) {
                                    itemActions = payload.Settings.ItemActions;
                                    parentDimensions = payload.SelectedGroupingDimensions;
                                }
                                else {
                                    itemActions = payload.ItemActions;
                                    parentDimensions = payload.ParentDimensions;
                                }

                                var selectedDimensions = [];
                                if (payload.SelectedGroupingDimensions != undefined)
                                    selectedDimensions = payload.SelectedGroupingDimensions;
                                else if (payload.GroupingDimensions != undefined)
                                    selectedDimensions = payload.GroupingDimensions;

                                BuildMenuAction(payload.FromTime, payload.ToTime, payload.FilterGroup, itemActions, parentDimensions, payload.DimensionFilters, selectedDimensions, payload.Period);
                                directiveAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition, ctrl.chartMenuActions);
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
                    CurrencyId: payLoad.CurrencyId,
                    TimePeriod: payLoad.TimePeriod
                };
                return queryFinalized;
            }

            function BuildMenuAction(fromTime, toTime, filterGroup, itemActions, parentDimensions, dimensionFilters, selectedDimensions, period) {

                ctrl.chartMenuActions.length = 0;

                if (itemActions != undefined) {
                    for (var i = 0; i < itemActions.length; i++) {
                        var itemAction = itemActions[i];
                        var settings = {
                            FromDate: fromTime,
                            ToDate: toTime,
                            Period: period,
                            FilterGroup: filterGroup,
                            TableId: tableId
                        };
                        ctrl.chartMenuActions.push({
                            name: itemAction.Title,
                            itemActionObject: itemAction,
                            settingsObject: settings,
                            clicked: function (event) {
                                this.settingsObject.DimensionFilters = getDimensionValues(event, selectedDimensions, ctrl.groupingDimensions, parentDimensions, dimensionFilters);
                                var payload = { ItemAction: this.itemActionObject, Settings: this.settingsObject };
                                return VR_Analytic_AnalyticItemActionService.excuteItemAction(payload);
                            },
                        });
                    }
                }
            }

            function getDimensionValues(event, selectedDimensions, groupingDimensions, parentDimensions, dimensionFilters) {
                var dimensionValues = [];
                var dataItemDimensionValues = event.point.xRecordValue != undefined ? event.point.xRecordValue.split(", ") : undefined;
                if (dataItemDimensionValues == undefined || dataItemDimensionValues.length == 0)
                    return [];

                if (selectedDimensions != undefined) {
                    for (var i = 0; i < selectedDimensions.length; i++) {
                        var selectedDimension = selectedDimensions[i];
                        dimensionValues.push({
                            Dimension: selectedDimension.DimensionName,
                            FilterValues: dataItemDimensionValues[i] != "null" ? [dataItemDimensionValues[i]] : [null]
                        });
                    }
                }

                if (dimensionFilters != undefined) {
                    for (var i = 0; i < dimensionFilters.length; i++) {
                        var currentDimension = dimensionFilters[i];
                        if (currentDimension == undefined)
                            continue;

                        dimensionValues.push({
                            Dimension: currentDimension.Dimension,
                            FilterValues: currentDimension.FilterValues
                        });
                    }
                }

                //if parentDimensions.length > 0 all groupingDimensions are already added to dimensionValues by selectedDimensions and dimensionFilters
                if ((parentDimensions == undefined || parentDimensions.length == 0) && groupingDimensions != undefined) {
                    for (var i = 0; i < groupingDimensions.length; i++) {
                        var groupingDimension = groupingDimensions[i];
                        if (groupingDimension == undefined || groupingDimension.DimensionName == undefined)
                            continue;

                        var groupingDimensionValue = dataItemDimensionValues[i];
                        var existingDimensionFilter = UtilsService.getItemByVal(dimensionValues, groupingDimension.DimensionName, "Dimension");
                        if (existingDimensionFilter != null && existingDimensionFilter.FilterValues.length == 1 && existingDimensionFilter.FilterValues[0] == groupingDimensionValue)
                            continue;

                        dimensionValues.push({
                            Dimension: groupingDimension.DimensionName,
                            FilterValues: [groupingDimensionValue]
                        });
                    }
                }

                return dimensionValues;
            }
        }

        return directiveDefinitionObject;
    }
]);
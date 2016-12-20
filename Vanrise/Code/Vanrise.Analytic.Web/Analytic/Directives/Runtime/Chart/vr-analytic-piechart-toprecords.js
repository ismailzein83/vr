"use strict";

app.directive("vrAnalyticPiechartToprecords", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic_OrderTypeEnum',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_OrderTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericChart = new AnalyticPieChart($scope, ctrl, $attrs);
                genericChart.initializeController();
            },
            controllerAs: 'analyticchartctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Modules/Analytic/Directives/Runtime/Chart/Templates/AnalyticPieChart.html";
            }
        };

        function AnalyticPieChart($scope, ctrl, $attrs) {
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
                            };
                            return VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput)
                                .then(function (response) {
                                   
                                    if (response && response.Data)
                                    {
                                        var data = [];
                                        for (var i = 0; i < response.Data.length;i++)
                                        {
                                            data.push(response.Data[i]);

                                        }
                                        renderCharts(data);
                                    } 
                               
                                    ctrl.showlaoder = false;
                                });

                            function renderCharts(response) {
                                var chartData = new Array();
                                for (var m = 0; m < ctrl.measures.length; m++) {
                                    var measureObject = new Object();

                                    for (var i = 0; i < response.length; i++) {
                                        var dimensionName = "";
                                        for (var d = 0; d < ctrl.groupingDimensions.length; d++) {
                                            dimensionName += response[i].DimensionValues[d].Name + ", ";
                                        }
                                        dimensionName = UtilsService.trim(dimensionName, ", ");
                                        var chartRecord = UtilsService.getItemByVal(chartData, dimensionName, "DimensionValue");
                                        if (chartRecord == undefined) {
                                            chartRecord = {};
                                            chartRecord["DimensionValue"] = dimensionName;
                                            chartData.push(chartRecord);
                                        }
                                        chartRecord[ctrl.measures[m].MeasureName] = response[i].MeasureValues[ctrl.measures[m].MeasureName].Value;
                                    }
                                }
                                var chartDefinition = {
                                    type: "pie",
                                    yAxisTitle: "Value"
                                };
                                var seriesDefinitions = [];
                                for (var i = 0; i < ctrl.measures.length; i++) {
                                    var measure = ctrl.measures[i];
                                    seriesDefinitions.push({
                                        title: measure.Title,
                                        valuePath: measure.MeasureName,
                                        titlePath: "DimensionValue"
                                    });
                                }
                                directiveAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
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

                if (payLoad.Dimensions != undefined) {
                    ctrl.dimensions = payLoad.Dimensions;
                }
                if (payLoad.DimensionsConfig != undefined) {
                    ctrl.dimensionsConfig = payLoad.DimensionsConfig;
                }

                if (payLoad.Settings != undefined) {
                    if (payLoad.Settings.Dimensions != undefined) {
                        ctrl.dimensions = payLoad.Settings.Dimensions;
                        for (var i = 0; i < payLoad.Settings.Dimensions.length; i++) {
                            var dimension = payLoad.Settings.Dimensions[i];
                            var groupingDimension = UtilsService.getItemByVal(ctrl.dimensions, dimension.DimensionName, 'DimensionName');
                            if (groupingDimension != undefined) {
                                ctrl.groupingDimensions.push(dimension);
                            }
                        }
                    }
                    if (payLoad.Settings.Measure != undefined) {
                        ctrl.measures.push(payLoad.Settings.Measure);
                    }
                }
                else {
                    if (payLoad.Measure != undefined) {
                        ctrl.measures.push(payLoad.Measure);
                    }
                    if (payLoad.GroupingDimensions != undefined) {
                        for (var i = 0; i < payLoad.GroupingDimensions.length; i++) {
                            var groupingDimensionObj = UtilsService.getItemByVal(ctrl.dimensions, payLoad.GroupingDimensions[i].DimensionName, 'DimensionName');
                            if (groupingDimensionObj !=undefined)
                                ctrl.groupingDimensions.push(groupingDimensionObj);
                        }
                    }
                }

                if (ctrl.groupingDimensions.length > 0)
                    ctrl.sortField = 'DimensionValues[0].Name';
                else
                    ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].MeasureName;
                var queryFinalized = {
                    Filters: payLoad.DimensionFilters,
                    DimensionFields: UtilsService.getPropValuesFromArray(ctrl.groupingDimensions, 'DimensionName'),
                    MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                    FromTime: fromTime,
                    ToTime: toTime,
                    TableId: payLoad.TableId,
                    FilterGroup: payLoad.FilterGroup,
                    TopRecords: payLoad.Settings.TopRecords,
                    OrderType: VR_Analytic_OrderTypeEnum.ByAllMeasures.value
                };
                return queryFinalized;
            }
        }

        return directiveDefinitionObject;
    }
]);
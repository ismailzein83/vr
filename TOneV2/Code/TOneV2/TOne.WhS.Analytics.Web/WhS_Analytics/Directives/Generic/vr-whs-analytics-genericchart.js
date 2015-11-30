"use strict";

app.directive("vrWhsAnalyticsGenericchart", ['WhS_Analytics_GenericAnalyticAPIService', 'WhS_Analytics_GenericAnalyticDimensionEnum', 'WhS_Analytics_GenericAnalyticMeasureEnum',
function (WhS_Analytics_GenericAnalyticAPIService, WhS_Analytics_GenericAnalyticDimensionEnum, WhS_Analytics_GenericAnalyticMeasureEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericChart = new GenericChart($scope, ctrl, $attrs);
                genericChart.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Modules/WhS_Analytics/Directives/Generic/Templates/GenericChartTemplate.html";
            }
        };
        function GenericChart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            ctrl.measures = [];
            var period;
            function initializeController() {
                var api = {};
                var isHour = false;


                api.loadChart = function (payload) {
                    var dimensions = [];
                    ctrl.measures.length = 0;
                    if (payload != undefined) {

                        if (payload.FixedDimensionFields != undefined) {
                            dimensions.push(payload.FixedDimensionFields);
                            for (var p in WhS_Analytics_GenericAnalyticDimensionEnum)
                                if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == payload.FixedDimensionFields)
                                    period = WhS_Analytics_GenericAnalyticDimensionEnum[p];

                            if (payload.FixedDimensionFields == WhS_Analytics_GenericAnalyticDimensionEnum.Hour.value) {
                                dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Date.value);
                                isHour = true;
                            }
                        }
                        if (payload.DimensionFields != undefined) {
                                for (var i = 0; i < payload.DimensionFields.length; i++)
                                    dimensions.push(payload.DimensionFields[i]);
                        }
                        if (payload.measures != undefined) {
                            for (var i = 0; i < payload.measures.length; i++) {
                                for (var p in WhS_Analytics_GenericAnalyticMeasureEnum)
                                    if (WhS_Analytics_GenericAnalyticMeasureEnum[p].value == payload.measures[i])
                                        ctrl.measures.push(WhS_Analytics_GenericAnalyticMeasureEnum[p]);
                            }
                        }
                       

                        for (var i = 0; i < ctrl.measures.length; i++) {
                            setChartApi(ctrl.measures[i]);
                        }
                        var query = {
                            Filters: payload.Filters,
                            DimensionFields: dimensions,
                            MeasureFields: payload.measures,
                            FromTime: payload.FromTime,
                            ToTime: payload.ToTime,
                            Currency: payload.Currency
                        }
                        var sortField;

                        if (dimensions.length > 0)
                            sortField = 'DimensionValues[0].Name';
                        else
                            sortField = 'MeasureValues.' + ctrl.measures[0].name;
                        var dataRetrievalInput = {
                            DataRetrievalResultType: 0,
                            IsSortDescending: false,
                            ResultKey: null,
                            SortByColumnName: sortField,
                            Query: query
                        }
                        return WhS_Analytics_GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                           .then(function (response) {
                               renderCharts(response);
                               ctrl.showCharts = true;
                           });
                    }

                    function setChartApi(chartObject) {
                        chartObject.chartSelectedEntityReady = function (api) {
                            chartObject.API = api;
                        }
                    }
                    function renderCharts(response) {
                        console.log(response);
                        ctrl.isGettingEntityStatistics = true;

                        var chartData = new Array();
                        var chartDefinition = [];
                        var dates = [];
                        var y = 0;
                        var seriesDefinitions = new Array();
                        var countDates = 0;

                        if (isHour) {
                            for (var i = 0; i < response.Data.length; i++) {
                                for (var j = 0; j < dates.length; j++) {

                                    if (dates[j] == response.Data[i].DimensionValues[1].Name) {
                                        y = 1;
                                        break;
                                    }
                                }
                                if (y == 0) {

                                    dates.push(response.Data[i].DimensionValues[1].Name);

                                    seriesDefinitions.push({
                                        title: response.Data[i].DimensionValues[1].Name,
                                        valuePath: 'date' + countDates + 'Value'
                                    });

                                    countDates++;
                                }

                                y = 0;
                            }

                            seriesDefinitions.sort(function (a, b) {
                                return new Date(b.title) - new Date(a.title);
                            });

                            for (var x = 0; x < ctrl.measures.length; x++) {
                                chartData[x] = new Array();
                            }

                            for (var x = 0; x < ctrl.measures.length; x++) {

                                for (var i = 0; i < response.Data.length; i++) {
                                    y = 0;
                                    var h = 0;
                                    for (var j = 0; j < chartData[x].length; j++) {
                                        if (chartData[x][j].hour == response.Data[i].DimensionValues[0].Name) {
                                            y = 1;
                                            h = j;
                                            break;
                                        }
                                    }

                                    if (y == 0 && period!=undefined) {
                                        //insert new data values
                                        var values = {};
                                        values[period.name] = response.Data[i].DimensionValues[0].Name;
                                        for (var k = 0; k < dates.length; k++) {
                                            if (response.Data[i].DimensionValues[1].Name == dates[k])
                                                values['date' + k + 'Value'] = response.Data[i].MeasureValues[ctrl.measures[x].name];
                                            else {
                                                values['date' + k + 'Value'] = 0;
                                            }
                                        }
                                        chartData[x].push(values);
                                    }
                                    else {
                                        //Update the data values
                                        chartData[x][j]['date' + k + 'Value'] = response.Data[i].MeasureValues[ctrl.measures[x].name];
                                    }
                                }

                                var title = "Traffic by " + period!=undefined?period.description:"" + " for " + ctrl.measures[x].description;
                                chartDefinition.push({
                                    type: "column",
                                    title: title,
                                    yAxisTitle: "Value"
                                });
                            }



                            var xAxisDefinition = {
                                titlePath: period != undefined ? period.name : ctrl.measures[0].description,
                                isDateTime: false
                            };
                            for (var i = 0; i < ctrl.measures.length; i++) {
                                if (ctrl.measures[i].API != undefined)
                                    ctrl.measures[i].API.renderChart(chartData[i], chartDefinition[i], seriesDefinitions, xAxisDefinition);
                            }

                        } else {

                            for (var x = 0; x < ctrl.measures.length; x++) {
                                chartData[x] = new Array();
                            }

                            for (var x = 0; x < ctrl.measures.length; x++) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    var values = {};
                                    if (period!=undefined)
                                        values[period.name] = response.Data[i].DimensionValues[0].Name;
                                   
                                    if (dimensions != undefined)
                                        values.name = [];
                                        for (var k = 0; k < response.Data[i].DimensionValues.length; k++)
                                            values.name.push(response.Data[i].DimensionValues[k].Name);
                                    values[ctrl.measures[x].name] = response.Data[i].MeasureValues[ctrl.measures[x].name];
                                    chartData[x].push(values);
                                }

                                var title = "Traffic by " + (period != undefined ? period.description : "") + " for " + ctrl.measures[x].description;
                                chartDefinition.push({
                                    type: "pie",
                                    title: title,
                                    yAxisTitle: "Value"
                                });

                                var sdef = new Array();
                              
                       
                                sdef.push({
                                    title: ctrl.measures[x].description,
                                    titlePath: "name",
                                    valuePath: ctrl.measures[x].name
                                });
                                seriesDefinitions.push(sdef);

                            }
                            var xAxisDefinition = {
                                titlePath: period != undefined ? period.name : ctrl.measures[0].description,
                                isDateTime: false
                            };

                            for (var i = 0; i < ctrl.measures.length; i++) {
                                if (ctrl.measures[i].API != undefined) {
                                    ctrl.measures[i].API.renderSingleDimensionChart(chartData[i], chartDefinition[i], seriesDefinitions[i]);
                                  //  ctrl.measures[i].API.renderChart(chartData[i], chartDefinition[i], seriesDefinitions[i], xAxisDefinition);
                                }
                            }
                        }

                        ctrl.isGettingEntityStatistics = false;
                    }
                }

            

                if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
}]);
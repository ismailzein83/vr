(function (angular, app) {

    "use strict";

    function vrDirectiveObj(GenericAnalyticAPIService, GenericAnalyticDimensionEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                filters: "=",
                fixeddimensionfields: "=",
                fromtime: "=",
                totime: "=",
                currency: "=",
                measurechart: "=",
                onReady: '='
            },
            controller: function () {
                var ctrl = this;
                var api = {};
                var isHour = false;
                for (var i = 0; i < ctrl.measurechart.length; i++) {
                    setChartApi(ctrl.measurechart[i]);
                }

                function setChartApi(chartObject) {
                    chartObject.chartSelectedEntityReady = function (api) {
                        chartObject.API = api;
                    }
                }

                api.LoadChart = function () {

                    var groupKeys = [];
                    var measureChartValues = [];
                    if (ctrl.fixeddimensionfields == undefined)
                        ctrl.fixeddimensionfields = null;

                    //ctrl.fixeddimensionfields.forEach(function (group) {
                    //    groupKeys.push(group.value);

                    //    if (group == GenericAnalyticDimensionEnum.Hour) {
                    //        groupKeys.push(GenericAnalyticDimensionEnum.Date.value);
                    //        isHour = true;
                    //    }
                    //});

                    if (ctrl.fixeddimensionfields != null) {
                        groupKeys.push(ctrl.fixeddimensionfields.value);

                        if (ctrl.fixeddimensionfields == GenericAnalyticDimensionEnum.Hour) {
                            groupKeys.push(GenericAnalyticDimensionEnum.Date.value);
                            isHour = true;
                        }
                    }
 


                    for (var i = 0, len = ctrl.measurechart.length; i < len; i++) {
                        measureChartValues.push(ctrl.measurechart[i].measure.value);
                    }

                    var query = {
                        Filters: ctrl.filters,
                        DimensionFields: groupKeys,
                        MeasureFields: measureChartValues,
                        FromTime: ctrl.fromtime,
                        ToTime: ctrl.totime,
                        Currency: ctrl.currency
                    }



                    var sortField = ctrl.measurechart[0].measure.name;

                    if (groupKeys.length > 0)
                        sortField = 'DimensionValues[0].Name';
                    else
                        sortField = 'MeasureValues.' + ctrl.measurechart[0].measure.name;

                    var dataRetrievalInput = {
                        DataRetrievalResultType: 0,
                        IsSortDescending: false,
                        ResultKey: null,
                        //SortByColumnName: 'DimensionValues[0].Name',
                        SortByColumnName: sortField,
                        Query: query
                    }

                    return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                        .then(function (response) {

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

                                for (var x = 0; x < ctrl.measurechart.length; x++) {
                                    chartData[x] = new Array();
                                }

                                for (var x = 0; x < ctrl.measurechart.length; x++) {

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

                                        if (y == 0) {
                                            //insert new data values
                                            var values = {};
                                            values[ctrl.fixeddimensionfields.name] = response.Data[i].DimensionValues[0].Name;
                                            for (var k = 0; k < dates.length; k++) {
                                                if (response.Data[i].DimensionValues[1].Name == dates[k])
                                                    values['date' + k + 'Value'] = response.Data[i].MeasureValues[ctrl.measurechart[x].measure.name];
                                                else {
                                                    values['date' + k + 'Value'] = 0;
                                                }
                                            }
                                            chartData[x].push(values);
                                        }
                                        else {
                                            //Update the data values
                                            chartData[x][j]['date' + k + 'Value'] = response.Data[i].MeasureValues[ctrl.measurechart[x].measure.name];
                                        }
                                    }

                                    var title = "Traffic by " + ctrl.fixeddimensionfields.description + " for " + ctrl.measurechart[x].measure.description;
                                    chartDefinition.push({
                                        type: "column",
                                        title: title,
                                        yAxisTitle: "Value"
                                    });
                                }

                                

                                var xAxisDefinition = {
                                    titlePath: ctrl.fixeddimensionfields.name,
                                    isDateTime: false
                                };
                                console.log(chartData);
                                console.log(chartDefinition);
                                console.log(seriesDefinitions);
                                console.log(xAxisDefinition);
                                for (var i = 0, len = ctrl.measurechart.length; i < len; i++) {
                                    if (ctrl.measurechart[i].API != undefined)
                                        ctrl.measurechart[i].API.renderChart(chartData[i], chartDefinition[i], seriesDefinitions, xAxisDefinition);
                                }

                            } else {
                                
                                for (var x = 0; x < ctrl.measurechart.length; x++) {
                                    chartData[x] = new Array();
                                }

                                for (var x = 0; x < ctrl.measurechart.length; x++) {
                                    for (var i = 0; i < response.Data.length; i++) {
                                        var values = {};
                                        values[ctrl.fixeddimensionfields.name] = response.Data[i].DimensionValues[0].Name;
                                        values[ctrl.measurechart[x].measure.name] = response.Data[i].MeasureValues[ctrl.measurechart[x].measure.name];
                                        chartData[x].push(values);
                                    }

                                    var title = "Traffic by " + ctrl.fixeddimensionfields.description + " for " + ctrl.measurechart[x].measure.description;
                                    chartDefinition.push({
                                        type: "column",
                                        title: title,
                                        yAxisTitle: "Value"
                                    });

                                    var sdef = new Array();

                                    sdef.push({
                                        title: ctrl.measurechart[x].measure.description,
                                        valuePath: ctrl.measurechart[x].measure.name
                                    });
                                    seriesDefinitions.push(sdef);

                                }
                                var xAxisDefinition = {
                                    titlePath: ctrl.fixeddimensionfields.name,
                                    isDateTime: false
                                };

                                for (var i = 0; i < ctrl.measurechart.length; i++) {
                                    if (ctrl.measurechart[i].API != undefined) {
                                        ctrl.measurechart[i].API.renderChart(chartData[i], chartDefinition[i], seriesDefinitions[i], xAxisDefinition);
                                    }   
                                }
                        }

                            ctrl.isGettingEntityStatistics = false;
                        });
                }

                if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Modules/Analytics/Directives/vr-generalchart.html";
            }
        };

        return directiveDefinitionObject;
    }

    vrDirectiveObj.$inject = ['GenericAnalyticAPIService', 'GenericAnalyticDimensionEnum'];

    app.directive('vrAnGeneralchart', vrDirectiveObj);

})(angular, app);
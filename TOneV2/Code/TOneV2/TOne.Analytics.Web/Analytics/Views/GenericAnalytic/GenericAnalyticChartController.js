(function (appControllers) {

    "use strict";

    genericAnalyticChartController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticDimensionEnum'];
    function genericAnalyticChartController($scope, GenericAnalyticAPIService, GenericAnalyticDimensionEnum) {

        defineScope();
        function defineScope() {

            $scope.subViewConnector.retrieveDataChart = function (value) {
                var groupKeys = [];
                var measureChartValues = [];
                $scope.MeasureChart = value.MeasureChart;
                if (value.FixedDimensionFields == undefined)
                    value.FixedDimensionFields = [];

                value.FixedDimensionFields.forEach(function (group) {
                    groupKeys.push(group.value);

                    if (group == GenericAnalyticDimensionEnum.Hour) {
                        groupKeys.push(GenericAnalyticDimensionEnum.Date.value);
                    }
                });

                for (var i = 0, len = value.MeasureChart.length; i < len; i++) {
                    measureChartValues.push(value.MeasureChart[i].measure.value);
                }

                var query = {
                    Filters: $scope.selectedobject.selectedfilters,
                    DimensionFields: groupKeys,
                    MeasureFields: measureChartValues,
                    FromTime: $scope.selectedobject.fromdate,
                    ToTime: $scope.selectedobject.todate,
                    Currency: $scope.selectedobject.currency
                }

                var dataRetrievalInput = {
                    DataRetrievalResultType: 0,
                    IsSortDescending: false,
                    ResultKey: null,
                    SortByColumnName: 'DimensionValues[0].Name',
                    Query: query
                }

                return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                    .then(function (response) {

                        $scope.isGettingEntityStatistics = true;

                        var chartData = new Array();
                        var chartDefinition = [];
                        var dates = [];
                        var y = 0;
                        var seriesDefinitions = [];
                        var countDates = 0;

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

                        for (var x = 0; x < value.MeasureChart.length; x++) {
                            chartData[x] = new Array();
                        }

                        for (var x = 0; x < value.MeasureChart.length; x++) {
                            
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
                                    values['hour'] = response.Data[i].DimensionValues[0].Name;
                                    for (var k = 0; k < dates.length; k++) {
                                        if (response.Data[i].DimensionValues[1].Name == dates[k])
                                            values['date' + k + 'Value'] = response.Data[i].MeasureValues[value.MeasureChart[x].measure.name];
                                        else {
                                            values['date' + k + 'Value'] = 0;
                                        }
                                    }
                                    chartData[x].push(values);
                                }
                                else {
                                    //Update the data values
                                    chartData[x][j]['date' + k + 'Value'] = response.Data[i].MeasureValues[value.MeasureChart[x].measure.name];
                                }
                            }

                            var title = "Traffic by " + value.FixedDimensionFields[0].description + " for " + value.MeasureChart[x].measure.description;
                            chartDefinition.push({
                                type: "column",
                                title: title,
                                yAxisTitle: "Value"
                            });
                        }
                        
                        var xAxisDefinition = {
                            titlePath: "hour",
                            isDateTime: false
                        };

                        for (var i = 0, len = value.MeasureChart.length; i < len; i++) {
                            if (value.MeasureChart[i].API!=undefined)
                                value.MeasureChart[i].API.renderChart(chartData[i], chartDefinition[i], seriesDefinitions, xAxisDefinition);
                        }
    
                        $scope.isGettingEntityStatistics = false;
                    });
            };
        }
    }
    appControllers.controller('GenericAnalyticChartController', genericAnalyticChartController);

})(appControllers);
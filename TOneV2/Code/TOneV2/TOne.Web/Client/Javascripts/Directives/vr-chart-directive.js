'use strict';


app.directive('vrChart', ['ChartDirService', function (ChartDirService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element) {
            var controller = this;
            var chartObj;

            var api = {};
            api.showLoader = function () {
                $scope.isLoading = true;
            };

            api.hideLoader = function () {
                $scope.isLoading = false;
            };

            api.hideChart = function () {
                if ($element.find('#divChart').highcharts() != undefined)
                    $element.find('#divChart').highcharts().destroy();
            };

            var currentChartData;
            function onDataItemClicked(dataItem) {
                console.log(dataItem);
                if (api.onDataItemClicked && typeof (api.onDataItemClicked) == 'function')
                    api.onDataItemClicked(dataItem);
            }

            api.renderSingleDimensionChart = function (chartData, chartDefinition, seriesDefinitions) {
                currentChartData = chartData;
                var series = [];
                angular.forEach(seriesDefinitions, function (sDef) {
                    var serie = {
                        name: sDef.title,
                        data: [],
                        events:
                        {
                            click: function (e) {
                                onDataItemClicked(currentChartData[e.point.index]);
                            }
                        }
                    };
                    series.push(serie);
                });

                angular.forEach(chartData, function (dataItem) {
                    for (var i = 0; i < series.length; i++) {
                        var sDef = seriesDefinitions[i];
                        series[i].data.push({
                            name: dataItem[seriesDefinitions[i].titleFieldName],
                            y: dataItem[seriesDefinitions[i].valueFieldName]
                        }
                            );
                    }
                });

                $element.find('#divChart').highcharts({
                    chart: {
                        type: chartDefinition.type,
                        options3d: {
                            enabled: true,
                            alpha: 35,
                            beta: 0
                        }
                    },
                    title: {
                        text: chartDefinition.title
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            depth: 35,
                            dataLabels: {
                                enabled: !chartDefinition.showLegendsWithValues,
                                format: '{point.name}'
                            },
                            showInLegend: true,
                        }
                    },
                    legend: {
                        enabled: true,
                        layout: 'vertical',
                        align: 'right',
                        //width: chartDefinition.showLegendsWithValues ? 220 : 10,
                        verticalAlign: 'top',
                        y: 15,
                        borderWidth: 0,
                        useHTML: true,
                        labelFormatter: function () {
                            if (chartDefinition.showLegendsWithValues)
                                return '<div style="width:200px"><span style="float:left" title="' + this.name + '">' + (this.name != null && this.name.length > 15 ? this.name.substring(0, 15) + '..' : this.name) + '</span><span style="float:right">' + this.y.toFixed(2) + '</span></div>';
                            else
                                return '<div style="width:10px" title="' + this.name + '">' + (this.name != null && this.name.length > 2 ? this.name.substring(0, 2) : this.name) + '</div>';
                        }
                    },
                    yAxis: {
                        title: {
                            text: chartDefinition.yAxisTitle
                        }
                    },
                    series: series,
                    tooltip: {
                        shared: true
                    },
                });
            };

            api.renderChart = function (chartData, chartDefinition, seriesDefinitions, xAxisDefinition) {
                currentChartData = chartData;
                var xAxis = [];
                var series = [];
                angular.forEach(seriesDefinitions, function (sDef) {
                    var serie = {
                        name: sDef.title,
                        data: [],
                        events:
                        {
                            click: function (e) {
                                onDataItemClicked(currentChartData[e.point.index]);
                            }
                        },
                        type: sDef.type ? sDef.type : chartDefinition.type
                    };
                    series.push(serie);
                });

                angular.forEach(chartData, function (dataItem) {

                    if (xAxisDefinition.groupFieldName != 'undefined' && xAxisDefinition.groupFieldName != null) {
                        var groupName = dataItem[xAxisDefinition.groupFieldName];
                        if (groupName == null) {
                            xAxis.push(dataItem[xAxisDefinition.titleFieldName]);
                        }
                        else {
                            var group = null;
                            angular.forEach(xAxis, function (grp) {
                                if (grp.name == groupName)
                                    group = grp;
                            });
                            if (group == null) {
                                group = {
                                    name: groupName,
                                    categories: []
                                };
                                xAxis.push(group);
                            }
                            group.categories.push(dataItem[xAxisDefinition.titleFieldName]);
                        }
                    }
                    else {
                        xAxis.push(dataItem[xAxisDefinition.titleFieldName]);
                    }
                    for (var i = 0; i < series.length; i++) {
                        series[i].data.push(dataItem[seriesDefinitions[i].valueFieldName]);
                    }
                });



                chartObj = $element.find('#divChart').highcharts({
                    chart: {
                        options3d: {
                            enabled: false,
                            alpha: 10,
                            beta: 10,
                            depth: 0,
                            viewDistance: 25
                        }
                    },
                    title: {
                        text: chartDefinition.title
                    },
                    plotOptions: {
                        column: {
                            depth: 25
                        }
                    },
                    xAxis: {
                        categories: xAxis
                    },
                    yAxis: {
                        title: {
                            text: chartDefinition.yAxisTitle
                        }
                    },
                    series: series,
                    tooltip: {
                        shared: true
                    },
                });
            };

            if (controller.onReady && typeof (controller.onReady) == 'function')
                controller.onReady(api);
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
                        
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    
                    
                }
            }
        },
        templateUrl: function (element, attrs) {
            return ChartDirService.dTemplate;
        }

    };

    return directiveDefinitionObject;

}]);
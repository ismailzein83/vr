'use strict';


app.directive('vrSolidgaugeChart', ['ChartDirService', 'VRModalService', 'UtilsService', function (ChartDirService, VRModalService, UtilsService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '=',
            hidesettings: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var chartElement = $element.find('#divChart');
            var menuActionsAttribute = $attrs.menuactions != undefined ? $scope.$parent.$eval($attrs.menuactions) : undefined;

            var chart = new Chart(ctrl, chartElement, $scope, VRModalService);
            chart.initializeController();
            chart.defineAPI();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return "/Client/Javascripts/Directives/Chart/vr-gauge-chart-template.html";
        }

    };

    function Chart(ctrl, chartElement, $scope, VRModalService) {
        var chartObj;
        var currentChartSource;
        var currentChartSettings;
        var isChartAvailable = false;
        var selectedDataItem;
        var menuActionsAttribute;
        var api = {};
        var newValue;
        function initializeController() {
        }

        function initializeChartSettings() {
            if (currentChartSettings == undefined) {
                currentChartSettings = {
                    showValuesWithLegends: true,
                    series: []
                };

                angular.forEach(currentChartSource.seriesDefinitions, function (sDef) {
                    var serieSettings = {
                        title: sDef.title,
                        selected: sDef.selected != undefined ? sDef.selected : true,
                        type: sDef.type ? sDef.type : currentChartSource.chartDefinition.type
                    };
                    currentChartSettings.series.push(serieSettings);
                });
            }
        }
        function renderChart(chartSource) {
            var chartData = chartSource.chartData;
            var chartDefinition = chartSource.chartDefinition;
            var seriesDefinitions = chartSource.seriesDefinitions;
            var yAxisDefinition = chartSource.yAxisDefinition;
            var series = [];
           
            angular.forEach(seriesDefinitions, function (sDef) {
                var serie = {
                    name: sDef.title,
                    tooltip: sDef.tooltip,

                    data: [],
                    events:
                    {
                        click: function (e) {
                        }
                    },
                };
                series.push(serie);
            });
            angular.forEach(chartData, function (dataItem) {
                for (var i = 0; i < series.length; i++) {
                    var sDef = seriesDefinitions[i];
                    var tooltip = sDef.tooltip;
                    series[i].data.push(dataItem);
                }
            });


            setTimeout(function () {
                chartElement.highcharts({
                    chart: {
                        type: chartDefinition.type,
                        events: {
                            load: function () {
                                var series = this.series;
                                setInterval(function () {
                                    if (newValue != undefined) {
                                        for (var i = 0; i < series.length; i++) {
                                            var serie = series[i];
                                            //for (var j = 0;j < serie.points.length; j++)
                                            //{
                                            //    var point = serie.points[j];
                                            //    point.update(newValue,false,true);
                                            //}
                                            series[i].addPoint(newValue, true, true);
                                        }
                                    }
                                });
                            }
                        }
                    },
                    title: {
                        text: chartDefinition.title
                    },
                    plotOptions: {
                        solidgauge: {
                            dataLabels: {
                                y: 5,
                                borderWidth: 1,
                                useHTML: true
                            },
                        }
                    },
                    yAxis: {
                        min: yAxisDefinition.min,
                        max: yAxisDefinition.max,
                        title: {
                            text: yAxisDefinition.title
                        },
                        stops: chartDefinition.ranges,
                        lineWidth: 0,
                        minorTickInterval: null,
                        tickLength: 0,
                        tickInterval: yAxisDefinition.interval,
                        labels: {
                            y: 16
                        }
                    },
                    series: series,
                    credits: {
                        enabled: false
                    },
                    pane: {
                        center: ['50%', '85%'],
                        size: '140%', 
                        startAngle: -90,
                        endAngle: 90,
                        background: {
                            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#EEE',
                            innerRadius: '60%',
                            outerRadius: '100%',
                            shape: 'arc'
                        }
                    }
                });
                isChartAvailable = true;
            }, 1)

        }

        function defineAPI() {
            api.renderChart = function (chartData, chartDefinition, seriesDefinitions, xAxisDefinition, yAxisDefinition) {
                currentChartSource = {
                    chartData: chartData,
                    chartDefinition: chartDefinition,
                    seriesDefinitions: seriesDefinitions,
                    xAxisDefinition: xAxisDefinition,
                    yAxisDefinition: yAxisDefinition,
                };
                initializeChartSettings();
                renderChart(currentChartSource);
            };
            api.updateValue = function (value) {
                newValue = value;
            };
            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }

    return directiveDefinitionObject;

}]);

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
            return ChartDirService.dTemplate;
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
                    //dataLabels: {
                    //    format: '<div style="text-align:center"><span style="font-size:25px;color:' +
                    //        ((Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black') + '">{y}</span><br/>' +
                    //           '<span style="font-size:12px;color:silver">' + chartData + '</span></div>'
                    //},
                };
                series.push(serie);
            });
            for (var i = 0; i < series.length; i++) {
                series[i].data.push(yAxisDefinition.max);
                series[i].data.push(yAxisDefinition.min);
            }

            setTimeout(function () {
                console.log(yAxisDefinition.max);
                chartElement.highcharts({
                    chart: {
                        type: chartDefinition.type,
                    },
                    title: {
                        text: chartDefinition.title
                    },
                    plotOptions: {
                        solidgauge: {
                            dataLabels: {
                                y: 5,
                                borderWidth: 0,
                                useHTML: true
                            }
                        }
                    },
                    yAxis: {
                        min: 0,
                        max: yAxisDefinition.max,
                    //    maxColor:'#55BF3B',
                        title: {
                            text: yAxisDefinition.title
                        },
                        stops: [
                            [0.1, '#55BF3B'], // green
                            [0.5, '#DDDF0D'], // yellow
                            [0.9, '#DF5353'] // red
                        ],
                        lineWidth: 0,
                        minorTickInterval: null,
                        tickAmount: 2,
                        showFirstLabel: false,
                        showLastLabel:true,
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

            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }

    return directiveDefinitionObject;

}]);

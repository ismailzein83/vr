'use strict';


app.directive('vrGaugeChart', ['ChartDirService', 'VRModalService', 'UtilsService', function (ChartDirService, VRModalService, UtilsService) {

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
          //  chart.defineMenu(menuActionsAttribute);
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
            return "/Client/Javascripts/Directives/Chart/vr-guage-chart-template.html";
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

            ctrl.actionClicked = function (action) {
                action.clicked(selectedDataItem);
            };
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
            var items = chartSource.items;

            var series = [];
            angular.forEach(seriesDefinitions, function (sDef) {
                var serie = {
                    name: sDef.title,
                    tooltip:sDef.tooltip,
                    data: [],
                    events:
                    {
                        click: function (e) {
                        }
                    },
                   
                };
                series.push(serie);
            });

            //angular.forEach(chartData, function (dataItem) {
            //    for (var i = 0; i < series.length; i++) {
            //        var sDef = seriesDefinitions[i];
            //        var titleValue = eval('dataItem.' + sDef.titlePath);
            //        var yValue = eval('dataItem.' + sDef.valuePath);
            //        var tooltip = sDef.tooltip;
            //        series[i].data.push({
            //            name: titleValue,
            //            y: yValue,
            //        });
            //    }
            //});

            for (var i = 0; i < series.length; i++) {
                series[i].data.push(chartData);
            }
            setTimeout(function () {
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

                        minorTickInterval: 'auto',
                        minorTickWidth: 1,
                        minorTickLength: 10,
                        minorTickPosition: 'inside',
                        minorTickColor: '#666',

                   //     tickAmount: 2,

                        tickPixelInterval: 30,
                        tickWidth: 2,
                        tickPosition: 'inside',
                        tickLength: 0,
                        tickColor: '#666',
                        labels: {
                            step: 1,
                            rotation: 'auto',
                            //formatter: function () {
                            //    if (this.value == 0) return yAxisDefinition.min;
                            //    if (this.value == 100) return yAxisDefinition.max;
                            //    return "";
                            //},
                            
                        },
                        //showFirstLabel: true,
                        //showLastLabel: true,
                        title: {
                            text: '%'
                        },
                        plotBands: [{
                            from: 0,
                            to: yAxisDefinition.mid,
                            color: '#55BF3B' // green
                        }, {
                            from: yAxisDefinition.mid,
                            to: yAxisDefinition.max,
                            color: '#DDDF0D' // yellow
                        },
                        //{
                        //    from: 40,
                        //    to: 100,
                        //    color: '#DF5353' // red
                        //}
                        ]
                    },
                    series: series,
                    pane: {
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
            api.renderChart = function (chartData, chartDefinition, seriesDefinitions, xAxisDefinition,yAxisDefinition,items) {
                currentChartSource = {
                    chartData: chartData,
                    chartDefinition: chartDefinition,
                    seriesDefinitions: seriesDefinitions,
                    xAxisDefinition: xAxisDefinition,
                    yAxisDefinition: yAxisDefinition,
                    items: items
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

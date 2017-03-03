'use strict';


app.directive('vrCustomgaugeChart', ['ChartDirService', 'VRModalService', 'UtilsService', function (ChartDirService, VRModalService, UtilsService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '=',
            hidesettings: '@',
            hideexporticon:'@',
            height: '@'
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
            return "/Client/Javascripts/Directives/Chart/vr-customgauge-chart.html";
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
        var chartAPI;
        function initializeController() {
            ctrl.chartStyle = { height: '300px' };
            if (ctrl.height != undefined)
                ctrl.chartStyle.height = ctrl.height;
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

            //define serires
            angular.forEach(seriesDefinitions, function (sDef) {

                //dialSettings
                var dialSettings = {
                    backgroundColor: '#777',
                    baseLength: '1%',
                    baseWidth: 15,
                    rearLength: 0,
                    topWidth: 3,
                    borderColor: '#B17964',
                    borderWidth: 0,
                };

                var serie = {
                    name: sDef.title,
                    tooltip: sDef.tooltip,
                    dial: dialSettings,
                    pivot: {
                        radius: 10,
                        backgroundColor: '#777',
                    },
                    data: [],
                    events:
                    {
                        click: function (e) {
                        }
                    },
                };
                series.push(serie);
            });

            // define data
            angular.forEach(chartData, function (dataItem) {
                for (var i = 0; i < series.length; i++) {
                    var sDef = seriesDefinitions[i];
                    var tooltip = sDef.tooltip;
                    series[i].data.push(dataItem);
                }
            });

            var plotBands = [];
            if (chartDefinition.ranges != undefined)
            {
                for(var i=0;i<chartDefinition.ranges.length ; i++)
                {
                    var range = chartDefinition.ranges[i];
                    plotBands.push({
                        from: range.from,
                        to: range.to,
                        innerRadius: '50%',
                        outerRadius: '95%',
                        color: range.color,
                    });
                }
            }



            //chartSettings
            var chartSettings = {
                type: chartDefinition.type,
                events: {
                    load: function () {
                        chartAPI = this;
                    }
                }
            };

            //plotOptions
            var plotOptions = {
                gauge: {
                    dial: // simpleDialNeedle
                    {
                        // needle extending from pivot
                        baseLength: 8, // how high the fat part rises
                        baseWidth: 2, // fat part of needle                   
                        rearLength: 20, // below the pivot                    
                        radius: 70, // this is how tall the needle goes (%)
                        borderColor: '#fff',
                        borderWidth: 0,
                    }
                }
            };

            //yAxis
            var yAxis =  [{
                min: yAxisDefinition.min,
                max: yAxisDefinition.max,
                tickInterval: yAxisDefinition.interval,
                endOnTick: false,
                //offset: -4,
                labels: {
                    distance: 15,
                    rotation: 'auto', // 0
                    align: 'center',
                    style: {
                        color: '#666',
                        // fontWeight: 'bold',
                        fontSize: '9px',
                    }
                },
                stops: chartDefinition.ranges,
                lineWidth: 5,
                lineColor: '#77c5db',
                tickLength: 10,
                tickWidth: 2,
                tickColor: '#77c5db',
                tickPosition: 'outside',

                // minor tick
                minorTickColor: '#00f',
                minorTickPosition: 'outside',
                minorTickLength: 0,
                plotBands: plotBands
            }];

           // setTimeout(function () {
                chartElement.highcharts({
                    chart: chartSettings,
                    title: {
                        text: null,
                    },
                    plotOptions: plotOptions,
                    yAxis: yAxis,
                    series: series,
                    exporting:{
                        enabled: ctrl.hideexporticon == undefined
                    },
                    pane: {
                        startAngle: -90,
                        endAngle: 90,
                        background: [],
                    }
                });
                isChartAvailable = true;
         //   }, 1)

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
                chartAPI.series[0].addPoint(value, true, true);
            }
            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        function resizeChart(time) {
            setTimeout(function () {
                if (chartElement.highcharts() != undefined)
                    chartElement.highcharts().setSize(chartElement.parent().width(), chartElement.height(), true);

            }, time);
        }
        $(window).resize(function () {
            resizeChart(500);

        });
        $scope.$on('menu-full', function (event, args) {
            resizeChart(500);
        });
        $scope.$on('menu-collapsed', function (event, args) {
            resizeChart(500);
        });

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }

    return directiveDefinitionObject;

}]);

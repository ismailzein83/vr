'use strict';


app.directive('vrChart', ['ChartDirService', '$modal', function (ChartDirService, $modal) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '=',
            hidesettings: '@'
        },
        controller: function ($scope, $element) {
            var controller = this;
            var chartObj;
            var currentChartSource;
            var currentChartSettings;
            var isChartAvailable = false;           


            function InitializeChartSettings()
            {
                if (currentChartSettings == undefined) {
                    currentChartSettings = {
                        isSingleDimension: currentChartSource.isSingleDimension,
                        showValuesWithLegends: false,
                        series: []
                    };

                    angular.forEach(currentChartSource.seriesDefinitions, function (sDef) {
                        var serieSettings = {
                            title: sDef.title,
                            selected: true,
                            type: sDef.type ? sDef.type : currentChartSource.chartDefinition.type
                        };
                        currentChartSettings.series.push(serieSettings);
                    });
                }
            }

            function onDataItemClicked(dataItem) {
                console.log(dataItem);
                if (api.onDataItemClicked && typeof (api.onDataItemClicked) == 'function')
                    api.onDataItemClicked(dataItem);
            }

            function renderSingleDimensionChart(chartSource) {
                var chartData = chartSource.chartData;
                var chartDefinition = chartSource.chartDefinition;
                var seriesDefinitions = chartSource.seriesDefinitions;

                var series = [];
                angular.forEach(seriesDefinitions, function (sDef) {
                    var serie = {
                        name: sDef.title,
                        data: [],
                        events:
                        {
                            click: function (e) {
                                onDataItemClicked(currentChartSource.chartData[e.point.index]);
                            }
                        }
                    };
                    series.push(serie);
                });

                angular.forEach(chartData, function (dataItem) {
                    for (var i = 0; i < series.length; i++) {
                        var sDef = seriesDefinitions[i];
                        var titleValue = eval('dataItem.' + sDef.titlePath);
                        var yValue = eval('dataItem.' + sDef.valuePath);

                        series[i].data.push({
                            name: titleValue,
                            y: yValue
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
                                enabled: !currentChartSettings.showValuesWithLegends,
                                format: '{point.name}'
                            },
                            showInLegend: true,
                        }
                    },
                    legend: {
                        enabled: true,
                        layout: 'vertical',
                        align: 'right',
                        verticalAlign: 'top',
                        y: 15,
                        borderWidth: 0,
                        useHTML: true,
                        labelFormatter: function () {
                            if (currentChartSettings.showValuesWithLegends)
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
                isChartAvailable = true;
            }

            function renderChart(chartSource) {
                var chartData = chartSource.chartData;
                var chartDefinition = chartSource.chartDefinition;
                var seriesDefinitions = chartSource.seriesDefinitions;
                var xAxisDefinition = chartSource.xAxisDefinition;
                var xAxis = [];
                var series = [];
                angular.forEach(seriesDefinitions, function (sDef) {
                    var serieSettings = $.grep(currentChartSettings.series, function (s) {
                        return sDef.title == s.title;
                    })[0];
                    if (serieSettings.selected) {
                        var serie = {
                            name: sDef.title,
                            data: [],
                            events:
                            {
                                click: function (e) {
                                    onDataItemClicked(currentChartSource.chartData[e.point.index]);
                                }
                            },
                            type: serieSettings.type ? serieSettings.type : (sDef.type ? sDef.type : chartDefinition.type)
                        };
                        series.push(serie);
                    }
                });

                angular.forEach(chartData, function (dataItem) {

                    var xValue = eval('dataItem.' + xAxisDefinition.titlePath);
                    if (xAxisDefinition.isDateTime)
                        xValue = dateFormat((new Date(xValue)), "dd-mmm-yy HH:mm:ss");
                    else if (xAxisDefinition.isDate)
                        xValue = dateFormat((new Date(xValue)), "dd-mmm-yy");// new Date(xValue);
                    if (xAxisDefinition.groupNamePath != 'undefined' && xAxisDefinition.groupNamePath != null) {
                        var groupName = eval('dataItem.' + xAxisDefinition.groupNamePath);
                        if (groupName == null) {
                            xAxis.push(xValue);
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
                            group.categories.push(xValue);
                        }
                    }
                    else {
                        xAxis.push(xValue);
                    }

                    for (var i = 0; i < series.length; i++) {
                        var sDef = seriesDefinitions[i];

                        var yValue = eval('dataItem.' + sDef.valuePath);
                        series[i].data.push(yValue);
                    }
                });

                var chartSettings = {
                    options3d: {
                        enabled: false,
                        alpha: 10,
                        beta: 10,
                        depth: 0,
                        viewDistance: 25
                    }
                };

                var titleSettings = {
                    text: chartDefinition.title
                };

                var plotOptionsSettings = {
                    column: {
                        depth: 25
                    }
                };

                var xAxisSettings = {
                    categories: xAxis
                };

                var yAxisSettings = {
                    title: {
                        text: chartDefinition.yAxisTitle
                    }
                };

                var seriesSettings = series;

                var tooltipSettings = {
                    shared: true
                };

                chartObj = $element.find('#divChart').highcharts({
                    chart: chartSettings,
                    title: titleSettings,
                    plotOptions: plotOptionsSettings,
                    xAxis: xAxisSettings,
                    yAxis: yAxisSettings,
                    series: seriesSettings,
                    tooltip: tooltipSettings,
                });
                isChartAvailable = true;
            }
           
            $scope.changeSettings = function () {
               
                var modalScope = $scope.$root.$new();
                modalScope.title = 'Chart Settings';
                modalScope.config = (JSON.parse(JSON.stringify(currentChartSettings)));
                var seriesTypes = [{ value: "column" },
                       { value: "line" },
                       { value: "spline" },
                       { value: "area" },
                       { value: "areaspline" },
                       { value: "scatter" },
                       { value: "bar" }
                ];
                
                angular.forEach(modalScope.config.series, function (s) {
                    s.seriesTypesOption = {
                        datasource: seriesTypes,
                        lastselectedvalue: $.grep(seriesTypes, function(sType){return s.type == sType.value;})[0]
                    };

                });

                modalScope.save = function () {
                    modalInstance.hide();
                    currentChartSettings = modalScope.config;
                    if (currentChartSettings.isSingleDimension) {
                        renderSingleDimensionChart(currentChartSource);
                    }
                    else {
                        angular.forEach(modalScope.config.series, function (s) {
                            s.type = s.seriesTypesOption.lastselectedvalue.value;
                        });
                        renderChart(currentChartSource);
                    }
                };
                var modalInstance = $modal({ scope: modalScope, template: '/Client/Templates/Directives/vr-chart-settings.html', show: true, animation: "am-fade-and-scale" });
            }

            $scope.isSettingsVisible = function () {
                return (controller.hidesettings == undefined || controller.hidesettings == false) && isChartAvailable;
            }
             
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
                isChartAvailable = false;
            };
            api.renderSingleDimensionChart = function (chartData, chartDefinition, seriesDefinitions) {
                currentChartSource = {
                    isSingleDimension: true,
                    chartData: chartData,
                    chartDefinition: chartDefinition,
                    seriesDefinitions: seriesDefinitions
                };
                InitializeChartSettings();
                renderSingleDimensionChart(currentChartSource);
            };

            api.renderChart = function (chartData, chartDefinition, seriesDefinitions, xAxisDefinition) {
                currentChartSource = {
                    isSingleDimension: false,
                    chartData: chartData,
                    chartDefinition: chartDefinition,
                    seriesDefinitions: seriesDefinitions,
                    xAxisDefinition: xAxisDefinition
                };
                InitializeChartSettings();
                renderChart(currentChartSource);
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
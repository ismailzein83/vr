'use strict';


app.directive('vrChart', ['ChartDirService', 'VR_ChartDefinitionTypeEnum', 'VRModalService', 'UtilsService', function (ChartDirService, VR_ChartDefinitionTypeEnum, VRModalService, UtilsService) {

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
            chart.defineMenu(menuActionsAttribute);
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
            ctrl.isSettingsVisible = function () {
                return (ctrl.hidesettings == undefined || ctrl.hidesettings == false) && isChartAvailable;
            };

            ctrl.changeSettings = changeSettings;

            ctrl.actionClicked = function (action) {
                action.clicked(selectedDataItem);
            };
        }

        function initializeChartSettings() {
            clearSettingsIfDifferentSeries();

            if (currentChartSettings == undefined) {
                currentChartSettings = {
                    isSingleDimension: currentChartSource.isSingleDimension,
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

        function clearSettingsIfDifferentSeries() {
            if (currentChartSettings != undefined) {
                if (currentChartSettings.series.length != currentChartSource.seriesDefinitions.length) {
                    currentChartSettings = undefined;
                    return;
                }

                for (var i = 0; i < currentChartSource.seriesDefinitions.length; i++) {
                    if (UtilsService.getItemByVal(currentChartSettings.series, currentChartSource.seriesDefinitions[i].title, "title") == null) {
                        currentChartSettings = undefined;
                        return;
                    }
                }
            }
        }

        function changeSettings() {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = 'Chart Settings';
                modalScope.config = (JSON.parse(JSON.stringify(currentChartSettings)));

                var seriesTypes = UtilsService.getArrayEnum(VR_ChartDefinitionTypeEnum);

                angular.forEach(modalScope.config.series, function (s) {
                    s.seriesTypesOption = {
                        datasource: seriesTypes,
                        lastselectedvalue: $.grep(seriesTypes, function (sType) { return s.type == sType.value; })[0]
                    };

                });
                
                modalScope.save = function () {
                    modalScope.modalContext.closeModal();
                    currentChartSettings = modalScope.config;                    
                    if (currentChartSettings.isSingleDimension) {
                        renderSingleDimensionChart(currentChartSource);
                    }
                    else {
                        angular.forEach(modalScope.config.series, function (s) {
                            s.type = (s.seriesTypesOption.lastselectedvalue != undefined) ? s.seriesTypesOption.lastselectedvalue.value : undefined;
                        });
                        renderChart(currentChartSource);
                    }
                };

                modalScope.validateChartDefinitionTypes = function () {
                    if (modalScope.config.isSingleDimension)
                        return null;
                    var previousOrientation;
                    for (var i = 0; i < modalScope.config.series.length; i++) {
                        if (!modalScope.config.series[i].selected)
                            continue;
                        var selectedType = modalScope.config.series[i].seriesTypesOption.lastselectedvalue;
                        if (selectedType == undefined)
                            continue;
                        if (previousOrientation == undefined)
                            previousOrientation = selectedType.orientation;
                        else if (selectedType.orientation != previousOrientation)
                            return 'All types must have the same orientation';
                    }
                    return null;
                };
            };

            VRModalService.showModal('/Client/Javascripts/Directives/Chart/vr-chart-settings.html', null, modalSettings);
        }

        function onDataItemClicked(dataItem) {
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
                            selectedDataItem = currentChartSource.chartData[e.point.index];
                            ctrl.menuLeft = e.chartX;
                            ctrl.menuTop = e.chartY;
                            $scope.$apply(function () {
                                $scope.selectedEntityTitle = eval('currentChartSource.chartData[e.point.index].' + sDef.titlePath);
                                showMenu(selectedDataItem);
                            });
                            if (e.point.sliced == true) {//it is sliced when it is clicked
                                //$scope.$apply(function () {
                                //    hideMenu();
                                //});
                                return;
                            }
                            onDataItemClicked(selectedDataItem)
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
            api.hideChart();
            setTimeout(function () {
                chartElement.highcharts({
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

                        formatter: function () {
                            var s = this.key;
                            s += '<br/><span style="color:' + this.point.color + '">\u25CF</span> ' + this.series.name + ': <b>' + formatValue(this.point.y) + '</b>';
                            return s;
                        },
                        shared: true
                    },
                });
                resizeChart(1);
                isChartAvailable = true;
            }, 1)


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
                        serieDefinition: sDef,
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
                    var serie = series[i];
                    var sDef = serie.serieDefinition;

                    var yValue = eval('dataItem.' + sDef.valuePath);
                    series[i].data.push(yValue);
                }
            });

            var chartSettings = {
                zoomType: 'x',
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
                formatter: function () {
                    var s = '<b>' + this.x + '</b>';
                    $.each(this.points, function (i, point) {
                        s += '<br/><span style="color:' + point.series.color + '">\u25CF</span> ' + point.series.name + ': ' + formatValue(point.y);
                    });

                    return s;
                },
                shared: true
            };
            api.hideChart();
            chartObj = chartElement.highcharts({
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

        function formatValue(value) {
            return (UtilsService.isIntegerValue(value) ? Highcharts.numberFormat(value, 0) : (UtilsService.isNumericValue(value) ? Highcharts.numberFormat(value, 4) : value));
        }

        function defineAPI() {
            api.hideChart = function () {
                if (chartElement.highcharts() != undefined)
                    chartElement.highcharts().destroy();
                isChartAvailable = false;
            };
            api.renderSingleDimensionChart = function (chartData, chartDefinition, seriesDefinitions) {
                currentChartSource = {
                    isSingleDimension: true,
                    chartData: chartData,
                    chartDefinition: chartDefinition,
                    seriesDefinitions: seriesDefinitions
                };
                //currentChartSource.isSingleDimension = false;
                //currentChartSource.chartDefinition.type = "column";
                //currentChartSource.xAxisDefinition = {
                //    titlePath: seriesDefinitions[0].titlePath
                //};
                initializeChartSettings();
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
                initializeChartSettings();
                renderChart(currentChartSource);
                resizeChart(1);
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        function showMenu(dataItem) {
            if (menuActionsAttribute != undefined) {
                var actions = typeof (menuActionsAttribute) == 'function' ? menuActionsAttribute(dataItem) : menuActionsAttribute;
                if (actions != undefined && actions != null && actions.length > 0) {
                    ctrl.menuActions = actions;
                    ctrl.showMenu = true;
                }
            }
        }

        function hideMenu() {
            ctrl.showMenu = false;
        }

        function defineMenu(menuActions) {
            menuActionsAttribute = menuActions;
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
        this.defineMenu = defineMenu;
    }

    return directiveDefinitionObject;

}]);

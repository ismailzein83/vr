'use strict';


app.directive('vrChart', ['ChartDirService', 'VR_ChartDefinitionTypeEnum', 'VRModalService', 'UtilsService', function (ChartDirService, VR_ChartDefinitionTypeEnum, VRModalService, UtilsService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '=',
            hidesettings: '@',
            hideexporticon: '@',
            height: '@'
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
        var chartAPI;
        var counter = 0;
        var xAxisValues = [];
        var enablePoints = null;
        function initializeController() {
            ctrl.isSettingsVisible = function () {
                return (ctrl.hidesettings == undefined || ctrl.hidesettings == false) && isChartAvailable;
            };

            ctrl.changeSettings = changeSettings;

            ctrl.actionClicked = function (action) {
                action.clicked(selectedDataItem);
            };

            ctrl.chartStyle = { height: '300px' };
            if (ctrl.height != undefined)
                ctrl.chartStyle.height = ctrl.height;
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

            // define Series
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

            // define data
            angular.forEach(chartData, function (dataItem) {
                for (var i = 0; i < series.length; i++) {
                    var sDef = seriesDefinitions[i];
                    var titleValue = eval('dataItem.' + sDef.titlePath);
                    var yValue = eval('dataItem.' + sDef.valuePath);

                    series[i].data.push({
                        name: titleValue,
                        y: yValue,
                        id: counter++
                    }
                        );
                }
            });

            api.hideChart();
           setTimeout(function () {

                //chartSettings
                var chartSettings = {
                    type: chartDefinition.type,
                    options3d: {
                        enabled: true,
                        alpha: 35,
                        beta: 0
                    },
                    events: {
                        load: function () {
                            chartAPI = this;
                        }
                    }
                };

                //plotOptions
                var plotOptions = {
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
                };

                //legend
                var legend = {
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
                };

                //tooltip
                var tooltip ={

                    formatter: function () {
                        var s = this.key;
                        s += '<br/><span style="color:' + this.point.color + '">\u25CF</span> ' + this.series.name + ': <b>' + formatValue(this.point.y) + '</b>';
                        return s;
                    },
                    shared: true
                };

                //yAxis
                var yAxis = {
                    title: {
                        text: chartDefinition.yAxisTitle
                    }
                };

                //exporting
                var exporting = {
                    enabled: ctrl.hideexporticon == undefined
                };

                chartElement.highcharts({
                    chart: chartSettings,
                    title: {
                        text: chartDefinition.title
                    },
                    plotOptions: plotOptions,
                    legend: legend,
                    exporting: exporting,
                    yAxis: yAxis,
                    series: series,
                    tooltip: tooltip,
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
            xAxisValues = [];
            var series = [];

            // define Series
            angular.forEach(seriesDefinitions, function (sDef) {
                var serieSettings = $.grep(currentChartSettings.series, function (s) {
                    return sDef.title == s.title;
                })[0];
                if (serieSettings.selected) {
                    var serie = {
                        name: sDef.title,
                        serieDefinition: sDef,
                        color: sDef.color,
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

            // define data and categories
            angular.forEach(chartData, function (dataItem) {
                var xValue = getXValue(dataItem);
                if (xAxisDefinition.groupNamePath != 'undefined' && xAxisDefinition.groupNamePath != null) {
                    var groupName = eval('dataItem.' + xAxisDefinition.groupNamePath);
                    if (groupName == null) {
                        xAxisValues.push(xValue);
                    }
                    else {
                        var group = null;
                        angular.forEach(xAxisValues, function (grp) {
                            if (grp.name == groupName)
                                group = grp;
                        });
                        if (group == null) {
                            group = {
                                name: groupName,
                                categories: []
                            };
                            xAxisValues.push(group);
                        }
                        group.categories.push(xValue);
                    }
                }
                else {
                    xAxisValues.push(xValue);
                }

                for (var i = 0; i < series.length; i++) {
                    var serie = series[i];
                    var yValue = getYValue(serie.serieDefinition, dataItem);
                    series[i].data.push(yValue);
                }
            });


            //chartSettings
            var chartSettings = {
                zoomType: 'x',
                //marginRight: 10,
              //  animation: Highcharts.svg,
                options3d: {
                    enabled: false,
                    alpha: 10,
                    beta: 10,
                    depth: 0,
                    viewDistance: 25
                },
                events: {
                    load: function () {
                        chartAPI = this;
                    }
                },
                
            };

           

            //titleSettings
            var titleSettings = {
                text: chartDefinition.title
            };

            //plotOptionsSettings
            var plotOptionsSettings = {
                
                column: {
                    depth: 25,
                    marker: {
                        enabled: enablePoints
                    }
                },
                areaspline: {
                    marker: {
                        enabled: enablePoints
                    }
                },
                line: {
                    marker: {
                        enabled: enablePoints
                    }
                },
                spline: {
                    marker: {
                        enabled: enablePoints
                    }

                },
                area: {
                    marker: {
                        enabled: enablePoints
                    }
                },
                scatter: {
                    marker: {
                        enabled: enablePoints
                    }
                },
                bar: {
                    marker: {
                        enabled: enablePoints
                    }
                }
            };

            //xAxisSettings
            var xAxisSettings = {
                categories: xAxisValues,
                visible: xAxisDefinition.hideAxes == undefined ? true : false,
                tickWidth: xAxisDefinition.hideAxes != undefined ? 0 : 1,
                lineWidth: xAxisDefinition.hideAxes != undefined ? 0 : 1,
                showEmpty: xAxisDefinition.hideAxes == undefined ? true : false,
                labels: {
                    enabled: xAxisDefinition.hideAxes == undefined ? true : false
                }
            };
      
            var plotLines = [];
            if (chartDefinition.lines != undefined) {
                for (var i = 0; i < chartDefinition.lines.length ; i++) {
                    var line = chartDefinition.lines[i];
                    plotLines.push({
                        value: line.value,
                        width: line.width,
                        color: line.color,
                        label: {
                            text: line.label
                        },
                        dashStyle: 'shortdash',
                    });
                }
            }

            //yAxisSettings
            var yAxisSettings = {
                title: {
                    text: chartDefinition.yAxisTitle
                },
                plotLines: plotLines

            };
            var seriesSettings = series;

            //tooltipSettings
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

            if (chartDefinition.useAnimation) {
                chartSettings.animation = {
                    duration: 2000,
                    easing: "jswing"
                };
                plotOptionsSettings.series = {
                    animation: {
                        duration: 2000,
                        easing: 'jswing'
                    },
                };
            }
            api.hideChart();
            setTimeout(function () {
                chartObj = chartElement.highcharts({
                    chart: chartSettings,
                    title: titleSettings,
                    plotOptions: plotOptionsSettings,
                    xAxis: xAxisSettings,
                    yAxis: yAxisSettings,
                    series: seriesSettings,
                    tooltip: tooltipSettings,
                    exporting: {
                        enabled: ctrl.hideexporticon == undefined
                    }
                });
                isChartAvailable = true;
            });
        }
        function getXValue(dataItem) {
            var xValue = eval('dataItem.' + currentChartSource.xAxisDefinition.titlePath);
            if (currentChartSource.xAxisDefinition.isDateTime)
                xValue = dateFormat((UtilsService.createDateFromString(xValue)), "dd-mmm-yy HH:MM:ss");

            else if (currentChartSource.xAxisDefinition.isTime)
                xValue = dateFormat((UtilsService.createDateFromString(xValue)), "HH:MM:ss");
            else if (currentChartSource.xAxisDefinition.isDate)
                xValue = dateFormat((UtilsService.createDateFromString(xValue)), "dd-mmm-yy");// new Date(xValue);

            return xValue;


        }

        function getYValue(sDef, dataItem) {
            return eval('dataItem.' + sDef.valuePath);
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
            api.addItem = function (item) {
                var axes = chartAPI.xAxis;
                var series = chartAPI.series;
                for (var i = 0; i < series.length; i++) {
                    var serie = series[i];
                    var sDef = currentChartSource.seriesDefinitions[i];
                    var xValue = getXValue(item);
                    var yValue = getYValue(sDef, item);
                    xAxisValues.push(xValue);
                    axes[0].setCategories(xAxisValues);
                    series[i].addPoint([xValue,yValue], true, true, {
                        duration: 2000,
                        easing: "jswing"
                    });
                    
                }
            };
            api.updateValues = function(items)
            {
                if (currentChartSource != undefined) {
                    if (items != undefined && items.length > 0) {
                        if (currentChartSource.isSingleDimension) {
                            var series = chartAPI.series;
                            for (var i = 0; i < series.length; i++) {
                                var serie = series[i];
                                var sDef = currentChartSource.seriesDefinitions[i];

                                var yValues = [];
                                for (var j = 0; j < items.length; j++) {
                                    var item = items[j];
                                    addToYValue(item);
                                }
                                function addToYValue(dataItem)
                                {
                                    var titleValue = eval('dataItem.' + sDef.titlePath);
                                    var yValue = getYValue(sDef, dataItem);
                                    yValues.push({
                                        name: titleValue,
                                        y: yValue,
                                        id:counter++
                                    });
                                }
                                serie.update({
                                    data: yValues
                                }, true);
                            }
                        }
                        else {
                            var axis = chartAPI.axes;
                            var series = chartAPI.series;
                                for (var i = 0; i < series.length; i++) {
                                    var serie = series[i];
                                    var sDef = currentChartSource.seriesDefinitions[i];

                                    var yValues = [];
                                    for (var j = 0; j < items.length; j++) {
                                        var updatedValue = items[j];
                                        var yValue = getYValue(sDef, updatedValue);
                                        yValues.push(yValue);
                                    }
                                    serie.update({
                                        data: yValues
                                    }, true);
                                    //series[i].setData(yValues, true, true, true);
                                }
                                var ax = axis[0];
                                var xValues = [];
                                for (var j = 0; j < items.length; j++) {
                                    var updatedValue = items[j];
                                    xValues.push(getXValue(updatedValue));
                                }
                                ax.setCategories(xValues,true);
                        }
                    }
                }
            }
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
                if (chartDefinition != undefined && chartDefinition.enablePoints != undefined)
                    enablePoints = chartDefinition.enablePoints;
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



/*
 * jQuery Easing v1.3 - http://gsgd.co.uk/sandbox/jquery/easing/
 *
 * Uses the built in easing capabilities added In jQuery 1.1
 * to offer multiple easing options
 *
 * TERMS OF USE - jQuery Easing
 * 
 * Open source under the BSD License. 
 * 
 * Copyright © 2008 George McGinley Smith
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of 
 * conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list 
 * of conditions and the following disclaimer in the documentation and/or other materials 
 * provided with the distribution.
 * 
 * Neither the name of the author nor the names of contributors may be used to endorse 
 * or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 *  COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
 *  GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 *
*/

// t: current time, b: begInnIng value, c: change In value, d: duration
jQuery.easing['jswing'] = jQuery.easing['swing'];

jQuery.extend(jQuery.easing,
{
    def: 'easeOutQuad',
    swing: function (x, t, b, c, d) {
        //alert(jQuery.easing.default);
        return jQuery.easing[jQuery.easing.def](x, t, b, c, d);
    },
    easeInQuad: function (x, t, b, c, d) {
        return c * (t /= d) * t + b;
    },
    easeOutQuad: function (x, t, b, c, d) {
        return -c * (t /= d) * (t - 2) + b;
    },
    easeInOutQuad: function (x, t, b, c, d) {
        if ((t /= d / 2) < 1) return c / 2 * t * t + b;
        return -c / 2 * ((--t) * (t - 2) - 1) + b;
    },
    easeInCubic: function (x, t, b, c, d) {
        return c * (t /= d) * t * t + b;
    },
    easeOutCubic: function (x, t, b, c, d) {
        return c * ((t = t / d - 1) * t * t + 1) + b;
    },
    easeInOutCubic: function (x, t, b, c, d) {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t + 2) + b;
    },
    easeInQuart: function (x, t, b, c, d) {
        return c * (t /= d) * t * t * t + b;
    },
    easeOutQuart: function (x, t, b, c, d) {
        return -c * ((t = t / d - 1) * t * t * t - 1) + b;
    },
    easeInOutQuart: function (x, t, b, c, d) {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    },
    easeInQuint: function (x, t, b, c, d) {
        return c * (t /= d) * t * t * t * t + b;
    },
    easeOutQuint: function (x, t, b, c, d) {
        return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
    },
    easeInOutQuint: function (x, t, b, c, d) {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
    },
    easeInSine: function (x, t, b, c, d) {
        return -c * Math.cos(t / d * (Math.PI / 2)) + c + b;
    },
    easeOutSine: function (x, t, b, c, d) {
        return c * Math.sin(t / d * (Math.PI / 2)) + b;
    },
    easeInOutSine: function (x, t, b, c, d) {
        return -c / 2 * (Math.cos(Math.PI * t / d) - 1) + b;
    },
    easeInExpo: function (x, t, b, c, d) {
        return (t == 0) ? b : c * Math.pow(2, 10 * (t / d - 1)) + b;
    },
    easeOutExpo: function (x, t, b, c, d) {
        return (t == d) ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b;
    },
    easeInOutExpo: function (x, t, b, c, d) {
        if (t == 0) return b;
        if (t == d) return b + c;
        if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
        return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;
    },
    easeInCirc: function (x, t, b, c, d) {
        return -c * (Math.sqrt(1 - (t /= d) * t) - 1) + b;
    },
    easeOutCirc: function (x, t, b, c, d) {
        return c * Math.sqrt(1 - (t = t / d - 1) * t) + b;
    },
    easeInOutCirc: function (x, t, b, c, d) {
        if ((t /= d / 2) < 1) return -c / 2 * (Math.sqrt(1 - t * t) - 1) + b;
        return c / 2 * (Math.sqrt(1 - (t -= 2) * t) + 1) + b;
    },
    easeInElastic: function (x, t, b, c, d) {
        var s = 1.70158; var p = 0; var a = c;
        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (!p) p = d * .3;
        if (a < Math.abs(c)) { a = c; var s = p / 4; }
        else var s = p / (2 * Math.PI) * Math.asin(c / a);
        return -(a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
    },
    easeOutElastic: function (x, t, b, c, d) {
        var s = 1.70158; var p = 0; var a = c;
        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (!p) p = d * .3;
        if (a < Math.abs(c)) { a = c; var s = p / 4; }
        else var s = p / (2 * Math.PI) * Math.asin(c / a);
        return a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * (2 * Math.PI) / p) + c + b;
    },
    easeInOutElastic: function (x, t, b, c, d) {
        var s = 1.70158; var p = 0; var a = c;
        if (t == 0) return b; if ((t /= d / 2) == 2) return b + c; if (!p) p = d * (.3 * 1.5);
        if (a < Math.abs(c)) { a = c; var s = p / 4; }
        else var s = p / (2 * Math.PI) * Math.asin(c / a);
        if (t < 1) return -.5 * (a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
        return a * Math.pow(2, -10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
    },
    easeInBack: function (x, t, b, c, d, s) {
        if (s == undefined) s = 1.70158;
        return c * (t /= d) * t * ((s + 1) * t - s) + b;
    },
    easeOutBack: function (x, t, b, c, d, s) {
        if (s == undefined) s = 1.70158;
        return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
    },
    easeInOutBack: function (x, t, b, c, d, s) {
        if (s == undefined) s = 1.70158;
        if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
        return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
    },
    easeInBounce: function (x, t, b, c, d) {
        return c - jQuery.easing.easeOutBounce(x, d - t, 0, c, d) + b;
    },
    easeOutBounce: function (x, t, b, c, d) {
        if ((t /= d) < (1 / 2.75)) {
            return c * (7.5625 * t * t) + b;
        } else if (t < (2 / 2.75)) {
            return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
        } else if (t < (2.5 / 2.75)) {
            return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
        } else {
            return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
        }
    },
    easeInOutBounce: function (x, t, b, c, d) {
        if (t < d / 2) return jQuery.easing.easeInBounce(x, t * 2, 0, c, d) * .5 + b;
        return jQuery.easing.easeOutBounce(x, t * 2 - d, 0, c, d) * .5 + c * .5 + b;
    }
});

/*
 *
 * TERMS OF USE - EASING EQUATIONS
 * 
 * Open source under the BSD License. 
 * 
 * Copyright © 2001 Robert Penner
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of 
 * conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list 
 * of conditions and the following disclaimer in the documentation and/or other materials 
 * provided with the distribution.
 * 
 * Neither the name of the author nor the names of contributors may be used to endorse 
 * or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 *  COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
 *  GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 *
 */
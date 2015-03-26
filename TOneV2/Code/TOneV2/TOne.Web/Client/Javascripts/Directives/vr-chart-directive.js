'use strict';


app.directive('vrChart', ['ChartDirService', function (ChartDirService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element) {
            //var controller = this;
            
            var api = {};
            api.showLoader = function () {
                $scope.isLoading = true;
            };

            api.hideLoader = function () {
                $scope.isLoading = false;
            };

            api.renderChart = function (chartData, chartDefinition, seriesDefinitions, xAxisDefinition) {
                var xAxis = [];
                var series = [];
                angular.forEach(seriesDefinitions, function (sDef) {
                    var serie = {
                        name: sDef.title,
                        data: [],
                        type: sDef.type ? sDef.type : chartDefinition.type
                    };
                    series.push(serie);
                });

                angular.forEach(chartData, function (dataItem) {

                    if (xAxisDefinition.groupFieldName != 'undefined' && xAxisDefinition.groupFieldName != null) {
                        var groupName = dataItem[xAxisDefinition.groupFieldName];
                        if (groupName == null) {
                            xAxis.push(dataItem[xAxisDefinition.fieldName]);
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
                            group.categories.push(dataItem[xAxisDefinition.fieldName]);
                        }
                    }
                    else {
                        xAxis.push(dataItem[xAxisDefinition.fieldName]);
                    }
                    for (var i = 0; i < series.length; i++) {
                        series[i].data.push(Number(dataItem[seriesDefinitions[i].fieldName]));
                    }
                });

                $element.find('#divChart').highcharts({
                    chart: {
                        options3d: {
                            enabled: false,
                            alpha: 15,
                            beta: 15,
                            depth: 50,
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
            if ($scope.onReady && typeof ($scope.onReady) == 'function')
                $scope.onReady(api);
        },
        //controllerAs: 'ctrl',
        //bindToController: true,
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
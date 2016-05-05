﻿(function (app) {

    'use strict';

    WidgetsChartDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_ChartDefinitionTypeEnum'];

    function WidgetsChartDefinition(UtilsService, VRUIUtilsService, VR_ChartDefinitionTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var widgetsChart = new WidgetsChart($scope, ctrl, $attrs);
                widgetsChart.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/Widgets/Chart/Templates/ChartWidgetDefinitionTemplate.html"

        };
        function WidgetsChart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;



            var dimensionSelectorAPI;
            var dimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var measureSelectorAPI;
            var measureReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dimensions = [];
                $scope.scopeModel.measures = [];
                $scope.scopeModel.topRecords = 10;
                $scope.scopeModel.chartTypes = [];
                $scope.scopeModel.selectedChartType;
                $scope.scopeModel.topMeasure;

                $scope.scopeModel.onDimensionSelectorDirectiveReady = function (api) {
                    dimensionSelectorAPI = api;
                    dimensionReadyDeferred.resolve();
                }

                $scope.scopeModel.onSelectDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name
                    };
                    $scope.scopeModel.dimensions.push(dataItem);
                }

                $scope.scopeModel.onDeselectDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                }

                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                }

                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    var dataItem = {
                        AnalyticItemConfigId: measure.AnalyticItemConfigId,
                        Title: measure.Title,
                        Name: measure.Name,
                    };
                    $scope.scopeModel.measures.push(dataItem);
                }

                $scope.scopeModel.onDeselectMeasureItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.Name, 'Name');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.tableIds != undefined) {
                        var tableIds = payload.tableIds;
                        var selectedDimensionIds;
                        var selectedMeasureIds;

                        var promises = [];

                        var loadDimensionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        dimensionReadyDeferred.promise.then(function () {
                            var payloadGroupingDirective = {
                                filter: { TableIds: tableIds },
                                selectedIds: selectedDimensionIds
                            };

                            VRUIUtilsService.callDirectiveLoad(dimensionSelectorAPI, payloadGroupingDirective, loadDimensionDirectivePromiseDeferred);
                        });
                        promises.push(loadDimensionDirectivePromiseDeferred.promise);

                        var loadMeasureDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        measureReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: { TableIds: tableIds },
                                selectedIds: selectedMeasureIds
                            };

                            VRUIUtilsService.callDirectiveLoad(measureSelectorAPI, payloadFilterDirective, loadMeasureDirectivePromiseDeferred);
                        });
                        promises.push(loadMeasureDirectivePromiseDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }


                };
                loadChartTypes();
                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.Widgets.AnalyticChartWidget, Vanrise.Analytic.MainExtensions ",
                        Measures: getMeasures(),
                        Dimensions: getDimensions(),
                        TopRecords: $scope.scopeModel.topRecords,
                        TopMeasure: $scope.scopeModel.topMeasure
                    }
                    return data;
                }

                function getDimensions() {
                    var dimensions;
                    if ($scope.scopeModel.dimensions != undefined && $scope.scopeModel.dimensions.length > 0) {
                        dimensions = [];
                        for (var i = 0; i < $scope.scopeModel.dimensions.length; i++) {
                            var dimension = $scope.scopeModel.dimensions[i];
                            dimensions.push({
                                DimensionName: dimension.Name,
                                Title: dimension.Title,
                                IsRootDimension: dimension.IsRootDimension,

                            });
                        }
                    }
                    return dimensions;
                }

                function getMeasures() {
                    var measures;
                    if ($scope.scopeModel.measures != undefined && $scope.scopeModel.measures.length > 0) {
                        measures = [];
                        for (var i = 0; i < $scope.scopeModel.measures.length; i++) {
                            var measure = $scope.scopeModel.measures[i];
                            measures.push({
                                MeasureName: measure.Name,
                                Title: measure.Title,
                            });
                        }
                    }
                    return measures;
                }

                function loadChartTypes() {
                    $scope.scopeModel.chartTypes = [];
                    for (var m in VR_ChartDefinitionTypeEnum) {
                        $scope.scopeModel.chartTypes.push(VR_ChartDefinitionTypeEnum[m]);
                    }
                    $scope.scopeModel.selectedChartType = $scope.scopeModel.chartTypes[0];
                }
            }
        }
    }

    app.directive('vrAnalyticWidgetsChartDefinition', WidgetsChartDefinition);

})(app);
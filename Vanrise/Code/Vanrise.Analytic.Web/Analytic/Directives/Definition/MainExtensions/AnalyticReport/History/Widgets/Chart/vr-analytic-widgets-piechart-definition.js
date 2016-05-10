(function (app) {

    'use strict';

    WidgetsPieChartDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_ChartDefinitionTypeEnum'];

    function WidgetsPieChartDefinition(UtilsService, VRUIUtilsService, VR_ChartDefinitionTypeEnum) {
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
                var widgetsChart = new WidgetsPieChart($scope, ctrl, $attrs);
                widgetsChart.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/History/Widgets/Chart/Templates/PieChartWidgetDefinitionTemplate.html"

        };
        function WidgetsPieChart($scope, ctrl, $attrs) {
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
                $scope.scopeModel.Measure;

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


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.tableIds != undefined) {
                        var tableIds = payload.tableIds;
                        var selectedDimensionIds;
                        var selectedMeasureIds;
                        if (payload.widgetEntity != undefined) {
                            $scope.scopeModel.rootDimensionsFromSearch = payload.widgetEntity.RootDimensionsFromSearchSection;
                            selectedDimensionIds = [];
                            if (payload.widgetEntity.Dimensions != undefined && payload.widgetEntity.Dimensions.length > 0) {
                                for (var i = 0; i < payload.widgetEntity.Dimensions.length; i++) {
                                    var dimension = payload.widgetEntity.Dimensions[i];
                                    selectedDimensionIds.push(dimension.DimensionName);
                                    $scope.scopeModel.dimensions.push({
                                        Name: dimension.DimensionName,
                                        Title: dimension.Title,
                                        IsRootDimension: dimension.IsRootDimension,
                                    });
                                }
                            }

                            selectedMeasureIds = [];
                            if (payload.widgetEntity.Measures != undefined && payload.widgetEntity.Measures.length > 0) {
                                for (var i = 0; i < payload.widgetEntity.Measures.length; i++) {
                                    var measure = payload.widgetEntity.Measures[i];
                                    selectedMeasureIds.push(measure.MeasureName);
                                    $scope.scopeModel.measures.push({
                                        Name: measure.MeasureName,
                                        Title: measure.Title,
                                    });
                                }
                            }
                        }
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

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.History.Widgets.AnalyticPieChartWidget, Vanrise.Analytic.MainExtensions ",
                        Dimensions: getDimensions(),
                        TopRecords: $scope.scopeModel.topRecords,
                        Measure: getMeasure($scope.scopeModel.Measure.MeasureName)
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

                function getMeasure(measureName) {
                    var measure = UtilsService.getItemByVal($scope.scopeModel.measures, measureName, "Name");
                    return measure;
                }
            }
        }
    }

    app.directive('vrAnalyticWidgetsPiechartDefinition', WidgetsPieChartDefinition);

})(app);
(function (app) {

    'use strict';

    WidgetsChartDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_ChartDefinitionTypeEnum','VR_Analytic_AutoRefreshType'];

    function WidgetsChartDefinition(UtilsService, VRUIUtilsService, VR_ChartDefinitionTypeEnum, VR_Analytic_AutoRefreshType) {
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
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/History/Widgets/Chart/Templates/ChartWidgetDefinitionTemplate.html"

        };
        function WidgetsChart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;



            var dimensionSelectorAPI;
            var dimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var seriesDimensionSelectorAPI;
            var seriesDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var measureSelectorAPI;
            var measureReadyDeferred = UtilsService.createPromiseDeferred();

            var orderTypeSelectorAPI;
            var orderTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.autoRefreshTypes = UtilsService.getArrayEnum(VR_Analytic_AutoRefreshType);

                $scope.scopeModel.dimensions = [];
                $scope.scopeModel.seriesDimensions = [];
                $scope.scopeModel.measures = [];
                $scope.scopeModel.chartTypes = [];
                $scope.scopeModel.selectedChartType;
                $scope.scopeModel.topMeasure;
                $scope.scopeModel.selectedOrderType;
                $scope.scopeModel.onDimensionSelectorDirectiveReady = function (api) {
                    dimensionSelectorAPI = api;
                    dimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onSeriesDimensionSelectorDirectiveReady = function (api) {
                    seriesDimensionSelectorAPI = api;
                    seriesDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                };

                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    var dataItem = {
                        AnalyticItemConfigId: measure.AnalyticItemConfigId,
                        Title: measure.Title,
                        Name: measure.Name,
                    };
                    $scope.scopeModel.measures.push(dataItem);
                };

                $scope.scopeModel.onDeselectMeasureItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.Name, 'Name');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.onOrderTypeSelectorReady = function (api) {
                    orderTypeSelectorAPI = api;
                    orderTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.seriesDimensionsCustomValidation = function () {
                    if ($scope.scopeModel.selectedSeriesDimensions != undefined && $scope.scopeModel.selectedSeriesDimensions.length > 0 && $scope.scopeModel.measures.length !=undefined  && $scope.scopeModel.measures.length > 1)
                        return "Only One Measure can be selected when Series Dimension is not empty";
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    loadChartTypes();

                    if (payload != undefined && payload.tableIds != undefined) {
                        var tableIds = payload.tableIds;
                        var selectedDimensionIds;
                        var selectedSeriesDimensionIds;
                        var selectedMeasureIds;

                        if (payload.widgetEntity != undefined) {
                            $scope.scopeModel.rootDimensionsFromSearch = payload.widgetEntity.RootDimensionsFromSearch;
                            selectedDimensionIds = [];
                            selectedSeriesDimensionIds = [];

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

                            if (payload.widgetEntity.SeriesDimensions != undefined && payload.widgetEntity.SeriesDimensions.length > 0) {
                                for (var i = 0; i < payload.widgetEntity.SeriesDimensions.length; i++) {
                                    var dimension = payload.widgetEntity.SeriesDimensions[i];
                                    selectedSeriesDimensionIds.push(dimension.DimensionName);
                                    $scope.scopeModel.seriesDimensions.push({
                                        Name: dimension.DimensionName,
                                        Title: dimension.Title
                                    });
                                }
                            }
                            if (payload.widgetEntity.AutoRefreshType != undefined) {
                                $scope.scopeModel.selectedAutoRefreshType = UtilsService.getItemByVal($scope.scopeModel.autoRefreshTypes, payload.widgetEntity.AutoRefreshType, "value");;
                                $scope.scopeModel.autoRefreshInterval = payload.widgetEntity.AutoRefreshInterval;
                                $scope.scopeModel.numberOfPoints = payload.widgetEntity.NumberOfPoints;
                            }
                            setTopMeasure(payload.widgetEntity.TopMeasure);
                            $scope.scopeModel.selectedChartType = UtilsService.getItemByVal($scope.scopeModel.chartTypes, payload.widgetEntity.ChartType, "value");
                          
                            $scope.scopeModel.topRecords = payload.widgetEntity.TopRecords;
                        }
                    };


                    var loadDimensionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    dimensionReadyDeferred.promise.then(function () {
                        var payloadGroupingDirective = {
                            filter: { TableIds: tableIds },
                            selectedIds: selectedDimensionIds
                        };

                        VRUIUtilsService.callDirectiveLoad(dimensionSelectorAPI, payloadGroupingDirective, loadDimensionDirectivePromiseDeferred);
                    });
                    promises.push(loadDimensionDirectivePromiseDeferred.promise);

                    var loadSeriesDimensionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    seriesDimensionReadyDeferred.promise.then(function () {
                        var payloadGroupingDirective = {
                            filter: { TableIds: tableIds },
                            selectedIds: selectedSeriesDimensionIds
                        };

                        VRUIUtilsService.callDirectiveLoad(seriesDimensionSelectorAPI, payloadGroupingDirective, loadSeriesDimensionDirectivePromiseDeferred);
                    });
                    promises.push(loadSeriesDimensionDirectivePromiseDeferred.promise);

                    var loadMeasureDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    measureReadyDeferred.promise.then(function () {
                        var payloadFilterDirective = {
                            filter: { TableIds: tableIds },
                            selectedIds: selectedMeasureIds
                        };

                        VRUIUtilsService.callDirectiveLoad(measureSelectorAPI, payloadFilterDirective, loadMeasureDirectivePromiseDeferred);
                    });
                    promises.push(loadMeasureDirectivePromiseDeferred.promise);


                    var orderTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(orderTypeSelectorLoadDeferred.promise);
                    orderTypeSelectorReadyDeferred.promise.then(function () {
                        var orderTypeSelectorPayload = { tableIds: payload.tableIds };
                        if (payload.widgetEntity != undefined)
                            orderTypeSelectorPayload.orderTypeEntity = { OrderType: payload.widgetEntity.OrderType, AdvancedOrderOptions: payload.widgetEntity.AdvancedOrderOptions };
                        VRUIUtilsService.callDirectiveLoad(orderTypeSelectorAPI, orderTypeSelectorPayload, orderTypeSelectorLoadDeferred);
                    });

                    orderTypeSelectorLoadDeferred.promise.then(function () {
                    });

                    return UtilsService.waitMultiplePromises(promises);


                };


                api.getData = getData;

                function getData() {
                    var orderTypeEntity = orderTypeSelectorAPI.getData();

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.History.Widgets.AnalyticChartWidget, Vanrise.Analytic.MainExtensions ",
                        Measures: getMeasures(),
                        Dimensions: getDimensions(),
                        SeriesDimensions: getSeriesDimensions(),
                        TopRecords: $scope.scopeModel.topRecords,
                        OrderType: orderTypeEntity != undefined ? orderTypeEntity.OrderType : undefined,
                        ChartType: $scope.scopeModel.selectedChartType.value,
                        RootDimensionsFromSearch: $scope.scopeModel.rootDimensionsFromSearch,
                        AdvancedOrderOptions: orderTypeEntity != undefined ? orderTypeEntity.AdvancedOrderOptions : undefined,
                        AutoRefreshType: $scope.scopeModel.selectedAutoRefreshType != undefined ? $scope.scopeModel.selectedAutoRefreshType.value : undefined,
                        AutoRefreshInterval: $scope.scopeModel.autoRefreshInterval,
                        NumberOfPoints: $scope.scopeModel.numberOfPoints
                    };
                    return data;
                }




                function getDimensions() {
                    var dimensions;
                    if (!$scope.scopeModel.rootDimensionsFromSearch) {
                        if ($scope.scopeModel.selectedDimensions != undefined && $scope.scopeModel.selectedDimensions.length > 0) {
                            dimensions = [];
                            for (var i = 0; i < $scope.scopeModel.selectedDimensions.length; i++) {
                                var dimension = $scope.scopeModel.selectedDimensions[i];
                                dimensions.push({
                                    DimensionName: dimension.Name,
                                    Title: dimension.Title,
                                    IsRootDimension: dimension.IsRootDimension,

                                });
                            }
                        }
                    }
                    return dimensions;
                }

                function getSeriesDimensions() {
                    var seriesDimensions;
                    if ($scope.scopeModel.selectedSeriesDimensions != undefined && $scope.scopeModel.selectedSeriesDimensions.length > 0) {
                        seriesDimensions = [];
                        for (var i = 0; i < $scope.scopeModel.selectedSeriesDimensions.length; i++) {
                            var seriesDimension = $scope.scopeModel.selectedSeriesDimensions[i];
                            seriesDimensions.push({
                                DimensionName: seriesDimension.Name,
                                Title: seriesDimension.Title
                            });
                        }
                    }
                    return seriesDimensions;
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

                function setTopMeasure(measureName) {
                    $scope.scopeModel.topMeasure = UtilsService.getItemByVal($scope.scopeModel.measures, measureName, "Name");
                }
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticWidgetsChartDefinition', WidgetsChartDefinition);

})(app);
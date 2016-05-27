(function (app) {

    'use strict';

    WidgetsGridDefinition.$inject = ["UtilsService", 'VRUIUtilsService'];

    function WidgetsGridDefinition(UtilsService, VRUIUtilsService) {
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
                var widgetsGrid = new WidgetsGrid($scope, ctrl, $attrs);
                widgetsGrid.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/History/Widgets/Grid/Templates/GridWidgetDefinitionTemplate.html"

        };
        function WidgetsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var dimensionSelectorAPI;
            var dimensionReadyDeferred = UtilsService.createPromiseDeferred();
            var measureSelectorAPI;
            var measureReadyDeferred = UtilsService.createPromiseDeferred();

            var measureStyleGridAPI;
            var measureStyleGridReadyDeferred = UtilsService.createPromiseDeferred();

            var orderTypeSelectorAPI;
            var orderTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onDimensionSelectorDirectiveReady = function (api) {
                    dimensionSelectorAPI = api;
                    dimensionReadyDeferred.resolve();
                }
                $scope.scopeModel.dimensions = [];
                $scope.scopeModel.isValidDimensions = function () {

                    if ($scope.scopeModel.dimensions.length > 0 || $scope.scopeModel.rootDimensionsFromSearch)
                        return null;
                    return "At least one dimention should be selected.";
                }
                $scope.scopeModel.onSelectDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsRootDimension: false
                    };
                    $scope.scopeModel.dimensions.push(dataItem);
                }
                $scope.scopeModel.onDeselectDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                }
                $scope.scopeModel.removeDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.onOrderTypeSelectorReady = function (api) {
                  orderTypeSelectorAPI = api;
                  orderTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.measures = [];
                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                }
                $scope.scopeModel.isValidMeasures = function () {

                    if ($scope.scopeModel.measures.length > 0)
                        return null;
                    return "At least one measure should be selected.";
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
                $scope.scopeModel.removeMeasure = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedMeasures, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedMeasures.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.AnalyticItemConfigId, 'Name');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.onMeasureStyleGridDirectiveReady = function(api)
                {
                    measureStyleGridAPI = api;
                    measureStyleGridReadyDeferred.resolve();
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
                            $scope.scopeModel.withSummary = payload.widgetEntity.WithSummary;
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


                        var loadMeasureStyleGridDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        measureStyleGridReadyDeferred.promise.then(function () {
                            var payloadMeasureStyleGridDirective = {
                                context:getContext(),
                                measureStyles: payload != undefined && payload.widgetEntity !=undefined?payload.widgetEntity.MeasureStyles:undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(measureStyleGridAPI, payloadMeasureStyleGridDirective, loadMeasureStyleGridDirectivePromiseDeferred);
                        });
                        promises.push(loadMeasureStyleGridDirectivePromiseDeferred.promise);

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
                        var orderTypeSelectorPayload = undefined;
                        if(payload.widgetEntity != undefined)
                            orderTypeSelectorPayload = { selectedIds : payload.widgetEntity.OrderType };
                        VRUIUtilsService.callDirectiveLoad(orderTypeSelectorAPI, orderTypeSelectorPayload, orderTypeSelectorLoadDeferred);
                        });



                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
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
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.History.Widgets.AnalyticGridWidget, Vanrise.Analytic.MainExtensions ",
                        RootDimensionsFromSearchSection: $scope.scopeModel.rootDimensionsFromSearch,
                        Dimensions: dimensions,
                        Measures: measures,
                        WithSummary: $scope.scopeModel.withSummary,
                    OrderType: orderTypeSelectorAPI.getSelectedIds(),
                    }
                    return data;
                }

            }

            function getContext()
            {
                var context = {
                    getMeasures :function()
                    {
                       return $scope.scopeModel.selectedMeasures;
                    }
                }
                return context;
            }
        }
    }

    app.directive('vrAnalyticWidgetsGridDefinition', WidgetsGridDefinition);

})(app);
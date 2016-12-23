﻿(function (app) {

    'use strict';

    WidgetsGridDefinition.$inject = ["UtilsService", 'VRUIUtilsService','VR_Analytic_AnalyticTypeEnum','VR_Analytic_AnalyticItemConfigAPIService','VR_Analytic_GridWidthEnum','VRCommon_GridWidthFactorEnum'];

    function WidgetsGridDefinition(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_GridWidthEnum, VRCommon_GridWidthFactorEnum) {
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
            controllerAs: "gridCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/History/Widgets/Grid/Templates/GridWidgetDefinitionTemplate.html",
        };
        function WidgetsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dimensionSelectorAPI;
            var dimensionReadyDeferred = UtilsService.createPromiseDeferred();
            var measureSelectorAPI;
            var measureReadyDeferred = UtilsService.createPromiseDeferred();

            var measureStyleGridAPI;
            var measureStyleGridReadyDeferred = UtilsService.createPromiseDeferred();

            var itemActionGridAPI;
            var itemActionGridReadyDeferred = UtilsService.createPromiseDeferred();

            var orderTypeSelectorAPI;
            var orderTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var tableIds;
            var measures;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDimensionSelectorDirectiveReady = function (api) {
                    dimensionSelectorAPI = api;
                    dimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.gridWidths = UtilsService.getArrayEnum(VR_Analytic_GridWidthEnum);

                $scope.scopeModel.dimensions = [];

                $scope.scopeModel.isValidDimensions = function () {

                    if ($scope.scopeModel.dimensions.length > 0 || $scope.scopeModel.rootDimensionsFromSearch)
                        return null;
                    return "At least one dimention should be selected.";
                };

                $scope.scopeModel.onSelectDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,                        
                        FixedWidth: null,
                        IsRootDimension: false
                    };
                    dataItem.onDimensionGridWidthFactorSelectorReady = function (api) {
                        dataItem.dimensionGridWidthFactorAPI = api;
                        var dataItemPayload = { selectedIds: VRCommon_GridWidthFactorEnum.Normal.value };
                        var setLoader = function (value) { $scope.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.dimensionGridWidthFactorAPI, dataItemPayload, setLoader);
                    };
                    dataItem.onWidthSelectionChanged = function () {
                        var obj = dataItem;
                        if (dataItem.dimensionGridWidthFactorAPI.getSelectedIds() == 'FixedWidth') {
                            obj.hasfixedwidth = true;
                        }
                        else
                            obj.hasfixedwidth = false;
                        $scope.scopeModel.dimensions[$scope.scopeModel.dimensions.indexOf(dataItem)] = obj;

                    };
                    $scope.scopeModel.dimensions.push(dataItem);
                };

                $scope.scopeModel.onDeselectDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                };

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

                $scope.scopeModel.measures =[];

                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                };

                $scope.scopeModel.isValidMeasures = function () {

                    if ($scope.scopeModel.measures.length > 0)
                        return null;
                    return "At least one measure should be selected.";
                };

                $scope.scopeModel.onSelectionMeasureChanged = function (measure) {
                    if (measureStyleGridAPI != undefined)
                        measureStyleGridAPI.reloadMeasures();
                };
              
                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    var dataItem = {
                        AnalyticItemConfigId: measure.AnalyticItemConfigId,
                        Title: measure.Title,
                        Name: measure.Name,
                        SelectedGridWidth: VR_Analytic_GridWidthEnum.Normal,
                        FixedWidth: null
                    };
                    dataItem.onMeasureGridWidthFactorSelectorReady = function (api) {
                        dataItem.measureGridWidthFactorAPI = api;
                        var dataItemPayload = { selectedIds: VRCommon_GridWidthFactorEnum.Normal.value };
                        var setLoader = function (value) { $scope.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.measureGridWidthFactorAPI, dataItemPayload, setLoader);
                    };
                    dataItem.onWidthSelectionChanged = function () {
                        var obj = dataItem;
                        if (dataItem.measureGridWidthFactorAPI.getSelectedIds() == 'FixedWidth') {
                            obj.hasfixedwidth = true;
                        }
                        else
                            obj.hasfixedwidth = false;
                        $scope.scopeModel.dimensions[$scope.scopeModel.dimensions.indexOf(dataItem)]= obj;

                    };
                    $scope.scopeModel.measures.push(dataItem);
                };

                $scope.scopeModel.onDeselectMeasureItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.Name, 'Name');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.removeMeasure = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedMeasures, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedMeasures.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.AnalyticItemConfigId, 'Name');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                    if (measureStyleGridAPI != undefined)
                        measureStyleGridAPI.reloadMeasures();
                };

                $scope.scopeModel.onMeasureStyleGridDirectiveReady = function (api) {
                    measureStyleGridAPI = api;
                    measureStyleGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onAnalyticItemActionDirectiveReady = function (api) {
                    itemActionGridAPI = api;
                    itemActionGridReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.tableIds != undefined) {
                        tableIds = payload.tableIds;
                        var promises = [];
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
                                    var dimensionGridField = {
                                        payload: dimension,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(dimensionGridField.loadPromiseDeferred.promise);
                                    addDimensionGridWidthAPI(dimensionGridField);
                                }
                            }

                            selectedMeasureIds = [];
                            if (payload.widgetEntity.Measures != undefined && payload.widgetEntity.Measures.length > 0) {
                                for (var i = 0; i < payload.widgetEntity.Measures.length; i++) {
                                    var measure = payload.widgetEntity.Measures[i];
                                    var measureGridField= {
                                        payload: measure,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    selectedMeasureIds.push(measure.MeasureName);
                                    promises.push(measureGridField.loadPromiseDeferred.promise);
                                    addMeasureGridWidthAPI(measureGridField);
                                }
                            }
                        }

                        

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
                                context: getContext(),

                                measureStyles: payload != undefined && payload.widgetEntity != undefined ? payload.widgetEntity.MeasureStyleRules : undefined
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



                        var loadItemActionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        itemActionGridReadyDeferred.promise.then(function () {
                            var payloadItemActionDirective = {
                                context: getContext(),

                                itemActions: payload != undefined && payload.widgetEntity != undefined ? payload.widgetEntity.ItemActions : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(itemActionGridAPI, payloadItemActionDirective, loadItemActionDirectivePromiseDeferred);
                        });
                        promises.push(loadItemActionDirectivePromiseDeferred.promise);



                        var orderTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(orderTypeSelectorLoadDeferred.promise);

                        orderTypeSelectorReadyDeferred.promise.then(function () {
                            var orderTypeSelectorPayload = undefined;
                            if (payload.widgetEntity != undefined)
                                orderTypeSelectorPayload = { selectedIds: payload.widgetEntity.OrderType };
                            VRUIUtilsService.callDirectiveLoad(orderTypeSelectorAPI, orderTypeSelectorPayload, orderTypeSelectorLoadDeferred);
                        });
                        promises.push(getAllMeasures());


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
                                Width: dimension.dimensionGridWidthFactorAPI.getSelectedIds(),
                                FixedWidth: (dimension.hasfixedwidth == true) ? dimension.FixedWidth : null
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
                                Width: measure.measureGridWidthFactorAPI.getSelectedIds(),
                                FixedWidth: (measure.hasfixedwidth == true) ? measure.FixedWidth: null
                            });
                        }
                    }
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.History.Widgets.AnalyticGridWidget, Vanrise.Analytic.MainExtensions ",
                        RootDimensionsFromSearchSection: $scope.scopeModel.rootDimensionsFromSearch,
                        Dimensions: dimensions,
                        Measures: measures,
                        MeasureStyleRules: measureStyleGridAPI != undefined ? measureStyleGridAPI.getData() : undefined,
                        WithSummary: $scope.scopeModel.withSummary,
                        OrderType: orderTypeSelectorAPI.getSelectedIds(),
                        ItemActions: itemActionGridAPI != undefined ? itemActionGridAPI.getData() : undefined
                    };
                    return data;
                }

            }
            function addDimensionGridWidthAPI(gridField)
            {
                var dataItemPayload = { selectedIds: VRCommon_GridWidthFactorEnum.Normal.value };
                var dataItem = {};
                if (gridField.payload !=undefined) {
                    dataItem.Name = gridField.payload.DimensionName;
                    dataItem.Title = gridField.payload.Title;
                    dataItem.IsRootDimension = gridField.payload.IsRootDimension;
                    dataItemPayload.selectedIds = gridField.payload.Width;
                    dataItem.FixedWidth = gridField.payload.FixedWidth;
                }
                dataItem.onDimensionGridWidthFactorSelectorReady = function (api) {
                    dataItem.dimensionGridWidthFactorAPI = api;
                    gridField.readyPromiseDeferred.resolve();
                };
                dataItem.onWidthSelectionChanged = function () {
                    var obj = dataItem;
                    if (dataItem.dimensionGridWidthFactorAPI.getSelectedIds() == 'FixedWidth') {
                        obj.hasfixedwidth = true;
                    }
                    else
                        obj.hasfixedwidth = false;
                    $scope.scopeModel.dimensions[$scope.scopeModel.dimensions.indexOf(dataItem)] = obj;

                };
                gridField.readyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.dimensionGridWidthFactorAPI, dataItemPayload, gridField.loadPromiseDeferred);
                });
                $scope.scopeModel.dimensions.push(dataItem);
            }
            function addMeasureGridWidthAPI(gridField) {
                var dataItemPayload = { selectedIds: VRCommon_GridWidthFactorEnum.Normal.value };
                var dataItem = {};
                if (gridField.payload != undefined) {
                    dataItem.Name = gridField.payload.MeasureName;
                    dataItem.Title = gridField.payload.Title;
                    dataItemPayload.selectedIds = gridField.payload.Width;
                    dataItem.FixedWidth = gridField.payload.FixedWidth;

                }
                dataItem.onMeasureGridWidthFactorSelectorReady = function (api) {
                    dataItem.measureGridWidthFactorAPI = api;
                    gridField.readyPromiseDeferred.resolve();
                };

                dataItem.onWidthSelectionChanged = function () {
                        var obj = dataItem;
                        if (dataItem.measureGridWidthFactorAPI.getSelectedIds() == 'FixedWidth') {
                            obj.hasfixedwidth = true;
                        }
                        else
                            obj.hasfixedwidth = false;
                            $scope.scopeModel.dimensions[$scope.scopeModel.dimensions.indexOf(dataItem)]= obj;

                 };
              
                gridField.readyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.measureGridWidthFactorAPI, dataItemPayload, gridField.loadPromiseDeferred);
                    });
                $scope.scopeModel.measures.push(dataItem);
            }
            function getAllMeasures() {
                var input = {
                    TableIds: tableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                };
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    measures = response;
                });
            }

            function getContext() {
                var context = {
                    getMeasures: function () {
                        var selectedMeasures = [];
                        for (var i = 0; i < $scope.scopeModel.selectedMeasures.length; i++) {
                            var selectedMeasure = $scope.scopeModel.selectedMeasures[i];
                            var matchItem = UtilsService.getItemByVal(measures, selectedMeasure.Name, "Name");
                            if (matchItem != undefined) {
                                selectedMeasure.FieldType = matchItem.Config.FieldType;
                            }
                            selectedMeasures.push(selectedMeasure);
                        }

                        return selectedMeasures;

                    }
                };
                return context;
            }
        }
    }

    app.directive('vrAnalyticWidgetsGridDefinition', WidgetsGridDefinition);

})(app);
(function (app) {

    'use strict';

    WidgetsGridDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic_GridWidthEnum', 'VRCommon_GridWidthFactorEnum'];

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
                $scope.scopeModel.subTables = [];
                $scope.scopeModel.selectedMeasures = [];
                $scope.scopeModel.isValidDimensions = function () {

                    if ($scope.scopeModel.dimensions.length > 0 || $scope.scopeModel.rootDimensionsFromSearch)
                        return null;
                    return "At least one dimension should be selected.";
                };

                $scope.scopeModel.onSelectDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsRootDimension: false
                    };
                    dataItem.onDimensionGridWidthFactorEditorReady = function (api) {
                        dataItem.dimensionGridWidthFactorAPI = api;
                        var dataItemPayload = {
                            data: {
                                Width: VRCommon_GridWidthFactorEnum.Normal.value
                            }
                        };
                        var setLoader = function (value) { $scope.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.dimensionGridWidthFactorAPI, dataItemPayload, setLoader);
                    };

                    dataItem.onDimensionGridStyleDefinitionReady = function (api) {
                        dataItem.dimensionGridStyleAPI = api;
                        var dataItemPayload;
                        var setLoader = function (value) { $scope.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.dimensionGridStyleAPI, dataItemPayload, setLoader);
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

                $scope.scopeModel.measures = [];

                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                };

                $scope.scopeModel.isValidMeasures = function () {

                    if ($scope.scopeModel.measures.length > 0)
                        return null;
                    return "At least one measure should be selected.";
                };

                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    var dataItem = {
                        AnalyticItemConfigId: measure.AnalyticItemConfigId,
                        Title: measure.Title,
                        Name: measure.Name
                    };
                    dataItem.onMeasureGridWidthFactorEditorReady = function (api) {
                        dataItem.measureGridWidthFactorAPI = api;
                        var dataItemPayload = {
                            data: {
                                Width: VRCommon_GridWidthFactorEnum.Normal.value
                            }
                        };
                        var setLoader = function (value) { $scope.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.measureGridWidthFactorAPI, dataItemPayload, setLoader);
                    };

                    dataItem.onMeasureGridStyleDefinitionReady = function (api) {
                        dataItem.measureGridStyleAPI = api;
                        var dataItemPayload;
                        var setLoader = function (value) { $scope.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.measureGridStyleAPI, dataItemPayload, setLoader);
                    };

                    $scope.scopeModel.measures.push(dataItem);
                };

                $scope.scopeModel.onDeselectMeasureItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.Name, 'Name');
                    resetReferenceMeasureSelector(dataItem.Name);
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.removeMeasure = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedMeasures, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedMeasures.splice(index, 1);
                    resetReferenceMeasureSelector(dataItem.Name);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.Name, 'Name');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);

                };


                $scope.scopeModel.onAnalyticItemActionDirectiveReady = function (api) {
                    itemActionGridAPI = api;
                    itemActionGridReadyDeferred.resolve();
                };

                $scope.scopeModel.addSubTableDefinition = function () {
                    var dataItem =
                    {
                        dimensionReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        dimensionLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                        measureReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        measureLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                        positionValueReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        positionValueLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                    };

                    dataItem.onMeasureGridWidthFactorEditorReady = function (api) {
                        dataItem.measureGridWidthFactorAPI = api;
                        var dataItemPayload = {
                            data: {
                                Width: VRCommon_GridWidthFactorEnum.Normal.value
                            }
                        };
                        var setLoader = function (value) { $scope.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.measureGridWidthFactorAPI, dataItemPayload, setLoader);
                    };

                    dataItem.onMeasureGridStyleDefinitionReady = function (api) {
                        dataItem.measureGridStyleAPI = api;
                        var dataItemPayload;
                        var setLoader = function (value) { $scope.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.measureGridStyleAPI, dataItemPayload, setLoader);
                    };

                    extendSubTableDefinitionDataItem(dataItem);
                };

                $scope.scopeModel.removeSubTableDefinition = function (dataItem) {
                    var index = $scope.scopeModel.subTables.indexOf(dataItem);
                    $scope.scopeModel.subTables.splice(index, 1);
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
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        styleReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        styleLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(dimensionGridField.loadPromiseDeferred.promise);
                                    promises.push(dimensionGridField.styleLoadPromiseDeferred.promise);
                                    addDimensionGridWidthAPI(dimensionGridField);
                                }
                            }

                            selectedMeasureIds = [];
                            if (payload.widgetEntity.Measures != undefined && payload.widgetEntity.Measures.length > 0) {
                                for (var i = 0; i < payload.widgetEntity.Measures.length; i++) {
                                    var measure = payload.widgetEntity.Measures[i];
                                    var measureGridField = {
                                        payload: measure,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        styleReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        styleLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    selectedMeasureIds.push(measure.MeasureName);
                                    promises.push(measureGridField.loadPromiseDeferred.promise);
                                    promises.push(measureGridField.styleLoadPromiseDeferred.promise);
                                    addMeasureGridWidthAPI(measureGridField);
                                }
                            }

                            if (payload.widgetEntity.SubTables != undefined && payload.widgetEntity.SubTables.length > 0) {
                                var subTables = payload.widgetEntity.SubTables;

                                for (var j = 0; j < subTables.length; j++) {
                                    var dataItem = {
                                        dimensionReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        dimensionLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        measureReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        measureLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        positionValueReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        positionValueLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        columnSettingsReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        columnSettingsLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        styleReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        styleLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(dataItem.columnSettingsLoadPromiseDeferred.promise);
                                    promises.push(dataItem.styleLoadPromiseDeferred.promise);
                                    promises.push(dataItem.dimensionLoadPromiseDeferred.promise);
                                    promises.push(dataItem.measureLoadPromiseDeferred.promise);
                                    promises.push(dataItem.positionValueLoadPromiseDeferred.promise);

                                    var subTableDefinitionEntity = subTables[j];

                                    extendSubTableDefinitionDataItem(dataItem, subTableDefinitionEntity);
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
                            var orderTypeSelectorPayload = { tableIds: payload.tableIds };
                            if (payload.widgetEntity != undefined)
                                orderTypeSelectorPayload.orderTypeEntity = { OrderType: payload.widgetEntity.OrderType, AdvancedOrderOptions: payload.widgetEntity.AdvancedOrderOptions };
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
                                ColumnSettings: dimension.dimensionGridWidthFactorAPI.getData(),
                                ColumnStyleId: dimension.dimensionGridStyleAPI.getSelectedIds()
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
                                ColumnSettings: measure.measureGridWidthFactorAPI.getData(),
                                ColumnStyleId: measure.measureGridStyleAPI.getSelectedIds(),
                                IsHidden: measure.isHidden,
                                IsHiddenInListView: measure.isHiddenInListView
                            });
                        }
                    }

                    var subTables;
                    if ($scope.scopeModel.subTables != undefined && $scope.scopeModel.subTables.length > 0) {
                        subTables = [];

                        for (var i = 0; i < $scope.scopeModel.subTables.length; i++) {
                            var subTable = $scope.scopeModel.subTables[i];
                            var measure = {
                                MeasureName: subTable.measureSelectorAPI != undefined ? subTable.measureSelectorAPI.getSelectedIds() : undefined,
                                Title: subTable.Title,
                                ColumnSettings: subTable.measureGridWidthFactorAPI.getData(),
                                ColumnStyleId: subTable.measureGridStyleAPI.getSelectedIds(),
                            };

                            var subTablePosition = {
                                ReferenceMeasure: subTable.positionValueSelectedvalue.showMeasure ? subTable.selectedReferenceMeasure.Name : undefined,
                                PositionValue: subTable.positionValueSelectorAPI != undefined ? subTable.positionValueSelectorAPI.getSelectedIds() : undefined
                            }
                            subTables.push({
                                Dimensions: subTable.dimensionSelectorAPI != undefined ? subTable.dimensionSelectorAPI.getSelectedIds() : undefined,
                                Measures: [measure],
                                SubTablePosition: subTablePosition
                            });
                        }
                    }

                    var orderTypeEntity = orderTypeSelectorAPI.getData();
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.History.Widgets.AnalyticGridWidget, Vanrise.Analytic.MainExtensions ",
                        RootDimensionsFromSearchSection: $scope.scopeModel.rootDimensionsFromSearch,
                        Dimensions: dimensions,
                        Measures: measures,
                        SubTables: subTables,
                        WithSummary: $scope.scopeModel.withSummary,
                        OrderType: orderTypeEntity != undefined ? orderTypeEntity.OrderType : undefined,
                        ItemActions: itemActionGridAPI != undefined ? itemActionGridAPI.getData() : undefined,
                        AdvancedOrderOptions: orderTypeEntity != undefined ? orderTypeEntity.AdvancedOrderOptions : undefined,
                    };
                    return data;
                }

            }
            function addDimensionGridWidthAPI(gridField) {
                var dataItemPayload = {
                    data: {
                        Width: VRCommon_GridWidthFactorEnum.Normal.value
                    }
                };
                var stylePayload;
                var dataItem = {};
                if (gridField.payload != undefined) {
                    dataItem.Name = gridField.payload.DimensionName;
                    dataItem.Title = gridField.payload.Title;
                    dataItem.IsRootDimension = gridField.payload.IsRootDimension;
                    dataItemPayload.data = gridField.payload.ColumnSettings;
                    stylePayload = { selectedIds: gridField.payload.ColumnStyleId };
                }
                dataItem.onDimensionGridWidthFactorEditorReady = function (api) {
                    dataItem.dimensionGridWidthFactorAPI = api;
                    gridField.readyPromiseDeferred.resolve();
                };
                dataItem.onDimensionGridStyleDefinitionReady = function (api) {
                    dataItem.dimensionGridStyleAPI = api;
                    gridField.styleReadyPromiseDeferred.resolve();
                };
                gridField.readyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.dimensionGridWidthFactorAPI, dataItemPayload, gridField.loadPromiseDeferred);
                    });
                gridField.styleReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.dimensionGridStyleAPI, stylePayload, gridField.styleLoadPromiseDeferred);
                    });

                $scope.scopeModel.dimensions.push(dataItem);
            }
            function addMeasureGridWidthAPI(gridField) {
                var dataItemPayload = {
                    data: {
                        Width: VRCommon_GridWidthFactorEnum.Normal.value
                    }
                };
                var stylePayload;
                var dataItem = {};
                if (gridField.payload != undefined) {
                    dataItem.Name = gridField.payload.MeasureName;
                    dataItem.Title = gridField.payload.Title;
                    dataItem.isHidden = gridField.payload.IsHidden;
                    dataItem.isHiddenInListView = gridField.payload.IsHiddenInListView;
                    dataItemPayload.data = gridField.payload.ColumnSettings;
                    stylePayload = { selectedIds: gridField.payload.ColumnStyleId };
                }
                dataItem.onMeasureGridWidthFactorEditorReady = function (api) {
                    dataItem.measureGridWidthFactorAPI = api;
                    gridField.readyPromiseDeferred.resolve();
                };
                dataItem.onMeasureGridStyleDefinitionReady = function (api) {
                    dataItem.measureGridStyleAPI = api;
                    gridField.styleReadyPromiseDeferred.resolve();
                };
                gridField.readyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.measureGridWidthFactorAPI, dataItemPayload, gridField.loadPromiseDeferred);
                    });
                gridField.styleReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.measureGridStyleAPI, stylePayload, gridField.styleLoadPromiseDeferred);
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

            function extendSubTableDefinitionDataItem(dataItem, payloadEntity) {
                var selectedDimensionIds = [];
                var selectedMeasureId;
                var subtablePosition;
                var selectedPositionValue;
                var stylePayload;
                var selectedReferenceMeasure;
                var columnSettingPayload = {
                    data: {
                        Width: VRCommon_GridWidthFactorEnum.Normal.value
                    }
                };

                dataItem.positionValueSelectionChangePromise = undefined;

                if (payloadEntity != undefined) {
                    selectedDimensionIds = payloadEntity.Dimensions;
                    subtablePosition = payloadEntity.SubTablePosition;
                    if (subtablePosition != undefined) {
                        selectedPositionValue = subtablePosition.PositionValue;
                        selectedReferenceMeasure = subtablePosition.ReferenceMeasure;
                    }

                    if (payloadEntity.Measures != undefined && payloadEntity.Measures.length > 0) {
                        var measure = payloadEntity.Measures[0];
                        selectedMeasureId = measure.MeasureName;
                        dataItem.Title = measure.Title;
                        columnSettingPayload.data = measure.ColumnSettings;
                        stylePayload = { selectedIds: measure.ColumnStyleId };
                    }

                    dataItem.onMeasureGridStyleDefinitionReady = function (api) {
                        dataItem.measureGridStyleAPI = api;
                        dataItem.styleReadyPromiseDeferred.resolve();
                    };

                    dataItem.columnSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.measureGridWidthFactorAPI, columnSettingPayload, dataItem.columnSettingsLoadPromiseDeferred);
                        });

                    dataItem.styleReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.measureGridStyleAPI, stylePayload, dataItem.styleLoadPromiseDeferred);
                        });


                    dataItem.positionValueSelectionChangePromise = UtilsService.createPromiseDeferred();
                    dataItem.referenceMeasureReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataItem.onReferenceMeasureSelectorDirectiveReady = function (api) {
                        dataItem.referenceMeasureSelectorAPI = api;
                        dataItem.referenceMeasureReadyPromiseDeferred.resolve();
                    };

                    dataItem.onMeasureGridWidthFactorEditorReady = function (api) {
                        dataItem.measureGridWidthFactorAPI = api;
                        dataItem.columnSettingsReadyPromiseDeferred.resolve();
                    };

                    dataItem.referenceMeasureReadyPromiseDeferred.promise
                        .then(function () {
                            var selectedValue = UtilsService.getItemByVal($scope.scopeModel.selectedMeasures, selectedReferenceMeasure, 'Name');
                            if (selectedValue != null)
                                dataItem.selectedReferenceMeasure = selectedValue;
                        });
                }

                dataItem.onDimensionSelectorDirectiveReady = function (api) {
                    dataItem.dimensionSelectorAPI = api;
                    dataItem.dimensionReadyPromiseDeferred.resolve();
                };

                dataItem.dimensionReadyPromiseDeferred.promise
                    .then(function () {
                        var dimensionDirectivePayload = {
                            filter: { TableIds: tableIds },
                            selectedIds: selectedDimensionIds
                        };
                        VRUIUtilsService.callDirectiveLoad(dataItem.dimensionSelectorAPI, dimensionDirectivePayload, dataItem.dimensionLoadPromiseDeferred);
                    });

                dataItem.onMeasureSelectorDirectiveReady = function (api) {
                    dataItem.measureSelectorAPI = api;
                    dataItem.measureReadyPromiseDeferred.resolve();
                };

                dataItem.measureReadyPromiseDeferred.promise
                    .then(function () {
                        var measureDirectivePayload = {
                            filter: { TableIds: tableIds },
                            selectedIds: selectedMeasureId
                        };
                        VRUIUtilsService.callDirectiveLoad(dataItem.measureSelectorAPI, measureDirectivePayload, dataItem.measureLoadPromiseDeferred);
                    });

                var orderTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataItem.onPositionSelectorReady = function (api) {
                    dataItem.positionValueSelectorAPI = api;
                    dataItem.positionValueReadyPromiseDeferred.resolve();
                };

                dataItem.positionValueReadyPromiseDeferred.promise.then(function () {
                    var positionValueSelectorPayload = {
                        positionValue: selectedPositionValue
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.positionValueSelectorAPI, positionValueSelectorPayload, dataItem.positionValueLoadPromiseDeferred);
                });

                dataItem.onPositionValueSelectionchanged = function (value) {
                    if (value != undefined) {
                        if (dataItem.positionValueSelectionChangePromise != undefined)
                            dataItem.positionValueSelectionChangePromise.resolve();

                        else {
                            dataItem.referenceMeasureReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                            dataItem.onReferenceMeasureSelectorDirectiveReady = function (api) {
                                dataItem.referenceMeasureSelectorAPI = api;
                                dataItem.referenceMeasureReadyPromiseDeferred.resolve();
                            };

                            dataItem.referenceMeasureReadyPromiseDeferred.promise
                                .then(function () {
                                    dataItem.positionValueSelectionChangePromise = undefined;
                                });
                        }
                    }
                };

                $scope.scopeModel.subTables.push(dataItem);
            };

            function resetReferenceMeasureSelector(measureName) {
                if ($scope.scopeModel.subTables == undefined || $scope.scopeModel.subTables.length == 0)
                    return;

                for (var i = 0; i < $scope.scopeModel.subTables.length; i++) {
                    var subTable = $scope.scopeModel.subTables[i];
                    if (subTable.positionValueSelectedvalue != undefined && subTable.positionValueSelectedvalue.showMeasure && subTable.selectedReferenceMeasure != undefined && subTable.selectedReferenceMeasure.Name == measureName) {
                        subTable.selectedReferenceMeasure = undefined;
                    }
                }
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
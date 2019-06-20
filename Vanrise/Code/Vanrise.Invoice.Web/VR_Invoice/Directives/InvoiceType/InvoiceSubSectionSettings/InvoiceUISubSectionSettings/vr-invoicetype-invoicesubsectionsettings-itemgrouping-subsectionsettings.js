"use strict";

app.directive("vrInvoicetypeInvoicesubsectionsettingsItemgroupingSubsectionsettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRCommon_GridWidthFactorEnum", "VRLocalizationService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_GridWidthFactorEnum, VRLocalizationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ItemGroupingSubSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSubSectionSettings/InvoiceUISubSectionSettings/Templates/ItemGroupingSubSectionSettingsTemplate.html"

        };

        function ItemGroupingSubSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var itemGroupingSubSectionsAPI;
            var itemGroupingSubSectionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var context;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.dimensionsItemGroupings = [];
                $scope.scopeModel.measuresItemGroupings = [];
                $scope.scopeModel.measures = [];
                $scope.scopeModel.dimensions = [];
                $scope.scopeModel.selectedDimensions = [];
                $scope.scopeModel.selectedMeasures = [];

                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();


                $scope.scopeModel.onSelectDimensionItem = function (dimension) {
                    var dataItem = {
                        DimensionItemFieldId: dimension.DimensionItemFieldId,
                        FieldDescription: dimension.FieldDescription,
                        FieldName: dimension.FieldName,
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
                    if ($scope.scopeModel.isLocalizationEnabled) {
                        dataItem.onTextResourceSelectorReady = function (api) {
                            dataItem.textResourceSeletorAPI = api;
                            var setLoader = function (value) { dataItem.isDimensionTextResourceSelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                        };
                    }
                    $scope.scopeModel.dimensions.push(dataItem);
                };
                $scope.scopeModel.onDeselectDimensionItem = function (dimension) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dimension.DimensionItemFieldId, 'DimensionItemFieldId');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.removeDimension = function (dimension) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedDimensions, dimension.DimensionItemFieldId, 'DimensionItemFieldId');
                    $scope.scopeModel.selectedDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dimension.DimensionItemFieldId, 'DimensionItemFieldId');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    var dataItem = {
                        MeasureItemFieldId: measure.MeasureItemFieldId,
                        FieldDescription: measure.FieldDescription,
                        FieldName: measure.FieldName,
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
                    if ($scope.scopeModel.isLocalizationEnabled) {
                        dataItem.onTextResourceSelectorReady = function (api) {
                            dataItem.textResourceSeletorAPI = api;
                            var setLoader = function (value) { dataItem.isMeasureTextResourceSelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                        };
                    }
                    $scope.scopeModel.measures.push(dataItem);
                };
                $scope.scopeModel.onDeselectMeasureItem = function (measure) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, measure.MeasureItemFieldId, 'MeasureItemFieldId');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.removeMeasure = function (measure) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedMeasures, measure.MeasureItemFieldId, 'MeasureItemFieldId');
                    $scope.scopeModel.selectedMeasures.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, measure.MeasureItemFieldId, 'MeasureItemFieldId');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };


                $scope.scopeModel.onItemGroupingSubSectionsReady = function (api) {
                    itemGroupingSubSectionsAPI = api;
                    itemGroupingSubSectionsReadyPromiseDeferred.resolve();

                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        var promises = [];

                        context = payload.context;
                        var subSectionSettingsEntity = payload.subSectionSettingsEntity;
                        var groupItemId = context.getItemGroupingId();
                        if (groupItemId != undefined) {
                            $scope.scopeModel.dimensionsItemGroupings = context.getGroupingDimensions(groupItemId);
                            $scope.scopeModel.measuresItemGroupings = context.getGroupingMeasures(groupItemId);
                        }
                        if (subSectionSettingsEntity != undefined) {
                            for (var i = 0; i < subSectionSettingsEntity.GridDimesions.length; i++) {
                                var gridDimension = subSectionSettingsEntity.GridDimesions[i];
                                var dimensionGridField = {
                                    payload: gridDimension,
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(dimensionGridField.loadPromiseDeferred.promise);
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(dimensionGridField.textResourceLoadPromiseDeferred.promise);
                                addSelectedDimension(dimensionGridField);
                            }
                            for (var j = 0; j < subSectionSettingsEntity.GridMeasures.length; j++) {
                                var gridMeasure = subSectionSettingsEntity.GridMeasures[j];
                                var measureGridField = {
                                    payload: gridMeasure,
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(measureGridField.loadPromiseDeferred.promise);
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(measureGridField.textResourceLoadPromiseDeferred.promise);

                                addSelectedMeasure(measureGridField);
                            }
                            function addSelectedDimension(gridDimension) {

                                var dataItemPayload = {
                                    data: {
                                        Width: VRCommon_GridWidthFactorEnum.Normal.value
                                    }
                                };
                                var textResourcePayload;
                                var groupItemDimension;

                                var dataItem = {};
                                if (gridDimension.payload != undefined) {
                                    groupItemDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, gridDimension.payload.DimensionId, "DimensionItemFieldId");
                                    dataItem.DimensionItemFieldId = gridDimension.payload.DimensionId;
                                    dataItem.FieldDescription = gridDimension.payload.Header;
                                    dataItem.FieldName = groupItemDimension.FieldName;
                                    dataItem.oldHeaderResourceKey = gridDimension.payload.HeaderResourceKey;
                                    dataItemPayload.data = gridDimension.payload.GridColumnSettings;
                                    textResourcePayload = { selectedValue: gridDimension.payload.HeaderResourceKey };

                                }
                                dataItem.onDimensionGridWidthFactorEditorReady = function (api) {
                                    dataItem.dimensionGridWidthFactorAPI = api;
                                    gridDimension.readyPromiseDeferred.resolve();
                                };
                                if ($scope.scopeModel.isLocalizationEnabled) {

                                    dataItem.onTextResourceSelectorReady = function (api) {
                                        dataItem.textResourceSeletorAPI = api;
                                        gridDimension.textResourceReadyPromiseDeferred.resolve();
                                    };

                                    gridDimension.textResourceReadyPromiseDeferred.promise
                                        .then(function () {
                                            VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, gridDimension.textResourceLoadPromiseDeferred);
                                        });
                                }
                                gridDimension.readyPromiseDeferred.promise
                                    .then(function () {
                                        VRUIUtilsService.callDirectiveLoad(dataItem.dimensionGridWidthFactorAPI, dataItemPayload, gridDimension.loadPromiseDeferred);
                                    });

                                if (groupItemDimension != undefined) {
                                    $scope.scopeModel.selectedDimensions.push(groupItemDimension);
                                    $scope.scopeModel.dimensions.push(dataItem);
                                }
                            }
                            function addSelectedMeasure(gridMeasure) {

                                var dataItemPayload = {
                                    data: {
                                        Width: VRCommon_GridWidthFactorEnum.Normal.value
                                    }
                                };
                                var textResourcePayload;
                                var dataItem = {};

                                var groupItemMeasure;
                                if (gridMeasure.payload != undefined) {
                                    groupItemMeasure = UtilsService.getItemByVal($scope.scopeModel.measuresItemGroupings, gridMeasure.payload.MeasureId, "MeasureItemFieldId");
                                    dataItem.MeasureItemFieldId = gridMeasure.payload.MeasureId;
                                    dataItem.FieldDescription = gridMeasure.payload.Header;
                                    dataItem.FieldName = groupItemMeasure.FieldName;
                                    dataItem.oldHeaderResourceKey = gridMeasure.payload.HeaderResourceKey;
                                    dataItemPayload.data = gridMeasure.payload.GridColumnSettings;
                                    textResourcePayload = {
                                        selectedValue: gridMeasure.payload.HeaderResourceKey
                                    };
                                }
                                dataItem.onMeasureGridWidthFactorEditorReady = function (api) {
                                    dataItem.measureGridWidthFactorAPI = api;
                                    gridMeasure.readyPromiseDeferred.resolve();
                                };

                                if ($scope.scopeModel.isLocalizationEnabled) {
                                    dataItem.onTextResourceSelectorReady = function (api) {
                                        dataItem.textResourceSeletorAPI = api;
                                        gridMeasure.textResourceReadyPromiseDeferred.resolve();
                                    };
                                    gridMeasure.textResourceReadyPromiseDeferred.promise
                                        .then(function () {
                                            VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, gridMeasure.textResourceLoadPromiseDeferred);
                                        });
                                }
                               

                                gridMeasure.readyPromiseDeferred.promise
                                    .then(function () {
                                        VRUIUtilsService.callDirectiveLoad(dataItem.measureGridWidthFactorAPI, dataItemPayload, gridMeasure.loadPromiseDeferred);
                                    });

                                if (groupItemMeasure != undefined) {
                                    $scope.scopeModel.selectedMeasures.push(groupItemMeasure);
                                    $scope.scopeModel.measures.push(dataItem);
                                }
                            }
                        }

                    }

                    var itemGroupingSubSectionsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    itemGroupingSubSectionsReadyPromiseDeferred.promise.then(function () {
                        var itemGroupingDirectivePayload = { context: getContext() };
                        if (subSectionSettingsEntity != undefined)
                            itemGroupingDirectivePayload.subSections = subSectionSettingsEntity.SubSections;
                        VRUIUtilsService.callDirectiveLoad(itemGroupingSubSectionsAPI, itemGroupingDirectivePayload, itemGroupingSubSectionsDeferredLoadPromiseDeferred);
                    });
                    promises.push(itemGroupingSubSectionsDeferredLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var dimensions;
                    if ($scope.scopeModel.dimensions != undefined && $scope.scopeModel.dimensions.length > 0) {
                        dimensions = [];
                        for (var i = 0; i < $scope.scopeModel.dimensions.length; i++) {
                            var dimension = $scope.scopeModel.dimensions[i];
                            dimensions.push({
                                DimensionId: dimension.DimensionItemFieldId,
                                Header: dimension.FieldDescription,
                                GridColumnSettings: dimension.dimensionGridWidthFactorAPI.getData(),
                                HeaderResourceKey: dimension.textResourceSeletorAPI != undefined ? dimension.textResourceSeletorAPI.getSelectedValues() : dimension.oldHeaderResourceKey
                            });
                        }
                    }

                    var measures;
                    if ($scope.scopeModel.measures != undefined && $scope.scopeModel.measures.length > 0) {
                        measures = [];
                        for (var i = 0; i < $scope.scopeModel.measures.length; i++) {
                            var measure = $scope.scopeModel.measures[i];
                            measures.push({
                                MeasureId: measure.MeasureItemFieldId,
                                Header: measure.FieldDescription,
                                GridColumnSettings: measure.measureGridWidthFactorAPI.getData(),
                                HeaderResourceKey: measure.textResourceSeletorAPI != undefined ? measure.textResourceSeletorAPI.getSelectedValues() : measure.oldHeaderResourceKey
                            });
                        }
                    }
                    return {
                        GridDimesions: dimensions,
                        GridMeasures: measures,
                        SubSections: itemGroupingSubSectionsAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined) {
                    currentContext = {};
                }
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);
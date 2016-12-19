"use strict";

app.directive("vrInvoicetypeInvoicesubsectionsettingsItemgroupingSubsectionsettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/SubSectionSettings/InvoiceUISubSectionSettings/Templates/ItemGroupingSubSectionSettingsTemplate.html"

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

                $scope.scopeModel.onSelectDimensionItem = function (dimension) {
                    var dataItem = {
                        DimensionItemFieldId: dimension.DimensionItemFieldId,
                        FieldDescription: dimension.FieldDescription,
                        FieldName: dimension.FieldName
                    };
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
                        FieldName: measure.FieldName
                    };
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
                                addSelectedDimension(gridDimension);
                            }
                            for (var j = 0; j < subSectionSettingsEntity.GridMeasures.length; j++) {
                                var gridMeasure = subSectionSettingsEntity.GridMeasures[j];
                                addSelectedMeasure(gridMeasure);
                            }
                            function addSelectedDimension(gridDimension) {

                                var groupItemDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, gridDimension.DimensionId, "DimensionItemFieldId");
                                if (groupItemDimension != undefined) {
                                    $scope.scopeModel.selectedDimensions.push(groupItemDimension);
                                    $scope.scopeModel.dimensions.push({
                                        DimensionItemFieldId: gridDimension.DimensionId,
                                        FieldDescription: gridDimension.Header,
                                        FieldName: groupItemDimension.FieldName,
                                    });

                                }

                            }
                            function addSelectedMeasure(gridMeasure) {
                                var groupItemMeasure = UtilsService.getItemByVal($scope.scopeModel.measuresItemGroupings, gridMeasure.MeasureId, "MeasureItemFieldId");
                                if (groupItemMeasure != undefined) {
                                    $scope.scopeModel.selectedMeasures.push(groupItemMeasure);
                                    $scope.scopeModel.measures.push({
                                        MeasureItemFieldId: gridMeasure.MeasureId,
                                        FieldDescription: gridMeasure.Header,
                                        FieldName: groupItemMeasure.FieldName,
                                    });

                                }
                            }
                        }

                    }
                    var promises = [];

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
                            });
                        }
                    }
                    return {
                        GridDimesions:dimensions,
                        GridMeasures: measures,
                        SubSections: itemGroupingSubSectionsAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext()
            {
                var currentContext = context;
                if(currentContext ==undefined)
                {
                    currentContext ={};
                }
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);
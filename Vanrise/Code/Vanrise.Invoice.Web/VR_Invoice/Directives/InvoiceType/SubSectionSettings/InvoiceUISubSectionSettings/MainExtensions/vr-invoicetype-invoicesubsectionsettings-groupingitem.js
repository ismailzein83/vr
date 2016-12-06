"use strict";

app.directive("vrInvoicetypeInvoicesubsectionsettingsGroupingitem", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GroupingItemSubSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/SubSectionSettings/InvoiceUISubSectionSettings/MainExtensions/Templates/GroupingItemSubSectionTemplate.html"

        };

        function GroupingItemSubSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.groupingItems = [];
                $scope.scopeModel.dimensionsGroupingItems = [];
                $scope.scopeModel.measuresGroupingItems = [];
                $scope.scopeModel.measures = [];
                $scope.scopeModel.dimensions = [];
                $scope.scopeModel.selectedDimensions = [];
                $scope.scopeModel.selectedMeasures = [];

                $scope.scopeModel.onSelectDimensionItem = function(dimension)
                {
                    var dataItem = {
                        DimensionItemFieldId: dimension.DimensionItemFieldId,
                        FieldDescription: dimension.FieldDescription,
                        FieldName: dimension.FieldName,
                    };
                    $scope.scopeModel.dimensions.push(dataItem);
                }
                $scope.scopeModel.onDeselectDimensionItem = function (dimension) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dimension.DimensionItemFieldId, 'DimensionItemFieldId');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                }
                $scope.scopeModel.removeDimension = function (dimension) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedDimensions, dimension.DimensionItemFieldId, 'DimensionItemFieldId');
                    $scope.scopeModel.selectedDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.dimensions, dimension.DimensionItemFieldId, 'DimensionItemFieldId');
                    $scope.scopeModel.dimensions.splice(datasourceIndex, 1);
                }

                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    var dataItem = {
                        MeasureItemFieldId: measure.MeasureItemFieldId,
                        FieldDescription: measure.FieldDescription,
                        FieldName: measure.FieldName,
                    };
                    $scope.scopeModel.measures.push(dataItem);
                }
                $scope.scopeModel.onDeselectMeasureItem = function (measure) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, measure.MeasureItemFieldId, 'MeasureItemFieldId');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                }
                $scope.scopeModel.removeMeasure = function (measure) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedMeasures, measure.MeasureItemFieldId, 'MeasureItemFieldId');
                    $scope.scopeModel.selectedMeasures.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, measure.MeasureItemFieldId, 'MeasureItemFieldId');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                }

                $scope.scopeModel.onGroupingItemSelectionChanged = function(selectedGroupItem)
                {
                    if (context != undefined && selectedGroupItem != undefined)
                    {
                        $scope.scopeModel.dimensions.length = 0;
                        $scope.scopeModel.measures.length = 0;
                        $scope.scopeModel.selectedDimensions.length = 0;
                        $scope.scopeModel.selectedMeasures.length = 0;
                        $scope.scopeModel.dimensionsGroupingItems = context.getGroupingDimensions(selectedGroupItem.GroupingItemId);
                        $scope.scopeModel.measuresGroupingItems = context.getGroupingMeasures(selectedGroupItem.GroupingItemId);
                    }
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    if (payload != undefined) {
                        context = payload.context;
                        var invoiceSubSectionSettingsEntity = payload.invoiceSubSectionSettingsEntity;
                        if (context != undefined)
                        {
                            $scope.scopeModel.groupingItems = context.getGroupingItemsInfo();
                        }
                        if(invoiceSubSectionSettingsEntity != undefined)
                        {
                            $scope.scopeModel.selectedGroupingItem = UtilsService.getItemByVal($scope.scopeModel.groupingItems, invoiceSubSectionSettingsEntity.GroupingItemId, "GroupingItemId");
                            if ($scope.scopeModel.selectedGroupingItem != undefined)
                            {
                                $scope.scopeModel.dimensionsGroupingItems = context.getGroupingDimensions($scope.scopeModel.selectedGroupingItem.GroupingItemId);
                                $scope.scopeModel.measuresGroupingItems = context.getGroupingMeasures($scope.scopeModel.selectedGroupingItem.GroupingItemId);
                            }
                           
                            for(var i =0;i<invoiceSubSectionSettingsEntity.GridDimesions.length;i++)
                            {
                                var gridDimension = invoiceSubSectionSettingsEntity.GridDimesions[i];
                                addSelectedDimension(gridDimension);
                            }
                            for (var j = 0; j < invoiceSubSectionSettingsEntity.GridMeasures.length; j++) {
                                var gridMeasure = invoiceSubSectionSettingsEntity.GridMeasures[j];
                                addSelectedMeasure(gridMeasure);
                            }
                            function addSelectedDimension(gridDimension)
                            {
                                console.log($scope.scopeModel.dimensionsGroupingItems);
                                console.log(gridDimension);
                                var groupItemDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsGroupingItems, gridDimension.DimensionId, "DimensionItemFieldId");
                                if (groupItemDimension != undefined)
                                {
                                    $scope.scopeModel.selectedDimensions.push(groupItemDimension);
                                    $scope.scopeModel.dimensions.push({
                                        DimensionItemFieldId: gridDimension.DimensionId,
                                        FieldDescription: gridDimension.Header,
                                        FieldName: groupItemDimension.FieldName,
                                    });
                                    
                                }
                             
                            }
                            function addSelectedMeasure(gridMeasure) {
                                var groupItemMeasure = UtilsService.getItemByVal($scope.scopeModel.measuresGroupingItems, gridMeasure.MeasureId, "MeasureItemFieldId");
                                if (groupItemMeasure != undefined)
                                {
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
                        $type: "Vanrise.Invoice.MainExtensions.GroupingItemSection ,Vanrise.Invoice.MainExtensions",
                        GroupingItemId: $scope.scopeModel.selectedGroupingItem.GroupingItemId,
                        GridDimesions:dimensions,
                        GridMeasures: measures
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);
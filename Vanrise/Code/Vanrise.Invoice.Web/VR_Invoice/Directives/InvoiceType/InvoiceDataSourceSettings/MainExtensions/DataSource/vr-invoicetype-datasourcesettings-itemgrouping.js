"use strict";

app.directive("vrInvoicetypeDatasourcesettingsItemgrouping", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ItemGroupingDataSourceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceDataSourceSettings/MainExtensions/DataSource/Templates/ItemGroupingDataSourceSettings.html"

        };

        function ItemGroupingDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var itemGroupingSelectedReadyPromiseDeferred;
            var selectedDimensions =[];
            var selectedMeasures =[];
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.itemGroupings = [];

                $scope.scopeModel.dimensionsItemGroupings = [];
                $scope.scopeModel.measuresItemGroupings = [];
                $scope.scopeModel.selectedDimensions = [];
                $scope.scopeModel.selectedMeasures = [];

                $scope.scopeModel.onItemGroupingSelectionChanged = function (selectedGroupItem) {
                    if (context != undefined && selectedGroupItem != undefined) {
                        if (itemGroupingSelectedReadyPromiseDeferred != undefined)
                            itemGroupingSelectedReadyPromiseDeferred.resolve();
                        else {
                            $scope.scopeModel.dimensionsItemGroupings = context.getGroupingDimensions(selectedGroupItem.ItemGroupingId);
                            $scope.scopeModel.measuresItemGroupings = context.getGroupingMeasures(selectedGroupItem.ItemGroupingId);
                            $scope.scopeModel.selectedDimensions.length = 0;
                            $scope.scopeModel.selectedMeasures.length = 0;
                            selectedDimensions.length = 0;
                            selectedMeasures.length = 0;
                        }
                    }
                }


                $scope.scopeModel.onSelectDimensionItem = function (dimension) {
                    selectedDimensions.push({ DimensionId: dimension.DimensionItemFieldId });
                }
                $scope.scopeModel.onDeselectDimensionItem = function (dimension) {
                    var dimensionIndex = UtilsService.getItemIndexByVal(selectedDimensions, dimension.DimensionItemFieldId, 'DimensionId');
                    selectedDimensions.splice(dimensionIndex, 1);
                }

                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    selectedMeasures.push({ MeasureId: measure.MeasureItemFieldId });
                }
                $scope.scopeModel.onDeselectMeasureItem = function (measure) {
                    var measureIndex = UtilsService.getItemIndexByVal(selectedMeasures, measure.MeasureItemFieldId, 'MeasureId');
                    selectedMeasures.splice(measureIndex, 1);
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        var dataSourceEntity = payload.dataSourceEntity;
                        if (context != undefined) {
                            $scope.scopeModel.itemGroupings = context.getItemGroupingsInfo();
                        }
                        if (dataSourceEntity != undefined) {
                            $scope.scopeModel.groupingClassFQTN = dataSourceEntity.GroupingClassFQTN;
                            $scope.scopeModel.selectedItemGrouping = UtilsService.getItemByVal($scope.scopeModel.itemGroupings, dataSourceEntity.ItemGroupingId, "ItemGroupingId");
                            if (dataSourceEntity.ItemGroupingId != undefined) {
                                itemGroupingSelectedReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(itemGroupingSelectedReadyPromiseDeferred.promise);
                                itemGroupingSelectedReadyPromiseDeferred.promise.then(function () {
                                    itemGroupingSelectedReadyPromiseDeferred = undefined;
                                    $scope.scopeModel.dimensionsItemGroupings = context.getGroupingDimensions(dataSourceEntity.ItemGroupingId);
                                    $scope.scopeModel.measuresItemGroupings = context.getGroupingMeasures(dataSourceEntity.ItemGroupingId);
                                    if (dataSourceEntity.Dimensions != undefined) {
                                        for (var i = 0; i < dataSourceEntity.Dimensions.length; i++) {
                                            var dimension = dataSourceEntity.Dimensions[i];
                                            $scope.scopeModel.selectedDimensions.push(UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, dimension.DimensionId, "DimensionItemFieldId"));
                                            selectedDimensions.push(dimension);
                                        }
                                    }
                                    if (dataSourceEntity.Measures != undefined) {
                                        for (var j = 0; j < dataSourceEntity.Measures.length; j++) {
                                            var measure = dataSourceEntity.Measures[j];
                                            $scope.scopeModel.selectedMeasures.push(UtilsService.getItemByVal($scope.scopeModel.measuresItemGroupings, measure.MeasureId, "MeasureItemFieldId"));
                                            selectedMeasures.push(measure);
                                        }
                                    }
                                });
                            }
                        }
                    }
                    
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.ItemGroupingDataSourceSettings ,Vanrise.Invoice.MainExtensions",
                        ItemGroupingId: $scope.scopeModel.selectedItemGrouping.ItemGroupingId,
                        Dimensions: selectedDimensions,
                        Measures: selectedMeasures,
                        GroupingClassFQTN: $scope.scopeModel.groupingClassFQTN
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);
"use strict";

app.directive("whsInvoicetypeGridactionsettingsCompare", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CompareInvoiceAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Extensions/InvoiceAction/Templates/CompareInvoiceActionTemplate.html"

        };

        function CompareInvoiceAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var itemGroupingSelectedReadyPromiseDeferred;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.itemGroupings = [];

                $scope.scopeModel.dimensionsItemGroupings = [];
                $scope.scopeModel.measuresItemGroupings = [];

                $scope.scopeModel.onItemGroupingSelectionChanged = function (selectedGroupItem) {
                    if (context != undefined && selectedGroupItem != undefined) {
                        if (itemGroupingSelectedReadyPromiseDeferred != undefined)
                            itemGroupingSelectedReadyPromiseDeferred.resolve();
                        else {
                            $scope.scopeModel.dimensionsItemGroupings = context.getGroupingDimensions(selectedGroupItem.ItemGroupingId);
                            $scope.scopeModel.measuresItemGroupings = context.getGroupingMeasures(selectedGroupItem.ItemGroupingId);
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceActionEntity;
                    if (payload != undefined) {
                        invoiceActionEntity = payload.invoiceActionEntity;
                        context = payload.context;
                        if(context != undefined)
                        {
                            $scope.scopeModel.itemGroupings = context.getItemGroupingsInfo();
                        }
                    }
                    var promises = [];

                    if (invoiceActionEntity != undefined) {
                        $scope.scopeModel.partnerLabel = invoiceActionEntity.PartnerLabel;
                        $scope.scopeModel.partnerAbbreviationLabel = invoiceActionEntity.PartnerAbbreviationLabel;
                        $scope.scopeModel.selectedItemGrouping = UtilsService.getItemByVal($scope.scopeModel.itemGroupings, invoiceActionEntity.ItemGroupingId, "ItemGroupingId");
                        if (invoiceActionEntity.ItemGroupingId != undefined) {
                            itemGroupingSelectedReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(itemGroupingSelectedReadyPromiseDeferred.promise);

                            itemGroupingSelectedReadyPromiseDeferred.promise.then(function () {
                                itemGroupingSelectedReadyPromiseDeferred = undefined;
                                $scope.scopeModel.dimensionsItemGroupings = context.getGroupingDimensions(invoiceActionEntity.ItemGroupingId);
                                $scope.scopeModel.measuresItemGroupings = context.getGroupingMeasures(invoiceActionEntity.ItemGroupingId);
                                
                                $scope.scopeModel.selectedZoneDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, invoiceActionEntity.ZoneDimensionId, "DimensionItemFieldId");
                                $scope.scopeModel.selectedFromDateDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, invoiceActionEntity.FromDateDimensionId, "DimensionItemFieldId");
                                $scope.scopeModel.selectedToDateDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, invoiceActionEntity.ToDateDimensionId, "DimensionItemFieldId");

                                $scope.scopeModel.selectedRateTypeDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, invoiceActionEntity.RateTypeDimensionId, "DimensionItemFieldId");
                                $scope.scopeModel.selectedCurrencyDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, invoiceActionEntity.CurrencyDimensionId, "DimensionItemFieldId");
                                $scope.scopeModel.selectedRateDimension = UtilsService.getItemByVal($scope.scopeModel.dimensionsItemGroupings, invoiceActionEntity.RateDimensionId, "DimensionItemFieldId");

                                $scope.scopeModel.selectedNumberOfCallsMeasure = UtilsService.getItemByVal($scope.scopeModel.measuresItemGroupings, invoiceActionEntity.NumberOfCallsMeasureId, "MeasureItemFieldId");
                                $scope.scopeModel.selectedAmountMeasure = UtilsService.getItemByVal($scope.scopeModel.measuresItemGroupings, invoiceActionEntity.AmountMeasureId, "MeasureItemFieldId");
                                $scope.scopeModel.selectedDurationMeasure = UtilsService.getItemByVal($scope.scopeModel.measuresItemGroupings, invoiceActionEntity.DurationMeasureId, "MeasureItemFieldId");
                            });
                        }
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.CompareInvoiceAction ,TOne.WhS.Invoice.Business",
                        PartnerLabel: $scope.scopeModel.partnerLabel,
                        PartnerAbbreviationLabel: $scope.scopeModel.partnerAbbreviationLabel,
                        ZoneDimensionId: $scope.scopeModel.selectedZoneDimension.DimensionItemFieldId,
                        FromDateDimensionId: $scope.scopeModel.selectedFromDateDimension.DimensionItemFieldId,
                        ToDateDimensionId: $scope.scopeModel.selectedToDateDimension.DimensionItemFieldId,

                        RateTypeDimensionId: $scope.scopeModel.selectedRateTypeDimension.DimensionItemFieldId,
                        CurrencyDimensionId: $scope.scopeModel.selectedCurrencyDimension.DimensionItemFieldId,
                        RateDimensionId: $scope.scopeModel.selectedRateDimension.DimensionItemFieldId,
                        NumberOfCallsMeasureId: $scope.scopeModel.selectedNumberOfCallsMeasure.MeasureItemFieldId,
                        AmountMeasureId: $scope.scopeModel.selectedAmountMeasure.MeasureItemFieldId,
                        DurationMeasureId: $scope.scopeModel.selectedDurationMeasure.MeasureItemFieldId,
                        ItemGroupingId: $scope.scopeModel.selectedItemGrouping.ItemGroupingId
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);
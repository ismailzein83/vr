"use strict";

app.directive("retailInterconnectInvoicetypeGridactionsettingsCompare", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_Interconnect_InvoiceType", "Retail_Interconnect_RetailModuleService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_Interconnect_InvoiceType, Retail_Interconnect_RetailModuleService) {

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
            templateUrl: "/Client/Modules/Retail_Interconnect/Directives/Extensions/InvoiceAction/Templates/CompareInvoiceActionTemplate.html"

        };

        function CompareInvoiceAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var invoiceCarrierTypeDirectiveAPI;
            var invoiceCarrierSelectorReadyPromiseDeffered = UtilsService.createPromiseDeferred();

            var itemGroupingVoiceSelectedReadyPromiseDeferred;
            var itemGroupingSMSSelectedReadyPromiseDeferred;

            var context;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isVoiceModuleEnabled = Retail_Interconnect_RetailModuleService.isVoiceModuleEnabled();
                $scope.scopeModel.isSMSModuleEnabled = Retail_Interconnect_RetailModuleService.isSMSModuleEnabled();

                $scope.scopeModel.voiceItemGroupings = [];
                $scope.scopeModel.smsItemGroupings = [];

                $scope.scopeModel.voiceDimensionsItemGroupings = [];
                $scope.scopeModel.smsDimensionsItemGroupings = [];

                $scope.scopeModel.voiceMeasuresItemGroupings = [];
                $scope.scopeModel.smsMeasuresItemGroupings = [];

                $scope.scopeModel.invoiceCarrierType = UtilsService.getArrayEnum(Retail_Interconnect_InvoiceType);

                $scope.scopeModel.onInvoiceCarrierTypeDirectiveReady = function (api) {
                    invoiceCarrierTypeDirectiveAPI = api;
                    invoiceCarrierSelectorReadyPromiseDeffered.resolve();
                };

                $scope.scopeModel.onVoiceItemGroupingSelectionChanged = function (selectedGroupItem) {
                    if (context != undefined && selectedGroupItem != undefined) {
                        if (itemGroupingVoiceSelectedReadyPromiseDeferred != undefined)
                            itemGroupingVoiceSelectedReadyPromiseDeferred.resolve();
                        else {
                            $scope.scopeModel.voiceDimensionsItemGroupings = context.getGroupingDimensions(selectedGroupItem.ItemGroupingId);
                            $scope.scopeModel.voiceMeasuresItemGroupings = context.getGroupingMeasures(selectedGroupItem.ItemGroupingId);

                            $scope.scopeModel.selectedZoneDimension = undefined;
                            $scope.scopeModel.selectedVoiceFromDateMeasure = undefined;
                            $scope.scopeModel.selectedVoiceToDateMeasure = undefined;

                            $scope.scopeModel.selectedVoiceRateTypeDimension = undefined;
                            $scope.scopeModel.selectedVoiceCurrencyDimension = undefined;
                            $scope.scopeModel.selectedVoiceRateDimension = undefined;

                            $scope.scopeModel.selectedNumberOfCallsMeasure = undefined;
                            $scope.scopeModel.selectedVoiceAmountMeasure = undefined;
                            $scope.scopeModel.selectedDurationMeasure = undefined;
                            $scope.scopeModel.selectedVoiceRateMeasure = undefined;

                        }
                    }
                };

                $scope.scopeModel.onSMSItemGroupingSelectionChanged = function (selectedGroupItem) {
                    if (context != undefined && selectedGroupItem != undefined) {
                        if (itemGroupingSMSSelectedReadyPromiseDeferred != undefined)
                            itemGroupingSMSSelectedReadyPromiseDeferred.resolve();
                        else {
                            $scope.scopeModel.smsDimensionsItemGroupings = context.getGroupingDimensions(selectedGroupItem.ItemGroupingId);
                            $scope.scopeModel.smsMeasuresItemGroupings = context.getGroupingMeasures(selectedGroupItem.ItemGroupingId);

                            $scope.scopeModel.selectedMobileNetworkDimension = undefined;
                            $scope.scopeModel.selectedSMSFromDateMeasure = undefined;
                            $scope.scopeModel.selectedSMSToDateMeasure = undefined;
                            ;
                            $scope.scopeModel.selectedSMSCurrencyDimension = undefined;
                            $scope.scopeModel.selectedSMSRateDimension = undefined;

                            $scope.scopeModel.selectedNumberOfSMSsMeasure = undefined;
                            $scope.scopeModel.selectedSMSAmountMeasure = undefined;
                            $scope.scopeModel.selectedSMSRateMeasure = undefined;


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
                        if (context != undefined) {
                            $scope.scopeModel.voiceItemGroupings = context.getItemGroupingsInfo();
                            $scope.scopeModel.smsItemGroupings = context.getItemGroupingsInfo();
                        }
                        if (payload.invoiceActionEntity != undefined && payload.invoiceActionEntity.InvoiceCarrierType != undefined)
                            $scope.scopeModel.selectedValue = UtilsService.getItemByVal($scope.scopeModel.invoiceCarrierType, payload.invoiceActionEntity.InvoiceCarrierType, "value");
                    }
                    var promises = [];

                    if (invoiceActionEntity != undefined) {
                        $scope.scopeModel.partnerLabel = invoiceActionEntity.PartnerLabel;
                        $scope.scopeModel.partnerAbbreviationLabel = invoiceActionEntity.PartnerAbbreviationLabel;

                        if ($scope.scopeModel.isVoiceModuleEnabled && invoiceActionEntity.VoiceSettings != undefined) {
                            var voiceSettings = invoiceActionEntity.VoiceSettings;

                            $scope.scopeModel.selectedVoiceItemGrouping = UtilsService.getItemByVal($scope.scopeModel.voiceItemGroupings, voiceSettings.ItemGroupingId, "ItemGroupingId");

                            if (voiceSettings.ItemGroupingId != undefined) {
                                itemGroupingVoiceSelectedReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(itemGroupingVoiceSelectedReadyPromiseDeferred.promise);
                                itemGroupingVoiceSelectedReadyPromiseDeferred.promise.then(function () {
                                    itemGroupingVoiceSelectedReadyPromiseDeferred = undefined;
                                    $scope.scopeModel.voiceDimensionsItemGroupings = context.getGroupingDimensions(voiceSettings.ItemGroupingId);
                                    $scope.scopeModel.voiceMeasuresItemGroupings = context.getGroupingMeasures(voiceSettings.ItemGroupingId);

                                    $scope.scopeModel.selectedZoneDimension = UtilsService.getItemByVal($scope.scopeModel.voiceDimensionsItemGroupings, voiceSettings.ZoneDimensionId, "DimensionItemFieldId");
                                    $scope.scopeModel.selectedVoiceFromDateMeasure = UtilsService.getItemByVal($scope.scopeModel.voiceMeasuresItemGroupings, voiceSettings.FromDateMeasureId, "MeasureItemFieldId");
                                    $scope.scopeModel.selectedVoiceToDateMeasure = UtilsService.getItemByVal($scope.scopeModel.voiceMeasuresItemGroupings, voiceSettings.ToDateMeasureId, "MeasureItemFieldId");

                                    $scope.scopeModel.selectedVoiceRateTypeDimension = UtilsService.getItemByVal($scope.scopeModel.voiceDimensionsItemGroupings, voiceSettings.RateTypeDimensionId, "DimensionItemFieldId");
                                    $scope.scopeModel.selectedVoiceCurrencyDimension = UtilsService.getItemByVal($scope.scopeModel.voiceDimensionsItemGroupings, voiceSettings.CurrencyDimensionId, "DimensionItemFieldId");
                                    $scope.scopeModel.selectedVoiceRateDimension = UtilsService.getItemByVal($scope.scopeModel.voiceDimensionsItemGroupings, voiceSettings.RateDimensionId, "DimensionItemFieldId");

                                    $scope.scopeModel.selectedNumberOfCallsMeasure = UtilsService.getItemByVal($scope.scopeModel.voiceMeasuresItemGroupings, voiceSettings.NumberOfCallsMeasureId, "MeasureItemFieldId");
                                    $scope.scopeModel.selectedVoiceAmountMeasure = UtilsService.getItemByVal($scope.scopeModel.voiceMeasuresItemGroupings, voiceSettings.AmountMeasureId, "MeasureItemFieldId");
                                    $scope.scopeModel.selectedDurationMeasure = UtilsService.getItemByVal($scope.scopeModel.voiceMeasuresItemGroupings, voiceSettings.DurationMeasureId, "MeasureItemFieldId");
                                    $scope.scopeModel.selectedVoiceRateMeasure = UtilsService.getItemByVal($scope.scopeModel.voiceMeasuresItemGroupings, voiceSettings.RateMeasureId, "MeasureItemFieldId");

                                });
                            }

                        };

                        if ($scope.scopeModel.isSMSModuleEnabled && invoiceActionEntity.SMSSettings != undefined) {
                            $scope.scopeModel.selectedSMSItemGrouping = UtilsService.getItemByVal($scope.scopeModel.smsItemGroupings, invoiceActionEntity.SMSSettings.ItemGroupingId, "ItemGroupingId");

                            if (invoiceActionEntity.SMSSettings != undefined) {
                                var smsSettings = invoiceActionEntity.SMSSettings;

                                $scope.scopeModel.selectedSMSItemGrouping = UtilsService.getItemByVal($scope.scopeModel.smsItemGroupings, smsSettings.ItemGroupingId, "ItemGroupingId");

                                if (smsSettings.ItemGroupingId != undefined) {
                                    itemGroupingSMSSelectedReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    promises.push(itemGroupingSMSSelectedReadyPromiseDeferred.promise);
                                    itemGroupingSMSSelectedReadyPromiseDeferred.promise.then(function () {
                                        itemGroupingSMSSelectedReadyPromiseDeferred = undefined;
                                        $scope.scopeModel.smsDimensionsItemGroupings = context.getGroupingDimensions(smsSettings.ItemGroupingId);
                                        $scope.scopeModel.smsMeasuresItemGroupings = context.getGroupingMeasures(smsSettings.ItemGroupingId);

                                        $scope.scopeModel.selectedMobileNetworkDimension = UtilsService.getItemByVal($scope.scopeModel.smsDimensionsItemGroupings, smsSettings.MobileNetworkDimensionId, "DimensionItemFieldId");
                                        $scope.scopeModel.selectedSMSFromDateMeasure = UtilsService.getItemByVal($scope.scopeModel.smsMeasuresItemGroupings, smsSettings.FromDateMeasureId, "MeasureItemFieldId");
                                        $scope.scopeModel.selectedSMSToDateMeasure = UtilsService.getItemByVal($scope.scopeModel.smsMeasuresItemGroupings, smsSettings.ToDateMeasureId, "MeasureItemFieldId");

                                        $scope.scopeModel.selectedSMSCurrencyDimension = UtilsService.getItemByVal($scope.scopeModel.smsDimensionsItemGroupings, smsSettings.CurrencyDimensionId, "DimensionItemFieldId");
                                        $scope.scopeModel.selectedSMSRateDimension = UtilsService.getItemByVal($scope.scopeModel.smsDimensionsItemGroupings, smsSettings.RateDimensionId, "DimensionItemFieldId");

                                        $scope.scopeModel.selectedNumberOfSMSsMeasure = UtilsService.getItemByVal($scope.scopeModel.smsMeasuresItemGroupings, smsSettings.NumberOfSMSsMeasureId, "MeasureItemFieldId");
                                        $scope.scopeModel.selectedSMSAmountMeasure = UtilsService.getItemByVal($scope.scopeModel.smsMeasuresItemGroupings, smsSettings.AmountMeasureId, "MeasureItemFieldId");
                                        $scope.scopeModel.selectedSMSRateMeasure = UtilsService.getItemByVal($scope.scopeModel.smsMeasuresItemGroupings, smsSettings.RateMeasureId, "MeasureItemFieldId");

                                    });
                                }

                            };

                        };
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj =
                    {
                        $type: "Retail.Interconnect.Business.Extensions.CompareInvoiceAction ,Retail.Interconnect.Business",
                        PartnerLabel: $scope.scopeModel.partnerLabel,
                        PartnerAbbreviationLabel: $scope.scopeModel.partnerAbbreviationLabel,
                        InvoiceCarrierType: $scope.scopeModel.selectedValue != undefined ? $scope.scopeModel.selectedValue.value : undefined,
                    };

                    if (Retail_Interconnect_RetailModuleService.isVoiceModuleEnabled())
                        obj.VoiceSettings = buildVoiceSettings();

                    if (Retail_Interconnect_RetailModuleService.isSMSModuleEnabled())
                        obj.SMSSettings = buildSMSSettings();

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildVoiceSettings() {
                var obj = {
                    ZoneDimensionId: $scope.scopeModel.selectedZoneDimension.DimensionItemFieldId,
                    RateTypeDimensionId: $scope.scopeModel.selectedVoiceRateTypeDimension.DimensionItemFieldId,
                    CurrencyDimensionId: $scope.scopeModel.selectedVoiceCurrencyDimension.DimensionItemFieldId,
                    RateDimensionId: $scope.scopeModel.selectedVoiceRateDimension.DimensionItemFieldId,
                    NumberOfCallsMeasureId: $scope.scopeModel.selectedNumberOfCallsMeasure.MeasureItemFieldId,
                    AmountMeasureId: $scope.scopeModel.selectedVoiceAmountMeasure.MeasureItemFieldId,
                    DurationMeasureId: $scope.scopeModel.selectedDurationMeasure.MeasureItemFieldId,
                    RateMeasureId: $scope.scopeModel.selectedVoiceRateMeasure.MeasureItemFieldId,
                    ItemGroupingId: $scope.scopeModel.selectedVoiceItemGrouping.ItemGroupingId,
                    FromDateMeasureId: $scope.scopeModel.selectedVoiceFromDateMeasure.MeasureItemFieldId,
                    ToDateMeasureId: $scope.scopeModel.selectedVoiceToDateMeasure.MeasureItemFieldId,
                };
                return obj;
            }

            function buildSMSSettings() {
                var obj = {
                    MobileNetworkDimensionId: $scope.scopeModel.selectedMobileNetworkDimension.DimensionItemFieldId,
                    CurrencyDimensionId: $scope.scopeModel.selectedSMSCurrencyDimension.DimensionItemFieldId,
                    RateDimensionId: $scope.scopeModel.selectedSMSRateDimension.DimensionItemFieldId,
                    NumberOfSMSsMeasureId: $scope.scopeModel.selectedNumberOfSMSsMeasure.MeasureItemFieldId,
                    AmountMeasureId: $scope.scopeModel.selectedSMSAmountMeasure.MeasureItemFieldId,
                    RateMeasureId: $scope.scopeModel.selectedSMSRateMeasure.MeasureItemFieldId,
                    ItemGroupingId: $scope.scopeModel.selectedSMSItemGrouping.ItemGroupingId,
                    FromDateMeasureId: $scope.scopeModel.selectedSMSFromDateMeasure.MeasureItemFieldId,
                    ToDateMeasureId: $scope.scopeModel.selectedSMSToDateMeasure.MeasureItemFieldId,
                };
                return obj;
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
﻿(function (app) {

    'use strict';

    SwapDealEditorController.$inject = ['$scope', 'WhS_Deal_SwapDealAPIService', 'WhS_Deal_DealContractTypeEnum', 'WhS_Deal_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Deal_SwapDealService', 'WhS_Deal_SwapDealAnalysisService', 'VRValidationService', 'WhS_Deal_DealStatusTypeEnum', 'VRDateTimeService', 'VRCommon_EntityFilterEffectiveModeEnum', 'WhS_Deal_SwapDealTimeZoneTypeEnum','WhS_Deal_DealDefinitionAPIService'];

    function SwapDealEditorController($scope, WhS_Deal_SwapDealAPIService, WhS_Deal_DealContractTypeEnum, WhS_Deal_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_SwapDealService, WhS_Deal_SwapDealService, VRValidationService, WhS_Deal_DealStatusTypeEnum, VRDateTimeService, VRCommon_EntityFilterEffectiveModeEnum, WhS_Deal_SwapDealTimeZoneTypeEnum, WhS_Deal_DealDefinitionAPIService) {
        var isEditMode;

        var dealId;
        var dealEntity;
        var carrierAccountSelectedPromise;
        var context;
        var isViewHistoryMode;
        var oldselectedCarrier;
        var isReadOnly;
        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var currencyDirectiveAPI;
        var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dealInboundAPI;
        var dealInboundReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dealOutboundAPI;
        var dealOutboundReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var isEditable;
        var carrierAccountInfo;

        var originalEED;

        var offset;
        var SwapDealTimeZone;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dealId = parameters.dealId;
                context = parameters.context;
                isEditable = parameters.isEditable;
                isReadOnly = parameters.isReadOnly;
                isViewHistoryMode = context != undefined && context.historyId != undefined;
            }

            isEditMode = (dealId != undefined);
            if (isReadOnly && !isEditable)
                UtilsService.setContextReadOnly($scope);
        }
        function defineScope() {

            $scope.scopeModel = {};
            //UtilsService.setContextReadOnly($scope.scopeModel);
            $scope.scopeModel.disabelType = (isEditMode);
            $scope.scopeModel.contractTypes = UtilsService.getArrayEnum(WhS_Deal_DealContractTypeEnum);
            $scope.scopeModel.agreementTypes = UtilsService.getArrayEnum(WhS_Deal_DealAgreementTypeEnum);
            $scope.scopeModel.dealStatus = UtilsService.getArrayEnum(WhS_Deal_DealStatusTypeEnum);
            $scope.scopeModel.SwapDealTimeZone = UtilsService.getArrayEnum(WhS_Deal_SwapDealTimeZoneTypeEnum);
            $scope.scopeModel.selectedTimeZone = WhS_Deal_SwapDealTimeZoneTypeEnum.Supplier;

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };
            if (!isEditMode)
                $scope.scopeModel.followSupplierTimeZone = false;
            $scope.scopeModel.onCarrierAccountSelectionChanged = function () {
                carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();

                if (carrierAccountInfo != undefined) {
                    updateDescription();
                    var payload = {
                        carrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                        sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId,
                        context: getContext(),
                        dealId: dealId
                    };
                    var payloadOutbound = {
                        carrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                        supplierId: carrierAccountInfo.CarrierAccountId,
                        context: getContext(),
                        dealId: dealId
                    };
                    if (carrierAccountSelectedPromise != undefined) {
                        carrierAccountSelectedPromise.resolve();
                        oldselectedCarrier = $scope.scopeModel.carrierAcount;
                    }
                    else if (oldselectedCarrier != undefined && oldselectedCarrier.CarrierAccountId != carrierAccountInfo.CarrierAccountId) {
                        if (dealInboundAPI.hasData() || dealOutboundAPI.hasData()) {
                            VRNotificationService.showConfirmation('Data will be lost,Are you sure you want to continue?').then(function (response) {
                                if (response) {
                                    dealInboundAPI.load(payload);
                                    dealOutboundAPI.load(payloadOutbound);
                                    oldselectedCarrier = carrierAccountInfo;
                                }
                                else {
                                    $scope.scopeModel.carrierAcount = oldselectedCarrier;
                                }
                            });
                        }
                        else {
                            dealInboundAPI.load(payload);
                            dealOutboundAPI.load(payloadOutbound);
                        }
                    }
                    else if (oldselectedCarrier == undefined) {
                        oldselectedCarrier = carrierAccountInfo;
                        dealInboundAPI.load(payload);
                        dealOutboundAPI.load(payloadOutbound);
                    }
                }
            };
            $scope.scopeModel.onBEDchanged = function () {
                updateDescription();
            };
            $scope.scopeModel.onEEDchanged = function () {

            };
            $scope.scopeModel.onCurrencySelectReady = function (api) {
                currencyDirectiveAPI = api;
                currencyDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.analyse = function () {
                WhS_BE_SwapDealAnalysisService.openSwapDealAnalysis(buildSwapDealObjFromScope());
            };

            $scope.scopeModel.onSwapDealInboundDirectiveReady = function (api) {
                dealInboundAPI = api;
                dealInboundReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.selectedDealStatus = WhS_Deal_DealStatusTypeEnum.Draft;
            $scope.scopeModel.onSwapDealOutboundDirectiveReady = function (api) {
                dealOutboundAPI = api;
                dealOutboundReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.validateGracePeriod = function () {
                if (!isGraceValid())
                    return "Grace period should be less than difference between BED and EED";
                return null;
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwapDeal() : insertSwapDeal();
            };
            $scope.scopeModel.isInactiveStatus = function () {
                if ($scope.scopeModel.selectedDealStatus != undefined)
                    return ($scope.scopeModel.selectedDealStatus.value == WhS_Deal_DealStatusTypeEnum.Inactive.value);
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.validateDealStatusDate = function () {
                var date = UtilsService.createDateFromString($scope.scopeModel.deActivationDate);
                var beginDate = UtilsService.createDateFromString($scope.scopeModel.beginDate);
                var endDate = UtilsService.createDateFromString($scope.scopeModel.endDate); 
                    return "Deactivation date must be greater than deal BED";
                if (date > endDate)
                    return "Deactivation date must be less than deal EED";
                return null;
            };
            $scope.scopeModel.validateBED = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.beginDate, $scope.scopeModel.endDate);
            };
            $scope.scopeModel.validateEED = function () {
                var today = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
                var eed = UtilsService.createDateFromString($scope.scopeModel.endDate);
                var originalExpiredDate = UtilsService.createDateFromString(originalEED);
                if (originalExpiredDate.getTime() < today.getTime() && eed.getTime() < today.getTime() && originalExpiredDate.getTime() != eed.getTime()) {
                    return "Deal expired, EED can only be extended to a date greater than today";
                }
                return VRValidationService.validateTimeRange($scope.scopeModel.beginDate, $scope.scopeModel.endDate);
            };

            $scope.scopeModel.validateSwapDealInbounds = function () {
                if (carrierAccountSelectorAPI == undefined)
                    return null;

                var selectedcarrier = carrierAccountSelectorAPI.getSelectedValues();
                if (selectedcarrier == undefined)
                    return 'Please select a Carrier Account.';
                if ($scope.scopeModel.beginDate == undefined || $scope.scopeModel.endDate == undefined)
                    return 'Please select Deal BED and Deal EED';
                if (dealInboundAPI != undefined && !dealInboundAPI.hasData())
                    return 'Please, one record must be added at least.';

                return null;
            };

            $scope.scopeModel.validateSwapDealOutbounds = function () {
                if (carrierAccountSelectorAPI == undefined)
                    return null;

                var selectedcarrier = carrierAccountSelectorAPI.getSelectedValues();
                if (selectedcarrier == undefined)
                    return 'Please select a Carrier Account.';
                if ($scope.scopeModel.beginDate == undefined || $scope.scopeModel.endDate == undefined)
                    return 'Please select Deal BED and Deal EED';
                if (dealOutboundAPI != undefined && !dealOutboundAPI.hasData())
                    return 'Please,one record must be added at least.';

                return null;
            };
            $scope.scopeModel.onDealStatusChanged = function () {
                if ($scope.scopeModel.selectedDealStatus != undefined && $scope.scopeModel.selectedDealStatus.value != WhS_Deal_DealStatusTypeEnum.Inactive.value)
                    $scope.scopeModel.deActivationDate = undefined;
                if ($scope.scopeModel.selectedDealStatus != undefined && $scope.scopeModel.deActivationDate == undefined && $scope.scopeModel.selectedDealStatus.value == WhS_Deal_DealStatusTypeEnum.Inactive.value)
                    $scope.scopeModel.deActivationDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
            };
            $scope.scopeModel.dataForBoundsReady = function () {
                return (carrierAccountInfo != undefined && $scope.scopeModel.beginDate != undefined && $scope.scopeModel.endDate != undefined && $scope.scopeModel.selectedTimeZone.value != undefined);
            };

            //UtilsService.setContextReadOnly($scope);
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getSwapDeal().then(function () {
                    loadAllControls().finally(function () {
                        dealEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getSwapDealHistory().then(function () {
                    loadAllControls().finally(function () {
                        dealEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }
            else {
                loadAllControls();
            }
        }

        function getSwapDealHistory() {
            return WhS_Deal_SwapDealAPIService.GetSwapDealHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                dealEntity = response;

            });
        }
        function getSwapDeal() {
            return WhS_Deal_SwapDealAPIService.GetDeal(dealId).then(function (response) {
                dealEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadOffset, loadCarrierBoundsSection, loadGraceperiod, loadCurrencySelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function isGraceValid() {
            var beginDate = new Date($scope.scopeModel.beginDate);
            var endDate = new Date($scope.scopeModel.endDate);
            return ($scope.scopeModel.gracePeriod < UtilsService.diffDays(beginDate, endDate));
        }
        function updateDescription() {
            if (!isEditMode) {
                setTimeout(function () {
                    if ($scope.scopeModel.carrierAccount != undefined)
                        $scope.scopeModel.description = "Deal _ " + $scope.scopeModel.carrierAccount.Name + " _ " + UtilsService.getShortDate($scope.scopeModel.beginDate);
                });
            }
        }
        function loadOffset() {
            if (dealEntity == undefined || dealEntity.Settings == undefined)
                return;
            offset = dealEntity.Settings.Offset;
        }
        function setTitle() {
            if (isEditMode) {
                if (dealEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dealEntity.Name, 'Swap Deal');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Swap Deal');
        }
        function loadStaticData() {
            if (dealEntity == undefined)
                return;
            originalEED = dealEntity.Settings.EEDToStore;
            $scope.scopeModel.isCommitmentAgreement = isCommitmentAgreement();
            $scope.scopeModel.description = dealEntity.Name;
            $scope.scopeModel.selectedContractType = UtilsService.getItemByVal($scope.scopeModel.contractTypes, dealEntity.Settings.DealContract, 'value');
            $scope.scopeModel.selectedAgreementType = UtilsService.getItemByVal($scope.scopeModel.agreementTypes, dealEntity.Settings.DealType, 'value');
            $scope.scopeModel.selectedDealStatus = UtilsService.getItemByVal($scope.scopeModel.dealStatus, dealEntity.Settings.Status, 'value');
            $scope.scopeModel.selectedTimeZone = UtilsService.getItemByVal($scope.scopeModel.SwapDealTimeZone, dealEntity.Settings.SwapDealTimeZone, 'value');
            $scope.scopeModel.beginDate = dealEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = dealEntity.Settings.EEDToStore;
            $scope.scopeModel.deActivationDate = dealEntity.Settings.DeActivationDate;
            $scope.scopeModel.active = dealEntity.Settings.Active;
            $scope.scopeModel.difference = dealEntity.Settings.Difference;
        }
        function loadCarrierBoundsSection() {
            var promises = [];

            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadCarrierAccountPromiseDeferred.promise);

            var payload;
            if (dealEntity != undefined && dealEntity.Settings != undefined) {
                payload = { selectedIds: dealEntity.Settings.CarrierAccountId };
                carrierAccountSelectedPromise = UtilsService.createPromiseDeferred();

            }
            carrierAccountSelectorReadyDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, loadCarrierAccountPromiseDeferred);
            });

            if (dealEntity != undefined && dealEntity.Settings != undefined) {

                var loadSwapDealInboundPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSwapDealInboundPromiseDeferred.promise);

                var loadSwapDealOutboundPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSwapDealOutboundPromiseDeferred.promise);

                UtilsService.waitMultiplePromises([dealInboundReadyPromiseDeferred.promise, dealOutboundReadyPromiseDeferred.promise, carrierAccountSelectedPromise.promise]).then(function () {
                    var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                    var payload = {
                        carrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                        sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId,
                        Inbounds: dealEntity.Settings.Inbounds,
                        lastInboundGroupNumber: dealEntity.Settings.LastInboundGroupNumber,
                        context: getContext(),
                        dealId: dealId
                    };
                    var payloadOutbound = {
                        carrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                        supplierId: carrierAccountInfo.CarrierAccountId,
                        Outbounds: dealEntity.Settings.Outbounds,
                        lastOutboundGroupNumber: dealEntity.Settings.LastOutboundGroupNumber,
                        context: getContext(),
                        dealId: dealId
                    };
                    if (dealEntity != undefined && dealEntity.Settings != undefined)
                        payloadOutbound.Outbounds = dealEntity.Settings.Outbounds;
                    if ((isEditMode && dealEntity != undefined) || !isEditMode)
                        VRUIUtilsService.callDirectiveLoad(dealInboundAPI, payload, loadSwapDealInboundPromiseDeferred);
                    VRUIUtilsService.callDirectiveLoad(dealOutboundAPI, payloadOutbound, loadSwapDealOutboundPromiseDeferred);
                    carrierAccountSelectedPromise = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadGraceperiod() {
            if (dealEntity != undefined && dealEntity.Settings.GracePeriod != undefined) {
                $scope.scopeModel.gracePeriod = dealEntity.Settings.GracePeriod;
                return;
            }
            return WhS_Deal_SwapDealAPIService.GetSwapDealSettingData().then(function (response) {
                $scope.scopeModel.gracePeriod = response.GracePeriod;
            });

        }
        function isCommitmentAgreement() {
            if (dealEntity != undefined && dealEntity.Settings != undefined)
                return (dealEntity.Settings.DealType == WhS_Deal_DealAgreementTypeEnum.Commitment.value);
        }

        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyPayload;
            if (dealEntity != undefined && dealEntity.Settings != undefined && dealEntity.Settings.CurrencyId > 0) {
                currencyPayload = { selectedIds: dealEntity.Settings.CurrencyId };
            }
            else {
                currencyPayload = { selectSystemCurrency: true };
            }

            currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred);

            });

            return loadCurrencySelectorPromiseDeferred.promise;
        }

        function insertSwapDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_SwapDealAPIService.AddDeal(buildSwapDealObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Swap Deal', response, 'Description')) {
                    if ($scope.onSwapDealAdded != undefined)
                        $scope.onSwapDealAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
                else {
                    $scope.validationMessages = response.ValidationMessages;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateSwapDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_SwapDealAPIService.UpdateDeal(buildSwapDealObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Swap Deal', response, 'Description')) {
                    if ($scope.onSwapDealUpdated != undefined)
                        $scope.onSwapDealUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
                else {
                    $scope.validationMessages = response.ValidationMessages;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSwapDealObjFromScope() {
            var inboundData = dealInboundAPI.getData();
            var outboundData = dealOutboundAPI.getData();
            var obj = {
                DealId: dealId,
                Name: $scope.scopeModel.description,
                Settings: {
                    $type: "TOne.WhS.Deal.Business.SwapDealSettings, TOne.WhS.Deal.Business",
                    CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                    BeginDate: $scope.scopeModel.beginDate,
                    EEDToStore: $scope.scopeModel.endDate,
                    GracePeriod: $scope.scopeModel.gracePeriod,
                    DealContract: $scope.scopeModel.selectedContractType.value,
                    DealType: $scope.scopeModel.selectedAgreementType.value,
                    Status: $scope.scopeModel.selectedDealStatus.value,
                    Inbounds: inboundData != undefined ? inboundData.inbounds : undefined,
                    Outbounds: outboundData != undefined ? outboundData.outbounds : undefined,
                    LastInboundGroupNumber: inboundData != undefined ? inboundData.lastInboundGroupNumber : undefined,
                    LastOutboundGroupNumber: outboundData != undefined ? outboundData.lastOutboundGroupNumber : undefined,
                    Difference: $scope.scopeModel.difference,
                    CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                    DeActivationDate: $scope.scopeModel.deActivationDate,
                    IsRecurrable: dealEntity != undefined && dealEntity.Settings != undefined ? dealEntity.Settings.IsRecurrable : true,
                    SwapDealTimeZone: $scope.scopeModel.selectedTimeZone.value

                }
            };
            return obj;
        }

        function getContext() {
            var shiftedBED ;
            var shiftedEED ;
            var shiftedDatesPromiseDeferred = UtilsService.createPromiseDeferred();
            if ($scope.scopeModel.beginDate != undefined && $scope.scopeModel.endDate != undefined) {
                GetShiftedDate($scope.scopeModel.beginDate).then(function (response) {
                    shiftedBED = response;
                    GetShiftedDate($scope.scopeModel.endDate).then(function (response) {
                        shiftedEED = response;
                        shiftedDatesPromiseDeferred.resolve();
                    });
                });
            }
            return {
                getSupplierZoneSelectorPayload: function (item) {
                    var payload;
                    payload = {
                        supplierId: carrierAccountSelectorAPI.getSelectedIds()
                    };
                    if (item != undefined) {
                        var zoneIds = [];
                        var itemZones = item.SupplierZones != undefined ? item.SupplierZones : item.Zones;
                        for (var x = 0; x < itemZones.length; x++) {
                            zoneIds.push(itemZones[x].ZoneId);
                        }
                        payload.selectedIds = zoneIds;
                        payload.filter = {
                            ExcludedZoneIds: getSelectedSupplierZonesIdsFromItems(zoneIds),
                            CountryIds: (item.CountryId != undefined) ? [item.CountryId] : undefined,
                            EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                            EffectiveDate: $scope.scopeModel.beginDate,
                            ExcludePendingClosedZones: true
                        };
                    }
                    else
                        payload.filter = {
                            ExcludedZoneIds: getSelectedSupplierZonesIdsFromItems(),
                            EffectiveDate: $scope.scopeModel.beginDate,
                            EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                            ExcludePendingClosedZones: true
                        };
                    shiftedDatesPromiseDeferred.promise.then(function(){
                        payload.filter.Filters = [{
                            $type: "TOne.WhS.Deal.Business.SupplierZoneFilter, TOne.WhS.Deal.Business",
                            CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                            DealId: dealId,
                            BED: shiftedBED,
                            EED: shiftedEED
                        }];

                        return payload;
                    });
                },

                getSaleZoneSelectorPayload: function (item) {
                    var carrierAccount = carrierAccountSelectorAPI.getSelectedValues();
                    var payload;
                    payload = {
                        sellingNumberPlanId: carrierAccount != undefined ? carrierAccount.SellingNumberPlanId : undefined
                    };
                    if (item != undefined) {
                        var sellZoneIds = [];
                        var itemZones = item.SaleZones != undefined ? item.SaleZones : item.Zones;
                        for (var j = 0; j < itemZones.length; j++) {
                            sellZoneIds.push(itemZones[j].ZoneId);
                        }
                        payload.selectedIds = sellZoneIds;
                        payload.filter = {
                            ExcludedZoneIds: getSelectedSaleZonesIdsFromItems(sellZoneIds),
                            CountryIds: (item.CountryId != undefined) ? [item.CountryId] : undefined,
                            EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                            EffectiveDate: $scope.scopeModel.beginDate,
                            ExcludePendingClosedZones :true
                        };
                    }
                    else {
                        payload.filter = {
                            ExcludedZoneIds: getSelectedSaleZonesIdsFromItems(),
                            EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                            EffectiveDate: $scope.scopeModel.beginDate,
                            ExcludePendingClosedZones: true
                        };
                    }
                    shiftedDatesPromiseDeferred.promise.then(function () {
                        payload.filter.Filters = [{
                            $type: "TOne.WhS.Deal.Business.SaleZoneFilter, TOne.WhS.Deal.Business",
                            CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                            DealId: dealId,
                            BED: shiftedBED,
                            EED: shiftedEED
                        }];

                        return payload;
                    });
                   
                },
                getEffectiveOnDate: function () {
                    return GetShiftedDate($scope.scopeModel.beginDate);
                },
                getDealBED: function () {
                    return $scope.scopeModel.beginDate;
                },
                getDealEED: function () {
                    return $scope.scopeModel.endDate;
                },
                getCarrierAccountId: function () {
                    return carrierAccountSelectorAPI.getSelectedIds();   
                },
                getSwapDealCurrency: function () {
                    return currencyDirectiveAPI.getSelectedValues();
                }
            };
        }
        function getSelectedSaleZonesIdsFromItems(includedIds) {
            var ids = getUsedSaleZonesIds();
            var filterdIds;
            if (ids != undefined) {
                filterdIds = [];
                for (var x = 0; x < ids.length; x++) {
                    if (includedIds != undefined && includedIds.indexOf(ids[x]) < 0)
                        filterdIds.push(ids[x]);
                    else if (includedIds == undefined)
                        filterdIds.push(ids[x]);
                }
            }
            return filterdIds;
        }
        function GetShiftedDate(date) {
            var isShifted = $scope.scopeModel.selectedTimeZone.value == WhS_Deal_SwapDealTimeZoneTypeEnum.Supplier.value ? true : false;
            return WhS_Deal_DealDefinitionAPIService.GetEffectiveOnDate(false, isShifted, carrierAccountSelectorAPI.getSelectedIds(), date, offset);
        }
        function getSelectedSupplierZonesIdsFromItems(includedIds) {
            var ids = getUsedSupplierZonesIds();
            var filterdIds;
            if (ids != undefined) {
                filterdIds = [];
                for (var x = 0; x < ids.length; x++) {
                    if (includedIds != undefined && includedIds.indexOf(ids[x]) < 0)
                        filterdIds.push(ids[x]);
                    else if (includedIds == undefined)
                        filterdIds.push(ids[x]);
                }
            }
            return filterdIds;
        }
        function getUsedSaleZonesIds() {
            var zonesIds;
            var items = dealInboundAPI.getData();
            if (items.inbounds.length > 0) {
                zonesIds = [];
                for (var i = 0; i < items.inbounds.length; i++) {
                    var zoneIds = [];
                    for (var x = 0; x < items.inbounds[i].SaleZones.length; x++) {
                        zoneIds.push(items.inbounds[i].SaleZones[x].ZoneId);
                    }
                    zonesIds = zonesIds.concat(zoneIds);
                }
            }
            return zonesIds;
        }
        function getUsedSupplierZonesIds() {
            var zonesIds;
            var items = dealOutboundAPI.getData();
            if (items.outbounds.length > 0) {
                zonesIds = [];
                for (var i = 0; i < items.outbounds.length; i++) {
                    var zoneIds = [];
                    for (var x = 0; x < items.outbounds[i].SupplierZones.length; x++) {
                        zoneIds.push(items.outbounds[i].SupplierZones[x].ZoneId);
                    }
                    zonesIds = zonesIds.concat(zoneIds);
                }
            }
            return zonesIds;
        }
    }

    app.controller('WhS_Deal_SwapDealEditorController', SwapDealEditorController);

})(app);
﻿(function (app) {

    'use strict';

    VolumeCommitmentEditorController.$inject = ['$scope', 'WhS_Deal_VolCommitmentDealAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Deal_VolumeCommitmentService', 'WhS_Deal_VolumeCommitmentTypeEnum', 'VRValidationService', 'VRDateTimeService', 'WhS_Deal_DealStatusTypeEnum', 'WhS_Deal_VolCommitmentTimeZoneTypeEnum', 'WhS_Deal_DealDefinitionAPIService','VRCommon_EntityFilterEffectiveModeEnum'];

    function VolumeCommitmentEditorController($scope, WhS_Deal_VolCommitmentDealAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_Deal_VolumeCommitmentService, WhS_Deal_VolumeCommitmentTypeEnum, VRValidationService, VRDateTimeService, WhS_Deal_DealStatusTypeEnum, WhS_Deal_VolCommitmentTimeZoneTypeEnum, WhS_Deal_DealDefinitionAPIService, VRCommon_EntityFilterEffectiveModeEnum) {

        var isEditMode;

        var dealId;
        var volumeCommitmentEntity;
        var lastGroupNumber = 0;
        var context;
        var isViewHistoryMode;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred;
        var carrierAccountSelectedPromise;

        var currencyDirectiveAPI;
        var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var volumeCommitmenetItemsAPI;
        var volumeCommitmenetItemsReadyDeferred = UtilsService.createPromiseDeferred();
        var volumeCommitmentType;

        var offset;

        var carrierAccountInfo;

        var originalEED;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dealId = parameters.dealId;
                context = parameters.context;

                isViewHistoryMode = context != undefined && context.historyId != undefined;
            }

            isEditMode = (dealId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.disabelType = isEditMode;
            $scope.scopeModel.volumeCommitmentTypes = UtilsService.getArrayEnum(WhS_Deal_VolumeCommitmentTypeEnum);
            $scope.scopeModel.dealStatus = UtilsService.getArrayEnum(WhS_Deal_DealStatusTypeEnum);

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value; };
                var payload = {
                    context: context
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountSelectorAPI, payload, setLoader, carrierAccountSelectorReadyDeferred);
                var payload = { context: getContext() };
                volumeCommitmenetItemsAPI.load(payload);
                if (!carrierAccountSelectedPromise)
                    $scope.scopeModel.description = undefined;
            };

            $scope.scopeModel.onVolumeCommitmentTypeChanged = function () {
                if ($scope.scopeModel.selectedVolumeCommitmentType != undefined) {
                    var datasource = [];
                    datasource.push(WhS_Deal_VolCommitmentTimeZoneTypeEnum.System);
                    if ($scope.scopeModel.selectedVolumeCommitmentType.value === WhS_Deal_VolumeCommitmentTypeEnum.Sell.value) {
                        datasource.push(WhS_Deal_VolCommitmentTimeZoneTypeEnum.Customer);
                        if (!isEditMode)
                            $scope.scopeModel.selectedTimeZone = WhS_Deal_VolCommitmentTimeZoneTypeEnum.System;
                    }
                    if ($scope.scopeModel.selectedVolumeCommitmentType.value === WhS_Deal_VolumeCommitmentTypeEnum.Buy.value) {
                        datasource.push(WhS_Deal_VolCommitmentTimeZoneTypeEnum.Supplier);
                        if (!isEditMode)
                            $scope.scopeModel.selectedTimeZone = WhS_Deal_VolCommitmentTimeZoneTypeEnum.Supplier;
                    }
                    $scope.scopeModel.VolTimeZone = datasource;
                }
            };

            $scope.scopeModel.selectedDealStatus = WhS_Deal_DealStatusTypeEnum.Draft;
            $scope.scopeModel.onCurrencySelectReady = function (api) {
                currencyDirectiveAPI = api;
                currencyDirectiveReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onBEDChanged = function () {
                updateDescription();
            };
            $scope.scopeModel.onEEDChanged = function () {

            };

            $scope.scopeModel.onCarrierAccountSelectionChanged = function () {
                carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                loadVolumeCommitmentItems();

            };

            $scope.scopeModel.onVolumeCommitmenetItemsReady = function (api) {
                volumeCommitmenetItemsAPI = api;
                volumeCommitmenetItemsReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateVolumeCommitment() : insertVolumeCommitment();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.isInactiveStatus = function () {
                if ($scope.scopeModel.selectedDealStatus != undefined)
                    return ($scope.scopeModel.selectedDealStatus.value == WhS_Deal_DealStatusTypeEnum.Inactive.value);
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
            $scope.scopeModel.showVolumeItemSection = function () {
                return (carrierAccountInfo != undefined && $scope.scopeModel.beginDate != undefined && $scope.scopeModel.endDate != undefined);
            };
            $scope.scopeModel.validateDealStatusDate = function () {
                var date = UtilsService.createDateFromString($scope.scopeModel.deActivationDate);
                var beginDate = UtilsService.createDateFromString($scope.scopeModel.beginDate);
                var endDate = UtilsService.createDateFromString($scope.scopeModel.endDate);
                if (date < beginDate)
                    return "Deactivation date must be greater than deal BED";
                if (date > endDate)
                    return "Deactivation date must be less than deal EED";
                return null;
            };
            $scope.scopeModel.onDealStatusChanged = function () {
                if ($scope.scopeModel.selectedDealStatus != undefined && $scope.scopeModel.selectedDealStatus.value != WhS_Deal_DealStatusTypeEnum.Inactive.value)
                    $scope.scopeModel.deActivationDate = undefined;
                if ($scope.scopeModel.selectedDealStatus != undefined && $scope.scopeModel.deActivationDate == undefined && $scope.scopeModel.selectedDealStatus.value == WhS_Deal_DealStatusTypeEnum.Inactive.value)
                    $scope.scopeModel.deActivationDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getVolumeCommitment().then(function () {
                    loadAllControls().finally(function () {
                        volumeCommitmentEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getVolumeCommitmentHistory().then(function () {
                    loadAllControls().finally(function () {
                        volumeCommitmentEntity = undefined;
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

        function getVolumeCommitmentHistory() {
            return WhS_Deal_VolCommitmentDealAPIService.GetVolumeCommitmentHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                volumeCommitmentEntity = response;

            });
        }
        function loadVolumeCommitmentItems() {
            if (carrierAccountInfo != undefined) {
                if (carrierAccountSelectedPromise != undefined)
                    carrierAccountSelectedPromise.resolve();
                else {
                    var payload = {
                        context: getContext(),
                    };
                    volumeCommitmenetItemsAPI.load(payload);
                    updateDescription();
                }
            }
        }
        function getVolumeCommitment() {
            return WhS_Deal_VolCommitmentDealAPIService.GetDeal(dealId).then(function (response) {
                volumeCommitmentEntity = response;
                lastGroupNumber = volumeCommitmentEntity.Settings.LastGroupNumber;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadOffset, loadStaticData, loadCarrierAccountDealItemsSection, loadCurrencySelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                if (volumeCommitmentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(volumeCommitmentEntity.Name, 'Volume Commitment');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Volume Commitment');
        }
        function loadOffset() {
            if (volumeCommitmentEntity == undefined || volumeCommitmentEntity.Settings == undefined)
                return;
            offset = volumeCommitmentEntity.Settings.Offset;
        }

        function loadStaticData() {
            if (volumeCommitmentEntity == undefined)
                return;
            originalEED = volumeCommitmentEntity.Settings.EEDToStore;
            $scope.scopeModel.description = volumeCommitmentEntity.Name;
            $scope.scopeModel.beginDate = volumeCommitmentEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = volumeCommitmentEntity.Settings.EEDToStore;
            $scope.scopeModel.selectedDealStatus = UtilsService.getItemByVal($scope.scopeModel.dealStatus, volumeCommitmentEntity.Settings.Status, 'value');
            $scope.scopeModel.deActivationDate = volumeCommitmentEntity.Settings.DeActivationDate;
            $scope.scopeModel.selectedVolumeCommitmentType = UtilsService.getItemByVal($scope.scopeModel.volumeCommitmentTypes, volumeCommitmentEntity.Settings.DealType, "value");

            var datasource = [];
            datasource.push(WhS_Deal_VolCommitmentTimeZoneTypeEnum.System);
            if ($scope.scopeModel.selectedVolumeCommitmentType.value === WhS_Deal_VolumeCommitmentTypeEnum.Sell.value) {
                datasource.push(WhS_Deal_VolCommitmentTimeZoneTypeEnum.Customer);
            }
            if ($scope.scopeModel.selectedVolumeCommitmentType.value === WhS_Deal_VolumeCommitmentTypeEnum.Buy.value) {
                datasource.push(WhS_Deal_VolCommitmentTimeZoneTypeEnum.Supplier);
            }
            $scope.scopeModel.VolTimeZone = datasource;
            $scope.scopeModel.selectedTimeZone = UtilsService.getItemByVal($scope.scopeModel.VolTimeZone, volumeCommitmentEntity.Settings.VolCommitmentTimeZone, 'value');
        }
        function loadCarrierAccountDealItemsSection() {
            if (volumeCommitmentEntity == undefined)
                return;

            var promises = [];

            carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(carrierAccountSelectorLoadDeferred.promise);

            var payload;
            if (volumeCommitmentEntity != undefined && volumeCommitmentEntity.Settings != undefined) {
                payload = { selectedIds: volumeCommitmentEntity.Settings.CarrierAccountId };
                carrierAccountSelectedPromise = UtilsService.createPromiseDeferred();

            }
            carrierAccountSelectorReadyDeferred.promise.then(function () {
                carrierAccountSelectorReadyDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountSelectorLoadDeferred);
            });

            if (volumeCommitmentEntity != undefined && volumeCommitmentEntity.Settings != undefined && volumeCommitmentEntity.Settings.Items.length > 0) {

                var volumeCommitmenetItemsLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(volumeCommitmenetItemsLoadDeferred.promise);
                UtilsService.waitMultiplePromises([volumeCommitmenetItemsReadyDeferred.promise, carrierAccountSelectedPromise.promise]).then(function () {

                    var payload = {
                        context: getContext(),
                    };
                    if (volumeCommitmentEntity != undefined) {
                        payload.volumeCommitmentItems = volumeCommitmentEntity.Settings.Items;
                    }
                    VRUIUtilsService.callDirectiveLoad(volumeCommitmenetItemsAPI, payload, volumeCommitmenetItemsLoadDeferred);

                    carrierAccountSelectedPromise = undefined;
                });
            }
            return UtilsService.waitMultiplePromises(promises);
        }
        function updateDescription() {
            setTimeout(function () {
                if ($scope.scopeModel.carrierAccount != undefined)
                    $scope.scopeModel.description = "Deal _ " + $scope.scopeModel.carrierAccount.Name + " _ " + UtilsService.getShortDate($scope.scopeModel.beginDate);
            });
        }
        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyPayload;
            if (volumeCommitmentEntity != undefined && volumeCommitmentEntity.Settings != undefined && volumeCommitmentEntity.Settings.CurrencyId > 0) {
                currencyPayload = { selectedIds: volumeCommitmentEntity.Settings.CurrencyId };
            }
            else {
                currencyPayload = { selectSystemCurrency: true };
            }

            currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred);

            });

            return loadCurrencySelectorPromiseDeferred.promise;
        }

        function insertVolumeCommitment() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_VolCommitmentDealAPIService.AddDeal(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Volume Commitment', response, 'Description')) {
                    if ($scope.onVolumeCommitmentAdded != undefined)
                        $scope.onVolumeCommitmentAdded(response.InsertedObject);
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
        function updateVolumeCommitment() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_VolCommitmentDealAPIService.UpdateDeal(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Volume Commitment', response, 'Description')) {
                    if ($scope.onVolumeCommitmentUpdated != undefined)
                        $scope.onVolumeCommitmentUpdated(response.UpdatedObject);
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

        function buildVolumeCommitmentObjFromScope() {
            var volumeCommitmenetItemsData = volumeCommitmenetItemsAPI.getData();
            var obj = {
                DealId: dealId,
                Name: $scope.scopeModel.description,
                Settings: {
                    $type: "TOne.WhS.Deal.Business.VolCommitmentDealSettings, TOne.WhS.Deal.Business",
                    DealType: $scope.scopeModel.selectedVolumeCommitmentType.value,
                    CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                    BeginDate: $scope.scopeModel.beginDate,
                    EEDToStore: $scope.scopeModel.endDate,
                    Items: volumeCommitmenetItemsData != undefined ? volumeCommitmenetItemsData.volumeCommitmentItems : undefined,
                    LastGroupNumber: volumeCommitmenetItemsData != undefined ? volumeCommitmenetItemsData.lastGroupNumber : undefined,
                    CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                    Status: $scope.scopeModel.selectedDealStatus.value,
                    DeActivationDate: $scope.scopeModel.deActivationDate,
                    IsRecurrable: volumeCommitmentEntity != undefined && volumeCommitmentEntity.Settings != undefined ? volumeCommitmentEntity.Settings.IsRecurrable : true,
                    VolCommitmentTimeZone: $scope.scopeModel.selectedTimeZone.value
                }
            };
            return obj;
        }

        function getContext() {
            var shiftedBED ;
            var shiftedEED ;
            var shiftedDatesPromiseDeferred = UtilsService.createPromiseDeferred();
            var payloadLoadedPromiseDeferred = UtilsService.createPromiseDeferred();
            if ($scope.scopeModel.beginDate != undefined && $scope.scopeModel.endDate != undefined) {
                GetShiftedDate($scope.scopeModel.beginDate).then(function (response) {
                    shiftedBED = response;
                    GetShiftedDate($scope.scopeModel.endDate).then(function (reponse) {
                        shiftedEED = response;
                        shiftedDatesPromiseDeferred.resolve();
                    });
                });
            }
            var context = {
                lastGroupNumber: lastGroupNumber,
                getRateEvaluatorSelective: function () {
                    if ($scope.scopeModel.selectedVolumeCommitmentType != undefined)
                        return $scope.scopeModel.selectedVolumeCommitmentType.RateEvaluatorSelective;
                },
                getZoneSelector: function () {
                    if ($scope.scopeModel.selectedVolumeCommitmentType != undefined)
                        return $scope.scopeModel.selectedVolumeCommitmentType.zoneSelector;
                },
                getZoneSelectorPayload: function (item) {
                    if ($scope.scopeModel.selectedVolumeCommitmentType != undefined) {
                        var payload;
                        switch ($scope.scopeModel.selectedVolumeCommitmentType.value) {
                            case WhS_Deal_VolumeCommitmentTypeEnum.Buy.value:

                                payload = {
                                    supplierId: carrierAccountSelectorAPI.getSelectedIds()
                                };
                                if (item != undefined) {
                                    var zoneIds = [];
                                    var itemZones = item.SaleZones != undefined ? item.SaleZones : item.Zones;
                                    for (var x = 0; x < itemZones.length; x++) {
                                        zoneIds.push(itemZones[x].ZoneId);
                                    }
                                    payload.selectedIds = zoneIds;
                                    payload.filter = {
                                        ExcludedZoneIds: getSelectedZonesIdsFromItems(zoneIds),
                                        CountryIds: (item.CountryId != undefined) ? [item.CountryId] : undefined,
                                        EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                                        EffectiveDate: $scope.scopeModel.beginDate,
                                        ExcludePendingClosedZones: true
                                    };
                                }
                                else
                                    payload.filter = {
                                        ExcludedZoneIds: getSelectedZonesIdsFromItems(),
                                        EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                                        EffectiveDate: $scope.scopeModel.beginDate,
                                        ExcludePendingClosedZones: true
                                    };
                                shiftedDatesPromiseDeferred.promise.then(function () {
                                    payload.filter.Filters = [{
                                        $type: "TOne.WhS.Deal.Business.SupplierZoneFilter, TOne.WhS.Deal.Business",
                                        CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                                        DealId: dealId,
                                        BED: $scope.scopeModel.beginDate,
                                        EED: $scope.scopeModel.endDate,
                                        FollowSystemTimeZone: $scope.scopeModel.followSystemTimeZone
                                    }];
                                    payloadLoadedPromiseDeferred.resolve();
                                });
                                break;
                            case WhS_Deal_VolumeCommitmentTypeEnum.Sell.value:
                                var carrierAccount = carrierAccountSelectorAPI.getSelectedValues();
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
                                        ExcludedZoneIds: getSelectedZonesIdsFromItems(sellZoneIds),
                                        CountryIds: (item.CountryId != undefined) ? [item.CountryId] : undefined,
                                         EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                                        EffectiveDate: $scope.scopeModel.beginDate,
                                        ExcludePendingClosedZones: true
                                    };
                                }
                                else
                                    payload.filter = {
                                        ExcludedZoneIds: getSelectedZonesIdsFromItems(),
                                        EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value,
                                        EffectiveDate: $scope.scopeModel.beginDate,
                                        ExcludePendingClosedZones: true
                                    };
                                shiftedDatesPromiseDeferred.promise.then(function () {
                                    payload.filter.Filters = [{
                                        $type: "TOne.WhS.Deal.Business.SaleZoneFilter, TOne.WhS.Deal.Business",
                                        CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                                        DealId: dealId,
                                        BED: $scope.scopeModel.beginDate,
                                        EED: $scope.scopeModel.endDate,
                                        FollowSystemTimeZone: $scope.scopeModel.followSystemTimeZone
                                    }];
                                    payloadLoadedPromiseDeferred.resolve();
                                   
                                });
                                break;
                        }
                        payloadLoadedPromiseDeferred.promise.then(function () {
                            return payload;
                        });
                    }
                },
                getZoneIdAttName: function () {
                    if ($scope.scopeModel.selectedVolumeCommitmentType != undefined) {
                        var idName;
                        switch ($scope.scopeModel.selectedVolumeCommitmentType.value) {
                            case WhS_Deal_VolumeCommitmentTypeEnum.Buy.value:
                                idName = 'SupplierZoneId';
                                break;
                            case WhS_Deal_VolumeCommitmentTypeEnum.Sell.value:
                                idName = 'SaleZoneId';
                                break;
                        }
                        return idName;
                    }
                },
                getVolumeType: function () {
                    return $scope.scopeModel.selectedVolumeCommitmentType;
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
                getVolumeCommitmentCurrency: function () {
                    return currencyDirectiveAPI.getSelectedValues();
                }

            };
            return context;
        }
        function GetShiftedDate(date) {
            var isSale;
            var isShifted;
            if ($scope.scopeModel.selectedTimeZone.value == WhS_Deal_VolCommitmentTimeZoneTypeEnum.System)
                isShifted = false;
            else {
                isShifted = true;
                isSale = $scope.scopeModel.selectedTimeZone.value == WhS_Deal_VolCommitmentTimeZoneTypeEnum.Customer ? true : false;
            }
            return WhS_Deal_DealDefinitionAPIService.GetEffectiveOnDate(isSale, isShifted, carrierAccountSelectorAPI.getSelectedIds(), date, offset);
        }
        function getSelectedZonesIdsFromItems(includedIds) {
            var ids = getUsedZonesIds();
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
        function getUsedZonesIds() {
            var zonesIds;
            var items = volumeCommitmenetItemsAPI.getData();
            if (items.volumeCommitmentItems.length > 0) {
                zonesIds = [];
                for (var i = 0; i < items.volumeCommitmentItems.length; i++) {
                    var zoneIds = [];
                    for (var x = 0; x < items.volumeCommitmentItems[i].SaleZones.length; x++) {
                        zoneIds.push(items.volumeCommitmentItems[i].SaleZones[x].ZoneId);
                    }
                    zonesIds = zonesIds.concat(zoneIds);
                }
            }
            return zonesIds;
        }
    }

    app.controller('WhS_Deal_VolumeCommitmentEditorController', VolumeCommitmentEditorController);

})(app);
(function (app) {

    'use strict';

    VolumeCommitmentEditorController.$inject = ['$scope', 'WhS_Deal_VolCommitmentDealAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Deal_VolumeCommitmentService', 'WhS_Deal_VolumeCommitmentTypeEnum', 'VRValidationService', 'VRDateTimeService', 'WhS_Deal_DealStatusTypeEnum'];

    function VolumeCommitmentEditorController($scope, WhS_Deal_VolCommitmentDealAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_Deal_VolumeCommitmentService, WhS_Deal_VolumeCommitmentTypeEnum, VRValidationService, VRDateTimeService, WhS_Deal_DealStatusTypeEnum) {

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
        };
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.disabelType = isEditMode;
            $scope.scopeModel.volumeCommitmentTypes = UtilsService.getArrayEnum(WhS_Deal_VolumeCommitmentTypeEnum);
            $scope.scopeModel.dealStatus = UtilsService.getArrayEnum(WhS_Deal_DealStatusTypeEnum);
            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountSelectorAPI, payload, setLoader, carrierAccountSelectorReadyDeferred);
                var payload = { context: getContext() };
                volumeCommitmenetItemsAPI.load(payload);
                if (!carrierAccountSelectedPromise)
                    $scope.scopeModel.description = undefined;
            };
            $scope.scopeModel.selectedDealStatus = WhS_Deal_DealStatusTypeEnum.Draft;
            $scope.scopeModel.onCurrencySelectReady = function (api) {
                currencyDirectiveAPI = api;
                currencyDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCarrierAccountSelectionChanged = function () {
                var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                if (carrierAccountInfo != undefined) {
                    if (carrierAccountSelectedPromise != undefined)
                        carrierAccountSelectedPromise.resolve();
                    else {
                        var payload = {
                            context: getContext(),
                            carrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                            dealId: dealId,
                            bed: $scope.scopeModel.beginDate,
                            eed: $scope.scopeModel.endDate,
                            volumeCommitmentType: $scope.scopeModel.selectedVolumeCommitmentType
                        };
                        volumeCommitmenetItemsAPI.load(payload);
                        updateDescription();
                    }
                }

                function updateDescription() {
                    setTimeout(function () {
                        $scope.scopeModel.description = "Deal _ " + $scope.scopeModel.carrierAccount.Name + " _ " + UtilsService.getShortDate(VRDateTimeService.getNowDateTime());
                    });
                }
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
            $scope.scopeModel.validateDatesRange = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.beginDate, $scope.scopeModel.endDate);
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
            };
        };
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
        };

        function getVolumeCommitmentHistory() {
            return WhS_Deal_VolCommitmentDealAPIService.GetVolumeCommitmentHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                volumeCommitmentEntity = response;

            });
        }
        function getVolumeCommitment() {
            return WhS_Deal_VolCommitmentDealAPIService.GetDeal(dealId).then(function (response) {
                volumeCommitmentEntity = response;
                lastGroupNumber = volumeCommitmentEntity.Settings.LastGroupNumber;
            });
        };

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCarrierAccountDealItemsSection, loadCurrencySelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };
        function setTitle() {
            if (isEditMode) {
                if (volumeCommitmentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(volumeCommitmentEntity.Name, 'Volume Commitment');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Volume Commitment');
        };
        function loadStaticData() {
            if (volumeCommitmentEntity == undefined)
                return;
            $scope.scopeModel.description = volumeCommitmentEntity.Name;
            $scope.scopeModel.beginDate = volumeCommitmentEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = volumeCommitmentEntity.Settings.EndDate;
            $scope.scopeModel.selectedDealStatus = UtilsService.getItemByVal($scope.scopeModel.dealStatus, volumeCommitmentEntity.Settings.Status, 'value');
            $scope.scopeModel.deActivationDate = volumeCommitmentEntity.Settings.DeActivationDate;
            //$scope.scopeModel.active = volumeCommitmentEntity.Settings.Active;
            $scope.scopeModel.selectedVolumeCommitmentType = UtilsService.getItemByVal($scope.scopeModel.volumeCommitmentTypes, volumeCommitmentEntity.Settings.DealType, "value");
        };
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
                        carrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                        dealId: dealId,
                        bed: $scope.scopeModel.beginDate,
                        eed: $scope.scopeModel.endDate,
                        volumeCommitmentType: $scope.scopeModel.selectedVolumeCommitmentType
                    };
                    if (volumeCommitmentEntity != undefined) {
                        payload.volumeCommitmentItems = volumeCommitmentEntity.Settings.Items;
                    }
                    VRUIUtilsService.callDirectiveLoad(volumeCommitmenetItemsAPI, payload, volumeCommitmenetItemsLoadDeferred);

                    carrierAccountSelectedPromise = undefined;
                });
            }
            return UtilsService.waitMultiplePromises(promises);
        };
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
        };

        function insertVolumeCommitment() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_VolCommitmentDealAPIService.AddDeal(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Volume Commitment', response, 'Description')) {
                    if ($scope.onVolumeCommitmentAdded != undefined)
                        $scope.onVolumeCommitmentAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };
        function updateVolumeCommitment() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_VolCommitmentDealAPIService.UpdateDeal(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Volume Commitment', response, 'Description')) {
                    if ($scope.onVolumeCommitmentUpdated != undefined)
                        $scope.onVolumeCommitmentUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

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
                    EndDate: $scope.scopeModel.endDate,
                    Items: volumeCommitmenetItemsData != undefined ? volumeCommitmenetItemsData.volumeCommitmentItems : undefined,
                    LastGroupNumber: volumeCommitmenetItemsData != undefined ? volumeCommitmenetItemsData.lastGroupNumber : undefined,
                    CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                    Status: $scope.scopeModel.selectedDealStatus.value,
                    DeActivationDate: $scope.scopeModel.deActivationDate
                }
            };
            return obj;
        };

        function getContext() {
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
                                        CountryIds: (item.CountryId != undefined) ? [item.CountryId] : undefined
                                    };
                                }
                                else
                                    payload.filter = {
                                        ExcludedZoneIds: getSelectedZonesIdsFromItems()
                                    };
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
                                        CountryIds: (item.CountryId != undefined) ? [item.CountryId] : undefined
                                    };
                                }
                                else
                                    payload.filter = {
                                        ExcludedZoneIds: getSelectedZonesIdsFromItems()
                                    };
                                break;
                        }
                        return payload;
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
                }

            };
            return context;
        };
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
        };
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
        };
    };

    app.controller('WhS_Deal_VolumeCommitmentEditorController', VolumeCommitmentEditorController);

})(app);
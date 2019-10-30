﻿(function (appControllers) {

    'use strict';

    SwapDealOutboundEditorController.$inject = ['$scope', 'UtilsService', 'UISettingsService', 'Whs_Deal_SubstituteRateTypeEnum', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VRCommon_EntityFilterEffectiveModeEnum'];

    function SwapDealOutboundEditorController($scope, UtilsService, UISettingsService, Whs_Deal_SubstituteRateTypeEnum, VRUIUtilsService, VRNavigationService, VRNotificationService, VRCommon_EntityFilterEffectiveModeEnum) {
        var isEditMode;

        var swapDealOutboundEntity;
        var supplierId;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierZoneDirectiveAPI;
        var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var substituteRateTypeApi;
        var substituteRateTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountId;

        var dealId;

        var countrySelectedPromiseDeferred;

        var context;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                supplierId = parameters.supplierId;
                swapDealOutboundEntity = parameters.swapDealOutbound;
                context = parameters.context;
                carrierAccountId = parameters.carrierAccountId;
                dealId = parameters.dealId;
            }

            isEditMode = (swapDealOutboundEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.FixedRateValue = Whs_Deal_SubstituteRateTypeEnum.FixedRate.value;

            $scope.longPrecision = UISettingsService.getLongPrecision();

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwapDealOutbound() : insertSwapDealOutbound();
            };

            var swapDealCurrency = context.getSwapDealCurrency();
            $scope.scopeModel.currency = swapDealCurrency != undefined ? swapDealCurrency.Symbol : undefined;

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            $scope.onSupplierZoneDirectiveReady = function (api) {
                supplierZoneDirectiveAPI = api;
                supplierZoneReadyPromiseDeferred.resolve();
            };

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.onSubstituteRateTypeSelectorReady = function (api) {
                substituteRateTypeApi = api;
                substituteRateTypeReadyPromiseDeferred.resolve();
            };

            $scope.onSubstituteRateTypeChange = function (val) {
                if (val != undefined)
                    $scope.scopeModel.substituteRateTypeValue = val.value;

                if (val.value != Whs_Deal_SubstituteRateTypeEnum.FixedRate.value)
                    $scope.scopeModel.fixedRate = null;
            };

            $scope.onCountrySelectionChanged = function () {
                var country = countryDirectiveApi.getSelectedIds();
                if (country != undefined) {
                    if (countrySelectedPromiseDeferred != undefined) {
                        countrySelectedPromiseDeferred.resolve();
                        return;
                    }
                    var setLoader = function (value) { $scope.isLoadingSelector = value; };
                    var payload = context != undefined ? context.getSupplierZoneSelectorPayload(swapDealOutboundEntity != undefined ? swapDealOutboundEntity : undefined) : undefined;
                    if (payload != undefined) {
                        payload.supplierId = supplierId;
                        payload.filter.CountryIds = countryDirectiveApi.getSelectedIds();
                        payload.filter.EffectiveMode = VRCommon_EntityFilterEffectiveModeEnum.Current.value;
                        payload.selectedIds = undefined;
                    }
                    else {
                        payload = {
                            supplierId: supplierId,
                            filter: { CountryIds: countryDirectiveApi.getSelectedIds() },
                            selectedIds: undefined,
                            EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.Current.value
                        };

                    }
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader);

                }
                else if (supplierZoneDirectiveAPI != undefined)
                    $scope.supplierzones.length = 0;
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls();
                loadStaticData();
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySupplierZoneSection, loadSubstituteRateTypeSelector]).then(function () {

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadSubstituteRateTypeSelector() {
            var loadSubstituteRateTypePromiseDeferred = UtilsService.createPromiseDeferred();

            substituteRateTypeReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: swapDealOutboundEntity != undefined ? swapDealOutboundEntity.SubstituteRateType : Whs_Deal_SubstituteRateTypeEnum.NormalRate.value
                };
                VRUIUtilsService.callDirectiveLoad(substituteRateTypeApi, payload, loadSubstituteRateTypePromiseDeferred);
            });
        }

        function loadCountrySupplierZoneSection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload;

            if (swapDealOutboundEntity != undefined && swapDealOutboundEntity.CountryIds != undefined) {
                payload = {};
                payload.selectedIds = swapDealOutboundEntity != undefined ? swapDealOutboundEntity.CountryIds : undefined;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });



            if (swapDealOutboundEntity != undefined && swapDealOutboundEntity.CountryIds != undefined) {
                var loadSupplierZonePromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadSupplierZonePromiseDeferred.promise);

                UtilsService.waitMultiplePromises([supplierZoneReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var zoneIds = undefined;
                    if (swapDealOutboundEntity != undefined) {
                        zoneIds = [];
                        for (var i = 0; i < swapDealOutboundEntity.SupplierZones.length; i++) {
                            zoneIds.push(swapDealOutboundEntity.SupplierZones[i].ZoneId);
                        }
                    }
                    var payload = context != undefined ? context.getSupplierZoneSelectorPayload(swapDealOutboundEntity != undefined ? swapDealOutboundEntity : undefined) : undefined;
                    if (payload != undefined) {
                        payload.supplierId = supplierId;
                        payload.filter.CountryIds = swapDealOutboundEntity.CountryIds;
                        payload.selectedIds = zoneIds;

                    }
                    else {
                        payload = {
                            supplierId: supplierId,
                            filter: { CountryIds: swapDealOutboundEntity.CountryIds },
                            selectedIds: zoneIds
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(supplierZoneDirectiveAPI, payload, loadSupplierZonePromiseDeferred);
                    loadSupplierZonePromiseDeferred.promise.then(function () {
                        countrySelectedPromiseDeferred = undefined;
                    });
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }


        function setTitle() {
            if (isEditMode) {
                if (swapDealOutboundEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealOutboundEntity.Name, 'Buying Part', $scope);
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Buying Part');
        }

        function loadStaticData() {
            if (swapDealOutboundEntity == undefined)
                return;
            $scope.scopeModel.name = swapDealOutboundEntity.Name;
            $scope.scopeModel.volume = swapDealOutboundEntity.Volume;
            $scope.scopeModel.rate = swapDealOutboundEntity.Rate;
            $scope.scopeModel.extraVolumeRate = swapDealOutboundEntity.ExtraVolumeRate;
            $scope.scopeModel.fixedRate = swapDealOutboundEntity.FixedRate;
        }

        function insertSwapDealOutbound() {
            $scope.scopeModel.isLoading = true;

            var swapDealOutboundObject = buildSwapDealOutboundObjFromScope();
            if ($scope.onSwapDealOutboundAdded != undefined) {
                $scope.onSwapDealOutboundAdded(swapDealOutboundObject);
                context.GetSwapDealsAboveCapacity();
            }
            $scope.modalContext.closeModal();
        }

        function updateSwapDealOutbound() {
            var swapDealOutboundObject = buildSwapDealOutboundObjFromScope();
            if ($scope.onSwapDealOutboundUpdated != undefined) {
                $scope.onSwapDealOutboundUpdated(swapDealOutboundObject);
                context.GetSwapDealsAboveCapacity();
            }
            $scope.modalContext.closeModal();
        }

        function buildSwapDealOutboundObjFromScope() {
            var supplierZones = [];
            var zoneIds = supplierZoneDirectiveAPI.getSelectedIds();
            for (var j = 0; j < zoneIds.length; j++) {
                supplierZones.push(
                    {
                        ZoneId: zoneIds[j]
                    });
            }
            var obj = {
                Name: $scope.scopeModel.name,
                SupplierZones: supplierZones,
                Volume: $scope.scopeModel.volume,
                ExtraVolumeRate: $scope.scopeModel.extraVolumeRate,
                Rate: $scope.scopeModel.rate,
                CountryIds: countryDirectiveApi.getSelectedIds(),
                SubstituteRateType: substituteRateTypeApi.getSelectedIds(),
                FixedRate: $scope.scopeModel.fixedRate != undefined ? $scope.scopeModel.fixedRate : undefined
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealOutboundEditorController', SwapDealOutboundEditorController);

})(appControllers);
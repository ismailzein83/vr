(function (appControllers) {

    'use strict';

    SwapDealInboundEditorController.$inject = ['$scope', 'UtilsService', 'UISettingsService', 'Whs_Deal_SubstituteRateTypeEnum','VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealInboundEditorController($scope, UtilsService, UISettingsService, Whs_Deal_SubstituteRateTypeEnum, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;
        var context;
        var swapDealInboundEntity;
        var sellingNumberPlanId;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var substituteRateTypeApi;
        var substituteRateTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountId;

        var countrySelectedPromiseDeferred;
        var saleZoneSelectorPayload;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingNumberPlanId = parameters.sellingNumberPlanId;
                swapDealInboundEntity = parameters.swapDealInbound;
                context = parameters.context;
                saleZoneSelectorPayload = parameters.context != undefined ? parameters.context.getSaleZoneSelectorPayload() : undefined;
            }

            isEditMode = (swapDealInboundEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.FixedRateValue = Whs_Deal_SubstituteRateTypeEnum.FixedRate.value;

            $scope.longPrecision = UISettingsService.getLongPrecision();

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwapDealInbound() : insertSwapDealInbound();
            };
            var swapDealCurrency = context.getSwapDealCurrency();
            $scope.scopeModel.currency = swapDealCurrency != undefined ? swapDealCurrency.Symbol : undefined;
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.onCountrySelectionChanged = function () {
                var country = countryDirectiveApi.getSelectedIds();

                if (country != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSelector = value; };
                    var payload = context != undefined ? context.getSaleZoneSelectorPayload(swapDealInboundEntity != undefined ? swapDealInboundEntity : undefined) : undefined;
                    if (payload != undefined) {
                        payload.sellingNumberPlanId = sellingNumberPlanId;
                        payload.filter.CountryIds = countryDirectiveApi.getSelectedIds();
                        payload.selectedIds = swapDealInboundEntity != undefined ? swapDealInboundEntity.SaleZoneIds : undefined;
                    }
                    else
                        payload = {
                            sellingNumberPlanId: sellingNumberPlanId,
                            filter: {
                                CountryIds: countryDirectiveApi.getSelectedIds(),
                            },
                            selectedIds: swapDealInboundEntity != undefined ? swapDealInboundEntity.SaleZoneIds : undefined,
                        };
                    console.log("get selected ids of country selector");
                    console.log(countryDirectiveApi.getSelectedIds());
                    //console.log(payload.excludedZoneIds);
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);

                }
                else if (saleZoneDirectiveAPI != undefined)
                    $scope.salezones.length = 0;
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();

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
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                loadAllControls();
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySaleZoneSection, loadSubstituteRateTypeSelector]).then(function () {
                swapDealInboundEntity = undefined;
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
                    selectedIds: swapDealInboundEntity != undefined ? swapDealInboundEntity.SubstituteRateType : Whs_Deal_SubstituteRateTypeEnum.NormalRate.value
                };
                VRUIUtilsService.callDirectiveLoad(substituteRateTypeApi, payload, loadSubstituteRateTypePromiseDeferred);
            });
        }

        function loadCountrySaleZoneSection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload = {};
            payload.filter = {
                Filters: [{
                    $type: 'TOne.WhS.BusinessEntity.Business.CountrySoldToCustomerFilter,TOne.WhS.BusinessEntity.Business',
                    CustomerId: context.getCarrierAccountId(),
                    EffectiveOn: context.getDealBED(),
                    IsEffectiveInFuture: false
                }]
            };
            if (swapDealInboundEntity != undefined && swapDealInboundEntity.CountryId != undefined) {
                payload.selectedIds = swapDealInboundEntity != undefined ? swapDealInboundEntity.CountryId : undefined;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });



            if (swapDealInboundEntity != undefined && swapDealInboundEntity.CountryId != undefined) {
                var loadSalesZonesPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadSalesZonesPromiseDeferred.promise);

                UtilsService.waitMultiplePromises([saleZoneReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var zoneIds = undefined;
                    if (swapDealInboundEntity != undefined) {
                        zoneIds = [];
                        for (var i = 0; i < swapDealInboundEntity.SaleZones.length; i++) {
                            zoneIds.push(swapDealInboundEntity.SaleZones[i].ZoneId);
                        }
                    }

                    var payload = context != undefined ? context.getSaleZoneSelectorPayload(swapDealInboundEntity != undefined ? swapDealInboundEntity : undefined) : undefined;
                    if (payload != undefined) {
                        payload.sellingNumberPlanId = sellingNumberPlanId;
                        payload.filter.CountryIds = [swapDealInboundEntity.CountryId];
                        payload.selectedIds = zoneIds;

                    }
                    else
                        payload = {
                            sellingNumberPlanId: sellingNumberPlanId,
                            filter: { CountryIds: [swapDealInboundEntity.CountryId] },
                            selectedIds: zoneIds,
                        };
                    // console.log(salezonePayload.excludedZoneIds);

                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSalesZonesPromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function setTitle() {
            if (isEditMode) {
                if (swapDealInboundEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealInboundEntity.Name, 'Selling Part', $scope);
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Selling Part');
        }

        function loadStaticData() {
            if (swapDealInboundEntity == undefined)
                return;
            $scope.scopeModel.name = swapDealInboundEntity.Name;
            $scope.scopeModel.volume = swapDealInboundEntity.Volume;
            $scope.scopeModel.rate = swapDealInboundEntity.Rate;
            $scope.scopeModel.extraVolumeRate = swapDealInboundEntity.ExtraVolumeRate;
            $scope.scopeModel.fixedRate = swapDealInboundEntity.FixedRate;
        }

        function insertSwapDealInbound() {
            $scope.scopeModel.isLoading = true;

            var swapDealInboundObject = buildSwapDealInboundObjFromScope();
            if ($scope.onSwapDealInboundAdded != undefined)
                $scope.onSwapDealInboundAdded(swapDealInboundObject);
            $scope.modalContext.closeModal();
        }

        function updateSwapDealInbound() {
            var swapDealInboundObject = buildSwapDealInboundObjFromScope();

            if ($scope.onSwapDealInboundUpdated != undefined)
                $scope.onSwapDealInboundUpdated(swapDealInboundObject);
            $scope.modalContext.closeModal();
        }

        function buildSwapDealInboundObjFromScope() {
            var saleZones = [];
            var zoneIds = saleZoneDirectiveAPI.getSelectedIds();
            for (var j = 0; j < zoneIds.length; j++) {
                saleZones.push(
                {
                    ZoneId: zoneIds[j]
                });
            }
            var obj = {
                Name: $scope.scopeModel.name,
                SaleZones: saleZones,
                Volume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
                ExtraVolumeRate: $scope.scopeModel.extraVolumeRate,
                CountryId: countryDirectiveApi.getSelectedIds(),
                SubstituteRateType: substituteRateTypeApi.getSelectedIds(),
                FixedRate: $scope.scopeModel.fixedRate != undefined ? $scope.scopeModel.fixedRate : undefined
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealInboundEditorController', SwapDealInboundEditorController);

})(appControllers);
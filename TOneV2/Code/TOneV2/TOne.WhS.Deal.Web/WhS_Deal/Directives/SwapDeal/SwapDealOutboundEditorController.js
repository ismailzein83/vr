(function (appControllers) {

    'use strict';

    SwapDealOutboundEditorController.$inject = ['$scope', 'UtilsService','UISettingsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealOutboundEditorController($scope, UtilsService, UISettingsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
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

            $scope.longPrecision = UISettingsService.getLongPrecision();

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwapDealOutbound() : insertSwapDealOutbound();
            };

            var swapDealCurrency = context.getSwapDealCurrency();
            $scope.scopeModel.currency = swapDealCurrency != undefined ? swapDealCurrency.Symbol: undefined;

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
            };

            $scope.onCountrySelectionChanged = function () {
                var country = countryDirectiveApi.getSelectedIds();
                if (country != undefined) {
                    var zoneIds = undefined;
                    if (swapDealOutboundEntity != undefined) {
                        zoneIds = [];
                        for (var i = 0; i < swapDealOutboundEntity.SupplierZones.length; i++) {
                            zoneIds.push(swapDealOutboundEntity.SupplierZones[i].ZoneId);
                        }
                    }
                    var setLoader = function (value) { $scope.isLoadingSelector = value; };
                    var payload = context != undefined ? context.getSupplierZoneSelectorPayload(swapDealOutboundEntity != undefined ? swapDealOutboundEntity : undefined) : undefined;
                    if (payload != undefined) {
                        payload.supplierId = supplierId;
                        payload.filter.CountryIds = [countryDirectiveApi.getSelectedIds()];
                        payload.selectedIds = zoneIds;
                    }
                    else {
                        payload = {
                            supplierId: supplierId,
                            filter: { CountryIds: [countryDirectiveApi.getSelectedIds()] },
                            selectedIds: zoneIds,
                        };

                    }
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);

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
                swapDealOutboundEntity = undefined;
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
                    selectedIds: swapDealOutboundEntity != undefined ? swapDealOutboundEntity.SubstituteRateType : undefined
                }
                VRUIUtilsService.callDirectiveLoad(substituteRateTypeApi, payload, loadSubstituteRateTypePromiseDeferred);
            });
        }

        function loadCountrySupplierZoneSection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload;

            if (swapDealOutboundEntity != undefined && swapDealOutboundEntity.CountryId != undefined) {
                payload = {};
                payload.selectedIds = swapDealOutboundEntity != undefined ? swapDealOutboundEntity.CountryId : undefined;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });



            if (swapDealOutboundEntity != undefined && swapDealOutboundEntity.CountryId != undefined) {
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
                        payload.filter.CountryIds = [swapDealOutboundEntity.CountryId];
                        payload.selectedIds = zoneIds;

                    }
                    else {
                        payload = {
                            supplierId: supplierId,
                            filter: { CountryIds: [swapDealOutboundEntity.CountryId] },
                            selectedIds: zoneIds
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(supplierZoneDirectiveAPI, payload, loadSupplierZonePromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
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
            if ($scope.onSwapDealOutboundAdded != undefined)
                $scope.onSwapDealOutboundAdded(swapDealOutboundObject);
            $scope.modalContext.closeModal();
        }

        function updateSwapDealOutbound() {
            var swapDealOutboundObject = buildSwapDealOutboundObjFromScope();
            if ($scope.onSwapDealOutboundUpdated != undefined)
                $scope.onSwapDealOutboundUpdated(swapDealOutboundObject);
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
                CountryId: countryDirectiveApi.getSelectedIds(),
                SubstituteRateType: substituteRateTypeApi.getSelectedIds(),
                FixedRate: $scope.scopeModel.fixedRate != undefined ? $scope.scopeModel.fixedRate : undefined
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealOutboundEditorController', SwapDealOutboundEditorController);

})(appControllers);
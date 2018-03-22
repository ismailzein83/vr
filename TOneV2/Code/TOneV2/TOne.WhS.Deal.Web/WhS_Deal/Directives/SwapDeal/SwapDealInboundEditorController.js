(function (appControllers) {

    'use strict';

    SwapDealInboundEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealInboundEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var swapDealInboundEntity;
        var sellingNumberPlanId;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var rateEvaluatorSelectiveDirectiveAPI;
        var rateEvaluatorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySelectedPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingNumberPlanId = parameters.sellingNumberPlanId;
                swapDealInboundEntity = parameters.swapDealInbound;
            }

            isEditMode = (swapDealInboundEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwapDealInbound() : insertSwapDealInbound();
            };

            $scope.scopeModel.onrateEvaluatorSelectiveReady = function (api) {
                rateEvaluatorSelectiveDirectiveAPI = api;
                rateEvaluatorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.onCountrySelectionChanged = function () {
                var country = countryDirectiveApi.getSelectedIds();
                if (country != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSelector = value };
                    var payload = {
                        sellingNumberPlanId: sellingNumberPlanId,
                        filter: { CountryIds: [countryDirectiveApi.getSelectedIds()] },
                        selectedIds: swapDealInboundEntity != undefined ? swapDealInboundEntity.SaleZoneIds : undefined

                    };
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySaleZoneSection, loadRateEvaluatorSelectiveDirective]).then(function () {
                swapDealInboundEntity = undefined;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadRateEvaluatorSelectiveDirective() {
            var loadREWSelectiveDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            rateEvaluatorReadyPromiseDeferred.promise.then(function () {

                var payload = undefined;
                if (swapDealInboundEntity != undefined && swapDealInboundEntity.EvaluatedRate != undefined)
                    payload =
                    {
                        evaluatedRate: swapDealInboundEntity.EvaluatedRate
                    };
                VRUIUtilsService.callDirectiveLoad(rateEvaluatorSelectiveDirectiveAPI, payload, loadREWSelectiveDirectivePromiseDeferred);
            });
            return loadREWSelectiveDirectivePromiseDeferred.promise;
        }

        function loadCountrySaleZoneSection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload;

            if (swapDealInboundEntity != undefined && swapDealInboundEntity.CountryId != undefined) {
                payload = {};
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
                    var salezonePayload = {
                        sellingNumberPlanId: sellingNumberPlanId,
                        filter: { CountryIds: [swapDealInboundEntity.CountryId] },
                        selectedIds: zoneIds
                    };

                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, salezonePayload, loadSalesZonesPromiseDeferred);
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
            // $scope.scopeModel.rate = swapDealInboundEntity.Rate;

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
                EvaluatedRate: rateEvaluatorSelectiveDirectiveAPI.getData(),
                CountryId: countryDirectiveApi.getSelectedIds()
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealInboundEditorController', SwapDealInboundEditorController);

})(appControllers);
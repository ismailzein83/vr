(function (app) {

    'use strict';

    SwapDealBuyRouteRuleSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Deal_SwapDealAPIService'];

    function SwapDealBuyRouteRuleSettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService, WhS_Deal_SwapDealAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SwapDealBuyRouteRuleSettingsCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/RouteRules/Templates/SwapDealBuyRouteRuleSettingsTemplate.html'
        };

        function SwapDealBuyRouteRuleSettingsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var supplierZoneSelectorAPI;
            var supplierZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var swapDealBuyRouteRuleExtendedSettingsDirectiveAPI;
            var swapDealBuyRouteRuleExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSupplierZoneSelectorReady = function (api) {
                    supplierZoneSelectorAPI = api;
                    supplierZoneSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onSwapDealBuyRouteRuleExtendedSettingsDirectiveReady = function (api) {
                    swapDealBuyRouteRuleExtendedSettingsDirectiveAPI = api;
                    swapDealBuyRouteRuleExtendedSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var swapDealId;

                    var swapDealBuyRouteRuleSettings;
                    var swapDealBuyRouteRuleContext;

                    if (payload != undefined) {
                        swapDealId = payload.swapDealId;
                        swapDealBuyRouteRuleSettings = payload.swapDealBuyRouteRuleSettings;
                        swapDealBuyRouteRuleContext = payload.swapDealBuyRouteRuleContext;
                    }

                    var loadSwapDealSettingsDirectivePromiseDeffered = UtilsService.createPromiseDeferred();

                    WhS_Deal_SwapDealAPIService.GetSwapDealSettingsDetail(swapDealId).then(function (response) {
                        var swapDealSettingsDetail = response;

                        var promises = [];

                        //Loading SupplierZone selector 
                        var supplierZoneSelectorLoadPromise = getSupplierZoneSelectorLoadPromise(swapDealSettingsDetail);
                        promises.push(supplierZoneSelectorLoadPromise);

                        //Loading ExtendedSettings directive
                        var swapDealBuyRouteRuleExtendedSettingsDirectiveLoadPromise = getSwapDealBuyRouteRuleExtendedSettingsDirectiveLoadPromise(swapDealSettingsDetail);
                        promises.push(swapDealBuyRouteRuleExtendedSettingsDirectiveLoadPromise);


                        UtilsService.waitMultiplePromises(promises).then(function () {
                            loadSwapDealSettingsDirectivePromiseDeffered.resolve();
                        });
                    });


                    function getSupplierZoneSelectorLoadPromise(swapDealSettingsDetail) {
                        var loadSupplierZoneSelectorDeferred = UtilsService.createPromiseDeferred();

                        if (swapDealBuyRouteRuleContext != undefined && swapDealBuyRouteRuleContext.setTimeSettings != undefined && typeof (swapDealBuyRouteRuleContext.setTimeSettings) == 'function') {
                            swapDealBuyRouteRuleContext.setTimeSettings(swapDealSettingsDetail.BED, swapDealSettingsDetail.EED);
                        }

                        supplierZoneSelectorReadyDeferred.promise.then(function () {

                            var supplierZoneSelectorPayload = {
                                availableZoneIds: swapDealSettingsDetail != undefined ? swapDealSettingsDetail.SupplierZoneIds : undefined,
                                supplierId: swapDealSettingsDetail != undefined ? swapDealSettingsDetail.CarrierAccountId : undefined
                            };
                            if (swapDealBuyRouteRuleSettings != undefined) {
                                supplierZoneSelectorPayload.selectedIds = swapDealBuyRouteRuleSettings.SupplierZoneIds;
                            }
                            VRUIUtilsService.callDirectiveLoad(supplierZoneSelectorAPI, supplierZoneSelectorPayload, loadSupplierZoneSelectorDeferred);
                        });

                        return loadSupplierZoneSelectorDeferred.promise;
                    }
                    function getSwapDealBuyRouteRuleExtendedSettingsDirectiveLoadPromise(swapDealSettingsDetail) {
                        var swapDealBuyRouteRuleExtendedSettingsDirectiveDeferred = UtilsService.createPromiseDeferred();

                        swapDealBuyRouteRuleExtendedSettingsDirectiveReadyDeferred.promise.then(function () {

                            var swapDealBuyRouteRuleExtendedSettingsDirectivePayload = {
                                swapDealId: swapDealId,
                                carrierAccountId: swapDealSettingsDetail != undefined ? swapDealSettingsDetail.CarrierAccountId : undefined
                            };
                            if (swapDealBuyRouteRuleSettings != undefined) {
                                swapDealBuyRouteRuleExtendedSettingsDirectivePayload.swapDealBuyRouteRuleExtendedSettings = swapDealBuyRouteRuleSettings.ExtendedSettings;
                            }
                            VRUIUtilsService.callDirectiveLoad(swapDealBuyRouteRuleExtendedSettingsDirectiveAPI, swapDealBuyRouteRuleExtendedSettingsDirectivePayload, swapDealBuyRouteRuleExtendedSettingsDirectiveDeferred);
                        });

                        return swapDealBuyRouteRuleExtendedSettingsDirectiveDeferred.promise;
                    }

                    return loadSwapDealSettingsDirectivePromiseDeffered.promise;
                };

                api.getData = function () {
                    return {
                        SupplierZoneIds: supplierZoneSelectorAPI.getSelectedIds(),
                        ExtendedSettings: swapDealBuyRouteRuleExtendedSettingsDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealSwapdealBuyrouteruleSettings', SwapDealBuyRouteRuleSettingsDirective);

})(app);
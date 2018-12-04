'use strict';

app.directive('vrWhsDealBuyrouteruleFixed', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new fixedBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'controller',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/RouteRules/MainExtensions/Templates/FixedBuyRouteRuleExtendedSettingsTemplate.html'
        };

        function fixedBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var carrierAccountSelectionChangedPromiseDeferred;

            var saleZoneSelectorAPI;
            var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var parentDealId;
            var parentDealBED;

            var sellingNumberPlanId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCarrierAccountDirectiveSelectionChanged = function (selectedCarrier) {
                    if (selectedCarrier == undefined)
                        return;
                    sellingNumberPlanId = selectedCarrier.SellingNumberPlanId;
                    if (carrierAccountSelectionChangedPromiseDeferred != undefined)
                        carrierAccountSelectionChangedPromiseDeferred.resolve();
                    onSellingNumberPlanChanged(sellingNumberPlanId);
                };

                $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorAPI = api;
                    saleZoneSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var customerId;
                    var parentCarrierAccountId;
                    var dealBuyRouteRuleExtendedSettings;
                    var saleZoneIds;
                    
                    if (payload != undefined) {
                        parentDealId = payload.dealId;
                        parentDealBED = payload.dealBED;
                        parentCarrierAccountId = payload.carrierAccountId;
                        dealBuyRouteRuleExtendedSettings = payload.dealBuyRouteRuleExtendedSettings;

                        if (dealBuyRouteRuleExtendedSettings != undefined) {
                            customerId = dealBuyRouteRuleExtendedSettings.CustomerId;
                            $scope.scopeModel.percentage = dealBuyRouteRuleExtendedSettings.Percentage;
                            saleZoneIds = dealBuyRouteRuleExtendedSettings.SaleZoneIds;
                        }
                    }

                    //loading CarrierAccount selector
                    var carrierAccountLoadPromiseDeferred = getCarrierAccountLoadPromiseDeferred();
                    promises.push(carrierAccountLoadPromiseDeferred);

                    if (saleZoneIds != undefined) {
                        carrierAccountSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();
                        carrierAccountSelectionChangedPromiseDeferred.promise.then(function () {
                            var loadSaleZoneSelectorPromiseDefferred = loadSaleZoneSelector(sellingNumberPlanId, saleZoneIds);
                            promises.push(loadSaleZoneSelectorPromiseDefferred);
                        });
                    }

                    function getCarrierAccountLoadPromiseDeferred() {
                        var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountPayload = {
                                selectedIds: customerId != undefined ? customerId : undefined,
                                filter: {
                                    ExcludedCarrierAccountIds: [parentCarrierAccountId]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, carrierAccountPayload, loadCarrierAccountPromiseDeferred);
                        });

                        return loadCarrierAccountPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.RouteRules.FixedDealBuyRouteRule, TOne.WhS.Deal.MainExtensions",
                        CustomerId: carrierAccountDirectiveAPI.getSelectedIds(),
                        SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
                        Percentage: $scope.scopeModel.percentage
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function onSellingNumberPlanChanged(sellingNumberPlanId) {
                $scope.scopeModel.showSaleZoneSelector = true;
                loadSaleZoneSelector(sellingNumberPlanId);
            }

            function loadSaleZoneSelector(sellingNumberPlanId, saleZoneIds) {
                var loadSaleZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            
                saleZoneSelectorReadyDeferred.promise.then(function () {
                    var saleZoneSelectorPayload = {
                        filter: { Filters: getSaleZoneSelectorFilters() },
                        sellingNumberPlanId : sellingNumberPlanId,
                        selectedIds : saleZoneIds
                    };

                    VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, loadSaleZoneSelectorLoadDeferred);
                });
                return loadSaleZoneSelectorLoadDeferred.promise;
            }

            function getSaleZoneSelectorFilters() {
                var saleZoneSelectorFilters = [];

                var saleZoneMatchingSupplierDealFilter = {
                    $type: "TOne.WhS.Routing.MainExtensions.SaleZoneMatchingSupplierDealFilter,TOne.WhS.Routing.MainExtensions",
                    SupplierDealId: parentDealId
                };
                saleZoneSelectorFilters.push(saleZoneMatchingSupplierDealFilter);

                var saleZoneSoldCountryToCusomerFilter = {
                    $type: "TOne.WhS.BusinessEntity.Business.SaleZoneSoldCountryToCustomerFilter,TOne.WhS.BusinessEntity.Business",
                    CustomerId: carrierAccountDirectiveAPI.getSelectedIds(),
                    EffectiveOn: parentDealBED,
                    IsEffectiveInFuture: false
                };
                saleZoneSelectorFilters.push(saleZoneSoldCountryToCusomerFilter);

                return saleZoneSelectorFilters;
            }
        }
    }]);

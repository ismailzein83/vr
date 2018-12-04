'use strict';

app.directive('vrWhsDealBuyrouteruleBysaledeal', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new bySaleDealBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'controller',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/RouteRules/MainExtensions/Templates/BySaleDealBuyRouteRuleExtendedSettingsTemplate.html'
        };

        function bySaleDealBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dealDefinitionSelectorAPI;
            var dealDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneSelectorAPI;
            var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var parentDealId;
            var sellingNumberPlanId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDealDefinitionSelectorReady = function (api) {
                    dealDefinitionSelectorAPI = api;
                    dealDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDealDefinitionSelectionChanged = function (selectedDeal) {
                    if (selectedDeal == undefined)
                        return;

                    sellingNumberPlanId = selectedDeal.SellingNumberPlanId;
                    onSellingNumberPlanChanged(sellingNumberPlanId);
                };

                $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorAPI = api;
                    saleZoneSelectorReadyDeferred.resolve();
                };
            }

            defineAPI();

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var selectedDealId;
                    var saleZoneIds;

                    if (payload != undefined) {
                        var dealBuyRouteRuleExtendedSettings = payload.dealBuyRouteRuleExtendedSettings;
                        parentDealId = payload.dealId;

                        if (dealBuyRouteRuleExtendedSettings != undefined) {
                            selectedDealId = dealBuyRouteRuleExtendedSettings.DealId;
                            $scope.scopeModel.percentage = dealBuyRouteRuleExtendedSettings.Percentage;
                            saleZoneIds = dealBuyRouteRuleExtendedSettings.SaleZoneIds;
                        }
                    }

                    //loading SaleDeal selector
                    var dealDefinitionSelectorLoadPromise = getDealDefinitionSelectorLoadPromise();
                    promises.push(dealDefinitionSelectorLoadPromise);

                    if (saleZoneIds != undefined) {
                        var loadSaleZoneSelectorPromiseDefferred = loadSaleZoneSelector(sellingNumberPlanId, saleZoneIds);
                        promises.push(loadSaleZoneSelectorPromiseDefferred);
                    }

                    function getDealDefinitionSelectorLoadPromise() {
                        var dealDefinitionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dealDefinitionSelectorReadyPromiseDeferred.promise.then(function () {

                            var dealDefinitionSelectorPayload = {
                                filter: {
                                    ExcludedDealDefinitionIds: [parentDealId],
                                    Filters: [{
                                        $type: "TOne.WhS.Deal.MainExtensions.DealDefinitionFilter.SaleDealFilter, TOne.WhS.Deal.MainExtensions"
                                    }]
                                }
                            };

                            if (selectedDealId != undefined)
                                dealDefinitionSelectorPayload.selectedIds = selectedDealId;

                            VRUIUtilsService.callDirectiveLoad(dealDefinitionSelectorAPI, dealDefinitionSelectorPayload, dealDefinitionSelectorLoadPromiseDeferred);
                        });

                        return dealDefinitionSelectorLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () { });
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.RouteRules.BySaleDealBuyRouteRule, TOne.WhS.Deal.MainExtensions",
                        DealId: dealDefinitionSelectorAPI.getSelectedIds(),
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
                        sellingNumberPlanId: sellingNumberPlanId,
                        selectedIds: saleZoneIds
                    };

                    VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, loadSaleZoneSelectorLoadDeferred);
                });
                return loadSaleZoneSelectorLoadDeferred.promise;
            }

            function getSaleZoneSelectorFilters() {
                var saleZoneSelectorFilters = [];

                var saleZoneMatchingSupplierDealFilter = {
                    $type: "TOne.WhS.Routing.MainExtensions.SaleZoneMatchingSupplierDealFilter,TOne.WhS.Routing.MainExtensions",
                    SupplierDealId: parentDealId,
                };
                saleZoneSelectorFilters.push(saleZoneMatchingSupplierDealFilter);

                var dealSaleZoneFilter = {
                    $type: "TOne.WhS.Deal.MainExtensions.DealSaleZonesFilter,TOne.WhS.Deal.MainExtensions",
                    DealId: dealDefinitionSelectorAPI.getSelectedIds()
                };
                saleZoneSelectorFilters.push(dealSaleZoneFilter);

                return saleZoneSelectorFilters;
            }
        }
    }]);

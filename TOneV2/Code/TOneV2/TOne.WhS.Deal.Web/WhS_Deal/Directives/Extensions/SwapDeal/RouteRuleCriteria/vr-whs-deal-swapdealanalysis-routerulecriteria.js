'use strict';

app.directive('vrWhsDealSwapdealanalysisRouterulecriteria', ['UtilsService', 'VRUIUtilsService', 'WhS_Deal_SwapDealAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Deal_SwapDealAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RouteRuleCriteriaCtor(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDeal/RouteRuleCriteria/Templates/SwapDealRouteRuleCriteriaDirective.html';
            }
        };

        function RouteRuleCriteriaCtor(ctrl, $scope) {
            var isLinkedRouteRule;
            var routingProductId;
            var routeRuleCriteria;
            var sellingNumberPlanId;
            var availableZoneIds;
            var dealDefinitionSelectorAPI;
            var dealDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dealDefinitionSelectionChangedPromiseDeferred;
            var defaultCriteriaValues;
            var routeRuleCriteriaContext;

            var saleZoneSelectorAPI;
            var saleZoneSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDealDefinitionSelectorReady = function (api) {
                    dealDefinitionSelectorAPI = api;
                    dealDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDealDefinitionSelectionChanged = function (selectedItem) {
                    var selectedDealDefinitionId = dealDefinitionSelectorAPI.getSelectedIds();

                    if (selectedItem != undefined) {

                        WhS_Deal_SwapDealAPIService.GetSwapDealSettingsDetail(selectedDealDefinitionId).then(function (response) {
                            sellingNumberPlanId = response.SellingNumberPlanId;
                            availableZoneIds = response.SaleZoneIds;
                            if (routeRuleCriteriaContext != undefined && routeRuleCriteriaContext.setTimeSettings != undefined && typeof (routeRuleCriteriaContext.setTimeSettings) == 'function') {
                                routeRuleCriteriaContext.setTimeSettings(response.BED, response.EED);
                            }
                            if (dealDefinitionSelectionChangedPromiseDeferred != undefined) {
                                dealDefinitionSelectionChangedPromiseDeferred.resolve();
                            }
                            else {
                                var saleZoneSelectorPayload = { sellingNumberPlanId: sellingNumberPlanId, availableZoneIds: availableZoneIds };

                                var setLoader = function (value) { $scope.scopeModel.isLoadingSaleZone = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneSelectorAPI, saleZoneSelectorPayload, setLoader, undefined);
                            }
                        });
                    }
                };

                $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorAPI = api;
                    saleZoneSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    $scope.scopeModel.disableCriteria = isLinkedRouteRule = payload.isLinkedRouteRule;
                    routingProductId = payload.routingProductId;
                    routeRuleCriteria = payload.routeRuleCriteria;
                    defaultCriteriaValues = payload.defaultCriteriaValues;
                    routeRuleCriteriaContext = payload.routeRuleCriteriaContext;

                    var dealDefinitionLoadPromise = loadDealDefinitionSelectorPromise();
                    promises.push(dealDefinitionLoadPromise);


                    var saleZoneLoadPromise = loadSaleZoneSelectorPromise();
                    if (saleZoneLoadPromise != undefined)
                        promises.push(saleZoneLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                function loadDealDefinitionSelectorPromise() {
                    if (routeRuleCriteria != undefined && routeRuleCriteria.SwapDealId != undefined)
                        dealDefinitionSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                    var dealDefinitionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    dealDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                        var dealDefinitionSelectorPayload = { filter: { Filters: [] } };
                        var swapDealDefinitionFilter = {
                            $type: "TOne.WhS.Deal.MainExtensions.SwapDeal.SwapDealDefinitionFilter, TOne.WhS.Deal.MainExtensions"
                        };
                        dealDefinitionSelectorPayload.filter.Filters.push(swapDealDefinitionFilter);

                        if (routeRuleCriteria != undefined)
                            dealDefinitionSelectorPayload.selectedIds = routeRuleCriteria.SwapDealId;
                        else
                            dealDefinitionSelectorPayload.selectedIds = defaultCriteriaValues != undefined ? defaultCriteriaValues.swapDealId : undefined;

                        VRUIUtilsService.callDirectiveLoad(dealDefinitionSelectorAPI, dealDefinitionSelectorPayload, dealDefinitionSelectorLoadPromiseDeferred);
                    });

                    return dealDefinitionSelectorLoadPromiseDeferred.promise;
                }

                function loadSaleZoneSelectorPromise() {
                    if (routeRuleCriteria == undefined || routeRuleCriteria.SwapDealId == undefined)
                        return;

                    var saleZoneSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([saleZoneSelectorReadyPromiseDeferred.promise, dealDefinitionSelectionChangedPromiseDeferred.promise]).then(function () {
                        dealDefinitionSelectionChangedPromiseDeferred = undefined;

                        var saleZoneSelectorPayload = { sellingNumberPlanId: sellingNumberPlanId, availableZoneIds: availableZoneIds };

                        if (routeRuleCriteria != undefined)
                            saleZoneSelectorPayload.selectedIds = routeRuleCriteria.ZoneIds;
                        VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, saleZoneSelectorLoadPromiseDeferred);
                    });

                    return saleZoneSelectorLoadPromiseDeferred.promise;
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.SwapDeal.RouteRuleCriteria.SwapDealRouteRuleCriteria, TOne.WhS.Deal.MainExtensions",
                        SwapDealId: dealDefinitionSelectorAPI.getSelectedIds(),
                        ZoneIds: saleZoneSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
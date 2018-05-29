'use strict';

app.directive('vrWhsDealSwapdealBuyrouteruleBysaledeal', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/RouteRules/MainExtensions/Templates/BySaleDealBuyRouteRuleExtendedSettingsTemplate.html'
        };

        function bySaleDealBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dealDefinitionSelectorAPI;
            var dealDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDealDefinitionSelectorReady = function (api) {
                    dealDefinitionSelectorAPI = api;
                    dealDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var selectedSwapDealId;
                    var parentSwapDealId;

                    if (payload != undefined) {
                        var swapDealBuyRouteRuleExtendedSettings = payload.swapDealBuyRouteRuleExtendedSettings;
                        parentSwapDealId = payload.swapDealId;

                        if (swapDealBuyRouteRuleExtendedSettings != undefined) {
                            selectedSwapDealId = swapDealBuyRouteRuleExtendedSettings.SwapDealId;
                            $scope.scopeModel.percentage = swapDealBuyRouteRuleExtendedSettings.Percentage;
                        }
                    }

                    //loading CarrierAccount selector
                    var dealDefinitionSelectorLoadPromise = getDealDefinitionSelectorLoadPromise();
                    promises.push(dealDefinitionSelectorLoadPromise);


                    function getDealDefinitionSelectorLoadPromise() {
                        var dealDefinitionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dealDefinitionSelectorReadyPromiseDeferred.promise.then(function () {

                            var dealDefinitionSelectorPayload = {
                                filter: {
                                    ExcludedDealDefinitionIds: [parentSwapDealId],
                                    Filters: [{
                                        $type: "TOne.WhS.Deal.MainExtensions.SwapDeal.SwapDealDefinitionFilter, TOne.WhS.Deal.MainExtensions"
                                    }]
                                }
                            };

                            if (selectedSwapDealId != undefined)
                                dealDefinitionSelectorPayload.selectedIds = selectedSwapDealId;

                            VRUIUtilsService.callDirectiveLoad(dealDefinitionSelectorAPI, dealDefinitionSelectorPayload, dealDefinitionSelectorLoadPromiseDeferred);
                        });

                        return dealDefinitionSelectorLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.SwapDealBuyRouteRules.BySaleDealSwapDealBuyRouteRule, TOne.WhS.Deal.MainExtensions",
                        SwapDealId: dealDefinitionSelectorAPI.getSelectedIds(),
                        Percentage: $scope.scopeModel.percentage
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

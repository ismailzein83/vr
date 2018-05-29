'use strict';

app.directive('vrWhsDealSwapdealBuyrouteruleFixed', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/RouteRules/MainExtensions/Templates/FixedBuyRouteRuleExtendedSettingsTemplate.html'
        };

        function fixedBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var customerId;
                    var parentCarrierAccountId;

                    if (payload != undefined) {
                        parentCarrierAccountId = payload.carrierAccountId;
                        var swapDealBuyRouteRuleExtendedSettings = payload.swapDealBuyRouteRuleExtendedSettings;

                        if (swapDealBuyRouteRuleExtendedSettings != undefined) {
                            customerId = swapDealBuyRouteRuleExtendedSettings.CustomerId;
                            $scope.scopeModel.percentage = swapDealBuyRouteRuleExtendedSettings.Percentage;
                        }
                    }

                    //loading CarrierAccount selector
                    var carrierAccountLoadPromiseDeferred = getCarrierAccountLoadPromiseDeferred();
                    promises.push(carrierAccountLoadPromiseDeferred);


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
                        $type: "TOne.WhS.Deal.MainExtensions.SwapDealBuyRouteRules.FixedSwapDealBuyRouteRule, TOne.WhS.Deal.MainExtensions",
                        CustomerId: carrierAccountDirectiveAPI.getSelectedIds(),
                        Percentage: $scope.scopeModel.percentage
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

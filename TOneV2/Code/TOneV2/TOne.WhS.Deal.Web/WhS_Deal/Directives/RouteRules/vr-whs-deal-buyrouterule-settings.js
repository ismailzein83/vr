(function (app) {

    'use strict';

    DealBuyRouteRuleSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_Deal_BuyRouteRuleAPIService'];

    function DealBuyRouteRuleSettingsDirective(UtilsService, VRUIUtilsService, WhS_Deal_BuyRouteRuleAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DealBuyRouteRuleSettingsCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/WhS_Deal/Directives/RouteRules/Templates/DealBuyRouteRuleSettingsTemplate.html'
        };

        function DealBuyRouteRuleSettingsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var dealBuyRouteRuleExtendedSettingsDirectiveAPI;
            var dealBuyRouteRuleExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDealBuyRouteRuleExtendedSettingsDirectiveReady = function (api) {
                    dealBuyRouteRuleExtendedSettingsDirectiveAPI = api;
                    dealBuyRouteRuleExtendedSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var dealId;
                    var dealBED;
                    var dealBuyRouteRuleSettings;

                    if (payload != undefined) {
                        dealId = payload.dealId;
                        dealBED = payload.dealBED;
                        dealBuyRouteRuleSettings = payload.dealBuyRouteRuleSettings;
                    }

                    var promises = [];

                    //Loading ExtendedSettings directive
                    var dealBuyRouteRuleExtendedSettingsDirectiveLoadPromise = getDealBuyRouteRuleExtendedSettingsDirectiveLoadPromise();
                    promises.push(dealBuyRouteRuleExtendedSettingsDirectiveLoadPromise);

                    UtilsService.waitMultiplePromises(promises);
                  
                    function getDealBuyRouteRuleExtendedSettingsDirectiveLoadPromise() {
                        var dealBuyRouteRuleExtendedSettingsDirectiveDeferred = UtilsService.createPromiseDeferred();

                        dealBuyRouteRuleExtendedSettingsDirectiveReadyDeferred.promise.then(function () {

                            WhS_Deal_BuyRouteRuleAPIService.GetCarrierAccountId(dealId).then(function (response) {
                                var dealBuyRouteRuleExtendedSettingsDirectivePayload = {
                                    dealId: dealId,
                                    dealBED: dealBED,
                                    carrierAccountId: response
                                };
                                if (dealBuyRouteRuleSettings != undefined) {
                                    dealBuyRouteRuleExtendedSettingsDirectivePayload.dealBuyRouteRuleExtendedSettings = dealBuyRouteRuleSettings.ExtendedSettings;
                                }
                                VRUIUtilsService.callDirectiveLoad(dealBuyRouteRuleExtendedSettingsDirectiveAPI, dealBuyRouteRuleExtendedSettingsDirectivePayload, dealBuyRouteRuleExtendedSettingsDirectiveDeferred);
                            });
                        });

                        return dealBuyRouteRuleExtendedSettingsDirectiveDeferred.promise;
                    }
                };

                api.getData = function () {
                    return {
                        ExtendedSettings: dealBuyRouteRuleExtendedSettingsDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealBuyrouteruleSettings', DealBuyRouteRuleSettingsDirective);

})(app);
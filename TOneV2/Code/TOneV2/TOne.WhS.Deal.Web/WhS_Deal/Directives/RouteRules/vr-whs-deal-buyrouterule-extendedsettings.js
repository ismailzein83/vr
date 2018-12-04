(function (app) {

    'use strict';

    DealBuyRouteRuleExtendedSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_Deal_BuyRouteRuleAPIService'];

    function DealBuyRouteRuleExtendedSettingsDirective(UtilsService, VRUIUtilsService, WhS_Deal_BuyRouteRuleAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DealBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "controller",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/RouteRules/Templates/DealBuyRouteRuleExtendedSettingsTemplate.html'
        };

        function DealBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dealId;
            var dealBED;
            var carrierAccountId;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    $scope.scopeModel.showSaleZoneSelector = false;
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };

                    var directivePayload = {
                        dealId: dealId,
                        dealBED: dealBED,
                        carrierAccountId: carrierAccountId,
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var dealBuyRouteRuleExtendedSettings;

                    if (payload != undefined) {
                        dealId = payload.dealId;
                        dealBED = payload.dealBED;
                        carrierAccountId = payload.carrierAccountId;
                        dealBuyRouteRuleExtendedSettings = payload.dealBuyRouteRuleExtendedSettings;
                    }

                    var getDealBuyRouteRuleExtendedSettingsConfigsPromise = GetDealBuyRouteRuleExtendedSettingsConfigsPromise();
                    promises.push(getDealBuyRouteRuleExtendedSettingsConfigsPromise);

                    if (dealBuyRouteRuleExtendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function GetDealBuyRouteRuleExtendedSettingsConfigsPromise() {
                        return WhS_Deal_BuyRouteRuleAPIService.GetDealBuyRouteRuleExtendedSettingsConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (dealBuyRouteRuleExtendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, dealBuyRouteRuleExtendedSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                dealId: dealId,
                                dealBED: dealBED,
                                carrierAccountId: carrierAccountId,
                                dealBuyRouteRuleExtendedSettings: dealBuyRouteRuleExtendedSettings,
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealBuyrouteruleExtendedsettings', DealBuyRouteRuleExtendedSettingsDirective);
})(app);
(function (app) {

    'use strict';

    SwapDealBuyRouteRuleExtendedSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_Deal_SwapDealBuyRouteRuleAPIService'];

    function SwapDealBuyRouteRuleExtendedSettingsDirective(UtilsService, VRUIUtilsService, WhS_Deal_SwapDealBuyRouteRuleAPIService) {
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
                var ctor = new SwapDealBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "controller",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/RouteRules/Templates/SwapDealBuyRouteRuleExtendedSettingsTemplate.html'
        };

        function SwapDealBuyRouteRuleExtendedSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var swapDealId;

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
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        swapDealId: swapDealId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var swapDealBuyRouteRuleExtendedSettings;

                    if (payload != undefined) {
                        swapDealId = payload.swapDealId;
                        swapDealBuyRouteRuleExtendedSettings = payload.swapDealBuyRouteRuleExtendedSettings;
                    }

                    var getSwapDealBuyRouteRuleExtendedSettingsConfigsPromise = GetSwapDealBuyRouteRuleExtendedSettingsConfigsPromise();
                    promises.push(getSwapDealBuyRouteRuleExtendedSettingsConfigsPromise);

                    if (swapDealBuyRouteRuleExtendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function GetSwapDealBuyRouteRuleExtendedSettingsConfigsPromise() {
                        return WhS_Deal_SwapDealBuyRouteRuleAPIService.GetSwapDealBuyRouteRuleExtendedSettingsConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (swapDealBuyRouteRuleExtendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, swapDealBuyRouteRuleExtendedSettings.ConfigId, 'ExtensionConfigurationId');
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
                                swapDealId: swapDealId,
                                swapDealBuyRouteRuleExtendedSettings: swapDealBuyRouteRuleExtendedSettings
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

    app.directive('vrWhsDealSwapdealBuyrouteruleExtendedsettings', SwapDealBuyRouteRuleExtendedSettingsDirective);

})(app);
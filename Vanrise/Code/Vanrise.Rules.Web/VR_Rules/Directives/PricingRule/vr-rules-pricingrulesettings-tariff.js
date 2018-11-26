"use strict";

app.directive('vrRulesPricingrulesettingsTariff', ['$compile', 'VR_Rules_PricingRuleAPIService', 'UtilsService', 'VRUIUtilsService',
    function ($compile, VR_Rules_PricingRuleAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new bePricingRuleTariffSetting(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Rules/Directives/PricingRule/Templates/PricingRuleTariffSettings.html"
        };

        function bePricingRuleTariffSetting(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var currencyDirectiveAPI;
            var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred;

            function initializeController() {
                $scope.pricingRuleTariffSettings = [];

                ctrl.onCurrencySelectReady = function (api) {
                    currencyDirectiveAPI = api;
                    currencyDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onPricingRuleTariffTemplateDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveReadyAPI, undefined, setLoader, directiveReadyPromiseDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    var loadCurrencySelectorPromise = loadCurrencySelector();
                    promises.push(loadCurrencySelectorPromise);

                    var loadTariffTemplatesPromise = loadTariffTemplates();
                    promises.push(loadTariffTemplatesPromise);

                    if (settings != undefined) {
                        directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function loadCurrencySelector() {
                        var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyDirectiveReadyPromiseDeferred.promise.then(function () {

                            var currencyPayload;
                            if (settings != undefined && settings.CurrencyId > 0) {
                                currencyPayload = { selectedIds: settings.CurrencyId };
                            }
                            else {
                                currencyPayload = { selectSystemCurrency: true };
                            }
                            VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred);
                        });

                        return loadCurrencySelectorPromiseDeferred.promise;
                    }
                    function loadTariffTemplates() {
                        return VR_Rules_PricingRuleAPIService.GetPricingRuleTariffTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.pricingRuleTariffSettings.push(response[i]);
                                }

                                if (settings != undefined) {
                                    $scope.selectedPricingRuleTariffSettings = UtilsService.getItemByVal($scope.pricingRuleTariffSettings, settings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyPromiseDeferred.promise.then(function () {
                            directiveReadyPromiseDeferred = undefined;

                            VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, settings, directiveLoadPromiseDeferred);
                        });

                        return directiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = directiveReadyAPI.getData();
                    obj.ConfigId = $scope.selectedPricingRuleTariffSettings.ExtensionConfigurationId;
                    obj.CurrencyId = currencyDirectiveAPI.getSelectedIds();
                    return obj;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
(function (app) {

    'use strict';

    RateValueSettingsDirective.$inject = ['VR_Rules_PricingRuleAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RateValueSettingsDirective(VR_Rules_PricingRuleAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var rateValueSettings = new RateValueSettings(ctrl, $scope, $attrs);
                rateValueSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Rules/Directives/PricingRule/Templates/PricingRuleRateValueSettings.html"
        };

        function RateValueSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            //var selectorAPI;

            var currencyDirectiveAPI;
            var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();



            function initializeController() {
                $scope.rateValueTemplates = [];
                $scope.selectedRateValueTemplate;

                $scope.onCurrencySelectReady = function (api) {
                    currencyDirectiveAPI = api;
                    currencyDirectiveReadyPromiseDeferred.resolve();
                };

                //$scope.onSelectorReady = function (api) {
                //    selectorAPI = api;

                //};

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.rateValueTemplates.length = 0;

                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    var currencySelectorLoadPromise = getCurrencySelectorLoadPromise();
                    promises.push(currencySelectorLoadPromise);

                    var loadTemplatesPromise = loadTemplates();
                    promises.push(loadTemplatesPromise);

                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    loadTemplatesPromise.then(function () {
                        if (settings != undefined) {
                            $scope.selectedRateValueTemplate = UtilsService.getItemByVal($scope.rateValueTemplates, settings.ConfigId, 'ExtensionConfigurationId');
                        }
                        else if ($scope.rateValueTemplates.length > 0)
                            $scope.selectedRateValueTemplate = $scope.rateValueTemplates[0];
                    });

                    function getCurrencySelectorLoadPromise()
                    {
                        var currencySelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var currencyPayload;
                        if (settings != undefined && settings.CurrencyId > 0) {
                            currencyPayload = { selectedIds: settings.CurrencyId };
                        }
                        else {
                            currencyPayload = { selectSystemCurrency: true };
                        }

                        currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, currencySelectorLoadPromiseDeferred);

                        });

                        return currencySelectorLoadPromiseDeferred.promise;
                    }
                    function loadTemplates() {
                        return VR_Rules_PricingRuleAPIService.GetPricingRuleRateValueTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.rateValueTemplates.push(response[i]);
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { settings: settings };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = directiveAPI.getData();
                    obj.ConfigId = $scope.selectedRateValueTemplate.ExtensionConfigurationId;
                    obj.CurrencyId = currencyDirectiveAPI.getSelectedIds();
                    return obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrRulesPricingrulesettingsRatevalue', RateValueSettingsDirective);

})(app);
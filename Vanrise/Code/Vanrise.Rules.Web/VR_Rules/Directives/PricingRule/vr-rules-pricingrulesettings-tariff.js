'use strict';
app.directive('vrRulesPricingrulesettingsTariff', ['$compile', 'VR_Rules_PricingRuleAPIService', 'UtilsService', 'VRUIUtilsService',
function ($compile, VR_Rules_PricingRuleAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
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
        var directiveReadyAPI;
        var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.pricingRuleTariffSettings = [];

            $scope.onPricingRuleTariffTemplateDirectiveReady = function (api) {
                directiveReadyAPI = api;
                var setLoader = function (value) { $scope.isLoadingDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveReadyAPI, undefined, setLoader, directiveReadyPromiseDeferred);
            }
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = directiveReadyAPI.getData();
                obj.ConfigId = $scope.selectedPricingRuleTariffSettings.TemplateConfigID;
                return obj;
            }

            api.load = function (payload) {
                var promises = [];

                var settings;

                if (payload != undefined) {
                    settings = payload.settings;
                }
                
                var loadTariffTemplatesPromise = loadTariffTemplates();
                promises.push(loadTariffTemplatesPromise);

                var loadDirectivePromise = loadDirective();
                promises.push(loadDirectivePromise);

                loadTariffTemplatesPromise.then(function () {
                    if (settings != undefined)
                        $scope.selectedPricingRuleTariffSettings = UtilsService.getItemByVal($scope.pricingRuleTariffSettings, settings.ConfigId, 'TemplateConfigID');
                    else if ($scope.pricingRuleTariffSettings.length > 0)
                        $scope.selectedPricingRuleTariffSettings = $scope.pricingRuleTariffSettings[0];
                });

                function loadTariffTemplates() {
                    return VR_Rules_PricingRuleAPIService.GetPricingRuleTariffTemplates().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.pricingRuleTariffSettings.push(response[i]);
                            }
                        }
                    })
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
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);
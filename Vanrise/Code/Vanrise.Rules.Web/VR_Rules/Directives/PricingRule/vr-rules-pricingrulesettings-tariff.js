﻿'use strict';
app.directive('vrRulesPricingrulesettingsTariff', ['$compile', 'VR_Rules_PricingRuleAPIService', 'UtilsService', 'VRUIUtilsService',
function ($compile, VR_Rules_PricingRuleAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            $scope.pricingRuleTariffSettings = [];
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
        var directiveReadyPromiseDeferred;
        function initializeController() {
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
                var configId;
                if (payload != undefined) {
                    configId = payload.ConfigId;

                }
                var promises = [];
                var loadTariffTemplatesPromiseDeferred = VR_Rules_PricingRuleAPIService.GetPricingRuleTariffTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.pricingRuleTariffSettings.push(itm);
                    });
                    if (configId != undefined) {
                        for (var j = 0; j < $scope.pricingRuleTariffSettings.length; j++)
                            if (configId == $scope.pricingRuleTariffSettings[j].TemplateConfigID)
                                $scope.selectedPricingRuleTariffSettings = $scope.pricingRuleTariffSettings[j];
                    }
                });
                promises.push(loadTariffTemplatesPromiseDeferred);
                if (payload != undefined) {
                    directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        directiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, payload, directiveLoadPromiseDeferred);
                    });
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
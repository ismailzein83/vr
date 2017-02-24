"use strict";

app.directive("vrWhsSalesPercentagecostcalculation", ["WhS_Routing_RoutRuleSettingsAPIService", "UtilsService", "VRUIUtilsService", function (WhS_Routing_RoutRuleSettingsAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var percentageCostCalculation = new PercentageCostCalculation(ctrl, $scope);
            percentageCostCalculation.initializeController();
        },
        controllerAs: "customPercentageCtrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/CostCalculation/Templates/PercentageCostCalculationTemplate.html"
    };
    function PercentageCostCalculation(ctrl, $scope) {
        this.initializeController = initializeController;

        var selectorAPI;

        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {
            $scope.extensionConfigs = [];

            $scope.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.onDirectiveReady = function (api) {
                directiveAPI = api;
                var directivePayload = undefined;
                var setDirectiveLoader = function (directiveLoaderValue) { $scope.isLoadingDirective = directiveLoaderValue; };
                return VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setDirectiveLoader, directiveReadyDeferred);
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var percentageSettings;

                if (payload != undefined) {
                    $scope.title = payload.Title;
                    percentageSettings = payload.PercentageSettings;
                }

                var promises = [];

                var loadExtensionConfigsPromise = loadExtensionConfigs();
                promises.push(loadExtensionConfigsPromise);

                if (percentageSettings != undefined) {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }

                function loadExtensionConfigs() {
                    return WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionPercentageSettingsTemplates().then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.extensionConfigs.push(response[i]);
                            }
                            if (percentageSettings != undefined)
                                $scope.selectedExtensionConfig = UtilsService.getItemByVal($scope.extensionConfigs, percentageSettings.ConfigId, 'ExtensionConfigurationId');
                        }
                    });
                }
                function loadDirective() {
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, percentageSettings, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data = {
                    $type: "TOne.WhS.Sales.MainExtensions.CostCalculation.PercentageCostCalculation, TOne.WhS.Sales.MainExtensions",
                    Title: $scope.title
                };
                var percentageSettings = directiveAPI.getData();
                if (percentageSettings != undefined) {
                    percentageSettings.ConfigId = $scope.selectedExtensionConfig.ExtensionConfigurationId;
                }
                data.PercentageSettings = percentageSettings;
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
"use strict";

app.directive("vrWhsSalesPercentagecostcalculation", ["WhS_Routing_RoutRuleSettingsAPIService", "UtilsService", "VRUIUtilsService", function (WhS_Routing_RoutRuleSettingsAPIService, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var percentageCostCalculation = new PercentageCostCalculation(ctrl, $scope);
            percentageCostCalculation.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/CostCalculation/Templates/PercentageCostCalculationTemplate.html"
    };

    function PercentageCostCalculation(ctrl, $scope) {
        this.initCtrl = initCtrl;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();
        var directivePayload;

        function initCtrl()
        {
            $scope.title;
            $scope.templates = [];
            $scope.selectedTemplate;

            $scope.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.title = payload.Title;
                    directivePayload = payload.PercentageSettings;
                    $scope.selectedTemplate = payload.SelectedTemplate;
                }

                var promises = [];

                var getTemplatesPromise = getTemplates();
                promises.push(getTemplatesPromise);

                if (directivePayload) {
                    directiveReadyDeferred.promise.then(function () {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(directiveLoadDeferred.promise);

                        $scope.isLoadingDirective = true;
                        directiveLoadDeferred.promise.finally(function () { $scope.isLoadingDirective = false; });

                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });
                }

                function getTemplates() {
                    return WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionPercentageSettingsTemplates().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.templates.push(response[i]);
                            }
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.CostCalculation.PercentageCostCalculation, TOne.WhS.Sales.MainExtensions",
                    Title: $scope.title,
                    SelectedTemplate: $scope.selectedTemplate,
                    PercentageSettings: directiveAPI ? directiveAPI.getData() : null
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);

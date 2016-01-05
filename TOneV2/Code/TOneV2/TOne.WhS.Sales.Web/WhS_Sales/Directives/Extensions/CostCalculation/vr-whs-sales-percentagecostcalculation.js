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

        function initCtrl() {
            defineScope();

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getAPI());

            function defineScope() {
                $scope.title;
                $scope.templates = [];
                $scope.selectedTemplate;

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };
            }
            function getAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload) {
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

                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Sales.Entities.CostCalculation.Extensions.PercentageCostCalculation, TOne.WhS.Sales.Entities",
                        Title: $scope.title,
                        SelectedTemplate: $scope.selectedTemplate,
                        PercentageSettings: directiveAPI.getData()
                    };
                };

                return api;
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
        }
    }
}]);

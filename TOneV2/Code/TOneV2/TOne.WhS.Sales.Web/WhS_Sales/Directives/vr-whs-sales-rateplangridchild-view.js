"use strict";

app.directive("vrWhsSalesRateplangridchildView", ["UtilsService", function (UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope) {
            var ctrl = this;

            var ratePlanChildView = new RatePlanGridChildView($scope, ctrl);
            ratePlanChildView.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "Client/Modules/WhS_Sales/Directives/Templates/RatePlanGridChildViewTemplate.html"
    };

    function RatePlanGridChildView($scope, ctrl) {
        this.initializeController = initializeController;

        function initializeController() {
            var routingProductSelectorAPI;
            var routingProductSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            defineChoices();
            load();

            $scope.onChoiceSelectionChanged = function () {
                if ($scope.selectedChoiceIndex != undefined) {
                    $scope.choices[$scope.selectedChoiceIndex].isLoaded = true;
                }
            };
            
            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                ctrl.onReady(buildAPI());
            }

            function buildAPI() {
                var api = {};

                api.load = function (payload) {
                    var routingProductSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    routingProductSelectorReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            filter: {},
                            selectedIds: null
                        };

                        VRUIUtilsService.callDirectiveLoad(routingProductSelectorAPI, payload, routingProductSelectorLoadPromiseDeferred);
                    });

                };

                return api;
            }

            function load() {

            }

            function defineChoices() {
                $scope.choices = [
                    { title: "Routing Product", editor: "vr-whs-be-routingproduct-selector", isLoaded: true, isSelected: true }
                ];
            }
        }
    }

}]);

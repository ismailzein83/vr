"use strict";

app.directive("vrWhsSalesTqimethodSuggestedpercentages", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var suggestedPercentagesTQIMethod = new SuggestedPercentagesTQIMethod(ctrl, $scope);
            suggestedPercentagesTQIMethod.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/TQIMethods/Templates/SuggestedAverageTQIMethodTemplate.html"
    };

    function SuggestedPercentagesTQIMethod(ctrl, $scope) {
        this.initializeController = initializeController;

        $scope.suggestedPercentages = [];

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.rpRouteDetail != undefined) {
                    for (var i = 0; i < payload.rpRouteDetail.RouteOptionsDetails.length; i++) {
                        $scope.suggestedPercentages.push(payload.rpRouteDetail.RouteOptionsDetails[i]);
                    }
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.SuggestedPercentagesTQIMethod, TOne.WhS.Sales.MainExtensions",
                    SuppliersPercentages: getSuggestedPercentages()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getSuggestedPercentages() {
            var suppliersPercentages = [];

            if ($scope.suggestedPercentages != undefined) {
                for (var i = 0; i < $scope.suggestedPercentages.length; i++)
                {
                    var suggestedPercentage = $scope.suggestedPercentages[i];
                    suppliersPercentages.push({ SupplierName: suggestedPercentage.SupplierName, MarginPercentage: suggestedPercentage.marginPercentage });
                }
            }

            return suppliersPercentages;
        }
    }
}]);

"use strict";

app.directive("vrWhsSalesTqimethodFirstoption", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var firstOptionTQIMethod = new FirstOptionTQIMethod(ctrl, $scope);
            firstOptionTQIMethod.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/TQIMethods/Templates/FirstOptionTQIMethodTemplate.html"
    };

    function FirstOptionTQIMethod(ctrl, $scope) {
        this.initializeController = initializeController;

        function initializeController() {
            ctrl.title;
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.rpRouteDetail != undefined) {
                    ctrl.firstOption = payload.rpRouteDetail.RouteOptionsDetails[0].ConvertedSupplierRate;
                }
               
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.FirstOptionTQIMethod, TOne.WhS.Sales.MainExtensions",
                    FirstOption: ctrl.firstOption
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);

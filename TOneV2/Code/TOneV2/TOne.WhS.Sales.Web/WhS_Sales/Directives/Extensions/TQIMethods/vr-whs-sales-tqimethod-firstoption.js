"use strict";

app.directive("vrWhsSalesTqimethodFirstoption", ['UtilsService', function (UtilsService) {

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

        var context;

        function initializeController() {
            ctrl.title;
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var rpRouteDetail;

                if (payload != undefined) {
                    rpRouteDetail = payload.rpRouteDetail;
                    context = payload.context;
                }

                if (rpRouteDetail != undefined && rpRouteDetail.RouteOptionsDetails != null && rpRouteDetail.RouteOptionsDetails.length > 0) {
                    var firstOptionValue = payload.rpRouteDetail.RouteOptionsDetails[0].ConvertedSupplierRate;

                    if (firstOptionValue != undefined && context != undefined && context.longPrecision != undefined) {
                        firstOptionValue = UtilsService.round(firstOptionValue, context.longPrecision);
                    }

                    ctrl.firstOption = firstOptionValue;
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
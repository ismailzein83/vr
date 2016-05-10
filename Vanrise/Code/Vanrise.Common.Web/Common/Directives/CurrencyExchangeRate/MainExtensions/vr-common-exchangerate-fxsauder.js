"use strict";

app.directive("vrCommonExchangerateFxsauder", [function () {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Common/Directives/CurrencyExchangeRate/MainExtensions/Templates/ExchangeRateFxSauderTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.url = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                
                return {   
                    $type: "Vanrise.Common.Business.ExchangeRateTasks.ExchangeRateTaskActionArgument, Vanrise.Common.Business",
                    URL: $scope.url
                };
            };


            api.load = function (payload) {
                if (payload != undefined && payload.data != undefined) {
                    $scope.url = payload.data.URL;
                }
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);

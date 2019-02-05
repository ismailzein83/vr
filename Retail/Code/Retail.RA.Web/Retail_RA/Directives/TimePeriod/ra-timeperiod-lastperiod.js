"use strict";

app.directive("raTimeperiodLastperiod", [function () {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new DirectiveConstructor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return '/Client/Modules/Retail_RA/Directives/TimePeriod/Template/LastPeriodTemplate.html';
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "Retail.RA.Business.LastPeriodTimePeriod,Retail.RA.Business",
                };
            };

            api.load = function (payload) {
                if (payload != undefined) {
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);

"use strict";

app.directive("vrCommonTimeperiodYesterday", ['UtilsService', function (UtilsService) {

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
        return "/Client/Modules/Common/Directives/VRTimePeriod/MainExtensions/Templates/YesterdayDirectiveTemplate.html";
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
                    $type: "Vanrise.Common.MainExtensions.YesterdayTimePeriod, Vanrise.Common.MainExtensions"
                };
            };

            api.load = function (payload) {
                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != undefined)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);

﻿"use strict";

app.directive("vrCommonTimeperiodCurrentweek", [function () {

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
        return "/Client/Modules/Common/Directives/VRTimePeriod/MainExtensions/Templates/CurrentWeekDirectiveTemplate.html";
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
                    $type: "Vanrise.Common.MainExtensions.CurrentWeekTimePeriod, Vanrise.Common.MainExtensions"
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

"use strict";

app.directive("vrCommonTimeperiodLastDays", [function () {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new TimePeriodLastDays($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Common/Directives/VRTimePeriod/MainExtensions/Templates/LastDaysDirectiveTemplate.html";
    }

    function TimePeriodLastDays($scope, ctrl) {
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined)
                    $scope.scopeModel.numberOfDays = payload.timePeriod.NumberOfDaysBack;
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Common.MainExtensions.LastDaysPeriod, Vanrise.Common.MainExtensions",
                    NumberOfDaysBack:$scope.scopeModel.numberOfDays,
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);

"use strict";

app.directive("vrCommonTimeperiodSpecificdays", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                isrequired: '=',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimePeriodLastTime($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRTimePeriod/MainExtensions/Templates/SpecificDaysDirectiveTemplate.html"
        };

        function TimePeriodLastTime($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                    defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];


                    if (payload != undefined && payload.timePeriod != undefined) {
                        $scope.scopeModel.daysBack = payload.timePeriod.DaysBack;
                        $scope.scopeModel.numberOfDays = payload.timePeriod.NumberOfDays;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.SpecificDaysTimePeriod, Vanrise.Common.MainExtensions",
                        DaysBack: $scope.scopeModel.daysBack,
                        NumberOfDays: $scope.scopeModel.numberOfDays
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
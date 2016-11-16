"use strict";

app.directive("vrRuntimeTasktriggerInterval", ['UtilsService', 'VRUIUtilsService', 'TimeSchedulerTypeEnum', 'IntervalTimeTypeEnum',
function (UtilsService, VRUIUtilsService, TimeSchedulerTypeEnum, IntervalTimeTypeEnum) {

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
            };
        },
        templateUrl: "/Client/Modules/Runtime/Directives/TaskTrigger/Templates/TaskTriggerInterval.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        function initializeController() {
            $scope.intervalTypes = UtilsService.getArrayEnum(IntervalTimeTypeEnum);

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            
            api.getData = function () {
                return {
                    $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                    TimerTriggerTypeFQTN: TimeSchedulerTypeEnum.Interval.FQTN,
                    Interval: $scope.interval,
                    IntervalType: $scope.selectedIntervalType.value
                };

            };

            api.load = function (payload) {
                if (payload != undefined && payload.data != undefined) {
                    var data = payload.data;
                    $scope.interval = data.Interval;
                    $scope.selectedIntervalType = UtilsService.getItemByVal($scope.intervalTypes, data.IntervalType, "value");
                }
                else {
                    setToDefaultValues();
                }
            };

            function setToDefaultValues() {
                $scope.interval = "30";
                $scope.selectedIntervalType = UtilsService.getItemByVal($scope.intervalTypes, IntervalTimeTypeEnum.Minute.value, "value");
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);

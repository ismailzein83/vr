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
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Runtime/Directives/TimerTrigger/Templates/TaskTriggerInterval.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            $scope.intervalTypes = UtilsService.getArrayEnum(IntervalTimeTypeEnum);
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
               

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);

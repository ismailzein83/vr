"use strict";

app.directive("vrRuntimeTasktriggerWeekly", ['UtilsService', 'VRUIUtilsService','TimeSchedulerTypeEnum', 'DaysOfWeekEnum',
function (UtilsService, VRUIUtilsService, TimeSchedulerTypeEnum, DaysOfWeekEnum) {

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
        templateUrl: "/Client/Modules/Runtime/Directives/TaskTrigger/Templates/TaskTriggerWeekly.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        function initializeController() {

            $scope.daysOfWeek = UtilsService.getArrayEnum(DaysOfWeekEnum);

            $scope.selectedDays = [];
            $scope.selectedTimes = [];

            $scope.validate = function () {
                if ($scope.selectedTimes == undefined || $scope.selectedTimes.length == 0)
                    return 'At least one value should be added';
                return null;
            };

            $scope.addTime = function () {
                var timeIsValid = true;

                if ($scope.selectedTime == undefined || $scope.selectedTime.length == 0) {
                    timeIsValid = false;
                }
                else {
                    angular.forEach($scope.selectedTimes, function (item) {
                        //Time to be added should not be repeated in the list of selected times
                        if (UtilsService.compareEqualsTimes($scope.selectedTime, item)) {
                            timeIsValid = false;
                        }
                    });
                }

                if (timeIsValid) {
                    $scope.selectedTimes.push($scope.selectedTime);
                    $scope.selectedTime = undefined;
                }
            };

            $scope.removeTime = function (timeToRemove) {
                $scope.selectedTimes.splice($scope.selectedTimes.indexOf(timeToRemove), 1);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getData = function () {
                var numbersOfSelectedDays = [];
                angular.forEach($scope.selectedDays, function (item) {
                    numbersOfSelectedDays.push(item.value);
                });
                return {
                    $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.WeeklyTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                    TimerTriggerTypeFQTN: TimeSchedulerTypeEnum.Weekly.FQTN,
                    ScheduledDays: numbersOfSelectedDays,
                    ScheduledTimesToRun: $scope.selectedTimes
                };

            };

            api.load = function (payload) {
                if (payload != undefined && payload.data != undefined) {
                    var data = payload.data;

                    angular.forEach(data.ScheduledDays, function (item) {
                        var selectedDay = UtilsService.getItemByVal($scope.daysOfWeek, item, "value");
                        $scope.selectedDays.push(selectedDay);
                    });

                    angular.forEach(data.ScheduledTimesToRun, function (item) {
                        $scope.selectedTimes.push(item);
                    });
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);

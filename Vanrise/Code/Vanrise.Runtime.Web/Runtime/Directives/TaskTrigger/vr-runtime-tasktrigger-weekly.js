"use strict";

app.directive("vrRuntimeTasktriggerWeekly", ['UtilsService', 'TimeSchedulerTypeEnum', 'DaysOfWeekEnum',
    function (UtilsService, TimeSchedulerTypeEnum, DaysOfWeekEnum) {

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
            templateUrl: "/Client/Modules/Runtime/Directives/TaskTrigger/Templates/TaskTriggerWeekly.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.daysOfWeek = UtilsService.getArrayEnum(DaysOfWeekEnum);
                $scope.selectedDays = [];
                $scope.selectedTimes = [];

                $scope.validate = function () {
                    if ($scope.selectedTimes == undefined || $scope.selectedTimes.length == 0)
                        return 'At least one value should be added';
                    return null;
                };

                $scope.validateTime = function () {
                    if ($scope.selectedTime == undefined)
                        return false;

                    for (var i = 0; i < $scope.selectedTimes.length; i++) {
                        if (UtilsService.compareEqualsTimes($scope.selectedTimes[i], $scope.selectedTime)) {
                            return false;
                        }
                    }

                    return true;
                };

                $scope.addTime = function () {
                    $scope.selectedTimes.push($scope.selectedTime);
                    UtilsService.sortTimes($scope.selectedTimes);
                    $scope.selectedTime = undefined;
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

                        UtilsService.sortTimes($scope.selectedTimes);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
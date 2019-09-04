"use strict";

app.directive("vrRuntimeTasktriggerDaily", ['UtilsService', 'TimeSchedulerTypeEnum',
    function (UtilsService, TimeSchedulerTypeEnum) {

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
            templateUrl: "/Client/Modules/Runtime/Directives/TaskTrigger/Templates/TaskTriggerDaily.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.selectedTimes = [];
                $scope.timeButtonIsDisabled = false;

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

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        var data = payload.data;
                        angular.forEach(data.ScheduledTimesToRun, function (item) {
                            $scope.selectedTimes.push(item);
                        });
                        UtilsService.sortTimes($scope.selectedTimes);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.DailyTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                        TimerTriggerTypeFQTN: TimeSchedulerTypeEnum.Daily.FQTN,
                        ScheduledTimesToRun: $scope.selectedTimes
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
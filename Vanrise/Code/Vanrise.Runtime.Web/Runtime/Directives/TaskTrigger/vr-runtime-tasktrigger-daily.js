"use strict";

app.directive("vrRuntimeTasktriggerDaily", ['UtilsService', 'VRUIUtilsService', 'TimeSchedulerTypeEnum',
function (UtilsService, VRUIUtilsService , TimeSchedulerTypeEnum) {

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
        return "/Client/Modules/Runtime/Directives/TimerTrigger/Templates/TaskTriggerDaily.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            $scope.selectedTimes = [];
            $scope.timeButtonIsDisabled = false;
            $scope.addTime = function () {
                var timeIsValid = true;

                if ($scope.selectedTime == undefined || $scope.selectedTime.length == 0) {
                    timeIsValid = false;
                }
                else {
                    angular.forEach($scope.selectedTimes, function (item) {
                        //Time to be added should not be repeated in the list of selected times
                        if ($scope.selectedTime === item) {
                            timeIsValid = false;
                        }
                    });
                }

                if (timeIsValid)
                    $scope.selectedTimes.push($scope.selectedTime);
            }

            $scope.removeTime = function (timeToRemove) {
                $scope.selectedTimes.splice($scope.selectedTimes.indexOf(timeToRemove), 1);
            }
            api.getData = function () {
                return {
                    $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.DailyTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                    TimerTriggerTypeFQTN: TimeSchedulerTypeEnum.Daily.FQTN,
                    ScheduledTimesToRun: $scope.selectedTimes
                };
            };


            api.load = function (payload) {
                if (payload != undefined && payload.data != undefined) {
                    var data = payload.data;
                    angular.forEach(data.ScheduledTimesToRun, function (item) {
                        $scope.selectedTimes.push(item);
                    });
                }
                   
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);

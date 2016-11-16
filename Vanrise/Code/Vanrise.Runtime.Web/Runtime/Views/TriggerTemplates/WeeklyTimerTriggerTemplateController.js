WeeklyTimeTriggerTemplateController.$inject = ['$scope', 'TimeSchedulerTypeEnum', 'DaysOfWeekEnum', 'UtilsService'];

function WeeklyTimeTriggerTemplateController($scope, TimeSchedulerTypeEnum, DaysOfWeekEnum, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.daysOfWeek = [];

        $scope.selectedDays = [];
        $scope.selectedTimes = [];

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
        };

        $scope.removeTime = function (timeToRemove) {
            $scope.selectedTimes.splice($scope.selectedTimes.indexOf(timeToRemove), 1);
        };

        $scope.schedulerTypeTaskTrigger.getData = function () {
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

        $scope.schedulerTypeTaskTrigger.loadTemplateData = function () {
            loadForm();
        };
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.schedulerTypeTaskTrigger.data == undefined || isFormLoaded)
            return;

        var data = $scope.schedulerTypeTaskTrigger.data;
        if (data != null) {

            angular.forEach(data.ScheduledDays, function (item) {
                var selectedDay = UtilsService.getItemByVal($scope.daysOfWeek, item, "value");
                $scope.selectedDays.push(selectedDay);
            });

            angular.forEach(data.ScheduledTimesToRun, function (item) {
                $scope.selectedTimes.push(item);
            });
        }

        isFormLoaded = true;
    }

    function load() {
        $scope.daysOfWeek = UtilsService.getArrayEnum(DaysOfWeekEnum);
        loadForm();
    }
}
appControllers.controller('Runtime_WeeklyTimeTriggerTemplateController', WeeklyTimeTriggerTemplateController);

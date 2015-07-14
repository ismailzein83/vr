WeeklyTimeTriggerTemplateController.$inject = ['$scope', 'TimeSchedulerTypeEnum', 'DaysOfWeekEnum', 'UtilsService'];

function WeeklyTimeTriggerTemplateController($scope, TimeSchedulerTypeEnum, DaysOfWeekEnum, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.daysOfWeek = [];

        $scope.selectedDays = [];
        $scope.selectedHours = [];

        $scope.addHour = function () {
            $scope.selectedHours.push($scope.time);
        }

        $scope.removeHour = function (hourToRemove) {
            $scope.selectedHours.splice($scope.selectedHours.indexOf(hourToRemove), 1);
        }

        $scope.schedulerTypeTaskTrigger.getData = function () {
            var numbersOfSelectedDays = [];
            angular.forEach($scope.selectedDays, function (item) {
                numbersOfSelectedDays.push(item.value);
            });

            return {
                $type: "Vanrise.Runtime.Business.WeeklyTimeSchedulerTaskTrigger, Vanrise.Runtime.Business",
                SelectedType: TimeSchedulerTypeEnum.Weekly.value,
                ScheduledDays: numbersOfSelectedDays,
                ScheduledHours: $scope.selectedHours
            };
        };

        $scope.schedulerTypeTaskTrigger.loadTemplateData = function () {
            loadForm();
        }
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

            angular.forEach(data.ScheduledHours, function (item) {
                $scope.selectedHours.push(item);
            });
        }
        else {
            $scope.selectedDays = [];
            $scope.selectedHours = [];
        }
        isFormLoaded = true;
    }

    function load() {
        UtilsService.waitMultipleAsyncOperations([loadDaysOfWeek]).finally(function () {
            loadForm();
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function loadDaysOfWeek() {
        for (var prop in DaysOfWeekEnum) {
            $scope.daysOfWeek.push(DaysOfWeekEnum[prop]);
        }
    }
}
appControllers.controller('Runtime_WeeklyTimeTriggerTemplateController', WeeklyTimeTriggerTemplateController);

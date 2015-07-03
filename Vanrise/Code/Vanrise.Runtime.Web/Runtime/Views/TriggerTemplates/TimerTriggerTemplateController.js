TimeTriggerTemplateController.$inject = ['$scope', 'DaysOfWeekEnum', 'UtilsService'];

function TimeTriggerTemplateController($scope, DaysOfWeekEnum, UtilsService) {

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

        $scope.schedulerTaskTrigger.getData = function () {
            var numbersOfSelectedDays = [];
            angular.forEach($scope.selectedDays, function (item) {
                numbersOfSelectedDays.push(item.value);
            });

            return {
                $type: "Vanrise.Runtime.Business.TimeSchedulerTaskTrigger, Vanrise.Runtime.Business",
                ScheduledDays: numbersOfSelectedDays,
                ScheduledHours: $scope.selectedHours
            };
        };
    }

    function loadForm() {
        
        if ($scope.schedulerTaskTrigger.data == undefined)
            return;
        var data = $scope.schedulerTaskTrigger.data;
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
    }

    function load() {
        loadDaysOfWeek();

        loadForm();
    }

    function loadDaysOfWeek() {
        for (var prop in DaysOfWeekEnum) {
            $scope.daysOfWeek.push(DaysOfWeekEnum[prop]);
        }
    }
}
appControllers.controller('Runtime_TimeTriggerTemplateController', TimeTriggerTemplateController);

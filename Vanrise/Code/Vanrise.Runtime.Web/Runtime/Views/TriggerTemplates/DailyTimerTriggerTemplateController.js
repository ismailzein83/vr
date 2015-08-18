DailyTimeTriggerTemplateController.$inject = ['$scope', 'TimeSchedulerTypeEnum', 'UtilsService'];

function DailyTimeTriggerTemplateController($scope, TimeSchedulerTypeEnum, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.selectedHours = [];

        $scope.addHour = function () {
            $scope.selectedHours.push($scope.time);
        }

        $scope.removeHour = function (hourToRemove) {
            $scope.selectedHours.splice($scope.selectedHours.indexOf(hourToRemove), 1);
        }

        $scope.schedulerTypeTaskTrigger.getData = function () {
            return {
                $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.DailyTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                SelectedType: TimeSchedulerTypeEnum.Daily.value,
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
            $scope.selectedHours = [];
        }
        isFormLoaded = true;
    }

    function load() {
        loadForm();
    }
}
appControllers.controller('Runtime_DailyTimeTriggerTemplateController', DailyTimeTriggerTemplateController);

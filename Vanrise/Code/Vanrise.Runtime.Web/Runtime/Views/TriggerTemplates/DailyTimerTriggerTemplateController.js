DailyTimeTriggerTemplateController.$inject = ['$scope', 'TimeSchedulerTypeEnum', 'UtilsService'];

function DailyTimeTriggerTemplateController($scope, TimeSchedulerTypeEnum, UtilsService) {

    defineScope();
    load();

    function defineScope() {

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
        };

        $scope.removeTime = function (timeToRemove) {
            $scope.selectedTimes.splice($scope.selectedTimes.indexOf(timeToRemove), 1);
        };

        $scope.schedulerTypeTaskTrigger.getData = function () {
            return {
                $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.DailyTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                TimerTriggerTypeFQTN: TimeSchedulerTypeEnum.Daily.FQTN,
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
            angular.forEach(data.ScheduledTimesToRun, function (item) {
                $scope.selectedTimes.push(item);
            });
        }

        isFormLoaded = true;
    }

    function load() {
        loadForm();
    }   
}
appControllers.controller('Runtime_DailyTimeTriggerTemplateController', DailyTimeTriggerTemplateController);

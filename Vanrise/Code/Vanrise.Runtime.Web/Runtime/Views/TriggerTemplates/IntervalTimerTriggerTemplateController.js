IntervalTimeTriggerTemplateController.$inject = ['$scope', 'TimeSchedulerTypeEnum', 'IntervalTimeTypeEnum', 'UtilsService'];

function IntervalTimeTriggerTemplateController($scope, TimeSchedulerTypeEnum, IntervalTimeTypeEnum, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.interval = undefined;
        $scope.intervalTypes = [];
        $scope.selectedIntervalType = undefined;

        $scope.schedulerTypeTaskTrigger.getData = function () {
            return {
                $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                TimerTriggerTypeFQTN: TimeSchedulerTypeEnum.Interval.FQTN,
                Interval: $scope.interval,
                IntervalType: $scope.selectedIntervalType.value
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
                $scope.interval = data.Interval;
                $scope.selectedIntervalType = UtilsService.getItemByVal($scope.intervalTypes, data.IntervalType, "value");
            }

            isFormLoaded = true;
        
    }

    function setFormToDefault()
    {
        $scope.interval = "30";
        $scope.selectedIntervalType = UtilsService.getItemByVal($scope.intervalTypes, IntervalTimeTypeEnum.Minute.value, "value");
    }

    function load() {
        $scope.intervalTypes = UtilsService.getArrayEnum(IntervalTimeTypeEnum);
        setFormToDefault();
        loadForm();
    }
}
appControllers.controller('Runtime_IntervalTimeTriggerTemplateController', IntervalTimeTriggerTemplateController);

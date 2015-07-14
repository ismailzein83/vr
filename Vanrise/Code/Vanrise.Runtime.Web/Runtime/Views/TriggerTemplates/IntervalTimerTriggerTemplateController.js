IntervalTimeTriggerTemplateController.$inject = ['$scope', 'TimeSchedulerTypeEnum', 'IntervalTimeTypeEnum', 'UtilsService'];

function IntervalTimeTriggerTemplateController($scope, TimeSchedulerTypeEnum, IntervalTimeTypeEnum, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.interval = "1";
        $scope.intervalTypes = [];
        $scope.selectedIntervalType = undefined;

        $scope.schedulerTypeTaskTrigger.getData = function () {
            return {
                $type: "Vanrise.Runtime.Business.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Business",
                SelectedType: TimeSchedulerTypeEnum.Interval.value,
                Interval: $scope.interval,
                IntervalType: $scope.selectedIntervalType.value
            };
        };

        $scope.schedulerTypeTaskTrigger.loadTemplateData = function()
        {
            loadForm();
        }
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
        else {
            $scope.interval = "1";
            $scope.selectedIntervalType = undefined;
        }
        isFormLoaded = true;
    }

    function load() {
        UtilsService.waitMultipleAsyncOperations([loadIntervalTypes]).finally(function () {
            loadForm();
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function loadIntervalTypes() {
        for (var prop in IntervalTimeTypeEnum) {
            $scope.intervalTypes.push(IntervalTimeTypeEnum[prop]);
        }
    }
}
appControllers.controller('Runtime_IntervalTimeTriggerTemplateController', IntervalTimeTriggerTemplateController);

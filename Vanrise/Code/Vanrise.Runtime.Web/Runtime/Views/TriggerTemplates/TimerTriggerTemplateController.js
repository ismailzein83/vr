TimeTriggerTemplateController.$inject = ['$scope'];

function TimeTriggerTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        $scope.schedulerTaskTrigger.getData = function () {
            return {
                $type: "Vanrise.Runtime.Entities.TimeSchedulerTaskTrigger, Vanrise.Runtime.Entities",
                DateToRun:  $scope.dateToRun,
                TimeToRun: $scope.timeToRun
            };
        };

        loadForm();
    }

    function loadForm() {
        
        if ($scope.schedulerTaskTrigger.data == undefined)
            return;
        var data = $scope.schedulerTaskTrigger.data;
        if (data != null) {
            $scope.dateToRun = data.DateToRun;
            $scope.timeToRun = data.TimeToRun;
        }
        else {
            $scope.timeToRun = '';
            $scope.dateToRun = '';
        }
    }

    function load() {
    }

}
appControllers.controller('Runtime_TimeTriggerTemplateController', TimeTriggerTemplateController);

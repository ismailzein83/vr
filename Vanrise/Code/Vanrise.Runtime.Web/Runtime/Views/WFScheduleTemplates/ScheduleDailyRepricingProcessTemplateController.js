ScheduleDailyRepricingProcessTemplateController.$inject = ['$scope'];

function ScheduleDailyRepricingProcessTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        $scope.bpDefinitions = [];

        $scope.schedulerTaskAction.processInputArguments.getData = function () {
            return {
                $type: "TOne.CDRProcess.Arguments.DailyRepricingProcessInput, TOne.CDRProcess.Arguments",
                RepricingDay: $scope.repricingDay,
                DivideProcessIntoSubProcesses: $scope.divideProcessIntoSubProcesses
            };
        };

        loadForm();
    }

    function loadForm() {

        if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
            return;
        var data = $scope.schedulerTaskAction.processInputArguments.data;
        if (data != null) {
            $scope.repricingDay = data.RepricingDay;
            $scope.divideProcessIntoSubProcesses = data.DivideProcessIntoSubProcesses;
        }
        else {
            $scope.repricingDay = '';
            $scope.divideProcessIntoSubProcesses = '';
        }
    }

    function load() {

    }

}
appControllers.controller('Runtime_ScheduleDailyRepricingProcessTemplateController', ScheduleDailyRepricingProcessTemplateController);

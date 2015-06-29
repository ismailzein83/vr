ScheduleDailyRepricingProcessTemplateController.$inject = ['$scope'];

function ScheduleDailyRepricingProcessTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        $scope.bpDefinitions = [];

        $scope.schedulerTaskAction.baseProcessInputArgument.getData = function () {
            return {
                $type: "TOne.CDRProcess.Arguments.DailyRepricingProcessInput, TOne.CDRProcess.Arguments",
                RepricingDay: $scope.repricingDay,
                DivideProcessIntoSubProcesses: $scope.divideProcessIntoSubProcesses
            };
        };

        loadForm();
    }

    function loadForm() {

        if ($scope.schedulerTaskAction.baseProcessInputArgument.data == undefined)
            return;
        var data = $scope.schedulerTaskAction.baseProcessInputArgument.data;
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

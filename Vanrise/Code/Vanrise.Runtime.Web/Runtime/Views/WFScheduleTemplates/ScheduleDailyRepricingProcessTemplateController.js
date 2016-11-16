ScheduleDailyRepricingProcessTemplateController.$inject = ['$scope', 'UtilsService'];

function ScheduleDailyRepricingProcessTemplateController($scope, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.bpDefinitions = [];
        $scope.repricingDay = undefined;
        $scope.selectedDateOption = undefined;
        $scope.divideProcessIntoSubProcesses = undefined;

        $scope.dateOptions = [{ Name: "Trigger Date", Value: 0 }, { Name: "Specific Date", Value: 1 }];
        $scope.selectedDateOption = UtilsService.getItemByVal($scope.dateOptions, 0, "Value");

        $scope.schedulerTaskAction.processInputArguments.getData = function () {
            return {
                $type: "TOne.CDRProcess.Arguments.DailyRepricingProcessInput, TOne.CDRProcess.Arguments",
                RepricingDay: $scope.repricingDay,
                DivideProcessIntoSubProcesses: $scope.divideProcessIntoSubProcesses
            };
        };

        $scope.schedulerTaskAction.processInputArguments.loadTemplateData = function () {
            loadForm();
        };

        $scope.schedulerTaskAction.rawExpressions.getData = function () {
            if ($scope.selectedDateOption.Value == 0)
            {
                $scope.repricingDay = '';
                return { "RepricingDay": "ScheduleTime" };
            }
            else
                return undefined;
        };

        $scope.dateOptionSelected = function () {
            if ($scope.selectedDateOption.Value == 0) {
                $scope.specificDateOptionSelected = false;
                $scope.repricingDay = '';
            }
            else {
                $scope.specificDateOptionSelected = true;
            }

        };
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.schedulerTaskAction.processInputArguments.data == undefined || isFormLoaded)
            return;
        var data = $scope.schedulerTaskAction.processInputArguments.data;
        if (data != null) {
            $scope.repricingDay = data.RepricingDay;
            $scope.divideProcessIntoSubProcesses = data.DivideProcessIntoSubProcesses;

            var dateOptionSelection = ($scope.schedulerTaskAction.rawExpressions.data != null) ? 0 : 1;
            $scope.selectedDateOption = UtilsService.getItemByVal($scope.dateOptions, dateOptionSelection, "Value");
            
        }

        isFormLoaded = true;
    }

    function load() {
        loadForm();
    }

}
appControllers.controller('Runtime_ScheduleDailyRepricingProcessTemplateController', ScheduleDailyRepricingProcessTemplateController);

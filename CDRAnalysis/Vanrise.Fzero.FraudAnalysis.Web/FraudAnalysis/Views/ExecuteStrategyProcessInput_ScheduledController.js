var ExecuteStrategyProcessInput_Scheduled = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.processInputArguments = [];

        $scope.strategies = [];
        loadStrategies();
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];


        $scope.periods = [];
        loadPeriods();
        $scope.selectedPeriod = "";


        $scope.schedulerTaskAction.rawExpressions.getData = function () {
                return { "ScheduleTime": "ScheduleTime" };
        };


        $scope.schedulerTaskAction.processInputArguments.getData = function () {

            angular.forEach($scope.selectedStrategies, function (itm) {
                $scope.selectedStrategyIds.push(itm.id);
            });

            return {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                StrategyIds: $scope.selectedStrategyIds,
                PeriodId: $scope.selectedPeriod.Id
            };
        };


        loadForm();
    }


    function loadPeriods() {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
        });
    }


    function loadStrategies() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
        });
    }

    function loadForm() {
        console.log($scope.schedulerTaskAction.processInputArguments.data)
        if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
            return;
        var data = $scope.schedulerTaskAction.processInputArguments.data;
      
        if (data != null) {
            $scope.repricingDay = data.RepricingDay;
            $scope.divideProcessIntoSubProcesses = data.DivideProcessIntoSubProcesses;

            var dateOptionSelection = ($scope.schedulerTaskAction.rawExpressions.data != null) ? 0 : 1;
            $scope.selectedDateOption = UtilsService.getItemByVal($scope.dateOptions, dateOptionSelection, "Value");

        }
        else {
            $scope.repricingDay = '';
            $scope.selectedDateOption = undefined;
            $scope.divideProcessIntoSubProcesses = '';
        }
        
    }

    function load() {

    }



    

}

ExecuteStrategyProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInput_Scheduled', ExecuteStrategyProcessInput_Scheduled)




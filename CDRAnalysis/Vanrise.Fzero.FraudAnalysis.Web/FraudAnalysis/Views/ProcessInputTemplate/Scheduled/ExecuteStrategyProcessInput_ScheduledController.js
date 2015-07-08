var ExecuteStrategyProcessInput_Scheduled = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.processInputArguments = [];

        $scope.strategies = [];
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];


        $scope.periods = [];
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

   

    function load() {
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadStrategies, loadPeriods])
        .then(function () {
                if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
                    return;
                var data = $scope.schedulerTaskAction.processInputArguments.data;

                if (data != null) {
                    $scope.selectedPeriod = $scope.periods[UtilsService.getItemIndexByVal($scope.periods, data.PeriodId, "Id")]

                    angular.forEach(data.StrategyIds, function (strategyId) {
                        $scope.selectedStrategies.push($scope.strategies[UtilsService.getItemIndexByVal($scope.strategies, strategyId, "id")]);
                    });

                   
                }
                $scope.isGettingData = false;

        })
        .catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    

}

ExecuteStrategyProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInput_Scheduled', ExecuteStrategyProcessInput_Scheduled)




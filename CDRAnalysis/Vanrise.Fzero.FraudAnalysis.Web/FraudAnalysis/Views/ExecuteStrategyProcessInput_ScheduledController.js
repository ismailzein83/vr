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


    function loadPeriods(id) {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
            if(id!=undefined)
                $scope.selectedPeriod = $scope.periods[UtilsService.getItemIndexByVal($scope.periods, id, "Id")]
        });
    }


    function loadStrategies(id) {

        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
            if (id != undefined)
                $scope.selectedStrategyIds = $scope.strategies[UtilsService.getItemIndexByVal($scope.strategies, id, "Id")]
        });

    }

    function loadForm() {
       
        if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
            return;
        var data = $scope.schedulerTaskAction.processInputArguments.data;

        console.log('data')
        console.log(data)

        if (data != null) {
            loadPeriods(data.PeriodId)
            loadStrategies(data.StrategyIds)
        }
    }

    function load() {

    }



    

}

ExecuteStrategyProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInput_Scheduled', ExecuteStrategyProcessInput_Scheduled)




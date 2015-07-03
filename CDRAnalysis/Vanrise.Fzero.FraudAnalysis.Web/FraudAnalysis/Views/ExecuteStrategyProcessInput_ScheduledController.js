var ExecuteStrategyProcessInput_Scheduled = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.createProcessInputObjects = [];

        $scope.strategies = [];
        loadStrategies();
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];


        $scope.periods = [];
        loadPeriods();
        $scope.selectedPeriod = "";

        $scope.schedulerTaskAction.rawExpressions.getData = function () {
                return [{ "ScheduleTime": "ScheduleTime" }];
          
        };


        $scope.schedulerTaskAction.processInputArguments.getData = function () {

            angular.forEach($scope.selectedStrategies, function (itm) {
                $scope.selectedStrategyIds.push(itm.id);
            });

            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    StrategyIds: $scope.selectedStrategyIds,
                    PeriodId: $scope.selectedPeriod.Id
                }
            });

            return $scope.createProcessInputObjects;


        };


















        $scope.schedulerTaskAction.rawExpressions.getData = function () {
                return undefined;
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

        if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
            return;
    }

    function load() {

    }



    

}

ExecuteStrategyProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInput_Scheduled', ExecuteStrategyProcessInput_Scheduled)




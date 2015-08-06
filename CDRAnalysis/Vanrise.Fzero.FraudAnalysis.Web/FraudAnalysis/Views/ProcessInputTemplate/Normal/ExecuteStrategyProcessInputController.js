var ExecuteStrategyProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    var pageLoaded = false;

    defineScope();

    function defineScope() {

        $scope.createProcessInputObjects = [];

        $scope.strategies = [];
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];

        $scope.periods = [];
        loadPeriods();
        $scope.selectedPeriod = "";
        


        $scope.createProcessInput.getData = function () {

            angular.forEach($scope.selectedStrategies, function (itm) {
                $scope.selectedStrategyIds.push(itm.id);
            });


            var StartingDate = new Date($scope.fromDate);

            console.log(StartingDate.toString())

            $scope.createProcessInputObjects.length = 0;




            if ($scope.selectedPeriod.Id == 1)//Hourly
            {
                var runningDate = StartingDate;
                while (runningDate < $scope.toDate) {
                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            StrategyIds: $scope.selectedStrategyIds,
                            FromDate: new Date(runningDate.setHours(runningDate.getHours(), 0, 0, 0)),
                            ToDate: new Date(runningDate.setHours(runningDate.getHours(), 59, 59, 999))
                        }
                    });
                    runningDate = new Date(runningDate.setHours(runningDate.getHours() + 1));
                }

            }

            else if ($scope.selectedPeriod.Id == 2) //Daily
            {
                var runningDate = StartingDate;
                while (runningDate < $scope.toDate) {
                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            StrategyIds: $scope.selectedStrategyIds,
                            FromDate: new Date(runningDate.setHours(0, 0, 0, 0)),
                            ToDate: new Date(runningDate.setHours(23, 59, 59, 999))
                        }
                    });

                    runningDate.setHours(0, 0, 0, 0)
                    runningDate = new Date(runningDate.setDate(runningDate.getDate() + 1));
                }


            }

            return $scope.createProcessInputObjects;

        };


    }



    function loadPeriods() {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
        });
    }



    $scope.selectedPeriodChanged = function () {
        if (pageLoaded) {
            loadStrategies($scope.selectedPeriod.Id);
        }
        else {
            pageLoaded = true;
        }
    }


    function loadStrategies(periodId) {
        $scope.strategies.length = 0;
        $scope.selectedStrategies.length = 0;
        return StrategyAPIService.GetStrategies(periodId).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name, periodId: itm.PeriodId });
            });
        });
    }



}

ExecuteStrategyProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInputController', ExecuteStrategyProcessInputController)




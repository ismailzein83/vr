var ExecuteStrategyProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();

    function defineScope() {

        $scope.createProcessInputObjects = [];

        $scope.strategies = [];
        loadStrategies();
        $scope.selectedStrategies = [];

        $scope.selectedStrategyIdsDaily = [];
        $scope.selectedStrategyIdsHourly = [];


        $scope.createProcessInput.getData = function () {

            angular.forEach($scope.selectedStrategies, function (itm) {

                if (itm.periodId == 1)//Hourly
                {
                    $scope.selectedStrategyIdsHourly.push(itm.id);
                }
                else if (itm.periodId == 2)//Daily
                {
                    $scope.selectedStrategyIdsDaily.push(itm.id);
                }
            });


            var StartingDate = new Date($scope.fromDate);
            //runningDate = new Date(runningDate.setHours(runningDate.getHours() + 3));

            console.log(StartingDate.toString())

            $scope.createProcessInputObjects.length = 0;




            if ($scope.selectedStrategyIdsHourly.length > 0)//Hourly
            {
                var runningDate = StartingDate;
                while (runningDate < $scope.toDate) {
                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            StrategyIds: $scope.selectedStrategyIdsHourly,
                            FromDate: new Date(runningDate.setHours(runningDate.getHours(), 0, 0, 0)),
                            ToDate: new Date(runningDate.setHours(runningDate.getHours(), 59, 59, 999))
                        }
                    });
                    runningDate = new Date(runningDate.setHours(runningDate.getHours() + 1));
                }

            }

            if ($scope.selectedStrategyIdsDaily.length > 0) //Daily
            {
                var runningDate = StartingDate;
                while (runningDate < $scope.toDate) {
                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            StrategyIds: $scope.selectedStrategyIdsDaily,
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


    function loadStrategies() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                console.log(itm);
                $scope.strategies.push({ id: itm.Id, name: itm.Name, periodId:itm.PeriodId });
            });
        });
    }



}

ExecuteStrategyProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInputController', ExecuteStrategyProcessInputController)




var ExecuteStrategyProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();
    //load();
    //loadForm();
    function defineScope() {

        $scope.customvalidateFrom = function (fromDate) {
            return validateDates(fromDate, $scope.toDate);
        };
        $scope.customvalidateTo = function (toDate) {
            return validateDates($scope.fromDate, toDate);
        };
        function validateDates(fromDate, toDate) {
            if (fromDate == undefined || toDate == undefined)
                return null;
            var from = new Date(fromDate);
            var to = new Date(toDate);
            if (from.getTime() > to.getTime())
                return "Start should be before end";
            else
                return null;
        }

        $scope.createProcessInputObjects = [];

        $scope.strategies = [];
        loadStrategies();
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];


        $scope.periods = [];
        loadPeriods();
        $scope.selectedPeriod = "";


        $scope.createProcessInput.getData = function () {

            angular.forEach($scope.selectedStrategies, function (itm) {
                $scope.selectedStrategyIds.push(itm.id);
            });
         
            var runningDate = new Date($scope.fromDate);
            //runningDate = new Date(runningDate.setHours(runningDate.getHours() + 3));


            $scope.createProcessInputObjects.length = 0;
            if ($scope.selectedPeriod.Id == 1)//Hourly
            {
                runningDate = new Date($scope.fromDate);
                while (runningDate < $scope.toDate)
                {
                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            StrategyIds: $scope.selectedStrategyIds,
                            FromDate:   new Date(runningDate.setHours(runningDate.getHours(), 0, 0, 0)) ,
                            ToDate: new Date(runningDate.setHours(runningDate.getHours(), 59, 59, 999)),
                            PeriodId: $scope.selectedPeriod.Id
                        }
                    });
                    runningDate = new Date(runningDate.setHours(runningDate.getHours() + 1));
                }

            }
            else if ($scope.selectedPeriod.Id == 2) //Daily
            {
                runningDate = new Date($scope.fromDate);
                while (runningDate < $scope.toDate) {
                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            StrategyIds: $scope.selectedStrategyIds,
                            FromDate: new Date(runningDate.setHours(0, 0, 0, 0)),
                            ToDate: new Date(runningDate.setHours(23, 59, 59, 999)),
                            PeriodId: $scope.selectedPeriod.Id
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


    function loadStrategies() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
        });
    }

          

}

ExecuteStrategyProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInputController', ExecuteStrategyProcessInputController)




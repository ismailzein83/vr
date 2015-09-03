var NumberProfilingProcessController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, HourEnum) {
    var pageLoaded = false;

    defineScope();

    function defineScope() {

        $scope.createProcessInputObjects = [];

        $scope.periods = [];
        loadPeriods();
        $scope.selectedPeriod = "";


        $scope.hours = [];
        angular.forEach(HourEnum, function (itm) {
            $scope.hours.push({ id: itm.id, name: itm.name })
        });

        $scope.selectedPeakHours = [];


        $scope.createProcessInput.getData = function () {



            var runningDate = new Date($scope.fromDate);

            $scope.createProcessInputObjects.length = 0;


            if ($scope.selectedPeriod.Id == 1)//Hourly
            {
                while (runningDate < $scope.toDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 1));

                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            FromDate: new Date(fromDate),
                            ToDate: new Date(toDate),
                            PeriodId:$scope.selectedPeriod.Id,
                            Parameters:{GapBetweenConsecutiveCalls:$scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls:$scope.gapBetweenFailedConsecutiveCalls,MaxLowDurationCall:$scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour , PeakHoursIds:[1,2,3]  }
                        }
                    });
                    runningDate = new Date(toDate);
                }

            }

            else if ($scope.selectedPeriod.Id == 2) //Daily
            {
                while (runningDate < $scope.toDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));

                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            FromDate: new Date(fromDate),
                            ToDate: new Date(toDate),
                            PeriodId: $scope.selectedPeriod.Id,
                            Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: [1, 2, 3] }
                        }
                    });

                    runningDate = new Date(toDate);
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

}

NumberProfilingProcessController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'HourEnum'];
appControllers.controller('FraudAnalysis_NumberProfilingProcessController', NumberProfilingProcessController)




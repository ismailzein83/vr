"use strict";

ExecuteStrategyProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function ExecuteStrategyProcessInput_Scheduled($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var pageLoaded = false;
    defineScope();
    load();

    function defineScope() {

        $scope.processInputArguments = [];
        $scope.strategies = [];
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];
        $scope.periods = [];
        $scope.selectedPeriod ;
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
                OverridePrevious: false
            };
        };
    }



    $scope.selectedPeriodChanged = function () {
        if (pageLoaded) {
            $scope.strategies.length = 0;
            $scope.selectedStrategies.length = 0;
            loadStrategies();
        }
        else {
            pageLoaded = true;
        }
    }


    function loadStrategies() {
        var isEnabled = true;

        var periodId = 0
        if ($scope.selectedPeriod != '' && $scope.selectedPeriod != null)
            periodId = $scope.selectedPeriod.Id;

        return StrategyAPIService.GetStrategies(periodId, isEnabled).then(function (response) {
            $scope.strategies.length = 0;

            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name, periodId: itm.PeriodId });

            });
        });
    }


    function load() {

        $scope.periods.length = 0;
        $scope.periods = [];
        StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });

            if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
                return;

            var data = $scope.schedulerTaskAction.processInputArguments.data;

            if (data != null) {
                $scope.isGettingData = true;
                UtilsService.waitMultipleAsyncOperations([loadStrategies])
                .then(function () {

                    var strategyIndex = UtilsService.getItemIndexByVal($scope.strategies, data.StrategyIds[0], "id");

                    if (strategyIndex > -1) {
                        var selectedStrategy = $scope.strategies[strategyIndex];

                        $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, selectedStrategy.periodId, "Id");

                        UtilsService.waitMultipleAsyncOperations([loadStrategies]).then(function () {
                            angular.forEach(data.StrategyIds, function (strategyId) {
                                $scope.selectedStrategies.push(UtilsService.getItemByVal($scope.strategies, strategyId, "id"));
                            });
                        })
                    }
                }
            )
            }
            $scope.isGettingData = false;


        }).catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });



    }

}

appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInput_Scheduled', ExecuteStrategyProcessInput_Scheduled)




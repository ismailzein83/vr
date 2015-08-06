﻿var ExecuteStrategyProcessInput_Scheduled = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var pageLoaded = false;
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
        return StrategyAPIService.GetAllStrategies(periodId).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name, periodId: itm.PeriodId });
                console.log('itm')
                console.log(itm)
            });
        });
    }




    function load() {
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadPeriods])
        .then(function () {
            if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
                return;
            var data = $scope.schedulerTaskAction.processInputArguments.data;

            if (data != null) {
                loadStrategies(0);

                console.log('data');
                console.log(data);

                console.log('data.StrategyIds[0]');
                console.log(data.StrategyIds[0]);


                console.log('$scope.strategies');
                console.log($scope.strategies);

                console.log('UtilsService.getItemIndexByVal($scope.strategies, data.StrategyIds[0], "id")');
                console.log(UtilsService.getItemIndexByVal($scope.strategies, data.StrategyIds[0], "id"));


                $scope.selectedPeriod =  UtilsService.getItemByVal($scope.periods, $scope.strategies[UtilsService.getItemIndexByVal($scope.strategies, data.StrategyIds[0], "id")].periodId, "Id");

                loadStrategies($scope.selectedPeriod.periodId);

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




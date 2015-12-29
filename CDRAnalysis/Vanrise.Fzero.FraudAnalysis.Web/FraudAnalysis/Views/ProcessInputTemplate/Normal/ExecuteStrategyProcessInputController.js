﻿"use strict";

ExecuteStrategyProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService','VRValidationService'];

function ExecuteStrategyProcessInputController ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService,VRValidationService) {
    var pageLoaded = false;

    defineScope();

    load();

    function defineScope() {
        var yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);

        $scope.fromDate = yesterday;
        $scope.toDate = new Date();

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.createProcessInputObjects = [];

        $scope.strategies = [];
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];
        $scope.periods = [];
        $scope.selectedPeriod ;

        $scope.createProcessInput.getData = function () {

            angular.forEach($scope.selectedStrategies, function (itm) {
                $scope.selectedStrategyIds.push(itm.id);
            });


            var runningDate = new Date($scope.fromDate);

            $scope.createProcessInputObjects.length = 0;


            if ($scope.selectedPeriod.Id == 1)//Hourly
            {
                while (runningDate < $scope.toDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 1));
                    createProcessInputObjects(fromDate, toDate);
                    runningDate = new Date(toDate);
                }

            }

            else if ($scope.selectedPeriod.Id == 2) //Daily
            {
                while (runningDate < $scope.toDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));
                    createProcessInputObjects(fromDate, toDate);
                    runningDate = new Date(toDate);
                }


            }

            return $scope.createProcessInputObjects;

        };


    }


    function createProcessInputObjects(fromDate, toDate) {
        $scope.createProcessInputObjects.push({
            InputArguments: {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                StrategyIds: $scope.selectedStrategyIds,
                FromDate: new Date(fromDate),
                ToDate: new Date(toDate),
                OverridePrevious: $scope.overridePrevious,
                IncludeWhiteList: $scope.includeWhiteList
            }
        });
    }



    function load()
    {
        loadPeriods();
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
        var isEnabled = true;
        $scope.strategies.length = 0;
        $scope.selectedStrategies.length = 0;
        return StrategyAPIService.GetStrategies(periodId, isEnabled).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name, periodId: itm.PeriodId });
            });
        });
    }



}

appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInputController', ExecuteStrategyProcessInputController)




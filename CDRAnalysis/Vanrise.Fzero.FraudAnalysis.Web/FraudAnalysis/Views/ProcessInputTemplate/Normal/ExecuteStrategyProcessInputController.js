"use strict";

ExecuteStrategyProcessInputController.$inject = ['$scope', 'UtilsService', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'VRValidationService','VRUIUtilsService'];

function ExecuteStrategyProcessInputController($scope, UtilsService, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, VRValidationService, VRUIUtilsService) {
    var pageLoaded = false;

    var prefixDirectiveAPI;
    var prefixReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

        $scope.selectedPrefixes = [];
        $scope.strategies = [];
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];
        $scope.periods = [];
        $scope.selectedPeriod;


        $scope.onPrefixDirectiveReady = function (api) {
            prefixDirectiveAPI = api;
            prefixReadyPromiseDeferred.resolve();
        }

        $scope.prefixLengths = [0, 1, 2, 3];
        $scope.selectedPrefixLength = $scope.prefixLengths[1];

        $scope.createProcessInput.getData = function () {

            angular.forEach($scope.selectedStrategies, function (itm) {
                $scope.selectedStrategyIds.push(itm.id);
            });


            var runningDate = new Date($scope.fromDate);
            var selectedToDate = new Date($scope.toDate);
            $scope.createProcessInputObjects.length = 0;


            if ($scope.selectedPeriod.Id == 1)//Hourly
            {
                while (runningDate < selectedToDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 1));
                    createProcessInputObjects(fromDate, toDate);
                    runningDate = new Date(toDate);
                }

            }

            else if ($scope.selectedPeriod.Id == 2) //Daily
            {
                while (runningDate < selectedToDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));
                    createProcessInputObjects(fromDate, toDate);
                    runningDate = new Date(toDate);
                }


            }

            return $scope.createProcessInputObjects;

        };


    }

    function getPrefixesInfo() {
        var prefixLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        prefixReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;

                VRUIUtilsService.callDirectiveLoad(prefixDirectiveAPI, directivePayload, prefixLoadPromiseDeferred);
            });
        return prefixLoadPromiseDeferred.promise;
    }

    function createProcessInputObjects(fromDate, toDate) {

        $scope.createProcessInputObjects.push({
            InputArguments: {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                StrategyIds: $scope.selectedStrategyIds,
                FixedPrefixes: ['88888888'],//prefixDirectiveAPI.getSelectedIds(),
                PrefixLength: $scope.selectedPrefixLength,
                FromDate: new Date(fromDate),
                ToDate: new Date(toDate),
                OverridePrevious: $scope.overridePrevious,
                IncludeWhiteList: $scope.includeWhiteList
            }
        });
    }



    function load()
    {
        $scope.isGettingData = true;
        loadPeriods();

        UtilsService.waitMultipleAsyncOperations([getPrefixesInfo]).then(function () {
        }).finally(function () {
            $scope.isGettingData = false;
        });



       
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




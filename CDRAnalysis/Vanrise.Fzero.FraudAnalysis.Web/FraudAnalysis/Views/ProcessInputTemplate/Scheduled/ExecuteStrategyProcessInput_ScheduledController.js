"use strict";

ExecuteStrategyProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

function ExecuteStrategyProcessInput_Scheduled($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
    var pageLoaded = false;

    var prefixDirectiveAPI;
    var prefixReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.selectedPrefixIds;
        $scope.isEditMode = false;
        $scope.processInputArguments = [];
        $scope.strategies = [];
        $scope.selectedStrategies = [];
        $scope.selectedStrategyIds = [];
        $scope.periods = [];
        $scope.selectedPeriod;
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
                FixedPrefixes: prefixDirectiveAPI.getSelectedIds(),
                PrefixLength: $scope.selectedPrefixLength,
                IncludeWhiteList: false
            };
        };

        $scope.onPrefixDirectiveReady = function (api) {
            prefixDirectiveAPI = api;
            prefixReadyPromiseDeferred.resolve();
        }

        $scope.prefixLengths = [0, 1, 2, 3];
        $scope.selectedPrefixLength = $scope.prefixLengths[1];

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

        $scope.isGettingData = true;
        $scope.periods.length = 0;
        $scope.periods = [];
        StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });


            var data = $scope.schedulerTaskAction.processInputArguments.data;

            if (data != null && data != undefined) {
                $scope.isEditMode = true;
                $scope.selectedPrefixIds = data.FixedPrefixes;
                $scope.selectedPrefixLength = data.PrefixLength;
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
            getPrefixesInfo();
        }).catch(function (error) {

            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });

    }

    function getPrefixesInfo() {
        var prefixLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        prefixReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = {};
                if (!$scope.isEditMode)
                    directivePayload.selectAll = true;
                directivePayload.selectedIds = $scope.selectedPrefixIds;
                VRUIUtilsService.callDirectiveLoad(prefixDirectiveAPI, directivePayload, prefixLoadPromiseDeferred);
            });
        return prefixLoadPromiseDeferred.promise;
    }

}

appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInput_Scheduled', ExecuteStrategyProcessInput_Scheduled)




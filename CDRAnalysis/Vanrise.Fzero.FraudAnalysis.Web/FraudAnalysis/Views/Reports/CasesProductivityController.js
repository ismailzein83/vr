"use strict";

CasesProductivityController.$inject = ['$scope', 'ReportingAPIService', 'VRUIUtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRValidationService', 'VRDateTimeService'];

function CasesProductivityController($scope, ReportingAPIService, VRUIUtilsService, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRValidationService, VRDateTimeService) {

    var strategySelectorAPI;
    var strategySelectorReadyDeferred = UtilsService.createPromiseDeferred();
    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {

        $scope.onStrategySelectorReady = function (api) {
            strategySelectorAPI = api;
            strategySelectorReadyDeferred.resolve();
        };

        var Now = VRDateTimeService.getNowDateTime();

        var Yesterday = VRDateTimeService.getNowDateTime();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        };

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.casesProductivity = [];

        $scope.selectedStrategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredCasesProductivity(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        };

        $scope.onGroupDailyChanged = function () {
            return retrieveData();
        };
    }

    function load() {

        $scope.isInitializing = true;

        return UtilsService.waitMultipleAsyncOperations([loadStrategySelector])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function loadStrategySelector() {
        var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
        strategySelectorReadyDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(strategySelectorAPI, undefined, strategySelectorLoadDeferred);
        });
        return strategySelectorLoadDeferred.promise;
    }

    function retrieveData() {

        var query = {
            StrategyIDs: strategySelectorAPI.getSelectedIds(),
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            GroupDaily: $scope.groupDaily
        };

        return mainGridAPI.retrieveData(query);
    }


}

appControllers.controller('FraudAnalysis_CasesProductivityController', CasesProductivityController);

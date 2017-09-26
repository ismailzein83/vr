"use strict";

BlockedLinesController.$inject = ['$scope', 'ReportingAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRValidationService', 'VRUIUtilsService', 'VRDateTimeService'];

function BlockedLinesController($scope, ReportingAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRValidationService, VRUIUtilsService, VRDateTimeService) {

    var strategySelectorAPI;
    var strategySelectorReadyDeferred = UtilsService.createPromiseDeferred();
    var mainGridAPI;
    var arrMenuAction = [];
    var accountNumbers = [];

    defineScope();
    load();

    function defineScope() {

        $scope.onStrategySelectorReady = function (api) {
            strategySelectorAPI = api;
            strategySelectorReadyDeferred.resolve();
        };

        $scope.gridMenuActions = [];

        var Yesterday = VRDateTimeService.getNowDateTime();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = VRDateTimeService.getNowDateTime();

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        };

        $scope.blockedLines = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredBlockedLines(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        };

        $scope.onGroupDailyChanged = function () {
            return retrieveData();
        };

        defineMenuActions();
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


    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "View List",
            clicked: viewList
        }];
    }

    function viewList(itm) {

        angular.forEach(itm.AccountNumbers, function (itm) {
            accountNumbers.push({ accountNumber: itm })
        });

        var params = {
            accountNumbers: accountNumbers
        };

        var settings = {

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Blocked Lines List";
        };

        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Reports/BlockedLinesDetails.html", params, settings);
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

appControllers.controller('FraudAnalysis_BlockedLinesController', BlockedLinesController);

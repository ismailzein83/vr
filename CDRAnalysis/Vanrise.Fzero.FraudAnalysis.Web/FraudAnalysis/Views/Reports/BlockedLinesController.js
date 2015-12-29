"use strict";

BlockedLinesController.$inject = ['$scope', 'ReportingAPIService', 'StrategyAPIService', 'UsersAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'KindEnum', 'StatusEnum', 'VRValidationService'];

function BlockedLinesController($scope, ReportingAPIService, StrategyAPIService, UsersAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, KindEnum, StatusEnum, VRValidationService) {

    var mainGridAPI;
    var arrMenuAction = [];
    var accountNumbers = [];

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = new Date();

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.strategies = [];
        $scope.selectedStrategies = [];

        $scope.blockedLines = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredBlockedLines(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

        $scope.onGroupDailyChanged = function () {
            return retrieveData();
        }

        defineMenuActions();
    }

    function load() {
        $scope.isInitializing = true;

        var periodId = 0; // all periods
        return StrategyAPIService.GetStrategies(periodId, '')
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.strategies.push({ id: item.Id, name: item.Name });
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
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
            StrategyIDs: UtilsService.getPropValuesFromArray($scope.selectedStrategies, "id"),
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            GroupDaily: $scope.groupDaily
        };

        return mainGridAPI.retrieveData(query);
    }

   


}

appControllers.controller('FraudAnalysis_BlockedLinesController', BlockedLinesController);

"use strict";

BlockedLinesController.$inject = ['$scope', 'ReportingAPIService', 'StrategyAPIService', 'UsersAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'KindEnum', 'StatusEnum'];

function BlockedLinesController($scope, ReportingAPIService, StrategyAPIService, UsersAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, KindEnum, StatusEnum) {

    var mainGridAPI;
    var arrMenuAction = [];
    var accountNumbers = [];

    defineScope();
    load();

    function defineScope() {
        $scope.showGrid = false;
        $scope.gridMenuActions = [];

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = new Date();


        $scope.strategies = [];

        $scope.blockedLines = [];


        $scope.selectedStrategies = [];
        

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredBlockedLines(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

        $scope.onGroupDailyChanged = function () {
            if ($scope.showGrid)
                return retrieveData();
        }

        defineMenuActions();
    }

    function load() {
        loadStrategies();
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

    function loadStrategies() {
        var periodId = 0; // all periods
        var isEnabled = ''; // all enabled and disabled
        return StrategyAPIService.GetStrategies(periodId, isEnabled).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
        });
    }

    function removeLastComma(strng) {
        var n = strng.lastIndexOf(",");
        var a = strng.substring(0, n)
        return a;
    }

    function retrieveData() {

        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });


        var query = {
            StrategiesList: removeLastComma(strategiesList),
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            GroupDaily: $scope.groupDaily
        };

        return mainGridAPI.retrieveData(query);
    }
}

appControllers.controller('FraudAnalysis_BlockedLinesController', BlockedLinesController);

"use strict";

CasesProductivityController.$inject = ['$scope', 'ReportingAPIService', 'StrategyAPIService', 'UsersAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'KindEnum', 'StatusEnum'];

function CasesProductivityController($scope, ReportingAPIService, StrategyAPIService, UsersAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, KindEnum, StatusEnum) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();


    function defineScope() {

        $scope.showGrid = false;

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.casesProductivity = [];

        loadStrategies();

        $scope.selectedStrategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredCasesProductivity(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

        $scope.onGroupDailyChanged = function () {
            if ($scope.showGrid)
                return retrieveData();
        }
    }

    function load() {

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

   

    function retrieveData() {

        var selectedStrategiesIDs = [];

        angular.forEach($scope.selectedStrategies, function (itm) {
            selectedStrategiesIDs.push(itm.Id);
        });


        var query = {
            StrategiesList: selectedStrategiesIDs,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            GroupDaily: $scope.groupDaily
        };

        return mainGridAPI.retrieveData(query);
    }
}

appControllers.controller('FraudAnalysis_CasesProductivityController', CasesProductivityController);

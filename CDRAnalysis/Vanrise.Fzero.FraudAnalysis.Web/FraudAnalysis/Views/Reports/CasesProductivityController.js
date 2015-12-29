"use strict";

CasesProductivityController.$inject = ['$scope', 'ReportingAPIService', 'StrategyAPIService', 'UsersAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'KindEnum', 'StatusEnum', 'VRValidationService'];

function CasesProductivityController($scope, ReportingAPIService, StrategyAPIService, UsersAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, KindEnum, StatusEnum, VRValidationService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {


        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.casesProductivity = [];

        $scope.selectedStrategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredCasesProductivity(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

        $scope.onGroupDailyChanged = function () {
            return retrieveData();
        }
    }

    function load() {
        $scope.isInitializing = true;

        var periodId = 0; // all periods
        var isEnabled = ''; // all enabled and disabled
        return StrategyAPIService.GetStrategies(periodId, isEnabled)
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

appControllers.controller('FraudAnalysis_CasesProductivityController', CasesProductivityController);

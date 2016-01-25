"use strict";

StrategyExecutionManagementController.$inject = ['$scope', 'StrategyExecutionAPIService', 'VR_Sec_UserAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'KindEnum', 'StatusEnum', 'VRValidationService', 'BusinessProcessService', 'BusinessProcessAPIService', 'StrategyAPIService'];

function StrategyExecutionManagementController($scope, StrategyExecutionAPIService, VR_Sec_UserAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, KindEnum, StatusEnum, VRValidationService, BusinessProcessService, BusinessProcessAPIService, StrategyAPIService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.gridMenuActions = [];

        $scope.strategyExecutions = [];
        $scope.strategies = [];
        $scope.selectedStrategies = [];

        $scope.filterDateType = [];
        $scope.filterDateType.push({ id: 1, name: 'By CDR' })
        $scope.filterDateType.push({ id: 2, name: 'By Strategy Execution' })

        $scope.selectedFilterDateType = $scope.filterDateType[0];


        $scope.getStatusColor = function (dataItem, colDef) {
            return BusinessProcessService.getStatusColor(dataItem.Status);
        };


        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;

            if (!$scope.isInitializing) // if the filters are loaded
                return retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.periods = [];

        $scope.selectedPeriods = [];

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return StrategyExecutionAPIService.GetFilteredStrategyExecutions(dataRetrievalInput)
                .then(function (response) {
                    angular.forEach(response.Data, function (item) {
                        item.StatusDescription = BusinessProcessService.getStatusDescription(item.Status)
                    });
                    onResponseReady(response);
                });
        }

        defineMenuActions();
    }


    function loadStrategies() {
        return StrategyAPIService.GetStrategies(0, "") // get all the enabled and disabled strategies (2nd arg) for all periods (1st arg)
           .then(function (response) {
               angular.forEach(response, function (item) {
                   $scope.strategies.push({
                       id: item.Id,
                       name: item.Name
                   });
               });
           })
        ;
    }

    

    function load() {
        $scope.isInitializing = true;

        return UtilsService.waitMultipleAsyncOperations([loadPeriods, loadStrategies])
            .then(function () {
                if (mainGridAPI != undefined)
                    return retrieveData();
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
            name: "Details",
            clicked: viewStrategyExecutionDetails
        }];
    }

    function retrieveData() {
        var query = {
            PeriodIDs: ($scope.selectedPeriods.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedPeriods, "id") : null,
            FromCDRDate: ($scope.selectedFilterDateType.id == 1 ? $scope.fromDate : null),
            ToCDRDate: ($scope.selectedFilterDateType.id == 1 ? $scope.toDate : null),
            FromExecutionDate: ($scope.selectedFilterDateType.id == 2 ? $scope.fromDate : null),
            ToExecutionDate: ($scope.selectedFilterDateType.id == 2 ? $scope.toDate : null),
            StrategyIDs: ($scope.selectedStrategies.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedStrategies, "id") : null,
        };

        return mainGridAPI.retrieveData(query);
    }


    function loadPeriods() {
        return StrategyAPIService.GetPeriods()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.periods.push({
                        id: item.Id,
                        name: item.Name
                    });
                });
            });
    }

    function viewStrategyExecutionDetails(gridObject) {
        var params = {
            strategyExecutionId: gridObject.Entity.Id
        };

        var settings = {
            width: '95%'
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor("Strategy Execution on: ", gridObject.Entity.ExecutionDate);
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/StrategyExecution/StrategyExecutionDetails.html", params, settings);
    }
}

appControllers.controller('FraudAnalysis_StrategyExecutionManagementController', StrategyExecutionManagementController);

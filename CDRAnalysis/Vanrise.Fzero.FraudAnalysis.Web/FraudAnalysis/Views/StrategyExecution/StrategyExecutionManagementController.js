"use strict";

StrategyExecutionManagementController.$inject = ['$scope', "VRUIUtilsService", 'CDRAnalysis_FA_PeriodAPIService', 'StrategyExecutionAPIService', 'VR_Sec_UserAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRValidationService', 'BusinessProcessService', 'BusinessProcessAPIService', 'StrategyAPIService'];

function StrategyExecutionManagementController($scope, VRUIUtilsService, CDRAnalysis_FA_PeriodAPIService, StrategyExecutionAPIService, VR_Sec_UserAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRValidationService, BusinessProcessService, BusinessProcessAPIService, StrategyAPIService) {

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

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.gridMenuActions = [];

        $scope.periods = [];

        $scope.strategyExecutions = [];
        $scope.selectedPeriods = [];

        $scope.filterDateTypes = [];
        $scope.filterDateTypes.push({ id: 1, name: 'By CDR' })
        $scope.filterDateTypes.push({ id: 2, name: 'By Strategy Execution' })

        $scope.selectedFilterDateType = $scope.filterDateTypes[0];

        $scope.getStatusColor = function (dataItem, colDef) {
            return BusinessProcessService.getStatusColor(dataItem.Status);
        };

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.searchClicked = function () {
            return retrieveData();
        }

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

    function loadStrategySelector() {
        var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
        strategySelectorReadyDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(strategySelectorAPI, undefined, strategySelectorLoadDeferred);
        });
        return strategySelectorLoadDeferred.promise;
    }

    function load() {
        $scope.isInitializing = true;

        return UtilsService.waitMultipleAsyncOperations([loadPeriods, loadStrategySelector])
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
            clicked: viewStrategyExecutionItem
        }];
    }

    function retrieveData() {
        var query = {
            PeriodIDs: ($scope.selectedPeriods.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedPeriods, "id") : null,
            FromCDRDate: ($scope.selectedFilterDateType.id == 1 ? $scope.fromDate : null),
            ToCDRDate: ($scope.selectedFilterDateType.id == 1 ? $scope.toDate : null),
            FromExecutionDate: ($scope.selectedFilterDateType.id == 2 ? $scope.fromDate : null),
            ToExecutionDate: ($scope.selectedFilterDateType.id == 2 ? $scope.toDate : null),
            StrategyIds: strategySelectorAPI.getSelectedIds()
        };

        return mainGridAPI.retrieveData(query);
    }

    function loadPeriods() {
        return CDRAnalysis_FA_PeriodAPIService.GetPeriods()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.periods.push({
                        id: item.Id,
                        name: item.Name
                    });
                });
            });
    }

    function viewStrategyExecutionItem(gridObject) {
        var params = {
            strategyExecutionId: gridObject.Entity.Id
        };

        var settings = {
            width: '95%'
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor("Strategy Execution on: ", gridObject.Entity.ExecutionDate);
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/StrategyExecution/StrategyExecutionItem.html", params, settings);
    }
}

appControllers.controller('FraudAnalysis_StrategyExecutionManagementController', StrategyExecutionManagementController);

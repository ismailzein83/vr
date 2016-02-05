"use strict";

StrategyExecutionManagementController.$inject = ['$scope', "VRUIUtilsService", 'StrategyExecutionAPIService', 'VR_Sec_UserAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRValidationService', 'BusinessProcessService', 'BusinessProcessAPIService', 'StrategyAPIService', 'CDRAnalysis_FA_StrategyExecutionFilterDateTypes', 'BPInstanceStatusEnum'];

function StrategyExecutionManagementController($scope, VRUIUtilsService, StrategyExecutionAPIService, VR_Sec_UserAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRValidationService, BusinessProcessService, BusinessProcessAPIService, StrategyAPIService, CDRAnalysis_FA_StrategyExecutionFilterDateTypes, BPInstanceStatusEnum) {

    var strategySelectorAPI;
    var strategySelectorReadyDeferred = UtilsService.createPromiseDeferred();

    var userSelectorAPI;
    var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();

    var periodSelectorAPI;
    var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {

        $scope.onStrategySelectorReady = function (api) {
            strategySelectorAPI = api;
            strategySelectorReadyDeferred.resolve();
        };

        $scope.onPeriodSelectorReady = function (api) {
            periodSelectorAPI = api;
            periodSelectorReadyDeferred.resolve();
        };

        $scope.onUserSelectorReady = function (api) {
            userSelectorAPI = api;
            userSelectorReadyDeferred.resolve();
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

        $scope.strategyExecutions = [];

        $scope.filterDateTypes = UtilsService.getArrayEnum(CDRAnalysis_FA_StrategyExecutionFilterDateTypes);

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

        $scope.onPeriodSelectionChanged = function (selectedPeriod) {
            if (selectedPeriod != undefined) {
                loadStrategySelector(selectedPeriod.Id);
            }
            else
                loadStrategySelector();
        };

        defineMenuActions();
    }

    function loadStrategySelector(periodId) {
        var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
        var payload = {
        };

        var filter = {};
        if (periodId != undefined)
            filter.PeriodId = periodId;

        payload.filter = filter;

        strategySelectorReadyDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(strategySelectorAPI, payload, strategySelectorLoadDeferred);
        });

        return strategySelectorLoadDeferred.promise;
    }

    function loadUserSelector() {
        var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();

        userSelectorReadyDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(userSelectorAPI, undefined, userSelectorLoadDeferred);
        });

        return userSelectorLoadDeferred.promise;
    }

    function loadPeriodSelector() {
        var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

        periodSelectorReadyDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, undefined, periodSelectorLoadDeferred);
        });

        return periodSelectorLoadDeferred.promise;
    }

    function load() {
        $scope.isInitializing = true;

        return UtilsService.waitMultipleAsyncOperations([loadStrategySelector, loadUserSelector, loadPeriodSelector])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function defineMenuActions(item) {
        $scope.gridMenuActions = [{
            name: "Cancel",
            clicked: cancelStrategyExecution
        }];
    }

    function defineMenuActions() {

        var menuActionsWithCancel = [{
            name: "Cancel",
            clicked: cancelStrategyExecution
        }];

        var defaultMenuActions = [

        ];

        $scope.gridMenuActions = function (dataItem) {
            if (dataItem.Status == BPInstanceStatusEnum.New.value || dataItem.Status == BPInstanceStatusEnum.Completed.value) {
                return menuActionsWithCancel;
            } else {
                return defaultMenuActions;
            }
        }
    }

    function retrieveData() {
        var query = {
            UserIds: userSelectorAPI.getSelectedIds(),
            FromCDRDate: ($scope.selectedFilterDateType.id == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCDRConnect.value ? $scope.fromDate : null),
            ToCDRDate: ($scope.selectedFilterDateType.id == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCDRConnect.value ? $scope.toDate : null),
            FromExecutionDate: ($scope.selectedFilterDateType.id == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByStrategyExecutionDate.value ? $scope.fromDate : null),
            ToExecutionDate: ($scope.selectedFilterDateType.id == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByStrategyExecutionDate.value ? $scope.toDate : null),
            FromCancellationDate: ($scope.selectedFilterDateType.id == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCancelDate.value ? $scope.fromDate : null),
            ToCancellationDate: ($scope.selectedFilterDateType.id == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCancelDate.value ? $scope.toDate : null),
            PeriodId: periodSelectorAPI.getSelectedIds(),
            StrategyIds: strategySelectorAPI.getSelectedIds()
        };

        return mainGridAPI.retrieveData(query);
    }

    function cancelStrategyExecution(gridObject) {
        $scope.issaving = true;
        var createProcessInputs = {
            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.CancelStrategyExecutionProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
            StrategyExecutionId: gridObject.Entity.Id
        };

        BusinessProcessAPIService.CreateNewProcess(createProcessInputs).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Bussiness Instance", response)) {
                if ($scope.onProcessInputCreated != undefined)
                    $scope.onProcessInputCreated(response.ProcessInstanceId);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        }).finally(function () {
            $scope.issaving = false;
        });
    }

}

appControllers.controller('FraudAnalysis_StrategyExecutionManagementController', StrategyExecutionManagementController);

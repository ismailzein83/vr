"use strict";

StrategyExecutionManagementController.$inject = ['$scope', "VRUIUtilsService", 'CDRAnalysis_FA_StrategyExecutionAPIService', 'VR_Sec_UserAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRValidationService',
    'BusinessProcess_BPInstanceAPIService', 'StrategyAPIService', 'CDRAnalysis_FA_StrategyExecutionFilterDateTypes', 'CDRAnalysis_FA_SuspicionOccuranceStatusEnum', 'LabelColorsEnum', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'PeriodEnum', 'VRDateTimeService'];

function StrategyExecutionManagementController($scope, VRUIUtilsService, CDRAnalysis_FA_StrategyExecutionAPIService, VR_Sec_UserAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRValidationService,
    BusinessProcess_BPInstanceAPIService, StrategyAPIService, CDRAnalysis_FA_StrategyExecutionFilterDateTypes, CDRAnalysis_FA_SuspicionOccuranceStatusEnum, LabelColorsEnum, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, PeriodEnum, VRDateTimeService) {

    var timeRangeDirectiveAPI;
    var timeRangeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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

        $scope.fromDate = new Date(VRDateTimeService.getNowDateTime().setHours(0, 0, 0, 0));
        $scope.toDate;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeDirectiveReadyDeferred.resolve();
        };

        $scope.selectedSuspicionOccuranceStatuses = [];

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

        $scope.gridMenuActions = [];

        $scope.strategyExecutions = [];

        $scope.suspicionOccuranceStatuses = UtilsService.getArrayEnum(CDRAnalysis_FA_SuspicionOccuranceStatusEnum);

        $scope.filterDateTypes = UtilsService.getArrayEnum(CDRAnalysis_FA_StrategyExecutionFilterDateTypes);

        $scope.selectedFilterDateType = $scope.filterDateTypes[0];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CDRAnalysis_FA_StrategyExecutionAPIService.GetFilteredStrategyExecutions(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                });
        };

        $scope.onPeriodSelectionChanged = function (selectedPeriod) {
            if (selectedPeriod != undefined) {
                loadStrategySelector(selectedPeriod.Id);
            }
            else
                loadStrategySelector();
        };

        $scope.getStatusColor = function (dataItem) {
            if (dataItem.Entity.Status == CDRAnalysis_FA_SuspicionOccuranceStatusEnum.Open.value) return LabelColorsEnum.New.color;
            else if (dataItem.Entity.Status == CDRAnalysis_FA_SuspicionOccuranceStatusEnum.Closed.value) return LabelColorsEnum.Success.color;
            else if (dataItem.Entity.Status == CDRAnalysis_FA_SuspicionOccuranceStatusEnum.Cancelled.value) return LabelColorsEnum.Error.color;
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

    function loadTimeRangeDirective() {
        var timeRangeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

        timeRangeDirectiveReadyDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, undefined, timeRangeDirectiveLoadDeferred);
        });

        return timeRangeDirectiveLoadDeferred.promise;
    }

    function load() {
        $scope.isInitializing = true;

        return UtilsService.waitMultipleAsyncOperations([loadStrategySelector, loadUserSelector, loadPeriodSelector, loadTimeRangeDirective])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function defineMenuActions() {

        var menuActionsWithCancel = [{
            name: "Cancel",
            clicked: cancelStrategyExecution
        }];

        var defaultMenuActions = [

        ];

        $scope.gridMenuActions = function (dataItem) {
            if (dataItem.Entity.Status == CDRAnalysis_FA_SuspicionOccuranceStatusEnum.Closed.value) {
                return menuActionsWithCancel;
            } else {
                return defaultMenuActions;
            }
        }
    }

    function retrieveData() {
        var query = {
            UserIds: userSelectorAPI.getSelectedIds(),
            FromCDRDate: ($scope.selectedFilterDateType.value == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCDRConnect.value ? $scope.fromDate : null),
            ToCDRDate: ($scope.selectedFilterDateType.value == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCDRConnect.value ? $scope.toDate : null),
            FromExecutionDate: ($scope.selectedFilterDateType.value == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByStrategyExecutionDate.value ? $scope.fromDate : null),
            ToExecutionDate: ($scope.selectedFilterDateType.value == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByStrategyExecutionDate.value ? $scope.toDate : null),
            FromCancellationDate: ($scope.selectedFilterDateType.value == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCancelDate.value ? $scope.fromDate : null),
            ToCancellationDate: ($scope.selectedFilterDateType.value == CDRAnalysis_FA_StrategyExecutionFilterDateTypes.ByCancelDate.value ? $scope.toDate : null),
            PeriodId: periodSelectorAPI.getSelectedIds(),
            StrategyIds: strategySelectorAPI.getSelectedIds(),
            StatusIds: UtilsService.getPropValuesFromArray($scope.selectedSuspicionOccuranceStatuses, "value")
        };
        return mainGridAPI.retrieveData(query);
    }

    function buildInstanceObjFromScope(strategyExecutionId) {
        return {
            InputArguments: {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.CancelStrategyExecutionProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                StrategyExecutionId: strategyExecutionId
            }
        };
    }

    function cancelStrategyExecution(gridObject) {
        VRNotificationService.showConfirmation()
               .then(function (response) {
                   if (response == true) {
                       $scope.isInitializing = true;
                       var createProcessInput = buildInstanceObjFromScope(gridObject.Entity.ID);

                       BusinessProcess_BPInstanceAPIService.CreateNewProcess(createProcessInput).then(function (response) {
                           if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                               return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                           }
                       }).catch(function (error) {
                           VRNotificationService.notifyException(error);
                       })
                       .finally(function () {
                           $scope.isInitializing = false;
                       });
                   }
               });
    }

}

appControllers.controller('FraudAnalysis_StrategyExecutionManagementController', StrategyExecutionManagementController);

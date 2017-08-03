"use strict";

SuspicionAnalysisController.$inject = ["$scope", "StrategyAPIService", "CDRAnalysis_FA_SuspicionLevelEnum", "CDRAnalysis_FA_CaseStatusEnum", "LabelColorsEnum", "UtilsService", "VRNotificationService", "VRModalService", "VRNavigationService", "VRValidationService", 'CDRAnalysis_FA_AccountCaseAPIService', 'VRUIUtilsService', 'PeriodEnum'];

function SuspicionAnalysisController($scope, StrategyAPIService, CDRAnalysis_FA_SuspicionLevelEnum, CDRAnalysis_FA_CaseStatusEnum, LabelColorsEnum, UtilsService, VRNotificationService, VRModalService, VRNavigationService, VRValidationService, CDRAnalysis_FA_AccountCaseAPIService, VRUIUtilsService, PeriodEnum) {

    var strategySelectorAPI;
    var strategySelectorReadyDeferred = UtilsService.createPromiseDeferred();

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.fromDate = new Date(new Date().setHours(0, 0, 0, 0));
        $scope.toDate;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };

        $scope.onStrategySelectorReady = function (api) {
            strategySelectorAPI = api;
            strategySelectorReadyDeferred.resolve();
        };

        $scope.accountNumber = undefined;

        $scope.strategies = [];

        $scope.accountStatuses = UtilsService.getArrayEnum(CDRAnalysis_FA_CaseStatusEnum);
        $scope.selectedAccountStatuses = [$scope.accountStatuses[0]]; // select the Open status by default

        $scope.suspicionLevels = UtilsService.getArrayEnum(CDRAnalysis_FA_SuspicionLevelEnum);
        $scope.selectedSuspicionLevels = [];

        $scope.accountSuspicionSummaries = [];
        $scope.gridMenuActions = [];

        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CDRAnalysis_FA_AccountCaseAPIService.GetFilteredAccountSuspicionSummaries(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {

                        if (item.SuspicionLevelID != 0) {
                            var suspicionLevel = UtilsService.getEnum(CDRAnalysis_FA_SuspicionLevelEnum, "value", item.SuspicionLevelID);
                            item.SuspicionLevelDescription = suspicionLevel.description;
                        }

                        var accountStatus = UtilsService.getEnum(CDRAnalysis_FA_CaseStatusEnum, "value", item.Status);
                        item.AccountStatusDescription = accountStatus.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        };

        $scope.summaryClicked = function (dataItem) {
            openSummaryDetails(dataItem.AccountNumber, dataItem.CaseID);
        };

        $scope.getSuspicionLevelColor = function (dataItem) {

            if (dataItem.SuspicionLevelID == CDRAnalysis_FA_SuspicionLevelEnum.Suspicious.value) return LabelColorsEnum.WarningLevel1.color;
            else if (dataItem.SuspicionLevelID == CDRAnalysis_FA_SuspicionLevelEnum.HighlySuspicious.value) return LabelColorsEnum.WarningLevel2.color;
            else if (dataItem.SuspicionLevelID == CDRAnalysis_FA_SuspicionLevelEnum.Fraud.value) return LabelColorsEnum.Error.color;
        };

        $scope.getCaseStatusColor = function (dataItem) {

            if (dataItem.Status == CDRAnalysis_FA_CaseStatusEnum.Open.value) return LabelColorsEnum.New.color;
            else if (dataItem.Status == CDRAnalysis_FA_CaseStatusEnum.Pending.value) return LabelColorsEnum.Processing.color;
            else if (dataItem.Status == CDRAnalysis_FA_CaseStatusEnum.ClosedFraud.value) return LabelColorsEnum.Error.color;
            else if (dataItem.Status == CDRAnalysis_FA_CaseStatusEnum.ClosedWhitelist.value) return LabelColorsEnum.Success.color;
        };
    }

    function retrieveData() {
        var query = {
            AccountNumber: ($scope.accountNumber != undefined && $scope.accountNumber != "") ? $scope.accountNumber : null,
            StrategyExecutionId: ($scope.strategyExecutionId != undefined && $scope.strategyExecutionId != "") ? $scope.strategyExecutionId : null,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            StrategyIDs: strategySelectorAPI.getSelectedIds(),
            Statuses: UtilsService.getPropValuesFromArray($scope.selectedAccountStatuses, "value"),
            SuspicionLevelIDs: UtilsService.getPropValuesFromArray($scope.selectedSuspicionLevels, "value")
        };

        return gridAPI.retrieveData(query);
    }

    function load() {
        $scope.isInitializing = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadStrategySelector, loadTimeRangeSelector]).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isInitializing = false;
        });

        function loadStrategySelector() {
            var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            strategySelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(strategySelectorAPI, undefined, strategySelectorLoadDeferred);
            });
            return strategySelectorLoadDeferred.promise;
        }

        function loadTimeRangeSelector() {
            var timeRangeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            timeRangeReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, undefined, timeRangeLoadPromiseDeferred);
            });
            return timeRangeLoadPromiseDeferred.promise;
        }
    }

    function openSummaryDetails(accountNumber, caseID) {
        var modalSettings = {};

        var parameters = {
            AccountNumber: accountNumber,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            CaseID: caseID,
            ModalLevel: 1
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details";

            modalScope.onAccountCaseUpdated = function (accountSuspicionSummary) {
                gridAPI.itemUpdated(accountSuspicionSummary);
            };
        };

        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/SuspiciousNumberDetails.html", parameters, modalSettings);
    }
}

appControllers.controller("FraudAnalysis_SuspicionAnalysisController", SuspicionAnalysisController);

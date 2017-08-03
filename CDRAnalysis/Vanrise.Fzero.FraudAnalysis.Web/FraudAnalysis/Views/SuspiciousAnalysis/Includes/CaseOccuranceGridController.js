"use strict";

CaseOccuranceGridController.$inject = ["$scope", "CDRAnalysis_FA_SuspicionLevelEnum", "CDRAnalysis_FA_CaseStatusEnum", "LabelColorsEnum", "UtilsService", "VRNotificationService",'CDRAnalysis_FA_AccountCaseHistoryAPIService','CDRAnalysis_FA_StrategyExecutionItemAPIService'];

function CaseOccuranceGridController($scope, CDRAnalysis_FA_SuspicionLevelEnum, CDRAnalysis_FA_CaseStatusEnum, LabelColorsEnum, UtilsService, VRNotificationService, CDRAnalysis_FA_AccountCaseHistoryAPIService, CDRAnalysis_FA_StrategyExecutionItemAPIService) {

    var gridApi = undefined;
    var gridApi_Logs = undefined;

    defineScope();

    function defineScope() {

        $scope.details = [];
        $scope.logs = [];

        $scope.gridReady = function (api) {
            gridApi = api;

            if ($scope.caseOccurancesSelected)
                return retrieveData();
        };

        $scope.gridReady_Logs = function (api) {
            gridApi_Logs = api;

            if ($scope.caseLogsSelected)
                return retrieveData_Logs();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CDRAnalysis_FA_StrategyExecutionItemAPIService.GetFilteredDetailsByCaseID(dataRetrievalInput)
                .then(function (response) {
                    angular.forEach(response.Data, function (item) {
                        var suspicionLevel = UtilsService.getEnum(CDRAnalysis_FA_SuspicionLevelEnum, "value", item.SuspicionLevelID);
                        item.SuspicionLevelDescription = suspicionLevel.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };

        $scope.dataRetrievalFunction_Logs = function (dataRetrievalInput, onResponseReady) {

            return CDRAnalysis_FA_AccountCaseHistoryAPIService.GetFilteredAccountCaseHistoryByCaseID(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {

                        var user = UtilsService.getItemByVal($scope.viewScope.users, item.UserID, "UserId");
                        item.UserName = (user != null) ? user.Name : "System";

                        var caseStatus = UtilsService.getEnum(CDRAnalysis_FA_CaseStatusEnum, "value", item.Status);
                        item.StatusDescription = caseStatus.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };

        $scope.onSelectedTabChanged = function () {

            if ($scope.selectedTabIndex == 0)
                return retrieveData();
            else if ($scope.selectedTabIndex == 1)
                return retrieveData_Logs();
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
            CaseID: $scope.dataItem.CaseID
        };

        return gridApi.retrieveData(query);
    }

    function retrieveData_Logs() {

        var query = {
            CaseID: $scope.dataItem.CaseID
        };

        return gridApi_Logs.retrieveData(query);
    }
}

appControllers.controller("FraudAnalysis_CaseOccuranceGridController", CaseOccuranceGridController);
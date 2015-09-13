"use strict";

CaseOccuranceGridController.$inject = ["$scope", "CaseManagementAPIService", "SuspicionLevelEnum", "SuspicionOccuranceStatusEnum", "CaseStatusEnum", "UtilsService", "VRNotificationService"];

function CaseOccuranceGridController($scope, CaseManagementAPIService, SuspicionLevelEnum, SuspicionOccuranceStatusEnum, CaseStatusEnum, UtilsService, VRNotificationService) {

    var gridApi = undefined;
    var gridApi_Logs = undefined;

    defineScope();
    load();

    function defineScope() {

        $scope.details = [];
        $scope.logs = [];

        $scope.gridReady = function (api) {
            gridApi = api;

            if ($scope.caseOccurancesSelected)
                return retrieveData();
        }

        $scope.gridReady_Logs = function (api) {
            gridApi_Logs = api;

            if ($scope.caseLogsSelected)
                return retrieveData_Logs();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredDetailsByCaseID(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {

                        var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", item.SuspicionLevelID);
                        item.SuspicionLevelDescription = suspicionLevel.description;

                        var detailStatus = UtilsService.getEnum(SuspicionOccuranceStatusEnum, "value", item.SuspicionOccuranceStatus);
                        item.SuspicionOccuranceStatusDescription = detailStatus.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.dataRetrievalFunction_Logs = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredAccountCaseLogsByCaseID(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {

                        var user = UtilsService.getItemByVal($scope.viewScope.users, item.UserID, "UserId");
                        item.UserName = (user != null) ? user.Name : "System";

                        var caseStatus = UtilsService.getEnum(CaseStatusEnum, "value", item.AccountCaseStatusID);
                        item.AccountCaseStatusDescription = caseStatus.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.onSelectedTabChanged = function () {

            if ($scope.selectedTabIndex == 0)
                return retrieveData();
            else if ($scope.selectedTabIndex == 1)
                return retrieveData_Logs();
        }
    }

    function load() {
    }

    function retrieveData() {

        var query = {
            AccountNumber: $scope.dataItem.AccountNumber,
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
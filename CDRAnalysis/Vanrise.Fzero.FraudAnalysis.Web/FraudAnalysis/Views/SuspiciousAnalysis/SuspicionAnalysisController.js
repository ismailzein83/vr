"use strict";

SuspicionAnalysisController.$inject = ["$scope", "CaseManagementAPIService", "StrategyAPIService", "SuspicionLevelEnum", "CaseStatusEnum", "UtilsService", "VRNotificationService", "VRModalService", "VRNavigationService"];

function SuspicionAnalysisController($scope, CaseManagementAPIService, StrategyAPIService, SuspicionLevelEnum, CaseStatusEnum, UtilsService, VRNotificationService, VRModalService, VRNavigationService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.accountNumber = undefined;

        $scope.executionDate = Date.now();

        $scope.strategies = [];
        $scope.selectedStrategies = [];

        $scope.accountStatuses = [];
        $scope.selectedAccountStatuses = [];

        $scope.suspicionLevels = [];
        $scope.selectedSuspicionLevels = [];

        $scope.accountSuspicionSummaries = [];
        $scope.gridMenuActions = [];

        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredAccountSuspicionSummaries(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {
                        var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", item.SuspicionLevelID);
                        item.SuspicionLevelDescription = suspicionLevel.description;

                        var accountStatus = UtilsService.getEnum(CaseStatusEnum, "value", item.AccountStatusID);
                        item.AccountStatusDescription = accountStatus.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        defineMenuActions();
    }

    function retrieveData() {
        var query = {
            AccountNumber: ($scope.accountNumber != undefined && $scope.accountNumber != "") ? $scope.accountNumber : null,
            ExecutionDate: $scope.executionDate,
            StrategyIDs: UtilsService.getPropValuesFromArray($scope.selectedStrategies, "value"),
            AccountStatusIDs: UtilsService.getPropValuesFromArray($scope.selectedAccountStatuses, "value"),
            SuspicionLevelIDs: UtilsService.getPropValuesFromArray($scope.selectedSuspicionLevels, "value")
        };

        return gridAPI.retrieveData(query);
    }

    function load() {
        $scope.isInitializing = true;

        $scope.suspicionLevels = UtilsService.getArrayEnum(SuspicionLevelEnum);
        $scope.accountStatuses = UtilsService.getArrayEnum(CaseStatusEnum);
        $scope.selectedAccountStatuses.push($scope.accountStatuses[0]); // select the Open status by default

        return StrategyAPIService.GetStrategies(0, "") // get all the enabled and disabled strategies (2nd arg) for all periods (1st arg)
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.strategies.push({
                        value: item.Id,
                        description: item.Name
                    });
                });
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
            clicked: detailFraudResult
        }];
    }

    function detailFraudResult(gridObject) {
        var modalSettings = {};

        var parameters = {
            AccountNumber: gridObject.AccountNumber,
            ExecutionDate: $scope.executionDate
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details";
            modalScope.onAccountCaseUpdated = function (accountSuspicionSummary) {
                gridAPI.itemUpdated(accountSuspicionSummary);
            }
        };

        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/SuspiciousNumberDetails.html", parameters, modalSettings);
    }
}

appControllers.controller("FraudAnalysis_SuspicionAnalysisController", SuspicionAnalysisController);

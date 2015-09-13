"use strict";

SuspicionAnalysisController.$inject = ["$scope", "CaseManagementAPIService", "StrategyAPIService", "SuspicionLevelEnum", "CaseStatusEnum", "UtilsService", "VRNotificationService", "VRModalService", "VRNavigationService"];

function SuspicionAnalysisController($scope, CaseManagementAPIService, StrategyAPIService, SuspicionLevelEnum, CaseStatusEnum, UtilsService, VRNotificationService, VRModalService, VRNavigationService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.accountNumber = undefined;

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.strategies = [];
        $scope.selectedStrategies = [];

        $scope.accountStatuses = UtilsService.getArrayEnum(CaseStatusEnum);
        $scope.selectedAccountStatuses = [$scope.accountStatuses[0]]; // select the Open status by default

        $scope.suspicionLevels = UtilsService.getArrayEnum(SuspicionLevelEnum);
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
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            StrategyIDs: UtilsService.getPropValuesFromArray($scope.selectedStrategies, "value"),
            AccountStatusIDs: UtilsService.getPropValuesFromArray($scope.selectedAccountStatuses, "value"),
            SuspicionLevelIDs: UtilsService.getPropValuesFromArray($scope.selectedSuspicionLevels, "value")
        };

        return gridAPI.retrieveData(query);
    }

    function load() {
        $scope.isInitializing = true;
                
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
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
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

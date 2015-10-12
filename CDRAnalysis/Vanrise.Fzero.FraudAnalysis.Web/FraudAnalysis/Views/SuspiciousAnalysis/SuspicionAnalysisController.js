"use strict";

SuspicionAnalysisController.$inject = ["$scope", "CaseManagementAPIService", "StrategyAPIService", "SuspicionLevelEnum", "CaseStatusEnum", "LabelColorsEnum", "UtilsService", "VRNotificationService", "VRModalService", "VRNavigationService"];

function SuspicionAnalysisController($scope, CaseManagementAPIService, StrategyAPIService, SuspicionLevelEnum, CaseStatusEnum, LabelColorsEnum, UtilsService, VRNotificationService, VRModalService, VRNavigationService) {

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

                        if (item.SuspicionLevelID != 0) {
                            var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", item.SuspicionLevelID);
                            item.SuspicionLevelDescription = suspicionLevel.description;
                        }
                        
                        var accountStatus = UtilsService.getEnum(CaseStatusEnum, "value", item.AccountStatusID);
                        item.AccountStatusDescription = accountStatus.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.summaryClicked = function (dataItem) {
            openSummaryDetails(dataItem.AccountNumber);
        }

        $scope.getSuspicionLevelColor = function (dataItem) {
            
            if (dataItem.SuspicionLevelID == SuspicionLevelEnum.Suspicious.value) return LabelColorsEnum.WarningLevel1.color;
            else if (dataItem.SuspicionLevelID == SuspicionLevelEnum.HighlySuspicious.value) return LabelColorsEnum.WarningLevel2.color;
            else if (dataItem.SuspicionLevelID == SuspicionLevelEnum.Fraud.value) return LabelColorsEnum.Error.color;
        }

        $scope.getCaseStatusColor = function (dataItem) {
            
            if (dataItem.AccountStatusID == CaseStatusEnum.Open.value) return LabelColorsEnum.New.color;
            else if (dataItem.AccountStatusID == CaseStatusEnum.Pending.value) return LabelColorsEnum.Processing.color;
            else if (dataItem.AccountStatusID == CaseStatusEnum.ClosedFraud.value) return LabelColorsEnum.Error.color;
            else if (dataItem.AccountStatusID == CaseStatusEnum.ClosedWhitelist.value) return LabelColorsEnum.Success.color;
        }
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

    function openSummaryDetails(accountNumber) {
        var modalSettings = {};

        var parameters = {
            AccountNumber: accountNumber,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            ModalLevel: 1
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details: " + accountNumber;

            modalScope.onAccountCaseUpdated = function (accountSuspicionSummary) {
                //if (accountSuspicionSummary.AccountNumber != null)
                    gridAPI.itemUpdated(accountSuspicionSummary);
            }
        };

        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/SuspiciousNumberDetails.html", parameters, modalSettings);
    }
}

appControllers.controller("FraudAnalysis_SuspicionAnalysisController", SuspicionAnalysisController);

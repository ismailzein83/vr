"use strict";

SuspicionAnalysis2Controller.$inject = ["$scope", "SuspicionAnalysisAPIService", "StrategyAPIService", "SuspicionLevelEnum2", "CaseStatusEnum2", "UtilsService", "VRNotificationService", "VRModalService", "VRNavigationService"];

function SuspicionAnalysis2Controller($scope, SuspicionAnalysisAPIService, StrategyAPIService, SuspicionLevelEnum2, CaseStatusEnum2, UtilsService, VRNotificationService, VRModalService, VRNavigationService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.accountNumber = undefined;

        //$scope.from = Date.now();
        //$scope.to = Date.now();
        $scope.from = "01/01/2014 00:00";
        $scope.to = "01/01/2014 00:00";

        $scope.strategies = [];
        $scope.selectedStrategies = [];

        $scope.suspicionLevels = [];
        $scope.selectedSuspicionLevels = [];

        $scope.caseStatuses = [];
        $scope.selectedCaseStatuses = [];

        $scope.fraudResults = [];
        $scope.gridMenuActions = [];

        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SuspicionAnalysisAPIService.GetFilteredAccountSuspicionSummaries(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);

                    angular.forEach(response.Data, function (item) {
                        var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum2, "value", item.SuspicionLevelID);
                        item.SuspicionLevelDescription = suspicionLevel.description;

                        var caseStatus = UtilsService.getEnum(CaseStatusEnum2, "value", item.CaseStatusID);
                        item.CaseStatusDescription = caseStatus.description;
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
            AccountNumber : $scope.accountNumber,
            From: $scope.from,
            To: $scope.to,
            SelectedStrategyIDs: UtilsService.getPropValuesFromArray($scope.selectedStrategies, "value"),
            SelectedSuspicionLevelIDs: UtilsService.getPropValuesFromArray($scope.selectedSuspicionLevels, "value"),
            SelectedCaseStatusIDs: UtilsService.getPropValuesFromArray($scope.selectedCaseStatuses, "value")
        };

        console.log(query);
        return gridAPI.retrieveData(query);
    }

    function load() {
        $scope.isInitializing = true;

        $scope.suspicionLevels = UtilsService.getArrayEnum(SuspicionLevelEnum2);
        $scope.caseStatuses = UtilsService.getArrayEnum(CaseStatusEnum2);

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

        var parameters = {
            AccountNumber: gridObject.AccountNumber,
            SuspicionLevelDescription: gridObject.SuspicionLevelDescription,
            From: $scope.from,
            To: $scope.to
        };

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details";
            modalScope.onAccountCaseUpdated = function (accountCase) {
                gridAPI.itemUpdated(accountCase);
            }
        };

        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousNumberDetails2.html", parameters, settings);
    }
}

appControllers.controller("FraudAnalysis_SuspicionAnalysis2Controller", SuspicionAnalysis2Controller);

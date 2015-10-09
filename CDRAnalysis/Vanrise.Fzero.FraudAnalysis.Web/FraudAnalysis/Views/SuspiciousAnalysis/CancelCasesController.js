"use strict";

CancelCasesController.$inject = ["$scope", "CaseManagementAPIService", "StrategyAPIService", "VRNotificationService", "LabelColorsEnum", "UtilsService", "CaseStatusEnum"];

function CancelCasesController($scope, CaseManagementAPIService, StrategyAPIService, VRNotificationService, LabelColorsEnum, UtilsService, CaseStatusEnum) {

    var gridAPI;

    definescope();
    load();

    function definescope() {

        var yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);

        $scope.from = yesterday;
        $scope.to = new Date();

        $scope.strategies = [];
        $scope.selectedStrategies = [];

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.cases = [];

        $scope.selectedCases = [];


        $scope.onGridReady = function (api) {
            gridAPI = api;
        }



        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredCasesByFilters(dataRetrievalInput)
            .then(function (response) {

                angular.forEach(response.Data, function (item) {
                    var caseStatus = UtilsService.getEnum(CaseStatusEnum, "value", item.StatusID);
                    item.CaseStatusDescription = caseStatus.description;
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }



        $scope.getCaseStatusColor = function (dataItem) {

            if (dataItem.StatusID == CaseStatusEnum.Open.value) return LabelColorsEnum.New.color;
            else if (dataItem.StatusID == CaseStatusEnum.Pending.value) return LabelColorsEnum.Processing.color;
            else if (dataItem.StatusID == CaseStatusEnum.ClosedFraud.value) return LabelColorsEnum.Error.color;
            else if (dataItem.StatusID == CaseStatusEnum.ClosedWhitelist.value) return LabelColorsEnum.Success.color;
        }

        $scope.cancelClicked = function () {
            return cancelCases();
        }

        $scope.cancelSelectedClicked = function () {
            return cancelSelectedCases();
        }


        


        


    }


    function retrieveData() {

        var query = buildAccountCaseObjectFromScope();

        return gridAPI.retrieveData(query);
    }


    function cancelCases() {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    var accountCaseObject = buildAccountCaseObjectFromScope();
                    return CaseManagementAPIService.CancelAccountCases(accountCaseObject)
                                          .then(function (response) {
                                              if (VRNotificationService.notifyOnItemUpdated("Account Cases", response)) {
                                              }
                                          }).catch(function (error) {
                                              VRNotificationService.notifyException(error, $scope);
                                          });
                }
            });
    }

    function cancelSelectedCases() {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    var selectedCaseIDs = [];

                    angular.forEach($scope.selectedCases, function (item) {
                        selectedCaseIDs.push(item.CaseID);
                    });
                   
                    console.log(selectedCaseIDs)
                    return CaseManagementAPIService.CancelSelectedAccountCases(selectedCaseIDs)
                                          .then(function (response) {
                                              if (VRNotificationService.notifyOnItemUpdated("Account Cases", response)) {
                                              }
                                          }).catch(function (error) {
                                              VRNotificationService.notifyException(error, $scope);
                                          });
                }
            });
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

    function buildAccountCaseObjectFromScope() {

        var accountCaseObject = {
            StrategyIds: UtilsService.getPropValuesFromArray($scope.selectedStrategies, "value"),
            AccountNumber: $scope.accountNumber,
            From: $scope.from,
            To: $scope.to
        };
        return accountCaseObject;
    }

    $scope.checkIfItemsSelected = function () {
        $scope.selectedCases.length = 0;
        angular.forEach($scope.cases, function (item) {
            if (item.isSelected)
                $scope.selectedCases.push(item);
        });
    }

}

appControllers.controller("FraudAnalysis_CancelCasesController", CancelCasesController);

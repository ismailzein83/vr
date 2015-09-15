"use strict";

CancelCasesController.$inject = ["$scope", "CaseManagementAPIService", "StrategyAPIService", "VRNotificationService"];

function CancelCasesController($scope, CaseManagementAPIService, StrategyAPIService, VRNotificationService) {
    definescope();
    load();

    function definescope() {
        $scope.strategies = [];
        $scope.selectedStrategy = '';

        $scope.cancelClicked = function () {
            return cancelCases();
        }

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

        var selectedStrategyId = 0

        if ($scope.selectedStrategy != undefined)
            selectedStrategyId = $scope.selectedStrategy.value;

        var accountCaseObject = {
            StrategyId: selectedStrategyId,
            AccountNumber: $scope.accountNumber,
            From: $scope.from,
            To: $scope.to
        };
        return accountCaseObject;
    }
}

appControllers.controller("FraudAnalysis_CancelCasesController", CancelCasesController);

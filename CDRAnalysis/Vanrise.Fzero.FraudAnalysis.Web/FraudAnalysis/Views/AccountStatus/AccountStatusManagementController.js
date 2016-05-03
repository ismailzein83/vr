(function (appControllers) {

    "use strict";

    accountStatusManagementController.$inject = ['$scope', "Fzero_FraudAnalysis_AccountStatusAPIService", "Fzero_FraudAnalysis_AccountStatusService", "CDRAnalysis_FA_CaseStatusEnum", "VRValidationService"];

    function accountStatusManagementController($scope, Fzero_FraudAnalysis_AccountStatusAPIService, Fzero_FraudAnalysis_AccountStatusService, CDRAnalysis_FA_CaseStatusEnum, VRValidationService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            var Now = new Date();

            var Yesterday = new Date();
            Yesterday.setDate(Yesterday.getDate() - 1);
            $scope.accountNumber;
            $scope.fromDate = Yesterday;
            $scope.toDate = Now;

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getQuery());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getQuery());
            }

            $scope.addNewAccountStatus = addNewAccountStatus;
            $scope.hasAddAccountStatusPermission = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.HasAddAccountStatusPermission();
            };


            $scope.uploadAccountStatuses = function () {

                Fzero_FraudAnalysis_AccountStatusService.uploadAccountStatuses();
            }
            $scope.hasUploadAccountStatusPermission = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.HasUploadAccountStatusPermission();
            };
        }

        function load() {
        }

        function getQuery() {
            var query = {
                Status: CDRAnalysis_FA_CaseStatusEnum.ClosedWhitelist.value,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                AccountNumber: ($scope.accountNumber == undefined ? '' : $scope.accountNumber)
            };
            return query;
        }

        function addNewAccountStatus() {
            var onAccountStatusAdded = function (accountStatusObj) {
                if (gridAPI != undefined) {
                    gridAPI.onAccountStatusAdded(accountStatusObj);
                }
            };

            Fzero_FraudAnalysis_AccountStatusService.addAccountStatus(onAccountStatusAdded);
        }
    }

    appControllers.controller('FraudAnalysis_AccountStatusManagementController', accountStatusManagementController);
})(appControllers);
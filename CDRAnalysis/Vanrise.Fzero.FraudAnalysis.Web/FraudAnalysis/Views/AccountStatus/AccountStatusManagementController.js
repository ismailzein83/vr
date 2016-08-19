(function (appControllers) {

    "use strict";

    accountStatusManagementController.$inject = ['$scope', "Fzero_FraudAnalysis_AccountStatusAPIService", "Fzero_FraudAnalysis_AccountStatusService", "CDRAnalysis_FA_CaseStatusEnum", "VRValidationService", "CDRAnalysis_FA_AccountStatusSourceEnum", "UtilsService", "VRUIUtilsService", "PeriodEnum"];

    function accountStatusManagementController($scope, Fzero_FraudAnalysis_AccountStatusAPIService, Fzero_FraudAnalysis_AccountStatusService, CDRAnalysis_FA_CaseStatusEnum, VRValidationService, CDRAnalysis_FA_AccountStatusSourceEnum, UtilsService, VRUIUtilsService, PeriodEnum) {
        var gridAPI;
        var userSelectorAPI;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        

        defineScope();
        load();

        function defineScope() {
            $scope.accountNumber;
            $scope.fromDate = new Date(new Date().setHours(0, 0, 0, 0));
            $scope.toDate;

           

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getQuery());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getQuery());
            }

            $scope.onUserSelectorReady = function (api) {
                userSelectorAPI = api;
                userSelectorReadyDeferred.resolve();
            };

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
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStaticSelectors, loadUserSelector]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadStaticSelectors() {
                $scope.sources = UtilsService.getArrayEnum(CDRAnalysis_FA_AccountStatusSourceEnum);
                $scope.selectedSources = [];
            }

            function loadUserSelector() {
                var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                userSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(userSelectorAPI, undefined, userSelectorLoadDeferred);
                });

                return userSelectorLoadDeferred.promise;
            }

        }

       

        function getQuery() {
            var query = {
                Status: CDRAnalysis_FA_CaseStatusEnum.ClosedWhitelist.value,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                AccountNumber: ($scope.accountNumber == undefined ? '' : $scope.accountNumber),
                UserIds: userSelectorAPI.getSelectedIds(),
                Sources: ($scope.selectedSources.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedSources, "value") : null,
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
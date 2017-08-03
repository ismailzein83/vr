(function (appControllers) {

    "use strict";

    accountStatusEditorController.$inject = ['$scope', "Fzero_FraudAnalysis_AccountStatusAPIService", 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CDRAnalysis_FA_CaseStatusEnum', 'VRValidationService'];

    function accountStatusEditorController($scope, Fzero_FraudAnalysis_AccountStatusAPIService, VRNotificationService, VRNavigationService, UtilsService, CDRAnalysis_FA_CaseStatusEnum, VRValidationService) {

        var accountNumber;
        var editMode;
        var accountStatusEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                accountNumber = parameters.AccountNumber;
            }
            editMode = (accountNumber != undefined);
        }
        function defineScope() {
            $scope.disableAccountNumber = editMode;
            $scope.accountNumber = accountNumber;

            $scope.validateIsValidDate = function () {
                return VRValidationService.validateTimeEqualorGreaterthanToday($scope.validTill);
            };

            $scope.saveAccountStatus = function () {
                if (editMode)
                    return updateAccountStatus();
                else
                    return insertAccountStatus();
            };
            $scope.hasSaveAccountStatusPermission = function () {
                if (editMode) {
                    return Fzero_FraudAnalysis_AccountStatusAPIService.HasEditAccountStatusPermission();
                }
                else {
                    return Fzero_FraudAnalysis_AccountStatusAPIService.HasAddAccountStatusPermission();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (editMode) {
                getAccountStatus().then(function () {
                    loadAllControls().finally(function () {
                        accountStatusEntity = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });
                });
            }
            else {
                loadAllControls();
            }
        }

        function getAccountStatus() {
            return Fzero_FraudAnalysis_AccountStatusAPIService.GetAccountStatus(accountNumber).then(function (accountStatus) {
                accountStatusEntity = accountStatus;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (editMode && accountStatusEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(accountStatusEntity.AccountNumber, "Account Status");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Account Status");
        }

        function buildAccountStatusObjFromScope() {
            var obj = {
                AccountNumber: $scope.accountNumber,
                ValidTill: $scope.validTill,
                Status: CDRAnalysis_FA_CaseStatusEnum.ClosedWhitelist.value,
                Reason: $scope.reason
            };
            return obj;
        }

        function loadStaticData() {

            if (accountStatusEntity == undefined)
                return;

            $scope.validTill = accountStatusEntity.ValidTill;
            $scope.reason = accountStatusEntity.Reason;
        }

        function insertAccountStatus() {
            $scope.isLoading = true;

            var accountStatusObject = buildAccountStatusObjFromScope();
            return Fzero_FraudAnalysis_AccountStatusAPIService.AddAccountStatus(accountStatusObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Account Status", response, "Account Number")) {
                    if ($scope.onAccountStatusAdded != undefined)
                        $scope.onAccountStatusAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateAccountStatus() {
            $scope.isLoading = true;

            var accountStatusObject = buildAccountStatusObjFromScope();

            Fzero_FraudAnalysis_AccountStatusAPIService.UpdateAccountStatus(accountStatusObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Account Status", response, "Account Name")) {
                    if ($scope.onAccountStatusUpdated != undefined)
                        $scope.onAccountStatusUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('FraudAnalysis_AccountStatusEditorController', accountStatusEditorController);
})(appControllers);

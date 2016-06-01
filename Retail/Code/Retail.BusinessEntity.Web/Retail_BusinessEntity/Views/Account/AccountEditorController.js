(function (appControllers) {

    'use strict';

    AccountEditorController.$inject = ['$scope', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountTypeEnum', 'UtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountEditorController($scope, Retail_BE_AccountAPIService, Retail_BE_AccountTypeEnum, UtilsService, VRNavigationService, VRNotificationService)
    {
        var isEditMode;

        var accountId;
        var accountEntity;
        var parentAccountId;

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined)
            {
                accountId = parameters.accountId;
                parentAccountId = parameters.parentAccountId;
            }

            isEditMode = (accountId != undefined);
        }

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.accountTypes = UtilsService.getArrayEnum(Retail_BE_AccountTypeEnum);
            $scope.scopeModel.showParentAccountName = (parentAccountId != undefined);

            $scope.scopeModel.save = function ()
            {
                return (isEditMode) ? updateAccount() : insertAccount();
            };

            $scope.hasSaveAccountPermission = function ()
            {
                return (isEditMode) ? VR_Sec_GroupAPIService.HasEditGroupPermission() : VR_Sec_GroupAPIService.HasAddGroupPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load()
        {
            var promises = [];
            $scope.scopeModel.isLoading = true;

            if (isEditMode)
            {
                getAccount().then(function () {
                    loadAllControls().finally(function () {
                        accountEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getAccount() {
            return Retail_BE_AccountAPIService.GetAccount(accountId).then(function (response) {
                accountEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, getParentAccountName, loadStaticData]).catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               }).finally(function () {
                   $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle()
        {
            if (isEditMode) {
                var accountName = (accountEntity != undefined) ? accountEntity.Name : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(accountName, 'Account');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Account');
            }
        }

        function getParentAccountName() {
            if (parentAccountId != undefined) {
                return Retail_BE_AccountAPIService.GetAccountName(parentAccountId).then(function (response) {
                    $scope.scopeModel.parentAccountName = response;
                });
            }
        }

        function loadStaticData()
        {
            if (accountEntity == undefined)
                return;

            $scope.scopeModel.name = accountEntity.Name;
            $scope.scopeModel.selectedAccountType = UtilsService.getItemByVal($scope.scopeModel.accountTypes, accountEntity.Type, 'value');
        }

        function insertAccount()
        {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountAPIService.AddAccount(accountObj).then(function (response)
            {
                if (VRNotificationService.notifyOnItemAdded('Account', response, 'Name'))
                {
                    if ($scope.onAccountAdded != undefined)
                        $scope.onAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateAccount()
        {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountAPIService.UpdateAccount(accountObj).then(function (response)
            {
                if (VRNotificationService.notifyOnItemUpdated('Account', response, 'Name'))
                {
                    if ($scope.onAccountUpdated != undefined) {
                        $scope.onAccountUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAccountObjFromScope()
        {
            var obj = {
                AccountId: accountId,
                Name: $scope.scopeModel.name,
                Type: $scope.scopeModel.selectedAccountType.value,
                Settings: null
            };

            if (!isEditMode) {
                obj.ParentAccountId = parentAccountId;
            }

            return obj;
        }
    }

    appControllers.controller('Retail_BE_AccountEditorController', AccountEditorController);

})(appControllers);
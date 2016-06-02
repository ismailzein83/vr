(function (appControllers) {

    'use strict';

    PackageAssignmentEditorController.$inject = ['$scope', 'Retail_BE_AccountPackageAPIService', 'Retail_BE_AccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function PackageAssignmentEditorController($scope, Retail_BE_AccountPackageAPIService, Retail_BE_AccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService)
    {
        var isEditMode;

        var accountId;
        var accountPackageId;
        var accountPackageEntity;

        var packageSelectorAPI;
        var packageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountId = parameters.accountId;
                accountPackageId = parameters.accountPackageId;
            }

            isEditMode = (accountPackageId != undefined);
        }

        function defineScope()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.bed = new Date();

            $scope.scopeModel.onPackageSelectorReady = function (api) {
                packageSelectorAPI = api;
                packageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccountPackage() : insertAccountPackage();
            };

            $scope.hasSaveAccountPackagePermission = function () {
                return (isEditMode) ? VR_Sec_GroupAPIService.HasEditGroupPermission() : VR_Sec_GroupAPIService.HasAddGroupPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getAccountPackage().then(function () {
                    loadAllControls().finally(function () {
                        accountPackageEntity = undefined;
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

        function getAccountPackage() {
            return Retail_BE_AccountPackageAPIService.GetAccountPackage(accountPackageId).then(function (response) {
                accountPackageEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadPackageSelector, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle()
        {
            if (isEditMode) {
                var accountPackageName = (accountPackageEntity != undefined) ?
                    accountPackageEntity.PackageName + ' for ' + accountPackageEntity.AccountName :
                    undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(accountPackageName, 'Account Package');
            }
            else {
                return Retail_BE_AccountAPIService.GetAccountName(accountId).then(function (response) {
                    var titleExtension = ' for ' + response;
                    $scope.title = UtilsService.buildTitleForAddEditor('Account Package') + titleExtension;
                });
            }
        }

        function loadPackageSelector() {
            var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            packageSelectorReadyDeferred.promise.then(function () {
                var packageSelectorPayload;
                if (accountPackageEntity != undefined) {
                    packageSelectorPayload = {
                        selectedIds: accountPackageEntity.PackageId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
            });

            return packageSelectorLoadDeferred.promise;
        }

        function loadStaticData() {
            if (accountPackageEntity == undefined)
                return;
            $scope.scopeModel.bed = accountEntity.BED;
            $scope.scopeModel.eed = accountEntity.EED;
        }

        function insertAccountPackage()
        {
            $scope.scopeModel.isLoading = true;

            var accountPackageObj = buildAccountPackageObjFromScope();

            return Retail_BE_AccountPackageAPIService.AddAccountPackage(accountPackageObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account Package', response, 'Name')) {
                    if ($scope.onAccountPackageAdded != undefined)
                        $scope.onAccountPackageAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateAccountPackage() {
            $scope.scopeModel.isLoading = true;

            var accountPackageObj = buildAccountPackageObjFromScope();

            return Retail_BE_AccountPackageAPIService.UpdateAccountPackage(accountPackageObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account Package', response, 'Name')) {
                    if ($scope.onAccountPackageUpdated != undefined) {
                        $scope.onAccountPackageUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAccountPackageObjFromScope()
        {
            var obj = {
                AccountPackageId: accountPackageId,
                AccountId: accountId,
                PackageId: packageSelectorAPI.getSelectedIds(),
                BED: $scope.scopeModel.bed,
                EED: $scope.scopeModel.eed
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_PackageAssignmentEditorController', PackageAssignmentEditorController);

})(appControllers);
﻿(function (appControllers) {

    'use strict';

    PackageAssignmentEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VRValidationService', 'Retail_BE_AccountPackageAPIService', 'Retail_BE_AccountBEAPIService'];

    function PackageAssignmentEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VRValidationService, Retail_BE_AccountPackageAPIService, Retail_BE_AccountBEAPIService)
    {
        var isEditMode;

        var accountBEDefinitionId;
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
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountId = parameters.accountId;
                accountPackageId = parameters.accountPackageId;
            }
            isEditMode = (accountPackageId != undefined);
        }
        function defineScope()
        {
            $scope.scopeModel = {};
            var bed = new Date();
            bed.setHours(0, 0, 0, 0);
            $scope.scopeModel.bed = bed;

            $scope.scopeModel.onPackageSelectorReady = function (api) {
                packageSelectorAPI = api;
                packageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.bed, $scope.scopeModel.eed);
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccountPackage() : insertAccountPackage();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModel.packageSelectorDisabled = false;
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                $scope.scopeModel.packageSelectorDisabled = true;
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
            if (isEditMode)
            {
                var accountName;
                var packageName;
                
                if (accountPackageEntity != undefined) {
                    accountName = accountPackageEntity.AccountName;
                    packageName = accountPackageEntity.PackageName;
                }

                $scope.title = 'Edit Package Assignment to Account';

                if (accountName != undefined && packageName != undefined)
                    $scope.title += ': ' + accountName + ' - ' + packageName;
            }
            else {
                return Retail_BE_AccountBEAPIService.GetAccountName(accountBEDefinitionId, accountId).then(function (response) {
                    $scope.title = 'Assign Package for ' + response;
                });
            }
        }
        function loadPackageSelector() {
            var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            packageSelectorReadyDeferred.promise.then(function ()
            {
                var packageSelectorPayload = {
                    filter: {
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.AccountAssignmentPackageFilter, Retail.BusinessEntity.Business",
                            AccountBEDefinitionId: accountBEDefinitionId,
                            AssignedToAccountId: accountId
                        }]
                    }
                };
                if (accountPackageEntity != undefined) {
                    packageSelectorPayload.selectedIds = accountPackageEntity.PackageId;
                    
                }

                VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
            });

            return packageSelectorLoadDeferred.promise;
        }
        function loadStaticData() {
            if (accountPackageEntity == undefined)
                return;

            $scope.scopeModel.bed = accountPackageEntity.BED;
            $scope.scopeModel.eed = accountPackageEntity.EED;
        }

        function insertAccountPackage()
        {
            $scope.scopeModel.isLoading = true;

            var accountPackageObj = buildAccountPackageObjFromScope();

            return Retail_BE_AccountPackageAPIService.AddAccountPackage(accountPackageObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account Package', response)) {
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
        function updateAccountPackage()
        {
            $scope.scopeModel.isLoading = true;

            var accountPackageObj = buildAccountPackageObjFromScope();

            return Retail_BE_AccountPackageAPIService.UpdateAccountPackage(accountPackageObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account Package', response)) {
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
            var obj = {};
            if (isEditMode) {
                obj = {
                    AccountPackageId: accountPackageId,
                    BED: $scope.scopeModel.bed,
                    EED: $scope.scopeModel.eed
                };
            }
            else {
                obj = {
                    AccountPackageId: accountPackageId,
                    AccountBEDefinitionId: accountBEDefinitionId,
                    AccountId: accountId,
                    PackageId: packageSelectorAPI.getSelectedIds(),
                    BED: $scope.scopeModel.bed,
                    EED: $scope.scopeModel.eed
                };
            }
           
            return obj;
        }
    }

    appControllers.controller('Retail_BE_PackageAssignmentEditorController', PackageAssignmentEditorController);

})(appControllers);
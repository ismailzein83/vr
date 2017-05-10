(function (appControllers) {

    'use strict';

    AccountTypeEditorController.$inject = ['$scope', 'Retail_BE_AccountTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountPartAvailabilityOptionsEnum', 'Retail_BE_AccountPartRequiredOptionsEnum', 'Retail_BE_EntityTypeEnum'];

    function AccountTypeEditorController($scope, Retail_BE_AccountTypeAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountPartAvailabilityOptionsEnum, Retail_BE_AccountPartRequiredOptionsEnum, Retail_BE_EntityTypeEnum) {

        var isEditMode;
        var accountBEDefinitionId;
        var accountTypeId;
        var accountTypeEntity;

        var businessEntityDefinitionSelectorAPI;
        var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var businessEntityDefinitionSelectionChangedDeferred;

        var accountTypeSettingsAPI;
        var accountTypeSettingsReadyDeferred = UtilsService.createPromiseDeferred();

      


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountTypeId = parameters.accountTypeId;
            }
            isEditMode = (accountTypeId != undefined);
        }
        function defineScope() {
           $scope.scopeModel = {};
          $scope.scopeModel.showSettings = false;
            

            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                businessEntityDefinitionSelectorAPI = api;
                businessEntityDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSettingsReady = function (api) {
                accountTypeSettingsAPI = api;
              accountTypeSettingsReadyDeferred.resolve();
            };

            

            $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    accountBEDefinitionId = selectedItem.BusinessEntityDefinitionId;
                    $scope.scopeModel.showSettings = true;
                    var businessEntityDefinitionForAccountTypeSettingsDeferred;
                    if (businessEntityDefinitionSelectionChangedDeferred != undefined) {
                        businessEntityDefinitionForAccountTypeSettingsDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([businessEntityDefinitionForAccountTypeSettingsDeferred.promise]).then(function () {
                            businessEntityDefinitionForAccountTypeSettingsDeferred = undefined;
                            if (businessEntityDefinitionSelectionChangedDeferred != undefined)
                                businessEntityDefinitionSelectionChangedDeferred.resolve();
                        });

                    }
                    var accountTypeSettingsPayload = {
                            AccountBEDefinitionId: accountBEDefinitionId
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isAccountTypeSettingsLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountTypeSettingsAPI, accountTypeSettingsPayload, setLoader, businessEntityDefinitionForAccountTypeSettingsDeferred);

                }
            };

            

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccountType() : insertAccountType();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.hasSaveAccountTypePermission = function () {
                return (isEditMode) ? Retail_BE_AccountTypeAPIService.HasUpdateAccountTypePermission() : Retail_BE_AccountTypeAPIService.HasAddAccountTypePermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getAccountType().then(function () {
                    loadAllControls().finally(function () {
                        accountTypeEntity = undefined;
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

        function getAccountType() {
            return Retail_BE_AccountTypeAPIService.GetAccountType(accountTypeId).then(function (response) {
                accountTypeEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode) {
                    var accountTypeName = (accountTypeEntity != undefined) ? accountTypeEntity.Name : undefined;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(accountTypeName, 'Account Type');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Account Type');
                }
            }
            function loadStaticData() {
                if (accountTypeEntity == undefined)
                    return;
                $scope.scopeModel.name = accountTypeEntity.Name;
                $scope.scopeModel.title = accountTypeEntity.Title;

                

            }
            function loadBusinessEntityDefinitionSelector() {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                            }]
                        },
                        selectedIds: accountTypeEntity != undefined ? accountTypeEntity.AccountBEDefinitionId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                });

                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }
            function loadAccountTypeSettings() {
                if (!isEditMode)
                    return;

                if (businessEntityDefinitionSelectionChangedDeferred == undefined)
                {
                    
                    businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                }
                    

                var accountTypeSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitMultiplePromises([accountTypeSettingsReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {
                    businessEntityDefinitionSelectionChangedDeferred = undefined;

                    var accountTypeSettingsPayload = {
                            AccountBEDefinitionId: accountBEDefinitionId
                    };
                    if (accountTypeEntity != undefined && accountTypeEntity.Settings!=null) {
                        accountTypeSettingsPayload.accountTypeSettings = accountTypeEntity.Settings;
                    }
                
                    VRUIUtilsService.callDirectiveLoad(accountTypeSettingsAPI, accountTypeSettingsPayload, accountTypeSettingsLoadDeferred);
                });

                return accountTypeSettingsLoadDeferred.promise;
            }
            

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBusinessEntityDefinitionSelector, loadAccountTypeSettings])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        

        function insertAccountType() {
            $scope.scopeModel.isLoading = true;

            var accountTypeObj = buildAccountTypeObjFromScope();

            return Retail_BE_AccountTypeAPIService.AddAccountType(accountTypeObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account Type', response, 'Name')) {
                    if ($scope.onAccountTypeAdded != undefined)
                        $scope.onAccountTypeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateAccountType() {
            $scope.scopeModel.isLoading = true;

            var accountTypeObj = buildAccountTypeObjFromScope();

            return Retail_BE_AccountTypeAPIService.UpdateAccountType(accountTypeObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account Type', response, 'Name')) {
                    if ($scope.onAccountTypeUpdated != undefined) {
                        $scope.onAccountTypeUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAccountTypeObjFromScope() {
           
            var obj = {
                AccountTypeId: accountTypeId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: accountTypeSettingsAPI.getData(),
                AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_AccountTypeEditorController', AccountTypeEditorController);

})(appControllers);
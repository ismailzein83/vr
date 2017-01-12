(function (appControllers) {

    "use strict";

    accountServiceEditorController.$inject = ['$scope', 'Retail_BE_AccountServiceAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function accountServiceEditorController($scope, Retail_BE_AccountServiceAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var accountServiceId;
        var accountServiceEntity;
        var accountBEDefinitionId;
        var accountId;

        var accountAPI;
        var accountReadyDeferred = UtilsService.createPromiseDeferred();

        var serviceTypeAPI;
        var serviceTypeReadyDeferred = UtilsService.createPromiseDeferred();

        var chargingPolicyAPI;
        var chargingPolicyReadyDeferred = UtilsService.createPromiseDeferred();

        var serviceTypeSelectedPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                accountServiceId = parameters.accountServiceId;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountId = parameters.accountId;
            }

            isEditMode = (accountServiceId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.accountSelectorDisabled = (accountId != undefined || accountServiceId != undefined);
            $scope.scopeModel.hasSaveAccountServicePermission = function () {
                if ($scope.scopeModel.isEditMode)
                    return Retail_BE_AccountServiceAPIService.HasUpdateAccountServicePermission();
                else
                    return Retail_BE_AccountServiceAPIService.HasAddAccountServicePermission();
            };

            $scope.scopeModel.saveAccountService = function () {
                if (isEditMode) {
                    return updateAccountService();
                }
                else {
                    return insertAccountService();
                }
            };

            $scope.scopeModel.onAccountSelectorReady = function (api) {
                accountAPI = api;
                accountReadyDeferred.resolve();
            };

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeAPI = api;
                serviceTypeReadyDeferred.resolve();
            };

            $scope.scopeModel.onChargingPolicySelectorReady = function (api) {
                chargingPolicyAPI = api;
                chargingPolicyReadyDeferred.resolve();
            };

            $scope.scopeModel.onServiceTypeSelectionChanged = function () {
                var serviceTypeId = serviceTypeAPI.getSelectedIds();
                if (serviceTypeId != undefined) {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payloadChargingPolicyDirective = {
                        filter: { ServiceTypeId: serviceTypeId },
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, chargingPolicyAPI, payloadChargingPolicyDirective, setLoader, serviceTypeSelectedPromiseDeferred);
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

        }
        function load() {
            $scope.isLoading = false;

            if (isEditMode) {
                getAccountService().then(function () {
                    loadAllControls()
                        .finally(function () {
                            accountServiceEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getAccountService() {
            return Retail_BE_AccountServiceAPIService.GetAccountService(accountServiceId).then(function (accountServiceObj) {
                accountServiceEntity = accountServiceObj;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadServiceTypeSelector, loadAccountSelector, loadChargingPolicySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(accountServiceEntity ? accountServiceEntity.Name : undefined, 'Account Service') : UtilsService.buildTitleForAddEditor('Account Service');
        }
        function loadStaticSection() {
            if (accountServiceEntity != undefined) {
            }

        }
        function loadServiceTypeSelector() {
            var loadServiceTypeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            if (accountServiceEntity != undefined && accountServiceEntity.ServiceTypeId != undefined)
                serviceTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

            serviceTypeReadyDeferred.promise.then(function () {
                var payloadServiceTypeDirective = {
                    selectedIds: accountServiceEntity != undefined ? accountServiceEntity.ServiceTypeId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(serviceTypeAPI, payloadServiceTypeDirective, loadServiceTypeDirectivePromiseDeferred);
            });
            return loadServiceTypeDirectivePromiseDeferred.promise;
        }
        function loadAccountSelector() {
            var loadAccountDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            accountReadyDeferred.promise.then(function () {
                var payloadAccountDirective = {
                    selectedIds: accountServiceEntity != undefined ? accountServiceEntity.AccountId : accountId != undefined ? accountId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(accountAPI, payloadAccountDirective, loadAccountDirectivePromiseDeferred);
            });
            return loadAccountDirectivePromiseDeferred.promise;
        }
        function loadChargingPolicySelector() {
            if (accountServiceEntity == undefined || accountServiceEntity.ServiceTypeId == undefined)
                return;
            var loadChargingPolicyDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([chargingPolicyReadyDeferred.promise, serviceTypeSelectedPromiseDeferred.promise]).then(function () {
                var payloadChargingPolicyDirective = {
                    selectedIds: accountServiceEntity != undefined ? accountServiceEntity.ServiceChargingPolicyId : undefined,
                    filter: accountServiceEntity != undefined ? { ServiceTypeId: accountServiceEntity.ServiceTypeId } : undefined
                };

                VRUIUtilsService.callDirectiveLoad(chargingPolicyAPI, payloadChargingPolicyDirective, loadChargingPolicyDirectivePromiseDeferred);
                serviceTypeSelectedPromiseDeferred = undefined;
            });
            return loadChargingPolicyDirectivePromiseDeferred.promise;
        }

        function insertAccountService() {
            $scope.isLoading = true;
            return Retail_BE_AccountServiceAPIService.AddAccountService(buildAccountServiceObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("AccountService", response, "Name")) {
                    if ($scope.onAccountServiceAdded != undefined)
                        $scope.onAccountServiceAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function updateAccountService() {
            $scope.isLoading = true;
            return Retail_BE_AccountServiceAPIService.UpdateAccountService(buildAccountServiceObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("AccountService", response, "Name")) {
                    if ($scope.onAccountServiceUpdated != undefined)
                        $scope.onAccountServiceUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildAccountServiceObjFromScope() {
            var obj = {
                AccountServiceId: accountServiceId,
                AccountBEDefinitionId: accountBEDefinitionId,
                AccountId: accountId != undefined ? accountId : accountAPI.getSelectedIds(),
                ServiceTypeId: serviceTypeAPI.getSelectedIds(),
                ServiceChargingPolicyId: chargingPolicyAPI.getSelectedIds(),
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_AccountServiceEditorController', accountServiceEditorController);
})(appControllers);

(function (appControllers) {

    'use strict';

    financialAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_FinancialAccountAPIService'];

    function financialAccountEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_FinancialAccountAPIService) {
        var accountId;
        var accountBEDefinitionId;

        var sequenceNumber;
        var financialAccountDefinitionSelectorDirectiveAPI;
        var financialAccountDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        var financialAccountEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountId = parameters.accountId;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                sequenceNumber = parameters.sequenceNumber;
            }
            $scope.scopeModel.isEditMode = (sequenceNumber != undefined);

        }

        function defineScope() {
            $scope.scopeModel.beginEffectiveDate = UtilsService.getDateFromDateTime(new Date());

            $scope.scopeModel.onFinancialAccountDefinitionSelectorReady = function (api) {
                financialAccountDefinitionSelectorDirectiveAPI = api;
                financialAccountDefinitionSelectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.validateDates = function (date) {
                return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };
            $scope.scopeModel.validateEEDDate = function (date) {
                if (!$scope.scopeModel.disableEED && $scope.scopeModel.endEffectiveDate != undefined && UtilsService.getDateFromDateTime($scope.scopeModel.endEffectiveDate) < UtilsService.getDateFromDateTime(new Date())) {
                    return "EED must not be less than today.";
                }
                return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };

            $scope.scopeModel.save = function () {
                if ($scope.scopeModel.isEditMode)
                    return updateFinancialAccount();
                else {
                    return insertFinancialAccount();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if ($scope.scopeModel.isEditMode) {
                getFinancialAccount().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            } else {
                loadAllControls();
            }
        }

        function getFinancialAccount() {
            return Retail_BE_FinancialAccountAPIService.GetFinancialAccountEditorRuntime(accountBEDefinitionId, accountId, sequenceNumber).then(function (response) {
                financialAccountEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                //if (isEditMode)
                //    $scope.title = UtilsService.buildTitleForUpdateEditor(response, 'Financial Account', $scope);
                //else
                //    $scope.title = UtilsService.buildTitleForAddEditor('Financial Account');
                $scope.title = 'Financial Account';
            }

            function loadStaticData() {
                if (financialAccountEntity != undefined) {
                    if (financialAccountEntity.FinancialAccount != undefined) {
                        $scope.scopeModel.beginEffectiveDate = financialAccountEntity.FinancialAccount.BED;
                        $scope.scopeModel.endEffectiveDate = financialAccountEntity.FinancialAccount.EED;
                        $scope.scopeModel.disableEED = (financialAccountEntity.FinancialAccount.EED != undefined && UtilsService.getDateFromDateTime(financialAccountEntity.FinancialAccount.EED) < UtilsService.getDateFromDateTime(new Date()));
                    }
                    if (financialAccountEntity.HasFinancialTransactions) {
                        $scope.scopeModel.disableAccountTypeAndBED = true;
                    }
                }
            }

            function loadFinancialAccountDefinitionSelector() {
                var loadFinancialAccountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                financialAccountDefinitionSelectorDirectiveReadyDeferred.promise.then(function () {
                    var payload = {
                        //filter: {
                        //    Filters: [{
                        //        $type: "Retail.BE.AccountBalance.Business.FinancialAccountTypeFilter, Retail.BE.AccountBalance.Business",
                        //        IsEditMode: $scope.scopeModel.isEditMode
                        //    }]
                        //},
                        selectedIds: financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined ? financialAccountEntity.FinancialAccount.FinancialAccountDefinitionId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(financialAccountDefinitionSelectorDirectiveAPI, payload, loadFinancialAccountDefinitionSelectorPromiseDeferred);
                });
                return loadFinancialAccountDefinitionSelectorPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFinancialAccountDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertFinancialAccount() {
            $scope.scopeModel.isLoading = true;

            var financialAccountObj = buildFinancialAccountObjFromScope();

            return Retail_BE_FinancialAccountAPIService.AddFinancialAccount(financialAccountObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Financial Account', response, 'Name')) {
                    if ($scope.onFinancialAccountAdded != undefined)
                        $scope.onFinancialAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateFinancialAccount() {
            $scope.scopeModel.isLoading = true;
            var financialAccountObj = buildFinancialAccountObjFromScope();
            return Retail_BE_FinancialAccountAPIService.UpdateFinancialAccount(financialAccountObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Financial Account', response, 'Name')) {
                    if ($scope.onFinancialAccountUpdated != undefined)
                        $scope.onFinancialAccountUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildFinancialAccountObjFromScope() {
            var obj = {
                FinancialAccount: {
                    SequenceNumber: sequenceNumber,
                    FinancialAccountDefinitionId: financialAccountDefinitionSelectorDirectiveAPI.getSelectedIds(),
                    BED: $scope.scopeModel.beginEffectiveDate,
                    EED: $scope.scopeModel.endEffectiveDate
                },
                AccountId: accountId,
                AccountBEDefinitionId: accountBEDefinitionId
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_FinancialAccountEditorController', financialAccountEditorController);

})(appControllers);
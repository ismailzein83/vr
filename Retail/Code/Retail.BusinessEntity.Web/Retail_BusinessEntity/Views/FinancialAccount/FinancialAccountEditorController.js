(function (appControllers) {

    'use strict';

    financialAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_FinancialAccountAPIService', 'Retail_BE_FinancialAccountDefinitionAPIService', 'VRDateTimeService'];

    function financialAccountEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_FinancialAccountAPIService, Retail_BE_FinancialAccountDefinitionAPIService, VRDateTimeService) {
        var accountId;
        var accountBEDefinitionId;

        var sequenceNumber;
        var financialAccountDefinitionSelectorDirectiveAPI;
        var financialAccountDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        var financialAccountEntity;

        var directiveAPI;
        var directiveReadyDeferred;


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
            $scope.scopeModel.beginEffectiveDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

            $scope.scopeModel.onFinancialAccountDefinitionSelectorReady = function (api) {
                financialAccountDefinitionSelectorDirectiveAPI = api;
                financialAccountDefinitionSelectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onFinancialAccountDefinitionSelectionChanged = function (value) {
                if (value != undefined) {
                    $scope.scopeModel.isLoading = true;
                    getFinancialAccountDefinitionSetting().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                } else {
                    $scope.scopeModel.financialAccountTypeRuntimeDirective = undefined;
                }
            };
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var directivePayload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };
            $scope.scopeModel.validateDates = function (date) {
                return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };
            $scope.scopeModel.validateEEDDate = function (date) {
                if (!$scope.scopeModel.disableEED && $scope.scopeModel.endEffectiveDate != undefined && UtilsService.getDateFromDateTime($scope.scopeModel.endEffectiveDate) < UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())) {
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

        function getFinancialAccountDefinitionSetting() {
            var selectedfinancialAccountDefinitionId = financialAccountDefinitionSelectorDirectiveAPI.getSelectedIds();
            return Retail_BE_FinancialAccountDefinitionAPIService.GetFinancialAccountDefinitionSettings(selectedfinancialAccountDefinitionId).then(function (response) {
                if (response != undefined && response.ExtendedSettings != undefined)
                    $scope.scopeModel.financialAccountTypeRuntimeDirective = response.ExtendedSettings.RuntimeEditor;
            });
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
                        $scope.scopeModel.disableEED = (financialAccountEntity.FinancialAccount.EED != undefined && UtilsService.getDateFromDateTime(financialAccountEntity.FinancialAccount.EED) < UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()));
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
                        filter: {
                            AccountBEDefinitionId: accountBEDefinitionId,
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.FinancialAccountDefinitionFilter ,Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: accountBEDefinitionId,
                                AccountId: accountId,
                                SequenceNumber: sequenceNumber
                            }]
                        },
                        selectedIds: financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined ? financialAccountEntity.FinancialAccount.FinancialAccountDefinitionId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(financialAccountDefinitionSelectorDirectiveAPI, payload, loadFinancialAccountDefinitionSelectorPromiseDeferred);
                });
                return loadFinancialAccountDefinitionSelectorPromiseDeferred.promise;
            }

            function loadDirective() {
                if (financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined && financialAccountEntity.FinancialAccount.ExtendedSettings != undefined) {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { extendedSettings: financialAccountEntity.FinancialAccount.ExtendedSettings};
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFinancialAccountDefinitionSelector, loadDirective]).catch(function (error) {
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
                    EED: $scope.scopeModel.endEffectiveDate,
                    ExtendedSettings: directiveAPI.getData()
                },
                AccountId: accountId,
                AccountBEDefinitionId: accountBEDefinitionId
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_FinancialAccountEditorController', financialAccountEditorController);

})(appControllers);
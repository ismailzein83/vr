(function (appControllers) {

    'use strict';

    financialAccountEditorController.$inject = ['$scope', 'WhS_AccountBalance_FinancialAccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','VR_AccountBalance_AccountTypeAPIService'];

    function financialAccountEditorController($scope, WhS_AccountBalance_FinancialAccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_AccountBalance_AccountTypeAPIService) {
        var carrierAccountId;
        var carrierProfileId;
     
        var financialAccountId;
        var financialAccountTypeSelectorDirectiveAPI;
        var financialAccountTypeSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
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
                carrierAccountId = parameters.carrierAccountId;
                carrierProfileId = parameters.carrierProfileId;
                financialAccountId = parameters.financialAccountId;
            }
            $scope.scopeModel.isEditMode = (financialAccountId != undefined);

        }

        function defineScope() {
            $scope.scopeModel.beginEffectiveDate = UtilsService.getDateFromDateTime(new Date());

            $scope.scopeModel.onFinancialAccountTypeSelectorReady = function (api) {
                financialAccountTypeSelectorDirectiveAPI = api;
                financialAccountTypeSelectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onFinancialAccountTypeSelectionChanged = function (value) {
                if(value != undefined)
                {
                    $scope.scopeModel.isLoading = true;
                    getFinancialAccountTypeSetting().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }else
                {
                    $scope.scopeModel.financialAccountTypeRuntimeDirective = undefined;
                }
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var directivePayload;
                if (financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined) {
                    directivePayload = {
                        carrierProfileId: financialAccountEntity.FinancialAccount.CarrierProfileId,
                        carrierAccountId: financialAccountEntity.FinancialAccount.CarrierAccountId
                    };
                } else {
                    directivePayload = { carrierProfileId: carrierProfileId, carrierAccountId: carrierAccountId };
                }
               
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.validateDates = function (date) {
                return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };
            $scope.scopeModel.validateEEDDate = function (date) {
                //if (!$scope.scopeModel.disableEED && $scope.scopeModel.endEffectiveDate != undefined && UtilsService.getDateFromDateTime($scope.scopeModel.endEffectiveDate) < UtilsService.getDateFromDateTime(new Date()))
                //{
                //    return "EED must not be less than today.";
                //}
                return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };
            $scope.scopeModel.save = function () {
                if ($scope.scopeModel.isEditMode)
                    return updateFinancialAccount();
                else
                {
                    return insertFinancialAccount();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if ($scope.scopeModel.isEditMode)
            {
                getFinancialAccount().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });;
            }else
            {
                loadAllControls();
            }
        }

        function getFinancialAccount() {
            return WhS_AccountBalance_FinancialAccountAPIService.GetFinancialAccountEditorRuntime(financialAccountId).then(function (response) {
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
                if(financialAccountEntity != undefined)
                {
                    if (financialAccountEntity.FinancialAccount != undefined)
                    {
                        $scope.scopeModel.beginEffectiveDate = financialAccountEntity.FinancialAccount.BED;
                        $scope.scopeModel.endEffectiveDate = financialAccountEntity.FinancialAccount.EED;
                        $scope.scopeModel.disableEED = (financialAccountEntity.FinancialAccount.EED != undefined && UtilsService.getDateFromDateTime(financialAccountEntity.FinancialAccount.EED) < UtilsService.getDateFromDateTime(new Date()));
                    }
                    if(financialAccountEntity.HasFinancialTransactions)
                    {
                        $scope.scopeModel.disableAccountTypeAndBED = true;
                    }
                }
            }

            function loadFinancialAccountTypeSelectorSelector() {
                var loadFinancialAccountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                financialAccountTypeSelectorDirectiveReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "TOne.WhS.AccountBalance.Business.FinancialAccountTypeFilter, TOne.WhS.AccountBalance.Business",
                                CarrierProfileId: financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined ? financialAccountEntity.FinancialAccount.CarrierProfileId : carrierProfileId,
                                CarrierAccountId: financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined ? financialAccountEntity.FinancialAccount.CarrierAccountId : carrierAccountId,
                                IsEditMode: $scope.scopeModel.isEditMode
                            }]
                        },
                       selectedIds: financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined && financialAccountEntity.FinancialAccount.Settings != undefined ? financialAccountEntity.FinancialAccount.Settings.AccountTypeId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(financialAccountTypeSelectorDirectiveAPI, payload, loadFinancialAccountTypeSelectorPromiseDeferred);
                });
                return loadFinancialAccountTypeSelectorPromiseDeferred.promise;
            }
            
            function loadDirective() {
                if (financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined && financialAccountEntity.FinancialAccount.Settings != undefined)
                {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { extendedSettings: financialAccountEntity.FinancialAccount.Settings.ExtendedSettings, carrierProfileId: financialAccountEntity.FinancialAccount.CarrierProfileId, carrierAccountId: financialAccountEntity.FinancialAccount.CarrierAccountId };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }
               
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFinancialAccountTypeSelectorSelector, loadDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    
        function getFinancialAccountTypeSetting()
        {
            var selectedfinancialAccountTypeId = financialAccountTypeSelectorDirectiveAPI.getSelectedIds();
            return VR_AccountBalance_AccountTypeAPIService.GetAccountTypeSettings(selectedfinancialAccountTypeId).then(function (response) {
                if (response != undefined && response.ExtendedSettings != undefined)
                    $scope.scopeModel.financialAccountTypeRuntimeDirective = response.ExtendedSettings.RuntimeEditor;
            });
        }

        function insertFinancialAccount() {
            $scope.scopeModel.isLoading = true;

            var financialAccountObj = buildFinancialAccountObjFromScope();

            return WhS_AccountBalance_FinancialAccountAPIService.AddFinancialAccount(financialAccountObj).then(function (response) {
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

            return WhS_AccountBalance_FinancialAccountAPIService.UpdateFinancialAccount(financialAccountObj).then(function (response) {
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
                FinancialAccountId:financialAccountId,
                CarrierAccountId: financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined ? financialAccountEntity.FinancialAccount.CarrierAccountId : carrierAccountId,
                CarrierProfileId: financialAccountEntity != undefined && financialAccountEntity.FinancialAccount != undefined ? financialAccountEntity.FinancialAccount.CarrierProfileId : carrierProfileId,
                Settings: {
                    AccountTypeId: financialAccountTypeSelectorDirectiveAPI.getSelectedIds(),
                    ExtendedSettings: directiveAPI.getData()
                },
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate
            };
            return obj;
        }
    }

    appControllers.controller('WhS_AccountBalance_FinancialAccountEditorController', financialAccountEditorController);

})(appControllers);
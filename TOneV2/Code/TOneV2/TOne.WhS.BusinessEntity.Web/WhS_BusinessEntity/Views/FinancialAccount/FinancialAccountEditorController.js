(function (appControllers) {

    'use strict';

    financialAccountEditorController.$inject = ['$scope', 'WhS_BE_FinancialAccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_FinancialAccountDefinitionAPIService'];

    function financialAccountEditorController($scope, WhS_BE_FinancialAccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_FinancialAccountDefinitionAPIService) {
        var carrierAccountId;
        var carrierProfileId;

        var financialAccountId;
        var financialAccountEntity;

        var isEditMode;

        var financialAccountDefinitionSelectorDirectiveAPI;
        var financialAccountDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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
            isEditMode = (financialAccountId != undefined);

        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.beginEffectiveDate = UtilsService.getDateFromDateTime(new Date());

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
                    $scope.scopeModel.financialAccountDefinitionRuntimeDirective = undefined;
                }
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var directivePayload;
                if (financialAccountEntity != undefined) {
                    directivePayload = {
                        carrierProfileId: financialAccountEntity.CarrierProfileId,
                        carrierAccountId: financialAccountEntity.CarrierAccountId
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
                return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
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

            if (isEditMode) {
                getFinancialAccount().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });;
            } else {
                loadAllControls();
            }
        }

        function getFinancialAccount() {
            return WhS_BE_FinancialAccountAPIService.GetFinancialAccount(financialAccountId).then(function (response) {
                financialAccountEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = 'Financial Account';
            }

            function loadStaticData() {
                if (financialAccountEntity != undefined) {
                    $scope.scopeModel.beginEffectiveDate = financialAccountEntity.BED;
                    $scope.scopeModel.endEffectiveDate = financialAccountEntity.EED;
                    $scope.scopeModel.disableAccountDefinitionAndBED = true;
                }
            }

            function loadFinancialAccountDefinitionSelector() {
                var loadFinancialAccountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                financialAccountDefinitionSelectorDirectiveReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "TOne.WhS.BusinessEntity.Business.FinancialAccountDefinitionFilter, TOne.WhS.BusinessEntity.Business",
                                CarrierProfileId: financialAccountEntity != undefined ? financialAccountEntity.CarrierProfileId : carrierProfileId,
                                CarrierAccountId: financialAccountEntity != undefined ? financialAccountEntity.CarrierAccountId : carrierAccountId,
                                FinancialAccountId: financialAccountEntity != undefined? financialAccountEntity.FinancialAccountId : undefined,
                            }]
                        },
                        selectedIds: financialAccountEntity != undefined ? financialAccountEntity.FinancialAccountDefinitionId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(financialAccountDefinitionSelectorDirectiveAPI, payload, loadFinancialAccountDefinitionSelectorPromiseDeferred);
                });
                return loadFinancialAccountDefinitionSelectorPromiseDeferred.promise;
            }

            function loadDirective() {
                if (financialAccountEntity != undefined && financialAccountEntity.Settings != undefined) {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { extendedSettings: financialAccountEntity.Settings.ExtendedSettings, carrierProfileId: financialAccountEntity.CarrierProfileId, carrierAccountId: financialAccountEntity.CarrierAccountId };
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

        function getFinancialAccountDefinitionSetting() {
            var selectedfinancialAccountDefinitionId = financialAccountDefinitionSelectorDirectiveAPI.getSelectedIds();
            return WhS_BE_FinancialAccountDefinitionAPIService.GetFinancialAccountDefinitionSettings(selectedfinancialAccountDefinitionId).then(function (response) {
                if (response != undefined && response.ExtendedSettings != undefined)
                    $scope.scopeModel.financialAccountDefinitionRuntimeDirective = response.ExtendedSettings.RuntimeEditor;
            });
        }

        function insertFinancialAccount() {
            $scope.scopeModel.isLoading = true;

            var financialAccountObj = buildFinancialAccountObjFromScope();

            return WhS_BE_FinancialAccountAPIService.AddFinancialAccount(financialAccountObj).then(function (response) {
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

            var financialAccountToEdit = buildFinancialAccountObjFromScope();

            return WhS_BE_FinancialAccountAPIService.UpdateFinancialAccount(financialAccountToEdit).then(function (response) {
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
            var financialAccount = {
                FinancialAccountId: financialAccountId,
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate,
                Settings: {
                    ExtendedSettings: directiveAPI.getData()
                },

            };
            if (financialAccountEntity == undefined)
            {
                financialAccount.CarrierAccountId = carrierAccountId;
                financialAccount.CarrierProfileId = carrierProfileId;
                financialAccount.FinancialAccountDefinitionId = financialAccountDefinitionSelectorDirectiveAPI.getSelectedIds();
            }
            return financialAccount;
        }
    }

    appControllers.controller('WhS_BE_FinancialAccountEditorController', financialAccountEditorController);

})(appControllers);
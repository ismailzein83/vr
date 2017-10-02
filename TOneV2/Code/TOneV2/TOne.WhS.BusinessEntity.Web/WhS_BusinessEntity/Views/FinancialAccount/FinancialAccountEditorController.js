﻿(function (appControllers) {

    'use strict';

    financialAccountEditorController.$inject = ['$scope', 'WhS_BE_FinancialAccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_FinancialAccountDefinitionAPIService', 'VRDateTimeService'];

    function financialAccountEditorController($scope, WhS_BE_FinancialAccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_FinancialAccountDefinitionAPIService, VRDateTimeService) {
        var carrierAccountId;
        var carrierProfileId;

        var financialAccountId;
        var financialAccountRuntimeEntity;

        var financialAccountDefinitionSettings;
        var isEditMode;

        var financialAccountDefinitionSelectorDirectiveAPI;
        var financialAccountDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedFinancialAccountDefinitionDeferred;

        var invoiceSettingSelectorAPI;
        var invoiceSettingSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.periodEndsAt = undefined;
            $scope.scopeModel.beginEffectiveDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
            $scope.scopeModel.onDateValueChanged = function () {
                evaluatePeriodEndsAt($scope.scopeModel.endEffectiveDate);
            };
            $scope.scopeModel.onFinancialAccountDefinitionSelectorReady = function (api) {
                financialAccountDefinitionSelectorDirectiveAPI = api;
                financialAccountDefinitionSelectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceSettingDirectiveReady = function (api) {
                invoiceSettingSelectorAPI = api;
                invoiceSettingSelectorReadyDeferred.resolve();
               
            };

            $scope.scopeModel.onFinancialAccountDefinitionSelectionChanged = function (value) {
                if (value != undefined) {
                    if (selectedFinancialAccountDefinitionDeferred != undefined)
                        selectedFinancialAccountDefinitionDeferred.resolve();
                    else {
                        $scope.scopeModel.isLoading = true;
                        getFinancialAccountDefinitionSetting().then(function () {

                            if ($scope.scopeModel.showInvoiceSettingSelector) {
                                invoiceSettingSelectorReadyDeferred.promise.then(function () {
                                    var setLoader = function (value) {
                                        $scope.scopeModel.isLoadingInvoiceSettingDirective = value;
                                    };
                                    var selectedValue = financialAccountDefinitionSelectorDirectiveAPI.getSelectedValue();
                                    var directivePayload = {
                                        invoiceTypeId: selectedValue.InvoiceTypeId,
                                        filter: {
                                            InvoiceTypeId: selectedValue.InvoiceTypeId,
                                        }
                                    };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, invoiceSettingSelectorAPI, directivePayload, setLoader);
                                });
                            }

                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }
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
                if (financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.FinancialAccount != undefined) {
                    directivePayload = {
                        carrierProfileId: financialAccountRuntimeEntity.FinancialAccount.CarrierProfileId,
                        carrierAccountId: financialAccountRuntimeEntity.FinancialAccount.CarrierAccountId
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
                    getFinancialAccountDefinitionSetting().then(function () {
                        loadAllControls();
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });;
            } else {
                loadAllControls();
            }
        }

        function getFinancialAccount() {
            return WhS_BE_FinancialAccountAPIService.GetFinancialAccountRuntimeEditor(financialAccountId).then(function (response) {
                financialAccountRuntimeEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = 'Financial Account';
            }

            function loadStaticData() {
                if (financialAccountRuntimeEntity != undefined) {
                    if (financialAccountRuntimeEntity.FinancialAccount != undefined)
                    {
                        $scope.scopeModel.beginEffectiveDate = financialAccountRuntimeEntity.FinancialAccount.BED;
                        $scope.scopeModel.endEffectiveDate = financialAccountRuntimeEntity.FinancialAccount.EED;
                        evaluatePeriodEndsAt(UtilsService.createDateFromString(financialAccountRuntimeEntity.FinancialAccount.EED));
                    }
                    $scope.scopeModel.disableFinancialAccountDefinition = true;
                    selectedFinancialAccountDefinitionDeferred = UtilsService.createPromiseDeferred();
                }
            }

            function loadFinancialAccountDefinitionSelector() {
                var loadFinancialAccountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                financialAccountDefinitionSelectorDirectiveReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "TOne.WhS.BusinessEntity.Business.FinancialAccountDefinitionFilter, TOne.WhS.BusinessEntity.Business",
                                CarrierProfileId: financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.FinancialAccount != undefined ? financialAccountRuntimeEntity.FinancialAccount.CarrierProfileId : carrierProfileId,
                                CarrierAccountId: financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.FinancialAccount != undefined ? financialAccountRuntimeEntity.FinancialAccount.CarrierAccountId : carrierAccountId,
                                FinancialAccountId: financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.FinancialAccount != undefined ? financialAccountRuntimeEntity.FinancialAccount.FinancialAccountId : undefined,
                            }]
                        },
                        selectedIds: financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.FinancialAccount != undefined ? financialAccountRuntimeEntity.FinancialAccount.FinancialAccountDefinitionId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(financialAccountDefinitionSelectorDirectiveAPI, payload, loadFinancialAccountDefinitionSelectorPromiseDeferred);
                });
                return loadFinancialAccountDefinitionSelectorPromiseDeferred.promise;
            }

            function loadDirective() {
                if (financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.FinancialAccount != undefined && financialAccountRuntimeEntity.FinancialAccount.Settings != undefined) {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { extendedSettings: financialAccountRuntimeEntity.FinancialAccount.Settings.ExtendedSettings, carrierProfileId: financialAccountRuntimeEntity.FinancialAccount.CarrierProfileId, carrierAccountId: financialAccountRuntimeEntity.FinancialAccount.CarrierAccountId };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }

            }

            function loadInvoiceSettingSelectorDirective() {
                if (financialAccountDefinitionSettings != undefined)
                {
                    if ($scope.scopeModel.showInvoiceSettingSelector) {
                        var invoiceSettingSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var promises = [];
                        promises.push(selectedFinancialAccountDefinitionDeferred.promise);
                        promises.push(invoiceSettingSelectorReadyDeferred.promise);

                        UtilsService.waitMultiplePromises(promises).then(function () {
                            selectedFinancialAccountDefinitionDeferred = undefined;
                            var invoiceSettingSelectorPayload = {
                                invoiceTypeId: financialAccountDefinitionSettings.InvoiceTypeId,
                                filter: {
                                    InvoiceTypeId: financialAccountDefinitionSettings.InvoiceTypeId,
                                },
                                selectedIds: financialAccountRuntimeEntity != undefined?financialAccountRuntimeEntity.InvoiceSettingId:undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(invoiceSettingSelectorAPI, invoiceSettingSelectorPayload, invoiceSettingSelectorLoadDeferred);
                        });
                        return invoiceSettingSelectorLoadDeferred.promise;
                    }
                }
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFinancialAccountDefinitionSelector, loadDirective, loadInvoiceSettingSelectorDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function getFinancialAccountDefinitionSetting() {
            var selectedfinancialAccountDefinitionId;
            if (financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.FinancialAccount != undefined)
                selectedfinancialAccountDefinitionId = financialAccountRuntimeEntity.FinancialAccount.FinancialAccountDefinitionId;
            else
              selectedfinancialAccountDefinitionId = financialAccountDefinitionSelectorDirectiveAPI.getSelectedIds();

            return WhS_BE_FinancialAccountDefinitionAPIService.GetFinancialAccountDefinitionSettings(selectedfinancialAccountDefinitionId).then(function (response) {
                if (response != undefined)
                {
                    financialAccountDefinitionSettings = response;
                    if (response.ExtendedSettings != undefined)
                        $scope.scopeModel.financialAccountDefinitionRuntimeDirective = response.ExtendedSettings.RuntimeEditor;
                    $scope.scopeModel.showInvoiceSettingSelector = (financialAccountDefinitionSettings.InvoiceTypeId != undefined);

                }
                    
            });
        }

        function insertFinancialAccount() {
            $scope.scopeModel.isLoading = true;

            var financialAccountObj = buildFinancialAccountToAddObjFromScope();

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

            var financialAccountToEdit = buildFinancialAccountToEditObjFromScope();

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

        function buildFinancialAccountToEditObjFromScope() {
            var financialAccount = {
                FinancialAccountId: financialAccountId,
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate,
                Settings: {
                    ExtendedSettings: directiveAPI.getData()
                },
                InvoiceSettingId: invoiceSettingSelectorAPI != undefined ? invoiceSettingSelectorAPI.getSelectedIds() : undefined,
                PartnerInvoiceSettingId: financialAccountRuntimeEntity != undefined ? financialAccountRuntimeEntity.PartnerInvoiceSettingId : undefined
            };
            
            return financialAccount;
        }

        function buildFinancialAccountToAddObjFromScope() {
            var financialAccount = {
                FinancialAccount:{
                    BED: $scope.scopeModel.beginEffectiveDate,
                    EED: $scope.scopeModel.endEffectiveDate,
                    Settings: {
                        ExtendedSettings: directiveAPI.getData()
                    },
                    CarrierAccountId: carrierAccountId,
                    CarrierProfileId: carrierProfileId,
                    FinancialAccountDefinitionId : financialAccountDefinitionSelectorDirectiveAPI.getSelectedIds()
                },
                InvoiceSettingId: invoiceSettingSelectorAPI != undefined? invoiceSettingSelectorAPI.getSelectedIds():undefined

            };
            return financialAccount;
        }
        function evaluatePeriodEndsAt(currentDate)
        {
            if (currentDate != undefined) {
                var date = currentDate;
                date.setDate(date.getDate() - 1);
                date.setHours(23);
                date.setMinutes(59);
                date.setSeconds(59);
                $scope.scopeModel.periodEndsAt = date;
            } else {
                $scope.scopeModel.periodEndsAt = undefined;
            }
        }
    }

    appControllers.controller('WhS_BE_FinancialAccountEditorController', financialAccountEditorController);

})(appControllers);
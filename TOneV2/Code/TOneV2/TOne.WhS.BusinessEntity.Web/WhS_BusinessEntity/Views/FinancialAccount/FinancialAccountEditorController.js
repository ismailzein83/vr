(function (appControllers) {

    'use strict';

    financialAccountEditorController.$inject = ['$scope', 'WhS_BE_FinancialAccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_FinancialAccountDefinitionAPIService', 'VRDateTimeService','WhS_BE_CommisssionTypeEnum'];

    function financialAccountEditorController($scope, WhS_BE_FinancialAccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_FinancialAccountDefinitionAPIService, VRDateTimeService, WhS_BE_CommisssionTypeEnum) {
        var carrierAccountId;
        var carrierProfileId;

        var financialAccountId;
        var financialAccountRuntimeEntity;

        var financialAccountDefinitionSettings;
        var isEditMode;

        var financialAccountDefinitionSelectorDirectiveAPI;
        var financialAccountDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedFinancialAccountDefinitionDeferred;


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
            $scope.scopeModel.invoiceSettings = [];
            $scope.scopeModel.commissionTypes = UtilsService.getArrayEnum(WhS_BE_CommisssionTypeEnum);

            $scope.scopeModel.periodEndsAt = undefined;
            $scope.scopeModel.beginEffectiveDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
            $scope.scopeModel.onDateValueChanged = function () {
                evaluatePeriodEndsAt($scope.scopeModel.endEffectiveDate);
            };
            $scope.scopeModel.onFinancialAccountDefinitionSelectorReady = function (api) {
                financialAccountDefinitionSelectorDirectiveAPI = api;
                financialAccountDefinitionSelectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onFinancialAccountDefinitionSelectionChanged = function (value) {
                if (value != undefined) {
                    if (selectedFinancialAccountDefinitionDeferred != undefined)
                        selectedFinancialAccountDefinitionDeferred.resolve();
                    else {
                        $scope.scopeModel.isLoading = true;
                        getFinancialAccountDefinitionSetting().then(function () {

                            if ($scope.scopeModel.showInvoiceSettingSelector) {
                                $scope.scopeModel.invoiceSettings.length = 0;
                                for (var i = 0; i < financialAccountDefinitionSettings.FinancialAccountInvoiceTypes.length; i++)
                                {
                                    var financialAccountInvoiceType = financialAccountDefinitionSettings.FinancialAccountInvoiceTypes[i];
                                    addInvoiceSettingAPIFunction(financialAccountInvoiceType);

                                }
                                function addInvoiceSettingAPIFunction(financialAccountInvoiceType) {
                                    var dataItem = {
                                        title: financialAccountInvoiceType.InvoiceSettingTitle,
                                        invoiceTypeId: financialAccountInvoiceType.InvoiceTypeId,
                                        selectedCommissionType: WhS_BE_CommisssionTypeEnum.Display,
                                        disableCommission: financialAccountInvoiceType.DisableCommission
                                    };
                                    dataItem.onInvoiceSettingDirectiveReady = function (api) {
                                        dataItem.directiveAPI = api;
                                        var setLoader = function (value) { $scope.isLoadingSupplierZonesSelector = value };
                                        var payload = {
                                            invoiceTypeId: financialAccountInvoiceType.InvoiceTypeId,
                                            filter: {
                                                InvoiceTypeId: financialAccountInvoiceType.InvoiceTypeId,
                                            }
                                        };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, payload, setLoader);
                                    };
                                    $scope.scopeModel.invoiceSettings.push(dataItem);
                                }
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
                if (financialAccountDefinitionSettings != undefined) {
                    if ($scope.scopeModel.showInvoiceSettingSelector) {
                        var promises = [];
                        promises.push(selectedFinancialAccountDefinitionDeferred.promise);
                        if (financialAccountDefinitionSettings.FinancialAccountInvoiceTypes != undefined)
                        {
                            for (var i = 0; i < financialAccountDefinitionSettings.FinancialAccountInvoiceTypes.length; i++) {
                                var invoiceSettingItem = {
                                    payload: financialAccountDefinitionSettings.FinancialAccountInvoiceTypes[i],
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(invoiceSettingItem.loadPromiseDeferred.promise);
                                addInvoiceSettingItemAPI(invoiceSettingItem);
                            }
                            function addInvoiceSettingItemAPI(invoiceSettingItem) {
                                var dataItem = {
                                    title: invoiceSettingItem.payload.InvoiceSettingTitle,
                                    invoiceTypeId: invoiceSettingItem.payload.InvoiceTypeId,
                                    selectedCommissionType: WhS_BE_CommisssionTypeEnum.Display,
                                    disableCommission: invoiceSettingItem.payload.DisableCommission
                                };
                                var selectedValue;
                                var financialAccountInvoiceSettingData;
                                if (financialAccountRuntimeEntity != undefined)
                                {
                                    if (financialAccountRuntimeEntity.InvoiceSettingsData != undefined) {
                                        for (var i = 0; i < financialAccountRuntimeEntity.InvoiceSettingsData.length; i++) {
                                            var invoiceSettingData = financialAccountRuntimeEntity.InvoiceSettingsData[i];
                                            if (invoiceSettingData.InvoiceTypeId == invoiceSettingItem.payload.InvoiceTypeId) {
                                                selectedValue = invoiceSettingData.InvoiceSettingId;
                                                break;
                                            }
                                        }
                                    }
                                    if (financialAccountRuntimeEntity.FinancialAccount != undefined && financialAccountRuntimeEntity.FinancialAccount.Settings != undefined && financialAccountRuntimeEntity.FinancialAccount.Settings.FinancialAccountInvoiceSettings != undefined)
                                    {
                                        for(var i =0 ;i<financialAccountRuntimeEntity.FinancialAccount.Settings.FinancialAccountInvoiceSettings.length;i++)
                                        {
                                            var financialAccountInvoiceSetting = financialAccountRuntimeEntity.FinancialAccount.Settings.FinancialAccountInvoiceSettings[i];
                                            if (financialAccountInvoiceSetting.InvoiceTypeId == invoiceSettingItem.payload.InvoiceTypeId)
                                            {
                                                financialAccountInvoiceSettingData = financialAccountInvoiceSetting;
                                                break;
                                            }
                                               
                                        }
                                    }
                                }

                                if (financialAccountInvoiceSettingData != undefined && financialAccountInvoiceSettingData.FinancialAccountCommission != undefined)
                                {
                                    dataItem.commission = financialAccountInvoiceSettingData.FinancialAccountCommission.Commission;
                                    dataItem.selectedCommissionType = UtilsService.getItemByVal($scope.scopeModel.commissionTypes, financialAccountInvoiceSettingData.FinancialAccountCommission.CommissionType, "value");
                                }
                                var dataItemPayload = {
                                    selectedIds: selectedValue,
                                    invoiceTypeId: invoiceSettingItem.payload.InvoiceTypeId,
                                    filter: {
                                        InvoiceTypeId: invoiceSettingItem.payload.InvoiceTypeId,
                                    }
                                };
                                dataItem.onInvoiceSettingDirectiveReady = function (api) {
                                    dataItem.directiveAPI = api;
                                    invoiceSettingItem.readyPromiseDeferred.resolve();
                                };
                                invoiceSettingItem.readyPromiseDeferred.promise
                                    .then(function () {
                                        VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, invoiceSettingItem.loadPromiseDeferred);
                                    });

                                $scope.scopeModel.invoiceSettings.push(dataItem);
                            }
                        }
                       
                        return UtilsService.waitMultiplePromises(promises);
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
                    $scope.scopeModel.showInvoiceSettingSelector = (financialAccountDefinitionSettings.FinancialAccountInvoiceTypes != undefined && financialAccountDefinitionSettings.FinancialAccountInvoiceTypes.length > 0);

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
            var invoiceSettingsData;
            var financialAccountInvoiceSetting;
            if ($scope.scopeModel.invoiceSettings != undefined && $scope.scopeModel.invoiceSettings.length > 0)
            {
                invoiceSettingsData = [];
                financialAccountInvoiceSetting = [];
                for(var i=0;i<$scope.scopeModel.invoiceSettings.length;i++)
                {
                    var invoiceSetting = $scope.scopeModel.invoiceSettings[i];
                    var partnerInvoiceSettingId;
                    if(financialAccountRuntimeEntity != undefined && financialAccountRuntimeEntity.InvoiceSettingsData != undefined)
                    {
                        for(var j=0;j<financialAccountRuntimeEntity.InvoiceSettingsData.length;j++)
                        {
                            var invoiceSettingData = financialAccountRuntimeEntity.InvoiceSettingsData[j];
                            if(invoiceSettingData.InvoiceTypeId == invoiceSetting.invoiceTypeId)
                            {
                                partnerInvoiceSettingId = invoiceSettingData.PartnerInvoiceSettingId;
                                break;
                            }
                        }
                    }
                    var selectedId = invoiceSetting.directiveAPI.getSelectedIds();
                    invoiceSettingsData.push({
                        InvoiceSettingId:invoiceSetting.directiveAPI.getSelectedIds(),
                        InvoiceTypeId:invoiceSetting.invoiceTypeId,
                        PartnerInvoiceSettingId: partnerInvoiceSettingId
                    });
                    financialAccountInvoiceSetting.push({
                        InvoiceTypeId: invoiceSetting.invoiceTypeId,
                        FinancialAccountCommission: invoiceSetting.disableCommission ? undefined : {
                            Commission:invoiceSetting.commission,
                            CommissionType: invoiceSetting.selectedCommissionType.value
                        }
                    });
                }
            }
            var financialAccount = {
                FinancialAccountId: financialAccountId,
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate,
                Settings: {
                    ExtendedSettings: directiveAPI.getData(),
                    FinancialAccountInvoiceSettings: financialAccountInvoiceSetting
                },
                InvoiceSettingsData:invoiceSettingsData
            };
            
            return financialAccount;
        }

        function buildFinancialAccountToAddObjFromScope() {
            var invoiceSettingsData;
            var financialAccountInvoiceSetting;

            if ($scope.scopeModel.invoiceSettings != undefined && $scope.scopeModel.invoiceSettings.length > 0)
            {
                financialAccountInvoiceSetting = [];
                invoiceSettingsData = [];
                for (var i = 0;i<$scope.scopeModel.invoiceSettings.length;i++)
                {
                    var invoiceSetting = $scope.scopeModel.invoiceSettings[i];
                    invoiceSettingsData.push({
                        InvoiceSettingId: invoiceSetting.directiveAPI.getSelectedIds(),
                        InvoiceTypeId:invoiceSetting.invoiceTypeId
                    });
                    financialAccountInvoiceSetting.push({
                        InvoiceTypeId: invoiceSetting.invoiceTypeId,
                        FinancialAccountCommission: invoiceSetting.disableCommission ? undefined : {
                            Commission: invoiceSetting.commission,
                            CommissionType: invoiceSetting.selectedCommissionType.value
                        }
                    });
                }
            }
            var financialAccount = {
                FinancialAccount:{
                    BED: $scope.scopeModel.beginEffectiveDate,
                    EED: $scope.scopeModel.endEffectiveDate,
                    Settings: {
                        ExtendedSettings: directiveAPI.getData(),
                        FinancialAccountInvoiceSettings: financialAccountInvoiceSetting

                    },
                    CarrierAccountId: carrierAccountId,
                    CarrierProfileId: carrierProfileId,
                    FinancialAccountDefinitionId : financialAccountDefinitionSelectorDirectiveAPI.getSelectedIds()
                },
                InvoiceSettingsData:invoiceSettingsData

            };
            return financialAccount;
        }
        function evaluatePeriodEndsAt(currentDate)
        {
           
            if (currentDate != undefined) {
                var date = new Date(currentDate);
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
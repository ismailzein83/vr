(function (appControllers) {

    'use strict';

    invoiceAccountEditorController.$inject = ['$scope', 'WhS_Invoice_InvoiceAccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_Invoice_InvoiceTypeAPIService'];

    function invoiceAccountEditorController($scope, WhS_Invoice_InvoiceAccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_Invoice_InvoiceTypeAPIService) {
        var carrierAccountId;
        var carrierProfileId;
     
        var invoiceAccountId;
        var invoiceTypeSelectorDirectiveAPI;
        var invoiceTypeSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        var invoiceAccountEntity;

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
                invoiceAccountId = parameters.invoiceAccountId;
            }
            $scope.scopeModel.isEditMode = (invoiceAccountId != undefined);
        }

        function defineScope() {
            $scope.scopeModel.beginEffectiveDate = UtilsService.getDateFromDateTime(new Date());

            $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                invoiceTypeSelectorDirectiveAPI = api;
                invoiceTypeSelectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceTypeSelectionChanged = function (value) {
                if(value != undefined)
                {
                    $scope.scopeModel.isLoading = true;
                    getInvoiceTypeSetting().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }else
                {
                    $scope.scopeModel.invoiceTypeRuntimeDirective = undefined;
                }
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var directivePayload = { carrierProfileId: carrierProfileId, carrierAccountId: carrierAccountId };

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
                    return updateInvoiceAccount();
                else
                {
                    return insertInvoiceAccount();
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
                getInvoiceAccount().then(function () {
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

        function getInvoiceAccount() {
            return WhS_Invoice_InvoiceAccountAPIService.GetInvoiceAccountEditorRuntime(invoiceAccountId).then(function (response) {
                invoiceAccountEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                //if (isEditMode)
                //    $scope.title = UtilsService.buildTitleForUpdateEditor(response, 'Financial Account', $scope);
                //else
                //    $scope.title = UtilsService.buildTitleForAddEditor('Financial Account');
                $scope.title = 'Invoice Account';
            }

            function loadStaticData() {
                if(invoiceAccountEntity != undefined)
                {
                    if (invoiceAccountEntity.InvoiceAccount != undefined)
                    {
                        $scope.scopeModel.beginEffectiveDate = invoiceAccountEntity.InvoiceAccount.BED;
                        $scope.scopeModel.endEffectiveDate = invoiceAccountEntity.InvoiceAccount.EED;
                        $scope.scopeModel.disableEED = (invoiceAccountEntity.InvoiceAccount.EED != undefined && UtilsService.getDateFromDateTime(invoiceAccountEntity.InvoiceAccount.EED) < UtilsService.getDateFromDateTime(new Date()));
                    }
                    if(invoiceAccountEntity.HasInvoices)
                    {
                        $scope.scopeModel.disableInvoiceTypeAndBED = true;
                    }
                }
            }

            function loadInvoiceTypeSelectorSelector() {
                var loadInvoiceTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceTypeSelectorDirectiveReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "TOne.WhS.AccountBalance.Business.InvoiceAccountTypeFilter, TOne.WhS.AccountBalance.Business",
                                CarrierProfileId: invoiceAccountEntity != undefined && invoiceAccountEntity.InvoiceAccount != undefined ? invoiceAccountEntity.InvoiceAccount.CarrierProfileId : carrierProfileId,
                                CarrierAccountId: invoiceAccountEntity != undefined && invoiceAccountEntity.InvoiceAccount != undefined ? invoiceAccountEntity.InvoiceAccount.CarrierAccountId : carrierAccountId,
                                IsEditMode: $scope.scopeModel.isEditMode
                            }]
                        },
                       selectedIds: invoiceAccountEntity != undefined && invoiceAccountEntity.InvoiceAccount != undefined && invoiceAccountEntity.InvoiceAccount.Settings != undefined ? invoiceAccountEntity.InvoiceAccount.Settings.InvoiceTypeId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorDirectiveAPI, payload, loadInvoiceTypeSelectorPromiseDeferred);
                });
                return loadInvoiceTypeSelectorPromiseDeferred.promise;
            }
            
            function loadDirective() {
                if (invoiceAccountEntity != undefined && invoiceAccountEntity.InvoiceAccount != undefined && invoiceAccountEntity.InvoiceAccount.Settings != undefined)
                {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { extendedSettings: invoiceAccountEntity.InvoiceAccount.Settings.ExtendedSettings, carrierProfileId: invoiceAccountEntity.InvoiceAccount.CarrierProfileId, carrierAccountId: invoiceAccountEntity.InvoiceAccount.CarrierAccountId };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }
               
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceTypeSelectorSelector, loadDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    
        function getInvoiceTypeSetting()
        {
            var selectedinvoiceTypeId = invoiceTypeSelectorDirectiveAPI.getSelectedIds();
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceTypeExtendedSettings(selectedinvoiceTypeId).then(function (response) {
                if (response != undefined)
                    $scope.scopeModel.invoiceTypeRuntimeDirective = response.RuntimeEditor;
            });
        }

        function insertInvoiceAccount() {
            $scope.scopeModel.isLoading = true;

            var invoiceAccountObj = buildInvoiceAccountObjFromScope();

            return WhS_Invoice_InvoiceAccountAPIService.AddInvoiceAccount(invoiceAccountObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Invoice Account', response, 'Name')) {
                    if ($scope.onInvoiceAccountAdded != undefined)
                        $scope.onInvoiceAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateInvoiceAccount() {
            $scope.scopeModel.isLoading = true;

            var invoiceAccountObj = buildInvoiceAccountObjFromScope();

            return WhS_Invoice_InvoiceAccountAPIService.UpdateInvoiceAccount(invoiceAccountObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Invoice Account', response, 'Name')) {
                    if ($scope.onInvoiceAccountUpdated != undefined)
                        $scope.onInvoiceAccountUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildInvoiceAccountObjFromScope() {
            var obj = {
                InvoiceAccountId:invoiceAccountId,
                CarrierAccountId: invoiceAccountEntity != undefined && invoiceAccountEntity.InvoiceAccount != undefined ? invoiceAccountEntity.InvoiceAccount.CarrierAccountId : carrierAccountId,
                CarrierProfileId: invoiceAccountEntity != undefined && invoiceAccountEntity.InvoiceAccount != undefined ? invoiceAccountEntity.InvoiceAccount.CarrierProfileId : carrierProfileId,
                Settings: {
                    InvoiceTypeId: invoiceTypeSelectorDirectiveAPI.getSelectedIds(),
                    ExtendedSettings: directiveAPI.getData()
                },
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Invoice_InvoiceAccountEditorController', invoiceAccountEditorController);

})(appControllers);
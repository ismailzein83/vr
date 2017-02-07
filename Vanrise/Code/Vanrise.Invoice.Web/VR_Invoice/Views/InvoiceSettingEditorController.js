(function (appControllers) {

    "use strict";

    invoiceSettingEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService','VR_Invoice_InvoiceSettingAPIService','VR_Invoice_InvoiceTypeConfigsAPIService'];

    function invoiceSettingEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceSettingAPIService, VR_Invoice_InvoiceTypeConfigsAPIService) {
        var isEditMode;
        var invoiceSettingId;
        var invoiceSettingEntity;
        var invoiceTypeId;
        var invoiceTypeEntity;
        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceSettingId = parameters.invoiceSettingId;
                invoiceTypeId = parameters.invoiceTypeId;
            }
            isEditMode = (invoiceSettingId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.sections = [];
            $scope.scopeModel.saveInvoiceSetting = function () {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    return updateInvoiceSetting();
                }
                else {
                    return insertInvoiceSetting();
                }
            };
            $scope.scopeModel.onRuntimeEditorReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            function buildInvoiceSettingObjFromScope() {
                var obj = {
                    InvoiceSettingId: invoiceSettingId,
                    InvoiceTypeId: invoiceTypeId,
                    IsDefault: invoiceSettingEntity != undefined ? invoiceSettingEntity.IsDefault : undefined,
                    Name: $scope.scopeModel.name,
                    Details: {
                        InvoiceSettingParts: runtimeEditorAPI.getData()
                    }
                };
                return obj;
            }
            function insertInvoiceSetting() {

                var invoiceSettingObject = buildInvoiceSettingObjFromScope();
                return VR_Invoice_InvoiceSettingAPIService.AddInvoiceSetting(invoiceSettingObject)
                .then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (VRNotificationService.notifyOnItemAdded("Invoice Setting", response)) {
                        if ($scope.onInvoiceSettingAdded != undefined)
                            $scope.onInvoiceSettingAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

            }
            function updateInvoiceSetting() {
                var invoiceSettingObject = buildInvoiceSettingObjFromScope();
                VR_Invoice_InvoiceSettingAPIService.UpdateInvoiceSetting(invoiceSettingObject)
                .then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (VRNotificationService.notifyOnItemUpdated("Invoice Setting", response)) {
                        if ($scope.onInvoiceSettingUpdated != undefined)
                            $scope.onInvoiceSettingUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            getInvoiceType().then(function () {
                if (isEditMode) {
                    getInvoiceSetting().then(function () {
                        loadAllControls()
                            .finally(function () {
                            });
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }
                else {
                    loadAllControls();
                }
            });

            function getInvoiceSetting() {
                return VR_Invoice_InvoiceSettingAPIService.GetInvoiceSetting(invoiceSettingId).then(function (response) {
                    invoiceSettingEntity = response;

                });
            }
            function getInvoiceType()
            {
                return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                    invoiceTypeEntity = response;
                });
            }
       
            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && invoiceSettingEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceSettingEntity.Name, 'Invoice Setting');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Invoice Setting');
                }

                function loadStaticData() {
                    if (invoiceSettingEntity != undefined) {
                        $scope.scopeModel.name = invoiceSettingEntity.Name;
                        if (invoiceSettingEntity.Details != undefined)
                        {
                            $scope.scopeModel.enableAutomaticInvoice = invoiceSettingEntity.Details.EnableAutomaticInvoice;
                            $scope.scopeModel.isDefault = invoiceSettingEntity.Details.IsDefault;
                        }
                       
                    }
                }

                function loadRuntimeEditor() {
                    if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.InvoiceSettingPartUISections != undefined)
                    {
                        var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
                        runtimeEditorReadyDeferred.promise.then(function () {
                            var runtimeEditorPayload = {
                                sections: invoiceTypeEntity.Settings.InvoiceSettingPartUISections,
                                selectedValues: (isEditMode) ? invoiceSettingEntity.Details.InvoiceSettingParts : undefined,
                                invoiceTypeId: invoiceTypeId,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
                        });

                        return runtimeEditorLoadDeferred.promise;
                    }
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRuntimeEditor])
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.scopeModel.isLoading = false;
                  });
            }
        }

        function getContext()
        {
            var requiredBillingPeriod = false;
            var context = {
                setRequiredBillingPeriod:function(value)
                {
                    requiredBillingPeriod = value;
                },
                isBillingPeriodRequired : function()
                {
                    return requiredBillingPeriod;
                }
            };
            return context;
        }
    }

    appControllers.controller('VR_Invoice_InvoiceSettingEditorController', invoiceSettingEditorController);
})(appControllers);

﻿(function (appControllers) {

    "use strict";

    partnerInvoiceSettingEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_PartnerInvoiceSettingAPIService','VR_Invoice_InvoiceSettingAPIService','VR_Invoice_InvoiceTypeConfigsAPIService'];

    function partnerInvoiceSettingEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_PartnerInvoiceSettingAPIService, VR_Invoice_InvoiceSettingAPIService, VR_Invoice_InvoiceTypeConfigsAPIService) {
        var partnerInvoiceSettingId;
        var invoiceSettingId;
        var invoiceSettingEntity;

        $scope.scopeModel = {};

        var partnerInvoiceSettingEntity;
        var invoiceSettingDefinition;
        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceSettingPartsConfigs;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                partnerInvoiceSettingId = parameters.partnerInvoiceSettingId;
                invoiceSettingId = parameters.invoiceSettingId;
            }
            $scope.scopeModel.isEditMode = (partnerInvoiceSettingId != undefined);
        }
        function defineScope() {
            $scope.scopeModel.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.overidablePartsInfo = [];
            $scope.scopeModel.selectedOveridablePartsInfo = [];
            $scope.scopeModel.onRuntimeEditorReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if ($scope.scopeModel.isEditMode) {
                    return updatePartnerInvoiceSetting();
                }
                else {
                    return addPartnerInvoiceSetting();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
          
            function addPartnerInvoiceSetting() {
                $scope.scopeModel.isLoading = true;
                var partnerInvoiceSettingObject = buildPartnerInvoiceSettingObjFromScope();
                return VR_Invoice_PartnerInvoiceSettingAPIService.AddPartnerInvoiceSetting(partnerInvoiceSettingObject)
               .then(function (response) {
                   if (VRNotificationService.notifyOnItemAdded("Partner Invoice Setting", response)) {
                       if ($scope.onPartnerInvoiceSettingAdded != undefined)
                           $scope.onPartnerInvoiceSettingAdded(response.InsertedObject);
                       $scope.modalContext.closeModal();
                   }
               })
               .catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
            }
            function updatePartnerInvoiceSetting() {
                $scope.scopeModel.isLoading = true;

                var partnerInvoiceSettingObject = buildPartnerInvoiceSettingObjFromScope();
                return VR_Invoice_PartnerInvoiceSettingAPIService.UpdatePartnerInvoiceSetting(partnerInvoiceSettingObject)
               .then(function (response) {
                   if (VRNotificationService.notifyOnItemAdded("Partner Invoice Setting", response)) {
                       if ($scope.onPartnerInvoiceSettingUpdated != undefined)
                           $scope.onPartnerInvoiceSettingUpdated(response.UpdatedObject);
                       $scope.modalContext.closeModal();
                   }
               })
               .catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if ($scope.scopeModel.isEditMode) {
                getPartnerInvoiceSetting().then(function () {
                    UtilsService.waitMultipleAsyncOperations([getInvoiceSettingPartsConfigs,getInvoiceSettingDefinition, getInvoiceSetting]).then(function () {
                        loadAllControls();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });;
            }
            else {
                UtilsService.waitMultipleAsyncOperations([getInvoiceSettingPartsConfigs,getInvoiceSettingDefinition, getInvoiceSetting]).then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });;
            }
            function getInvoiceSetting() {
                var settingId = partnerInvoiceSettingEntity != undefined ? partnerInvoiceSettingEntity.InvoiceSettingID : invoiceSettingId;
                return VR_Invoice_InvoiceSettingAPIService.GetInvoiceSetting(settingId).then(function (reposnse) {
                    invoiceSettingEntity = reposnse;
                });
            }
            function getInvoiceSettingDefinition() {
                var settingId = partnerInvoiceSettingEntity != undefined ? partnerInvoiceSettingEntity.InvoiceSettingID : invoiceSettingId;

                return VR_Invoice_InvoiceSettingAPIService.GetOverridableInvoiceSetting(settingId).then(function (response) {
                    invoiceSettingDefinition = response;
                });
            }
            function getPartnerInvoiceSetting() {
                return VR_Invoice_PartnerInvoiceSettingAPIService.GetPartnerInvoiceSetting(partnerInvoiceSettingId).then(function (response) {
                    partnerInvoiceSettingEntity = response;
                });
            }

        }
        function loadAllControls() {
            function setTitle() {
                if ($scope.scopeModel.isEditMode)
                    $scope.title = "Partner Invoice Setting";
                else
                    $scope.title = "Partner Invoice Setting";
            }
            function loadRuntimeEditor() {
                if (invoiceSettingDefinition != undefined) {
                    loadOveridablePartsInfo(invoiceSettingDefinition);
                    var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
                    runtimeEditorReadyDeferred.promise.then(function () {
                        var runtimeEditorPayload = {
                            sections: invoiceSettingDefinition,
                            invoiceTypeId: invoiceSettingEntity != undefined ? invoiceSettingEntity.InvoiceTypeId : undefined,
                            selectedValues: partnerInvoiceSettingEntity != undefined && partnerInvoiceSettingEntity.Details != undefined ? partnerInvoiceSettingEntity.Details.InvoiceSettingParts : undefined,
                             context: getContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
                    });

                    return runtimeEditorLoadDeferred.promise;
                }
            }
            function loadOveridablePartsInfo(invoiceSettings) {
                $scope.scopeModel.overidablePartsInfo.length = 0;
                if (invoiceSettings != undefined) {
                    for (var i = 0; i < invoiceSettings.length; i++) {
                        var item = invoiceSettings[i];
                        if (item.Rows != undefined) {
                            for (var j = 0; j < item.Rows.length; j++) {
                                var row = item.Rows[j];
                                if (row.Parts != undefined) {
                                    for (var k = 0; k < row.Parts.length; k++) {
                                        var part = row.Parts[k];
                                        var invoiceSettingPartsConfig = UtilsService.getItemByVal(invoiceSettingPartsConfigs, part.PartConfigId, "ExtensionConfigurationId");
                                        if (invoiceSettingPartsConfig != undefined) {
                                            $scope.scopeModel.overidablePartsInfo.push({
                                                Name: invoiceSettingPartsConfig.Title,
                                                Id: invoiceSettingPartsConfig.ExtensionConfigurationId
                                            });
                                        }
                                        invoiceSettingDefinition[i].Rows[j].Parts[k].isVisible = false;
                                    }
                                }
                            }
                        }
                    }
                }
                if(partnerInvoiceSettingEntity != undefined && partnerInvoiceSettingEntity.Details != undefined && partnerInvoiceSettingEntity.Details.InvoiceSettingParts != undefined)
                {
                    for(var prob in partnerInvoiceSettingEntity.Details.InvoiceSettingParts)
                    {
                        var partInfo = UtilsService.getItemByVal($scope.scopeModel.overidablePartsInfo, prob, "Id");
                        if (partInfo != undefined)
                            $scope.scopeModel.selectedOveridablePartsInfo.push(partInfo);
                    }
                }
            }
            function loadPartnerSelectorDirective() {
                var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                partnerSelectorReadyDeferred.promise.then(function () {
                    var partnerSelectorPayload = {
                        invoiceTypeId: invoiceSettingEntity != undefined ? invoiceSettingEntity.InvoiceTypeId : undefined,
                        partnerInvoiceFilters: [{
                            $type: "Vanrise.Invoice.Business.InvoicePartnerFilter.NotAssignedPartnerToInvoiceSettingFilter, Vanrise.Invoice.Business",
                            EditablePartnerId: partnerInvoiceSettingEntity != undefined ? partnerInvoiceSettingEntity.PartnerId : undefined
                        }]
                    };
                    if (partnerInvoiceSettingEntity != undefined) {
                        partnerSelectorPayload.selectedIds = partnerInvoiceSettingEntity.PartnerId;
                    }
                    VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
                });
                return partnerSelectorPayloadLoadDeferred.promise;
            }
            function loadStaticData() {
                if (partnerInvoiceSettingEntity != undefined) {
                }
            }


            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerSelectorDirective, loadRuntimeEditor])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }
        function getInvoiceSettingPartsConfigs() {
            return VR_Invoice_InvoiceTypeConfigsAPIService.GetInvoiceSettingPartsConfigs().then(function (response) {
                invoiceSettingPartsConfigs = response;
            });
        }
        function getContext()
        {
            var context = {
                setVisibility: function (part) {
                    if(part != undefined)
                    {
                        for(var i=0;i<$scope.scopeModel.selectedOveridablePartsInfo.length;i++)
                        {
                           var partInfo = $scope.scopeModel.selectedOveridablePartsInfo[i];
                           if (partInfo.Id == part.PartConfigId)
                           {
                               return true;
                           }
                        }
                    }
                    return false;
                }
            };
            return context;
        }
        function buildPartnerInvoiceSettingObjFromScope() {
            var partnerObject = partnerSelectorAPI.getSelectedIds();
            var obj = {
                InvoiceSettingID: partnerInvoiceSettingEntity != undefined ? partnerInvoiceSettingEntity.InvoiceSettingID : invoiceSettingId,
                PartnerInvoiceSettingId: partnerInvoiceSettingId,
                PartnerId: partnerObject != undefined ? partnerObject.selectedIds : undefined,
                Details: {
                    InvoiceSettingParts: runtimeEditorAPI != undefined?runtimeEditorAPI.getData():undefined
                }
            };
            return obj;
        }
    }

    appControllers.controller('VR_Invoice_PartnerInvoiceSettingEditorController', partnerInvoiceSettingEditorController);
})(appControllers);

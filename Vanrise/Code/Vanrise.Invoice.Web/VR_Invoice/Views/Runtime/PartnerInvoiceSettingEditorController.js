(function (appControllers) {

    "use strict";

    partnerInvoiceSettingEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_PartnerInvoiceSettingAPIService','VR_Invoice_InvoiceSettingAPIService'];

    function partnerInvoiceSettingEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_PartnerInvoiceSettingAPIService, VR_Invoice_InvoiceSettingAPIService) {
        var partnerInvoiceSettingId;
        var invoiceSettingId;
        var invoiceSettingEntity

        $scope.scopeModel = {};

        var partnerInvoiceSettingEntity;

        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
                    getInvoiceSetting().then(function () {
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
                getInvoiceSetting().then(function () {
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


            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildPartnerInvoiceSettingObjFromScope() {
            var partnerObject = partnerSelectorAPI.getSelectedIds();

            var obj = {
                InvoiceSettingID: partnerInvoiceSettingEntity != undefined ? partnerInvoiceSettingEntity.InvoiceSettingID : invoiceSettingId,
                PartnerInvoiceSettingId: partnerInvoiceSettingId,
                PartnerId: partnerObject != undefined ? partnerObject.selectedIds : undefined,
            };
            return obj;
        }
    }

    appControllers.controller('VR_Invoice_PartnerInvoiceSettingEditorController', partnerInvoiceSettingEditorController);
})(appControllers);

(function (appControllers) {

    "use strict";

    invoiceSettingEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceSettingAPIService'];

    function invoiceSettingEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceSettingAPIService) {
        var isEditMode;
        var invoiceSettingId;
        var invoiceSettingEntity;

        var invoiceTypeId;

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

            $scope.scopeModel.saveInvoiceSetting = function () {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    return updateInvoiceSetting();
                }
                else {
                    return insertInvoiceSetting();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            function buildInvoiceSettingObjFromScope() {
                var obj = {
                    InvoiceSettingId: invoiceSettingId,
                    InvoiceTypeId:invoiceTypeId,
                    Name: $scope.scopeModel.name,
                    Details: {
                        EnableAutomaticInvoice: $scope.scopeModel.enableAutomaticInvoice
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

            if (isEditMode) {
                getInvoiceSetting().then(function () {
                    loadAllControls()
                        .finally(function () {
                            invoiceSettingEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function getInvoiceSetting() {
                return VR_Invoice_InvoiceSettingAPIService.GetInvoiceSetting(invoiceSettingId).then(function (response) {
                    invoiceSettingEntity = response;

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
                        $scope.scopeModel.enableAutomaticInvoice = invoiceSettingEntity.Details != undefined ? invoiceSettingEntity.Details.EnableAutomaticInvoice : undefined;
                    }
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.scopeModel.isLoading = false;
                  });
            }
        }
    }

    appControllers.controller('VR_Invoice_InvoiceSettingEditorController', invoiceSettingEditorController);
})(appControllers);

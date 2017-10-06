(function (appControllers) {

    'use strict';

    originalInvoiceDataTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Invoice_InvoiceAPIService','VR_Invoice_InvoiceAPIService','VRCommon_VRMailAPIService'];

    function originalInvoiceDataTemplateController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_Invoice_InvoiceAPIService, VR_Invoice_InvoiceAPIService, VRCommon_VRMailAPIService) {

        var invoiceAccountEntity;
        var invoiceId;
        var invoiceTypeId;
        var fileAPI;
        var invoiceEntity;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                invoiceId = parameters.invoiceId;
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.uploadedAttachements = [];
            $scope.scopeModel.save = function () {
                return updateOriginalInvoiceData();
            };
            $scope.scopeModel.onUploadedAttachementFileReady = function (api) {
                fileAPI = api;
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModel.addUploadedAttachement = function (obj) {
                if (obj != undefined) {
                    $scope.scopeModel.uploadedAttachements.push(obj);
                    fileAPI.clearFileUploader();
                }
            };
            $scope.scopeModel.downloadAttachement = function (attachedfileId) {
                $scope.scopeModel.isLoading = true;
                return VRCommon_VRMailAPIService.DownloadAttachement(attachedfileId).then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (response != undefined)
                        UtilsService.downloadFile(response.data, response.headers);
                });
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            UtilsService.waitMultipleAsyncOperations([getInvoice]).then(function () {
                loadAllControls();
            });
        }

        function getInvoice()
        {
            return VR_Invoice_InvoiceAPIService.GetInvoiceDetail(invoiceId).then(function (response) {
                invoiceEntity = response;
                if (invoiceEntity != undefined && invoiceEntity.Entity != undefined && invoiceEntity.Entity.Details != undefined)
                {
                    $scope.scopeModel.originalAmount = invoiceEntity.Entity.Details.OriginalAmount;
                    $scope.scopeModel.reference = invoiceEntity.Entity.Details.Reference;
                }
            });
        }

        function loadAllControls() {
         
            function setTitle() {
                $scope.title = 'Original Invoice Data';
            }

            function loadStaticData() {
                if (invoiceAccountEntity != undefined) {
                }
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateOriginalInvoiceData() {
            $scope.scopeModel.isLoading = true;

            var originalInvoiceDataObj = buildOriginalInvoiceDataObjFromScope();

            return WhS_Invoice_InvoiceAPIService.UpdateOriginalInvoiceData(originalInvoiceDataObj).then(function (response) {
                if ($scope.onOriginalInvoiceDataUpdated != undefined)
                    $scope.onOriginalInvoiceDataUpdated(response);
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildOriginalInvoiceDataObjFromScope() {

            var attachementFileIds;
            if($scope.scopeModel.uploadedAttachements != undefined)
                attachementFileIds = $scope.scopeModel.uploadedAttachements.map(function (a) { return a.fileId; });
            var obj = {
                OriginalAmount: $scope.scopeModel.originalAmount,
                Reference: $scope.scopeModel.reference,
                AttachementsFileIds: attachementFileIds,
                InvoiceId :invoiceId,
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Invoice_OriginalInvoiceDataTemplateController', originalInvoiceDataTemplateController);

})(appControllers);
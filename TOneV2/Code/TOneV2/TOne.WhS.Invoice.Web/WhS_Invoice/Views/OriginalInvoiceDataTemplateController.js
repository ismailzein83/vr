(function (appControllers) {

    'use strict';

    originalInvoiceDataTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Invoice_InvoiceAPIService','VR_Invoice_InvoiceAPIService','VRCommon_VRMailAPIService'];

    function originalInvoiceDataTemplateController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_Invoice_InvoiceAPIService, VR_Invoice_InvoiceAPIService, VRCommon_VRMailAPIService) {

        var invoiceAccountEntity;
        var invoiceId;
        var invoiceTypeId;
        var fileAPI;
        var originalInvoiceDataRuntime;
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
            UtilsService.waitMultipleAsyncOperations([getOriginalInvoiceDataRuntime]).then(function () {
                loadAllControls();
            });
        }

        function getOriginalInvoiceDataRuntime()
        {
            return WhS_Invoice_InvoiceAPIService.GetOriginalInvoiceDataRuntime(invoiceId).then(function (response) {
                originalInvoiceDataRuntime = response;
                if (originalInvoiceDataRuntime != undefined)
                {
                    $scope.scopeModel.originalAmount = originalInvoiceDataRuntime.OriginalAmount;
                    $scope.scopeModel.reference = originalInvoiceDataRuntime.Reference;
                    $scope.scopeModel.includeInSettlement = originalInvoiceDataRuntime.IncludeOriginalAmountInSettlement;
                    if(originalInvoiceDataRuntime.AttachementFilesRuntime != undefined)
                    {
                        for (var i = 0; i < originalInvoiceDataRuntime.AttachementFilesRuntime.length; i++)
                        {
                            var attachementFileRuntime = originalInvoiceDataRuntime.AttachementFilesRuntime[i];
                            $scope.scopeModel.uploadedAttachements.push({
                                fileId:attachementFileRuntime.FileId,
                                fileName: attachementFileRuntime.FileName
                            });
                        }
                    }
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
            if ($scope.scopeModel.uploadedAttachements != undefined)
            {
                attachementFileIds = [];
                for(var i=0;i<$scope.scopeModel.uploadedAttachements.length;i++)
                {
                    var uploadedAttachement = $scope.scopeModel.uploadedAttachements[i];
                    attachementFileIds.push({
                        FileId: uploadedAttachement.fileId
                    });
                }
            }
            var obj = {
                OriginalAmount: $scope.scopeModel.originalAmount,
                Reference: $scope.scopeModel.reference,
                AttachementFiles: attachementFileIds,
                InvoiceId: invoiceId,
                IncludeOriginalAmountInSettlement: $scope.scopeModel.includeInSettlement,
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Invoice_OriginalInvoiceDataTemplateController', originalInvoiceDataTemplateController);

})(appControllers);
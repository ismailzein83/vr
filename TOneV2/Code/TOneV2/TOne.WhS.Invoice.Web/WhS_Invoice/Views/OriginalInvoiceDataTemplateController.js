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
            $scope.scopeModel.originalDataCurrency = [];
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
                    if (originalInvoiceDataRuntime.OriginalDataCurrency != undefined)
                    {
                        for( var p in originalInvoiceDataRuntime.OriginalDataCurrency)
                        {
                            if (p != "$type")
                            {
                                var item = originalInvoiceDataRuntime.OriginalDataCurrency[p];
                                $scope.scopeModel.originalDataCurrency.push({
                                    Entity: {
                                        Currency: item.CurrencySymbol,
                                        CurrencyId: p,
                                        IncludeOriginalAmountInSettlement: item.IncludeOriginalAmountInSettlement,
                                        OriginalAmount: item.OriginalAmount,
                                    }
                                });
                            }
                            
                        }
                    }
                    $scope.scopeModel.reference = originalInvoiceDataRuntime.Reference;
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

            var originalDataCurrencyDic;
            if ($scope.scopeModel.originalDataCurrency != undefined) {
                originalDataCurrencyDic = {};
                for (var i = 0; i < $scope.scopeModel.originalDataCurrency.length; i++) {
                    var originalDataCurrency = $scope.scopeModel.originalDataCurrency[i];
                    if (originalDataCurrency.Entity.OriginalAmount != undefined)
                    {
                        originalDataCurrencyDic[originalDataCurrency.Entity.CurrencyId] = {
                            CurrencySymbol: originalDataCurrency.Entity.CurrencySymbol,
                            OriginalAmount: originalDataCurrency.Entity.OriginalAmount,
                            IncludeOriginalAmountInSettlement: originalDataCurrency.Entity.IncludeOriginalAmountInSettlement
                        };
                    }
                }
            }

            var obj = {
                OriginalDataCurrency: originalDataCurrencyDic,
                Reference: $scope.scopeModel.reference,
                AttachementFiles: attachementFileIds,
                InvoiceId: invoiceId,
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Invoice_OriginalInvoiceDataTemplateController', originalInvoiceDataTemplateController);

})(appControllers);
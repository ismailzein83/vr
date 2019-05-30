(function (appControllers) {

    'use strict';

    originalInvoiceDataTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Invoice_InvoiceAPIService', 'VR_Invoice_InvoiceAPIService', 'VRCommon_VRMailAPIService', 'WhS_BE_ToneModuleService'];

    function originalInvoiceDataTemplateController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_Invoice_InvoiceAPIService, VR_Invoice_InvoiceAPIService, VRCommon_VRMailAPIService, WhS_BE_ToneModuleService) {

        var invoiceAccountEntity;
        var invoiceId;
        var invoiceTypeId;
        var invoiceCarrierType;
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
                invoiceCarrierType = parameters.invoiceCarrierType;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.uploadedAttachements = [];
            $scope.scopeModel.originalDataCurrency = [];
            $scope.scopeModel.isSMSModuleEnabled = false;
            $scope.scopeModel.isVoiceModuleEnabled = false;

            $scope.scopeModel.showVoiceColumn = false;
            $scope.scopeModel.showSMSColumn = false;
            $scope.scopeModel.showRecurringChargeColumn = false;
            $scope.scopeModel.showDealColumn = false;

            $scope.scopeModel.save = function () {
                return updateOriginalInvoiceData();
            };

            $scope.scopeModel.validateOriginalData = function () {
                if ($scope.scopeModel.includeOriginalAmountInSettlement) {
                    var doesVoiceExist = false;
                    var doesSMSExist = false;
                    var doesRecurringChargeExist = false;
                    var doesDealExist = false;
                    for (var i = 0; i < $scope.scopeModel.originalDataCurrency.length; i++) {
                        var item = $scope.scopeModel.originalDataCurrency[i].Entity;
                        if (item.TrafficAmount != undefined || item.SMSAmount != undefined || item.DealAmount != undefined || item.RecurringChargeAmount != undefined) {
                            if (item.TrafficAmount != undefined)
                                doesVoiceExist = true;
                            if (item.SMSAmount != undefined)
                                doesSMSExist = true;
                            if (item.DealAmount != undefined)
                                doesDealExist = true;
                            if (item.RecurringChargeAmount != undefined)
                                doesRecurringChargeExist = true;
                        }
                    }
                    for (var j = 0; j < $scope.scopeModel.originalDataCurrency.length; j++) {
                        var entity = $scope.scopeModel.originalDataCurrency[j].Entity;
                        entity.IsTrafficRequired = doesVoiceExist;
                        entity.IsSMSRequired = doesSMSExist;
                        entity.IsDealRequired = doesDealExist;
                        entity.IsRecurringChargeRequired = doesRecurringChargeExist;
                    }

                    if (!doesVoiceExist && !doesSMSExist && !doesDealExist && !doesRecurringChargeExist)
                        return "At least one value must be added.";
                }
                else {
                    for (var k = 0; k < $scope.scopeModel.originalDataCurrency.length; k++) {
                        var data = $scope.scopeModel.originalDataCurrency[k].Entity;
                        data.IsTrafficRequired = false;
                        data.IsSMSRequired = false;
                        data.IsDealRequired = false;
                        data.IsRecurringChargeRequired = false;
                    }
                    return null;
                }
            };
            $scope.scopeModel.onUploadedAttachementFileReady = function (api) {
                fileAPI = api;
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
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
            return WhS_Invoice_InvoiceAPIService.GetOriginalInvoiceDataRuntime(invoiceId, invoiceCarrierType).then(function (response) {
                originalInvoiceDataRuntime = response;
                if (originalInvoiceDataRuntime != undefined) {
                    $scope.scopeModel.includeOriginalAmountInSettlement = originalInvoiceDataRuntime.IncludeOriginalAmountInSettlement;
                    if (originalInvoiceDataRuntime.OriginalDataCurrency != undefined) {
                        var noVoiceCount = 0;
                        var noSMSCount = 0;
                        var noRecurringChargeCount = 0;
                        var noDealCount = 0;
                        var originalDataCurrencyCount = 0;
                        for (var p in originalInvoiceDataRuntime.OriginalDataCurrency) {
                            if (p != "$type") {
                                var item = originalInvoiceDataRuntime.OriginalDataCurrency[p];
                                $scope.scopeModel.originalDataCurrency.push({
                                    Entity: {
                                        Currency: item.CurrencySymbol,
                                        CurrencyId: p,
                                        OriginalAmount: item.OriginalAmount,
                                        TrafficAmount: item.HasTrafficAmount ? item.TrafficAmount : undefined,
                                        SMSAmount: item.HasSMSAmount ? item.SMSAmount : undefined,
                                        RecurringChargeAmount: item.HasRecurringChargeAmount ? item.RecurringChargeAmount : undefined,
                                        DealAmount: item.HasDealAmount ? item.DealAmount : undefined,
                                        HasTrafficAmount: item.HasTrafficAmount,
                                        HasSMSAmount: item.HasSMSAmount,
                                        HasRecurringChargeAmount: item.HasRecurringChargeAmount,
                                        HasDealAmount: item.HasDealAmount
                                    }
                                });
                                originalDataCurrencyCount++;
                                if (!item.HasTrafficAmount)
                                    noVoiceCount++;
                                if (!item.HasSMSAmount)
                                    noSMSCount++;
                                if (!item.HasRecurringChargeAmount)
                                    noRecurringChargeCount++;
                                if (!item.HasDealAmount)
                                    noDealCount++;
                            }
                        }
                        $scope.scopeModel.showVoiceColumn = originalDataCurrencyCount != noVoiceCount;
                        $scope.scopeModel.showSMSColumn = originalDataCurrencyCount != noSMSCount;
                        $scope.scopeModel.showRecurringChargeColumn = originalDataCurrencyCount != noRecurringChargeCount;
                        $scope.scopeModel.showDealColumn = originalDataCurrencyCount != noDealCount;
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
            function getModuleFlags() {
                $scope.scopeModel.isSMSModuleEnabled = WhS_BE_ToneModuleService.isSMSModuleEnabled();
                $scope.scopeModel.isVoiceModuleEnabled = WhS_BE_ToneModuleService.isVoiceModuleEnabled();
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, getModuleFlags]).catch(function (error) {
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
                for(var i=0; i<$scope.scopeModel.uploadedAttachements.length; i++)
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
                    originalDataCurrencyDic[originalDataCurrency.Entity.CurrencyId] = {
                                CurrencySymbol: originalDataCurrency.Entity.CurrencySymbol,
                                OriginalAmount: originalDataCurrency.Entity.OriginalAmount,
                                TrafficAmount: originalDataCurrency.Entity.TrafficAmount,
                                SMSAmount: originalDataCurrency.Entity.SMSAmount,
                                RecurringChargeAmount: originalDataCurrency.Entity.RecurringChargeAmount,
                                DealAmount: originalDataCurrency.Entity.DealAmount
                    };
                }
            }

            var obj = {
                OriginalDataCurrency: originalDataCurrencyDic,
                Reference: $scope.scopeModel.reference,
                AttachementFiles: attachementFileIds,
                InvoiceId: invoiceId,
                InvoiceCarrierType: invoiceCarrierType,
                IncludeOriginalAmountInSettlement: $scope.scopeModel.includeOriginalAmountInSettlement
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Invoice_OriginalInvoiceDataTemplateController', originalInvoiceDataTemplateController);

})(appControllers);
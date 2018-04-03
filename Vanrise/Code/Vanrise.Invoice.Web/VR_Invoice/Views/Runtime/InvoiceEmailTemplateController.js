(function (appControllers) {

    "use strict";

    invoiceTemplateEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceEmailActionAPIService','VR_Invoice_InvoiceTypeAPIService','VRCommon_VRMailAPIService'];

    function invoiceTemplateEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceEmailActionAPIService, VR_Invoice_InvoiceTypeAPIService, VRCommon_VRMailAPIService) {
        var invoiceId;
        var invoiceActionId;
        var invoiceTypeId;
        var invoiceActionEntity;
        var invoiceTemplateEntity;

        var invoiceMailTemplateReadyAPI;
        var invoiceMailTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var fileAPI;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceId = parameters.invoiceId;
                invoiceActionId = parameters.invoiceActionId;
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.uploadedAttachements = [];
            $scope.scopeModel.onUploadedAttachementFileReady = function (api) {
                fileAPI = api;
            };
            $scope.scopeModel.downloadAttachement = function (attachedfileId) {
                $scope.scopeModel.isLoading = true;
                return VRCommon_VRMailAPIService.DownloadAttachement(attachedfileId).then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (response != undefined)
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };
            $scope.scopeModel.addUploadedAttachement = function (obj) {
                if (obj != undefined) {
                    $scope.scopeModel.uploadedAttachements.push(obj);
                    fileAPI.clearFileUploader();
                }
            };
            $scope.scopeModel.onInvoiceMailTemplateSelectorReady = function (api) {
                invoiceMailTemplateReadyAPI = api;
                invoiceMailTemplateReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.sendEmail = function () {
                    return sendEmail();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onAttachmentItemClicked = function (dataItem) {
                if (dataItem != undefined)
                {
                    return downloadAttachment(dataItem.AttachmentId);
                }
               
            };

            $scope.scopeModel.onInvoiceMailTemplateSelectionChanged = function (value) {
                $scope.scopeModel.isLoading = true;
                if (value != undefined) {
                    getInvoiceEmail().then(function () {
                        if (invoiceTemplateEntity != undefined && invoiceTemplateEntity.VRMailEvaluatedTemplate != undefined)
                        {
                            $scope.scopeModel.cc = invoiceTemplateEntity.VRMailEvaluatedTemplate.CC;
                            $scope.scopeModel.to = invoiceTemplateEntity.VRMailEvaluatedTemplate.To;
                            $scope.scopeModel.subject = invoiceTemplateEntity.VRMailEvaluatedTemplate.Subject;
                            $scope.scopeModel.body = invoiceTemplateEntity.VRMailEvaluatedTemplate.Body;
                            $scope.scopeModel.from = invoiceTemplateEntity.VRMailEvaluatedTemplate.From;

                        }
                        $scope.scopeModel.attachments = invoiceTemplateEntity.EmailAttachments;

                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
                else {
                    $scope.scopeModel.cc = undefined;
                    $scope.scopeModel.to = undefined;
                    $scope.scopeModel.subject = undefined;
                    $scope.scopeModel.body = undefined;
                    $scope.scopeModel.from = undefined;
                    $scope.scopeModel.isLoading = false;
                }

            };

            function sendEmail() {
                $scope.scopeModel.isLoading = true;

                var emailObject = buildInvoiceTemplateObjFromScope();
                return VR_Invoice_InvoiceEmailActionAPIService.SendEmail(emailObject)
               .then(function (response) {
                       if ($scope.onInvoiceEmailSend != undefined)
                           $scope.onInvoiceEmailSend(response);
                       $scope.modalContext.closeModal();
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
            getInvoiceAction().then(function () {
                loadMailMsgTemplateSelector().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            });
           
          

        }

        function getInvoiceAction()
        {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceAction(invoiceTypeId, invoiceActionId).then(function(response)
            {
                invoiceActionEntity = response;
            })
        }

        function loadMailMsgTemplateSelector() {
            var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            invoiceMailTemplateReadyPromiseDeferred.promise.then(function () {
                var selectorPayload = { filter: { VRMailMessageTypeId: invoiceActionEntity.Settings.InvoiceMailTypeId }, selectFirstItem: true };
                VRUIUtilsService.callDirectiveLoad(invoiceMailTemplateReadyAPI, selectorPayload, mailMsgTemplateSelectorLoadDeferred);
            });
            return mailMsgTemplateSelectorLoadDeferred.promise;
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = "Invoice Email";
            }
            function loadStaticData() {
                if (invoiceTemplateEntity != undefined) {
                    $scope.scopeModel.cc = invoiceTemplateEntity.CC;
                    $scope.scopeModel.to = invoiceTemplateEntity.To;
                    $scope.scopeModel.subject = invoiceTemplateEntity.Subject;
                    $scope.scopeModel.body = invoiceTemplateEntity.Body;
                    $scope.scopeModel.from = invoiceTemplateEntity.From;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, getInvoiceEmail])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function getInvoiceEmail() {
            var invoiceMailTemplateId = invoiceMailTemplateReadyAPI.getSelectedIds();
            if (invoiceMailTemplateId != undefined) {
                return VR_Invoice_InvoiceEmailActionAPIService.GetEmailTemplate(invoiceId, invoiceMailTemplateId, invoiceActionId).then(function (response) {
                    invoiceTemplateEntity = response;
                });
            }
        }

        function buildInvoiceTemplateObjFromScope() {
            var attachementFileIds = $scope.scopeModel.uploadedAttachements.map(function (a) { return a.fileId; });
          
            var obj = {
                InvoiceId:invoiceId,
                InvoiceActionId: invoiceActionId,
                EmailTemplate: {
                    From: $scope.scopeModel.from,
                    CC: $scope.scopeModel.cc,
                    To: $scope.scopeModel.to,
                    Subject: $scope.scopeModel.subject,
                    Body: $scope.scopeModel.body,
                },
                AttachementFileIds: attachementFileIds
            };
            return obj;
        }

        function downloadAttachment(attachmentId)
        {
            $scope.scopeModel.isLoading = true;
            return VR_Invoice_InvoiceEmailActionAPIService.DownloadAttachment(invoiceId, attachmentId).then(function (response) {
                $scope.scopeModel.isLoading = false;
                if (response != undefined)
                   UtilsService.downloadFile(response.data, response.headers);
            });
        }
    }

    appControllers.controller('VR_Invoice_InvoiceTemplateEditorController', invoiceTemplateEditorController);
})(appControllers);

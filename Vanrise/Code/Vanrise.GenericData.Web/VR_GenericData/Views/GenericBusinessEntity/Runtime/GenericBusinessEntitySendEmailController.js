//(function (appControllers) {

//    "use strict";

//    GenericBusinessEntitySendEmailController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceEmailActionAPIService', 'VR_Invoice_InvoiceTypeAPIService', 'VRCommon_VRMailAPIService'];

//    function GenericBusinessEntitySendEmailController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceEmailActionAPIService, VR_Invoice_InvoiceTypeAPIService, VRCommon_VRMailAPIService) {
//        var invoiceId;
//        var invoiceActionId;
//        var invoiceTypeId;
//        var invoiceActionEntity;
//        var genericBETemplateEntity;

//        var genericBEMailTemplateReadyAPI;
//        var genericBEMailTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//        var fileAPI;
//        defineScope();
//        loadParameters();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);
//            if (parameters != undefined && parameters != null) {
//                invoiceId = parameters.invoiceId;
//                invoiceActionId = parameters.invoiceActionId;
//                invoiceTypeId = parameters.invoiceTypeId;
//            }
//        }

//        function defineScope() {
//            $scope.scopeModel = {};
//            $scope.scopeModel.uploadedAttachements = [];
//            $scope.scopeModel.onUploadedAttachementFileReady = function (api) {
//                fileAPI = api;
//            };
//            $scope.scopeModel.downloadAttachement = function (attachedfileId) {
//                $scope.scopeModel.isLoading = true;
//                return VRCommon_VRMailAPIService.DownloadAttachement(attachedfileId).then(function (response) {
//                    $scope.scopeModel.isLoading = false;
//                    if (response != undefined)
//                        UtilsService.downloadFile(response.data, response.headers);
//                });
//            };
//            $scope.scopeModel.addUploadedAttachement = function (obj) {
//                if (obj != undefined) {
//                    $scope.scopeModel.uploadedAttachements.push(obj);
//                    fileAPI.clearFileUploader();
//                }
//            };
//            $scope.scopeModel.onGenericBEMailTemplateSelectorReady = function (api) {
//                genericBEMailTemplateReadyAPI = api;
//                genericBEMailTemplateReadyPromiseDeferred.resolve();
//            };
//            $scope.scopeModel.sendEmail = function () {
//                return sendEmail();
//            };
//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };

//            $scope.scopeModel.onGenericBEMailTemplateSelectionChanged = function (value) {
//                $scope.scopeModel.isLoading = true;
//                if (value != undefined) {
//                    getInvoiceEmail().then(function () {
//                        if (genericBETemplateEntity != undefined && genericBETemplateEntity.VRMailEvaluatedTemplate != undefined) {
//                            $scope.scopeModel.cc = genericBETemplateEntity.VRMailEvaluatedTemplate.CC != undefined ? genericBETemplateEntity.VRMailEvaluatedTemplate.CC.split(';') : [];
//                            $scope.scopeModel.to = genericBETemplateEntity.VRMailEvaluatedTemplate.To != undefined ? genericBETemplateEntity.VRMailEvaluatedTemplate.To.split(';') : [];
//                            $scope.scopeModel.subject = genericBETemplateEntity.VRMailEvaluatedTemplate.Subject;
//                            $scope.scopeModel.body = genericBETemplateEntity.VRMailEvaluatedTemplate.Body;
//                            $scope.scopeModel.from = genericBETemplateEntity.VRMailEvaluatedTemplate.From != "" ? genericBETemplateEntity.VRMailEvaluatedTemplate.From : null;

//                        }
//                        $scope.scopeModel.attachments = genericBETemplateEntity.EmailAttachments;

//                    }).catch(function (error) {
//                        VRNotificationService.notifyExceptionWithClose(error, $scope);
//                        $scope.scopeModel.isLoading = false;
//                    }).finally(function () {
//                        $scope.scopeModel.isLoading = false;
//                    });
//                }
//                else {
//                    $scope.scopeModel.cc = [];
//                    $scope.scopeModel.to = [];
//                    $scope.scopeModel.subject = undefined;
//                    $scope.scopeModel.body = undefined;
//                    $scope.scopeModel.from = null;
//                    $scope.scopeModel.isLoading = false;
//                }

//            };

//            function sendEmail() {
//                $scope.scopeModel.isLoading = true;

//                var emailObject = buildGenericBETemplateObjFromScope();
//                return VR_Invoice_InvoiceEmailActionAPIService.SendEmail(emailObject)
//                    .then(function (response) {
//                        if ($scope.onInvoiceEmailSend != undefined)
//                            $scope.onInvoiceEmailSend(response);
//                        $scope.modalContext.closeModal();
//                    })
//                    .catch(function (error) {
//                        VRNotificationService.notifyException(error, $scope);
//                    }).finally(function () {
//                        $scope.scopeModel.isLoading = false;
//                    });
//            }
//        }

//        function load() {
//            $scope.scopeModel.isLoading = true;
//            getInvoiceAction().then(function () {
//                loadMailMsgTemplateSelector().then(function () {
//                    loadAllControls();
//                }).catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                    $scope.scopeModel.isLoading = false;
//                });
//            });



//        }

//        function getInvoiceAction() {
//            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceAction(invoiceTypeId, invoiceActionId).then(function (response) {
//                invoiceActionEntity = response;
//            });
//        }

//        function loadMailMsgTemplateSelector() {
//            var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
//            invoiceMailTemplateReadyPromiseDeferred.promise.then(function () {
//                var selectorPayload = { filter: { VRMailMessageTypeId: invoiceActionEntity.Settings.InvoiceMailTypeId }, selectFirstItem: true };
//                VRUIUtilsService.callDirectiveLoad(invoiceMailTemplateReadyAPI, selectorPayload, mailMsgTemplateSelectorLoadDeferred);
//            });
//            return mailMsgTemplateSelectorLoadDeferred.promise;
//        }

//        function loadAllControls() {

//            function setTitle() {
//                $scope.title = "Invoice Email";
//            }
//            function loadStaticData() {
//                if (genericBETemplateEntity != undefined) {
//                    $scope.scopeModel.cc = genericBETemplateEntity.CC != undefined ? genericBETemplateEntity.CC.split(';') : [];
//                    $scope.scopeModel.from = genericBETemplateEntity.From != "" ? genericBETemplateEntity.From : null;
//                    $scope.scopeModel.subject = genericBETemplateEntity.Subject;
//                    $scope.scopeModel.body = genericBETemplateEntity.Body;
//                    $scope.scopeModel.to = genericBETemplateEntity.To != undefined ? genericBETemplateEntity.To.split(';') : [];
//                }
//            }

//            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, getInvoiceEmail])
//                .catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                })
//                .finally(function () {
//                    $scope.scopeModel.isLoading = false;
//                });
//        }

//        function getInvoiceEmail() {
//            var invoiceMailTemplateId = invoiceMailTemplateReadyAPI.getSelectedIds();
//            if (invoiceMailTemplateId != undefined) {
//                return VR_Invoice_InvoiceEmailActionAPIService.GetEmailTemplate(invoiceId, invoiceMailTemplateId, invoiceActionId).then(function (response) {
//                    genericBETemplateEntity = response;
//                });
//            }
//        }

//        function buildGenericBETemplateObjFromScope() {
//            var attachementFileIds = $scope.scopeModel.uploadedAttachements.map(function (a) { return a.fileId; });

//            var obj = {
//                InvoiceId: invoiceId,
//                InvoiceActionId: invoiceActionId,
//                EmailTemplate: {
//                    From: $scope.scopeModel.from,
//                    CC: $scope.scopeModel.cc.join(';'),
//                    To: $scope.scopeModel.to.join(';'),
//                    Subject: $scope.scopeModel.subject,
//                    Body: $scope.scopeModel.body,
//                },
//                AttachementFileIds: attachementFileIds
//            };
//            return obj;
//        }

//        function downloadAttachment(attachmentId) {
//            $scope.scopeModel.isLoading = true;
//            return VR_Invoice_InvoiceEmailActionAPIService.DownloadAttachment(invoiceId, attachmentId).then(function (response) {
//                $scope.scopeModel.isLoading = false;
//                if (response != undefined)
//                    UtilsService.downloadFile(response.data, response.headers);
//            });
//        }
//    }

//    appControllers.controller('VR_Invoice_GenericBusinessEntitySendEmailController', GenericBusinessEntitySendEmailController);
//})(appControllers);

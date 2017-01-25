(function (appControllers) {

    "use strict";

    invoiceTemplateEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceEmailActionAPIService','VR_Invoice_InvoiceTypeAPIService'];

    function invoiceTemplateEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceEmailActionAPIService, VR_Invoice_InvoiceTypeAPIService) {
        var invoiceId;
        var invoiceActionId;
        var invoiceTypeId;
        var invoiceActionEntity;
        var invoiceTemplateEntity;

        var invoiceMailTemplateReadyAPI;
        var invoiceMailTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onInvoiceMailTemplateSelectionChanged = function (value) {
                $scope.scopeModel.isLoading = true;
                if (value != undefined) {
                    getInvoiceEmail().then(function () {
                        $scope.scopeModel.cc = invoiceTemplateEntity.CC;
                        $scope.scopeModel.to = invoiceTemplateEntity.To;
                        $scope.scopeModel.subject = invoiceTemplateEntity.Subject;
                        $scope.scopeModel.body = invoiceTemplateEntity.Body;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }

            }

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
            return VR_Invoice_InvoiceEmailActionAPIService.GetEmailTemplate(invoiceId, invoiceActionId, invoiceMailTemplateReadyAPI.getSelectedIds()).then(function (response) {
                invoiceTemplateEntity = response;
            });
        }

        function buildInvoiceTemplateObjFromScope() {
            var obj = {
                InvoiceId:invoiceId,
                InvoiceActionId: invoiceActionId,
                EmailTemplate: {
                    CC: $scope.scopeModel.cc,
                    To: $scope.scopeModel.to,
                    Subject: $scope.scopeModel.subject,
                    Body: $scope.scopeModel.body,
                }
               
            };
            return obj;
        }
    }

    appControllers.controller('VR_Invoice_InvoiceTemplateEditorController', invoiceTemplateEditorController);
})(appControllers);

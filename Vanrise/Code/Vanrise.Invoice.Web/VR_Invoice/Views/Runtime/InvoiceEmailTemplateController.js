(function (appControllers) {

    "use strict";

    invoiceTemplateEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceEmailActionAPIService'];

    function invoiceTemplateEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceEmailActionAPIService) {
        var invoiceId;
        var invoiceActionId;
        var invoiceTemplateEntity;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceId = parameters.invoiceId;
                invoiceActionId = parameters.invoiceActionId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.sendEmail = function () {
                    return sendEmail();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
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
            getInvoiceEmail().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            function getInvoiceEmail() {
                return VR_Invoice_InvoiceEmailActionAPIService.GetEmailTemplate(invoiceId).then(function (response) {
                    invoiceTemplateEntity = response;
                });
            }

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

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
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

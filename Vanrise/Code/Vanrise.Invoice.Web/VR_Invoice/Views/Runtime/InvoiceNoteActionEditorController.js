(function (appControllers) {

    "use strict";

    invoiceNoteActionEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VR_Invoice_InvoiceAPIService'];

    function invoiceNoteActionEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VR_Invoice_InvoiceAPIService) {
        var invoiceId;
        var invoiceActionId;
        var invoiceEntity;
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

            $scope.scopeModel.save = function () {
                return updateNote();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function updateNote() {
                $scope.scopeModel.isLoading = true;
                var invoiceNote = null;
                if ($scope.scopeModel.invoiceNote != undefined)
                    invoiceNote = $scope.scopeModel.invoiceNote;
                return VR_Invoice_InvoiceAPIService.UpdateInvoiceNote(invoiceActionId, invoiceId, $scope.scopeModel.invoiceNote)
               .then(function (response) {
                   if ($scope.onInvoiceNoteAdded != undefined)
                       $scope.onInvoiceNoteAdded(response);
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
            getInvoice().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
            function getInvoice() {
                return VR_Invoice_InvoiceAPIService.GetInvoice(invoiceId).then(function (response) {
                    invoiceEntity = response;
                });
            }
        }
        function loadAllControls() {

            function setTitle() {
                $scope.title = "Invoice Note";
            }
            function loadStaticData() {
                if (invoiceEntity != undefined) {
                    $scope.scopeModel.invoiceNote = invoiceEntity.Note;
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

    appControllers.controller('VR_Invoice_InvoiceNoteActionEditorController', invoiceNoteActionEditorController);
})(appControllers);

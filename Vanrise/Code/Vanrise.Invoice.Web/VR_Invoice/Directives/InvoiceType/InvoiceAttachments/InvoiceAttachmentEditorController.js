(function (appControllers) {

    "use strict";

    attachmentEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function attachmentEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var attachmentTypeAPI;
        var attachmentTypeReadyDeferred = UtilsService.createPromiseDeferred();
        var attachmentEntity;
        var isEditMode;
        var context;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                attachmentEntity = parameters.attachmentEntity;
                context = parameters.context;
            }
            isEditMode = (attachmentEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onAttachmentTypeReady = function (api) {
                attachmentTypeAPI = api;
                attachmentTypeReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return save();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            function save() {
                var attachmentObject = buildAttachmentObjFromScope();
                if ($scope.onAttachmentAdded != undefined)
                    $scope.onAttachmentAdded(attachmentObject);
                $scope.modalContext.closeModal();
            }

            function update() {
                var attachmentObject = buildAttachmentObjFromScope();
                if ($scope.onAttachmentUpdated != undefined)
                    $scope.onAttachmentUpdated(attachmentObject);
                $scope.modalContext.closeModal();
            }

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {

            function setTitle() {
                if (isEditMode && attachmentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(attachmentEntity.Title, 'Attachment');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Attachment');
            }
            function loadAttachmentTypeDirective() {
                var attachmentTypeLoadDeferred = UtilsService.createPromiseDeferred();
                attachmentTypeReadyDeferred.promise.then(function () {
                    var attachmentTypePayload = { context: getContext() };
                    if (attachmentEntity != undefined) {
                        attachmentTypePayload.invoiceFileConverter = attachmentEntity.InvoiceFileConverter;
                    }
                    VRUIUtilsService.callDirectiveLoad(attachmentTypeAPI, attachmentTypePayload, attachmentTypeLoadDeferred);
                });
                return attachmentTypeLoadDeferred.promise;
            }
            function loadStaticData() {
                if (attachmentEntity != undefined) {
                    $scope.scopeModel.attachmentTitle = attachmentEntity.Title;
                }
            }


            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAttachmentTypeDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildAttachmentObjFromScope() {
            var obj = {
                InvoiceAttachmentId: attachmentEntity != undefined ? attachmentEntity.InvoiceAttachmentId : UtilsService.guid(),
                Title: $scope.scopeModel.attachmentTitle,
                InvoiceFileConverter: attachmentTypeAPI.getData()
            };
            return obj;
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_Invoice_AttachmentEditorController', attachmentEditorController);
})(appControllers);

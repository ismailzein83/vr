(function (appControllers) {

    'use strict';

    fileAttachmentEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function fileAttachmentEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var fileAttachmentEntity;
        var invoiceAttachmentsAPI;
        var invoiceAttachmentsReadyDeferred = UtilsService.createPromiseDeferred();

        var isEditMode;
    
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                fileAttachmentEntity = parameters.fileAttachmentEntity;
            }
            isEditMode = (fileAttachmentEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateFileAttachment() : addeFileAttachment();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onInvoiceAttachmentsReady = function (api) {
                invoiceAttachmentsAPI = api;
                invoiceAttachmentsReadyDeferred.resolve();
            };
            function builFileAttachmentObjFromScope() {
                return {
                    Name: $scope.scopeModel.name,
                    AttachmentId: invoiceAttachmentsAPI.getSelectedIds()
                };
            }
            function addeFileAttachment() {
                var fileAttachmentObj = builFileAttachmentObjFromScope();
                if ($scope.onFileAttachmentAdded != undefined) {
                    $scope.onFileAttachmentAdded(fileAttachmentObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateFileAttachment() {
                var fileAttachmentObj = builFileAttachmentObjFromScope();
                if ($scope.onFileAttachmentUpdated != undefined) {
                    $scope.onFileAttachmentUpdated(fileAttachmentObj);
                }
                $scope.modalContext.closeModal();
            }
        }
        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && fileAttachmentEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(fileAttachmentEntity.Name, 'File');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('File');
                }
                function loadStaticData() {
                    if (fileAttachmentEntity != undefined) {
                        $scope.scopeModel.name = fileAttachmentEntity.Name;

                    }
                }
                function loadInvoiceAttachmentsDirective() {
                    var invoiceAttachmentsLoadDeferred = UtilsService.createPromiseDeferred();
                    invoiceAttachmentsReadyDeferred.promise.then(function () {
                        var invoiceAttachmentsPayload = { context: getContext() };
                        if (fileAttachmentEntity != undefined)
                            invoiceAttachmentsPayload.selectedIds = fileAttachmentEntity.AttachmentId;
                        VRUIUtilsService.callDirectiveLoad(invoiceAttachmentsAPI, invoiceAttachmentsPayload, invoiceAttachmentsLoadDeferred);
                    });
                    return invoiceAttachmentsLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceAttachmentsDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
        }
        function getContext() {
            return context;
        }

    }
    appControllers.controller('VR_Invoice_FileAttachmentEditorController', fileAttachmentEditorController);

})(appControllers);
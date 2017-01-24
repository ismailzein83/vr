(function (appControllers) {

    "use strict";

    emailAttachmentEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function emailAttachmentEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var sendEmailAttachmentTypeAPI;
        var sendEmailAttachmentTypeReadyDeferred = UtilsService.createPromiseDeferred();
        var emailAttachmentEntity;
        var isEditMode;
        var context;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                emailAttachmentEntity = parameters.emailAttachmentEntity;
                context = parameters.context;
            }
            isEditMode = (emailAttachmentEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSendEmailAttachmentTypeReady = function (api) {
                sendEmailAttachmentTypeAPI = api;
                sendEmailAttachmentTypeReadyDeferred.resolve();
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
                var emailAttachmentObject = buildEmailAttachmentObjFromScope();
                if ($scope.onEmailAttachmentAdded != undefined)
                    $scope.onEmailAttachmentAdded(emailAttachmentObject);
                $scope.modalContext.closeModal();
            }

            function update() {
                var emailAttachmentObject = buildEmailAttachmentObjFromScope();
                if ($scope.onEmailAttachmentUpdated != undefined)
                    $scope.onEmailAttachmentUpdated(emailAttachmentObject);
                $scope.modalContext.closeModal();
            }

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {

            function setTitle() {
                if (isEditMode && emailAttachmentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(emailAttachmentEntity.Title, 'Email Attachment');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Email Attachment');
            }
            function loadEmailAttachmentTypeDirective() {
                var sendEmailAttachmentTypeLoadDeferred = UtilsService.createPromiseDeferred();
                sendEmailAttachmentTypeReadyDeferred.promise.then(function () {
                    var sendEmailAttachmentTypePayload = { context: getContext() };
                    if (emailAttachmentEntity != undefined) {
                        sendEmailAttachmentTypePayload.invoiceFileConverter = emailAttachmentEntity.InvoiceFileConverter;
                    }
                    VRUIUtilsService.callDirectiveLoad(sendEmailAttachmentTypeAPI, sendEmailAttachmentTypePayload, sendEmailAttachmentTypeLoadDeferred);
                });
                return sendEmailAttachmentTypeLoadDeferred.promise;
            }
            function loadStaticData() {
                if(emailAttachmentEntity !=undefined)
                {
                    $scope.scopeModel.attachmentTitle = emailAttachmentEntity.Title;
                }
            }


            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEmailAttachmentTypeDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildEmailAttachmentObjFromScope() {
            var obj = {
                Title: $scope.scopeModel.attachmentTitle,
                InvoiceFileConverter: sendEmailAttachmentTypeAPI.getData()
            };
            return obj;
        }

        function getContext() {
            var currentContext = context;
            if(currentContext == undefined)
                currentContext  = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_Invoice_EmailAttachmentEditorController', emailAttachmentEditorController);
})(appControllers);

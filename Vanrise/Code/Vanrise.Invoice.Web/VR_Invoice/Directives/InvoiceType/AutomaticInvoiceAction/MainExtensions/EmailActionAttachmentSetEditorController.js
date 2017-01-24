(function (appControllers) {

    "use strict";

    emailAttachmentSetEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function emailAttachmentSetEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var sendEmailAttachmentsAPI;
        var sendEmailAttachmentsReadyDeferred = UtilsService.createPromiseDeferred();
        var emailAttachmentSetEntity;
        var isEditMode;
        var context;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                emailAttachmentSetEntity = parameters.emailAttachmentSetEntity;
                context = parameters.context;
            }
            isEditMode = (emailAttachmentSetEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSendEmailAttachmentsReady = function (api) {
                sendEmailAttachmentsAPI = api;
                sendEmailAttachmentsReadyDeferred.resolve();
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
                var emailAttachmentSetObject = buildEmailAttachmentSetObjFromScope();
                if ($scope.onEmailAttachmentSetAdded != undefined)
                    $scope.onEmailAttachmentSetAdded(emailAttachmentSetObject);
                $scope.modalContext.closeModal();
            }

            function update() {
                var emailAttachmentSetObject = buildEmailAttachmentSetObjFromScope();
                if ($scope.onEmailAttachmentSetUpdated != undefined)
                    $scope.onEmailAttachmentSetUpdated(emailAttachmentSetObject);
                $scope.modalContext.closeModal();
            }

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {

            function setTitle() {
                if (isEditMode && emailAttachmentSetEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(emailAttachmentSetEntity.Title, 'Email Attachment Set');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Email Attachment Set');
            }
            function loadEmailAttachmentSetTypeDirective() {
                var sendEmailAttachmentsLoadDeferred = UtilsService.createPromiseDeferred();
                sendEmailAttachmentsReadyDeferred.promise.then(function () {
                    var sendEmailAttachmentsPayload = { context: getContext() };
                    if (emailAttachmentSetEntity != undefined) {
                        sendEmailAttachmentsPayload.emailAttachmentsEntity = emailAttachmentSetEntity.EmailAttachments;
                    }
                    VRUIUtilsService.callDirectiveLoad(sendEmailAttachmentsAPI, sendEmailAttachmentsPayload, sendEmailAttachmentsLoadDeferred);
                });
                return sendEmailAttachmentsLoadDeferred.promise;
            }
            function loadStaticData() {
                if (emailAttachmentSetEntity != undefined) {
                    $scope.scopeModel.name = emailAttachmentSetEntity.Name;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEmailAttachmentSetTypeDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildEmailAttachmentSetObjFromScope() {
            var obj = {
                EmailActionAttachmentSetId:emailAttachmentSetEntity!=undefined?emailAttachmentSetEntity.EmailActionAttachmentSetId:UtilsService.guid(),
                Name: $scope.scopeModel.name,
                EmailAttachments: sendEmailAttachmentsAPI.getData()
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

    appControllers.controller('VR_Invoice_EmailAttachmentSetEditorController', emailAttachmentSetEditorController);
})(appControllers);

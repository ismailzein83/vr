(function (appControllers) {

    "use strict";

    emailAttachmentSetEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function emailAttachmentSetEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var emailAttachmentSetEntity;
        var isEditMode;
        var context;
        var invoiceFilterConditionAPI;
        var invoiceFilterConditionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceAttachmentSelectorAPI;
        var invoiceAttachmentSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var mailMessageTypeAPI;
        var mailMessageTypeReadyDeferred = UtilsService.createPromiseDeferred();
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
            $scope.scopeModel.onInvoiceFilterConditionReady = function (api) {
                invoiceFilterConditionAPI = api;
                invoiceFilterConditionReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onInvoiceAttachmentSelectorReady = function (api) {
                invoiceAttachmentSelectorAPI = api;
                invoiceAttachmentSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                mailMessageTypeAPI = api;
                mailMessageTypeReadyDeferred.resolve();
            };
            $scope.scopeModel.isMailMessageTypeRequired = function()
            {
                if (context != undefined && context.isMailMessageTypeSelected())
                    return false;
                return true;
            }
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
                    $scope.title = UtilsService.buildTitleForUpdateEditor(emailAttachmentSetEntity.Name, 'Email Attachment Set');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Email Attachment Set');
            }
            function loadStaticData() {
                if (emailAttachmentSetEntity != undefined) {
                    $scope.scopeModel.name = emailAttachmentSetEntity.Name;
                }
            }
            function loadInvoiceFilterConditionDirective() {
                var invoiceFilterConditionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceFilterConditionReadyPromiseDeferred.promise.then(function () {
                    var invoiceFilterConditionPayload = { context: getContext() };
                    if (emailAttachmentSetEntity != undefined) {
                        invoiceFilterConditionPayload.invoiceFilterConditionEntity = emailAttachmentSetEntity.FilterCondition;
                    }
                    VRUIUtilsService.callDirectiveLoad(invoiceFilterConditionAPI, invoiceFilterConditionPayload, invoiceFilterConditionLoadPromiseDeferred);
                });
                return invoiceFilterConditionLoadPromiseDeferred.promise;
            }
            function loadInvoiceAttachmentSelectorDirective() {
                var invoiceAttachmentSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceAttachmentSelectorReadyDeferred.promise.then(function () {
                    var invoiceAttachmentSelectorPayload = { context: getContext() };
                    if (emailAttachmentSetEntity != undefined) {
                        invoiceAttachmentSelectorPayload.selectedIds = emailAttachmentSetEntity.AttachmentsIds;
                    }
                    VRUIUtilsService.callDirectiveLoad(invoiceAttachmentSelectorAPI, invoiceAttachmentSelectorPayload, invoiceAttachmentSelectorLoadPromiseDeferred);
                });
                return invoiceAttachmentSelectorLoadPromiseDeferred.promise;
            }
            function loadMailMessageTypeSelector() {
                var mailMessageTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceAttachmentSelectorReadyDeferred.promise.then(function () {
                    var mailMessageTypeSelectorPayload = { context: getContext() };
                    if (emailAttachmentSetEntity != undefined) {
                        mailMessageTypeSelectorPayload.selectedIds = emailAttachmentSetEntity.MailMessageTypeId;
                    }
                    VRUIUtilsService.callDirectiveLoad(mailMessageTypeAPI, mailMessageTypeSelectorPayload, mailMessageTypeSelectorLoadPromiseDeferred);
                });
                return mailMessageTypeSelectorLoadPromiseDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceFilterConditionDirective, loadInvoiceAttachmentSelectorDirective, loadMailMessageTypeSelector])
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
                FilterCondition: invoiceFilterConditionAPI.getData(),
                AttachmentsIds: invoiceAttachmentSelectorAPI.getSelectedIds(),
                MailMessageTypeId: mailMessageTypeAPI.getSelectedIds()
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

(function (appControllers) {

    "use strict";

    saveInvoiceToFileActionSetEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function saveInvoiceToFileActionSetEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var invoiceToFileActionSetEntity;
        var isEditMode;
        var context;
        var invoiceFilterConditionAPI;
        var invoiceFilterConditionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceAttachmentSelectorAPI;
        var invoiceAttachmentSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceToFileActionSetEntity = parameters.invoiceToFileActionSetEntity;
                context = parameters.context;
            }
            isEditMode = (invoiceToFileActionSetEntity != undefined);
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
                var invoiceToFileActionSetObject = buildInvoiceToFileActionSetObjFromScope();
                if ($scope.onInvoiceToFileActionSetAdded != undefined)
                    $scope.onInvoiceToFileActionSetAdded(invoiceToFileActionSetObject);
                $scope.modalContext.closeModal();
            }

            function update() {
                var invoiceToFileActionSetObject = buildInvoiceToFileActionSetObjFromScope();
                if ($scope.onInvoiceToFileActionSetUpdated != undefined)
                    $scope.onInvoiceToFileActionSetUpdated(invoiceToFileActionSetObject);
                $scope.modalContext.closeModal();
            }

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {

            function setTitle() {
                if (isEditMode && invoiceToFileActionSetEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceToFileActionSetEntity.Name, 'Invoice File Set');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Invoice File Set');
            }

            function loadStaticData() {
                if (invoiceToFileActionSetEntity != undefined) {
                    $scope.scopeModel.name = invoiceToFileActionSetEntity.Name;
                }
            }

            function loadInvoiceFilterConditionDirective() {
                var invoiceFilterConditionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceFilterConditionReadyPromiseDeferred.promise.then(function () {
                    var invoiceFilterConditionPayload = { context: getContext() };
                    if (invoiceToFileActionSetEntity != undefined) {
                        invoiceFilterConditionPayload.invoiceFilterConditionEntity = invoiceToFileActionSetEntity.FilterCondition;
                    }
                    VRUIUtilsService.callDirectiveLoad(invoiceFilterConditionAPI, invoiceFilterConditionPayload, invoiceFilterConditionLoadPromiseDeferred);
                });
                return invoiceFilterConditionLoadPromiseDeferred.promise;
            }

            function loadInvoiceAttachmentSelectorDirective() {
                var invoiceAttachmentSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceAttachmentSelectorReadyDeferred.promise.then(function () {
                    var invoiceAttachmentSelectorPayload = { context: getContext() };
                    if (invoiceToFileActionSetEntity != undefined) {
                        invoiceAttachmentSelectorPayload.selectedIds = invoiceToFileActionSetEntity.AttachmentsIds;
                    }
                    VRUIUtilsService.callDirectiveLoad(invoiceAttachmentSelectorAPI, invoiceAttachmentSelectorPayload, invoiceAttachmentSelectorLoadPromiseDeferred);
                });
                return invoiceAttachmentSelectorLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceFilterConditionDirective, loadInvoiceAttachmentSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildInvoiceToFileActionSetObjFromScope() {
            var obj = {
                InvoiceToFileActionSetId: invoiceToFileActionSetEntity != undefined ? invoiceToFileActionSetEntity.InvoiceToFileActionSetId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                FilterCondition: invoiceFilterConditionAPI.getData(),
                AttachmentsIds: invoiceAttachmentSelectorAPI.getSelectedIds(),
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

    appControllers.controller('VR_Invoice_SaveInvoiceToFileActionSetEditorController', saveInvoiceToFileActionSetEditorController);
})(appControllers);

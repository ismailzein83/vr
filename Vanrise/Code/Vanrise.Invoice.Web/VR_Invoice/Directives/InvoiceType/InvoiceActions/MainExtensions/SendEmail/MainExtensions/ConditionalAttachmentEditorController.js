(function (appControllers) {

    'use strict';

    ConditionalAttachmentEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VR_Invoice_InvoiceFieldEnum', 'VRCommon_GridWidthFactorEnum', 'VRUIUtilsService'];

    function ConditionalAttachmentEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VR_Invoice_InvoiceFieldEnum, VRCommon_GridWidthFactorEnum, VRUIUtilsService) {

        var context;
        var conditionalAttachmentEntity;
        var invoiceFilterConditionAPI;
        var invoiceFilterConditionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var isEditMode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                conditionalAttachmentEntity = parameters.conditionalAttachmentEntity;
            }
          
            isEditMode = (conditionalAttachmentEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onInvoiceFilterConditionReady = function (api) {
                invoiceFilterConditionAPI = api;
                invoiceFilterConditionReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.invoiceAttachments = context != undefined ? context.getInvoiceAttachmentsInfo() : [];

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateConditionalAttachment() : addConditionalAttachment();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            function builConditionalAttachmentObjFromScope() {
                return {
                    Name: $scope.scopeModel.name,
                    InvoiceAttachmentId: $scope.scopeModel.selectedInvoiceAttachment.InvoiceAttachmentId,
                    Condition: invoiceFilterConditionAPI.getData(),
                };
            }

            function addConditionalAttachment() {
                var conditionalAttachmentObj = builConditionalAttachmentObjFromScope();
                if ($scope.onConditionalAttachmentAdded != undefined) {
                    $scope.onConditionalAttachmentAdded(conditionalAttachmentObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateConditionalAttachment() {
                var conditionalAttachmentObj = builConditionalAttachmentObjFromScope();
                if ($scope.onConditionalAttachmentUpdated != undefined) {
                    $scope.onConditionalAttachmentUpdated(conditionalAttachmentObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && conditionalAttachmentEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(conditionalAttachmentEntity.Name, 'Conditional Attachment');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Conditional Attachment');
                }

                function loadStaticData() {
                    if (conditionalAttachmentEntity != undefined) {
                        $scope.scopeModel.name = conditionalAttachmentEntity.Name;
                        $scope.scopeModel.selectedInvoiceAttachment = UtilsService.getItemByVal($scope.scopeModel.invoiceAttachments, conditionalAttachmentEntity.InvoiceAttachmentId, "InvoiceAttachmentId");

                    }
                }

              function loadInvoiceFilterConditionDirective() {
                var invoiceFilterConditionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceFilterConditionReadyPromiseDeferred.promise.then(function () {
                    var invoiceFilterConditionPayload = { context: getContext() };
                    if (conditionalAttachmentEntity != undefined) {
                        invoiceFilterConditionPayload.invoiceFilterConditionEntity = conditionalAttachmentEntity.Condition;
                    }
                    VRUIUtilsService.callDirectiveLoad(invoiceFilterConditionAPI, invoiceFilterConditionPayload, invoiceFilterConditionLoadPromiseDeferred);
                });
                return invoiceFilterConditionLoadPromiseDeferred.promise;
            }

              return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceFilterConditionDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }

        }
        function getContext()
        {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }
    appControllers.controller('VR_Invoice_ConditionalAttachmentEditorController', ConditionalAttachmentEditorController);

})(appControllers);
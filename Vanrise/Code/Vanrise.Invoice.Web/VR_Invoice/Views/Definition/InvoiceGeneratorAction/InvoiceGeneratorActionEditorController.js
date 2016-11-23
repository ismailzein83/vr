(function (appControllers) {

    'use strict';

    invoiceGeneratorActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function invoiceGeneratorActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var invoiceGeneratorActionEntity;

        var isEditMode;

        var invoiceActionsSelectorAPI;
        var invoiceActionsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var buttonTypesSelectorAPI;
        var buttonTypesSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                invoiceGeneratorActionEntity = parameters.invoiceGeneratorActionEntity;
            }
            isEditMode = (invoiceGeneratorActionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onInvoiceActionsSelectorReady = function (api) {
                invoiceActionsSelectorAPI = api;
                invoiceActionsSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onButtonTypesSelectorReady = function(api)
            {
                buttonTypesSelectorAPI = api;
                buttonTypesSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateInvoiceGeneratorAction() : addeInvoiceGeneratorAction();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            function builInvoiceGeneratorActionObjFromScope() {
                return {
                    Title: $scope.scopeModel.actionTitle,
                    InvoiceGeneratorActionId: invoiceActionsSelectorAPI.getSelectedIds(),
                    ButtonType: buttonTypesSelectorAPI.getSelectedIds()
                };
            }

            function addeInvoiceGeneratorAction() {
                var invoiceGeneratorActionObj = builInvoiceGeneratorActionObjFromScope();
                if ($scope.onInvoiceGeneratorActionAdded != undefined) {
                    $scope.onInvoiceGeneratorActionAdded(invoiceGeneratorActionObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateInvoiceGeneratorAction() {
                var invoiceGeneratorActionObj = builInvoiceGeneratorActionObjFromScope();
                if ($scope.onInvoiceGeneratorActionUpdated != undefined) {
                    $scope.onInvoiceGeneratorActionUpdated(invoiceGeneratorActionObj);
                }
                $scope.modalContext.closeModal();
            }

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && invoiceGeneratorActionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceGeneratorActionEntity.Title, 'Invoice Generator Action');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Invoice Generator Action');
                }

                function loadStaticData() {
                    if (invoiceGeneratorActionEntity != undefined) {
                        $scope.scopeModel.actionTitle = invoiceGeneratorActionEntity.Title;
                    }
                }

                function loadInvoiceActionsSelector() {
                    var invoiceActionsSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceActionsSelectorReadyPromiseDeferred.promise.then(function () {
                        var invoiceActionsSelectorPayload = { context: getContext() };
                        if (invoiceGeneratorActionEntity != undefined) {
                            invoiceActionsSelectorPayload.selectedIds = invoiceGeneratorActionEntity.InvoiceGeneratorActionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceActionsSelectorAPI, invoiceActionsSelectorPayload, invoiceActionsSelectorLoadPromiseDeferred);
                    });
                    return invoiceActionsSelectorLoadPromiseDeferred.promise;
                }
                function loadButtonTypesSelector() {
                    var buttonTypesSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    buttonTypesSelectorReadyPromiseDeferred.promise.then(function () {
                        var buttonTypesSelectorPayload = { context: getContext() };
                        if (invoiceGeneratorActionEntity != undefined) {
                            buttonTypesSelectorPayload.selectedIds = invoiceGeneratorActionEntity.ButtonType;
                        }
                        VRUIUtilsService.callDirectiveLoad(buttonTypesSelectorAPI, buttonTypesSelectorPayload, buttonTypesSelectorLoadPromiseDeferred);
                    });
                    return buttonTypesSelectorLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceActionsSelector, loadButtonTypesSelector]).then(function () {

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
    appControllers.controller('VR_Invoice_InvoiceGeneratorActionEditorController', invoiceGeneratorActionEditorController);

})(appControllers);
(function (appControllers) {

    'use strict';

    menualBulkActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

   function menualBulkActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var actionEntity;

        var isEditMode;

        var invoiceBulkActionsSelectorAPI;
        var invoiceBulkActionsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                actionEntity = parameters.actionEntity;
            }
            isEditMode = (actionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onInvoiceBulkActionsSelectorReady = function (api) {
                invoiceBulkActionsSelectorAPI = api;
                invoiceBulkActionsSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateInvoiceBulkAction() : addInvoiceBulkAction();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builInvoiceBulkActionObjFromScope() {
                return {
                    Title: $scope.scopeModel.actionTitle,
                    InvoiceMenualBulkActionId:actionEntity != undefined?actionEntity.InvoiceMenualBulkActionId:UtilsService.guid(),
                    InvoiceBulkActionId: invoiceBulkActionsSelectorAPI.getSelectedIds(),
                };
            }

            function addInvoiceBulkAction() {
                var bulkActionObj = builInvoiceBulkActionObjFromScope();
                if ($scope.onInvoiceBulkActionAdded != undefined) {
                    $scope.onInvoiceBulkActionAdded(bulkActionObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateInvoiceBulkAction() {
                var bulkActionObj = builInvoiceBulkActionObjFromScope();
                if ($scope.onInvoiceBulkActionUpdated != undefined) {
                    $scope.onInvoiceBulkActionUpdated(bulkActionObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && actionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(actionEntity.Title, 'Action');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Action');
                }

                function loadStaticData() {
                    if (actionEntity != undefined) {
                        $scope.scopeModel.actionTitle = actionEntity.Title;
                    }
                }

                function loadInvoiceBulkActionsSelector() {
                    var invoiceBulkActionsSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceBulkActionsSelectorReadyPromiseDeferred.promise.then(function () {
                        var invoiceBulkActionsSelectorPayload = { context: getContext() };
                        if (actionEntity != undefined) {
                            invoiceBulkActionsSelectorPayload.selectedIds = actionEntity.InvoiceBulkActionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceBulkActionsSelectorAPI, invoiceBulkActionsSelectorPayload, invoiceBulkActionsSelectorLoadPromiseDeferred);
                    });
                    return invoiceBulkActionsSelectorLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceBulkActionsSelector]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
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
    appControllers.controller('VR_Invoice_MenualBulkActionEditorController',menualBulkActionEditorController);

})(appControllers);
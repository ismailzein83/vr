(function (appControllers) {

    'use strict';

    invoiceItemActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

    function invoiceItemActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var actionEntity;

        var isEditMode;

        var invoiceFilterConditionAPI;
        var invoiceFilterConditionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceActionsSelectorAPI;
        var invoiceActionsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onInvoiceActionsSelectorReady = function(api)
            {
                invoiceActionsSelectorAPI = api;
                invoiceActionsSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onInvoiceFilterConditionReady = function (api) {
                invoiceFilterConditionAPI = api;
                invoiceFilterConditionReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateGridAction() : addGridAction();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builGridActionObjFromScope() {
                return {
                    Title: $scope.scopeModel.actionTitle,
                    ReloadGridItem: $scope.scopeModel.reloadGridItem,
                    FilterCondition: invoiceFilterConditionAPI.getData(),
                    InvoiceGridActionId: invoiceActionsSelectorAPI.getSelectedIds()
                };
            }

            function addGridAction() {
                var gridActionObj = builGridActionObjFromScope();
                if ($scope.onGridActionAdded != undefined) {
                    $scope.onGridActionAdded(gridActionObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateGridAction() {
                var gridActionObj = builGridActionObjFromScope();
                if ($scope.onGridActionUpdated != undefined) {
                    $scope.onGridActionUpdated(gridActionObj);
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
                        $scope.title = UtilsService.buildTitleForUpdateEditor(actionEntity.ActionTypeName, 'Grid Action');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grid Action');
                }

                function loadStaticData() {
                    if (actionEntity != undefined) {
                        $scope.scopeModel.actionTitle = actionEntity.Title;
                        $scope.scopeModel.reloadGridItem = actionEntity.ReloadGridItem;

                    }
                }

                function loadInvoiceFilterConditionDirective() {
                    var invoiceFilterConditionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceFilterConditionReadyPromiseDeferred.promise.then(function () {
                        var invoiceFilterConditionPayload = { context: getContext() };
                        if (actionEntity != undefined) {
                            invoiceFilterConditionPayload.invoiceFilterConditionEntity = actionEntity.FilterCondition;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceFilterConditionAPI, invoiceFilterConditionPayload, invoiceFilterConditionLoadPromiseDeferred);
                    });
                    return invoiceFilterConditionLoadPromiseDeferred.promise;
                }

                function loadInvoiceActionsSelector() {
                    var invoiceActionsSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceActionsSelectorReadyPromiseDeferred.promise.then(function () {
                        var invoiceActionsSelectorPayload = { context: getContext() };
                        if (actionEntity != undefined) {
                            invoiceActionsSelectorPayload.selectedIds = actionEntity.InvoiceGridActionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceActionsSelectorAPI, invoiceActionsSelectorPayload, invoiceActionsSelectorLoadPromiseDeferred);
                    });
                    return invoiceActionsSelectorLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceFilterConditionDirective, loadInvoiceActionsSelector]).then(function () {

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
    appControllers.controller('VR_Invoice_InvoiceItemActionEditorController', invoiceItemActionEditorController);

})(appControllers);
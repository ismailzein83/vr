(function (appControllers) {

    'use strict';

    InvoiceBulkActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function InvoiceBulkActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var invoiceBulkActionEntity;

        var isEditMode;

        var invoiceBulkActionSettingsAPI;
        var invoiceBulkActionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                invoiceBulkActionEntity = parameters.invoiceBulkActionEntity;
            }
            isEditMode = (invoiceBulkActionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onInvoiceBulkActionSettingsReady = function (api) {
                invoiceBulkActionSettingsAPI = api;
                invoiceBulkActionSettingsReadyPromiseDeferred.resolve();
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
                    InvoiceBulkActionId: invoiceBulkActionEntity != undefined ? invoiceBulkActionEntity.InvoiceBulkActionId : UtilsService.guid(),
                    Settings: invoiceBulkActionSettingsAPI.getData(),
                };
            }
            function addInvoiceBulkAction() {
                var invoiceBulkActionObj = builInvoiceBulkActionObjFromScope();
                if ($scope.onInvoiceBulkActionAdded != undefined) {
                    $scope.onInvoiceBulkActionAdded(invoiceBulkActionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateInvoiceBulkAction() {
                var invoiceBulkActionObj = builInvoiceBulkActionObjFromScope();
                if ($scope.onInvoiceBulkActionUpdated != undefined) {
                    $scope.onInvoiceBulkActionUpdated(invoiceBulkActionObj);
                }
                $scope.modalContext.closeModal();
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && invoiceBulkActionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceBulkActionEntity.Title, 'Invoice Bulk Action');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Invoice Bulk Action');
            }
            function loadStaticData() {
                if (invoiceBulkActionEntity != undefined) {
                    $scope.scopeModel.actionTitle = invoiceBulkActionEntity.Title;
                }
            }
            function loadInvoiceBulkActionSettingsDirective() {
                var invoiceBulkActionSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceBulkActionSettingsReadyPromiseDeferred.promise.then(function () {
                    var invoiceBulkActionPayload = { context: getContext() };
                    if (invoiceBulkActionEntity != undefined) {
                        invoiceBulkActionPayload.invoiceBulkActionEntity = invoiceBulkActionEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(invoiceBulkActionSettingsAPI, invoiceBulkActionPayload, invoiceBulkActionSettingsLoadPromiseDeferred);
                });
                return invoiceBulkActionSettingsLoadPromiseDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceBulkActionSettingsDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }
    appControllers.controller('VR_Invoice_InvoiceBulkActionEditorController', InvoiceBulkActionEditorController);

})(appControllers);
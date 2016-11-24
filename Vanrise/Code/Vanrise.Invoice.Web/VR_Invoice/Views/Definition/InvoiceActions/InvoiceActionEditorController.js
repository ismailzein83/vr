(function (appControllers) {

    'use strict';

    InvoiceActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function InvoiceActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var invoiceActionEntity;

        var isEditMode;

        var invoiceActionSettingsAPI;
        var invoiceActionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                invoiceActionEntity = parameters.invoiceActionEntity;
                console.log(invoiceActionEntity);
            }
            isEditMode = (invoiceActionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onInvoiceActionSettingsReady = function (api) {
                invoiceActionSettingsAPI = api;
                invoiceActionSettingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateInvoiceAction() : addInvoiceAction();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builInvoiceActionObjFromScope() {
                return {
                    Title: $scope.scopeModel.actionTitle,
                    InvoiceActionId:invoiceActionEntity != undefined?invoiceActionEntity.InvoiceActionId: UtilsService.guid(),
                    Settings: invoiceActionSettingsAPI.getData()
                };
            }
            function addInvoiceAction() {
                var invoiceActionObj = builInvoiceActionObjFromScope();
                if ($scope.onInvoiceActionAdded != undefined) {
                    $scope.onInvoiceActionAdded(invoiceActionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateInvoiceAction() {
                var invoiceActionObj = builInvoiceActionObjFromScope();
                if ($scope.onInvoiceActionUpdated != undefined) {
                    $scope.onInvoiceActionUpdated(invoiceActionObj);
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
                if (isEditMode && invoiceActionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceActionEntity.Title, 'Invoice Action');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Invoice Action');
            }
            function loadStaticData() {
                if (invoiceActionEntity != undefined) {
                    $scope.scopeModel.actionTitle = invoiceActionEntity.Title;
                }
            }
            function loadInvoiceActionSettingsDirective() {
                var invoiceActionSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceActionSettingsReadyPromiseDeferred.promise.then(function () {
                    var invoiceActionPayload = { context: getContext() };
                    if (invoiceActionEntity != undefined) {
                        invoiceActionPayload.invoiceActionEntity = invoiceActionEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(invoiceActionSettingsAPI, invoiceActionPayload, invoiceActionSettingsLoadPromiseDeferred);
                });
                return invoiceActionSettingsLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceActionSettingsDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function getContext()
        {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }
    appControllers.controller('VR_Invoice_InvoiceActionEditorController', InvoiceActionEditorController);

})(appControllers);
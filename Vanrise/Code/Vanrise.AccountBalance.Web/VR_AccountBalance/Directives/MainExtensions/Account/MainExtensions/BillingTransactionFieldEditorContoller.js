(function (appControllers) {

    'use strict';

    BillingTransactionFieldEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function BillingTransactionFieldEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var billingTransactionFieldEntity;

        var isEditMode;

        var transactionTypeSelectorAPI;
        var transactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                billingTransactionFieldEntity = parameters.billingTransactionFieldEntity;
            }
            isEditMode = (billingTransactionFieldEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onTransactionTypeSelectorReady = function (api) {
                transactionTypeSelectorAPI = api;
                transactionTypeSelectorReadyPromiseDeferred.resolve();
            };
            
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateBillingTransactionField() : addBillingTransactionField();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            function buildBillingTransactionFieldObjFromScope() {
                return {
                    FieldId:billingTransactionFieldEntity!=undefined?billingTransactionFieldEntity.FieldId:UtilsService.guid(),
                    FieldTitle: $scope.scopeModel.fieldTitle,
                    FieldName:  $scope.scopeModel.fieldName,
                    IsCredit:  $scope.scopeModel.isCredit,
                    TransactionTypeIds: transactionTypeSelectorAPI.getSelectedIds(),
                };
            }

            function addBillingTransactionField() {
                var billingTransactionFieldObj = buildBillingTransactionFieldObjFromScope();
                if ($scope.onBillingTransactionFieldAdded != undefined) {
                    $scope.onBillingTransactionFieldAdded(billingTransactionFieldObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateBillingTransactionField() {
                var billingTransactionFieldObj = buildBillingTransactionFieldObjFromScope();
                if ($scope.onBillingTransactionFieldUpdated != undefined) {
                    $scope.onBillingTransactionFieldUpdated(billingTransactionFieldObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && billingTransactionFieldEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(billingTransactionFieldEntity.FieldTitle, 'Billing Transaction Field');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Billing Transaction Field');
                }
                function loadStaticData() {
                    if (billingTransactionFieldEntity != undefined) {
                        $scope.scopeModel.fieldTitle = billingTransactionFieldEntity.FieldTitle;
                        $scope.scopeModel.fieldName = billingTransactionFieldEntity.FieldName;
                        $scope.scopeModel.isCredit = billingTransactionFieldEntity.IsCredit;
                    }
                }
                function loadTransactionTypeSelector() {
                    var transactionTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    transactionTypeSelectorReadyPromiseDeferred.promise.then(function () {
                        var transactionTypePayload = { context: getContext() };
                        if (billingTransactionFieldEntity != undefined)
                            transactionTypePayload.selectedIds = billingTransactionFieldEntity.TransactionTypeIds;
                        VRUIUtilsService.callDirectiveLoad(transactionTypeSelectorAPI, transactionTypePayload, transactionTypeSelectorLoadPromiseDeferred);
                    });
                    return transactionTypeSelectorLoadPromiseDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTransactionTypeSelector]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
        }
        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }
            return currentContext;
        }

    }
    appControllers.controller('VR_AccountBalance_BillingTransactionFieldEditorController', BillingTransactionFieldEditorController);

})(appControllers);
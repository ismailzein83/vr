(function (appControllers) {

    'use strict';

    AccountBulkActionsController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_AccountBulkActions_HandlingErrorOptionsEnum', 'BusinessProcess_BPInstanceService'];
    function AccountBulkActionsController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, Retail_BE_AccountBEAPIService, Retail_BE_AccountBulkActions_HandlingErrorOptionsEnum, BusinessProcess_BPInstanceService) {

        var viewId;
        var bulkAction;
        var bulkActionId;
        var accountBEDefinitionId;
        var bulkActionDraftFinalState;

        var accountManagementDirectiveAPI;
        var accountManagementDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                viewId = parameters.viewId;
                bulkAction = parameters.bulkAction;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.handlingOnErrorOptions = UtilsService.getArrayEnum(Retail_BE_AccountBulkActions_HandlingErrorOptionsEnum);
            $scope.scopeModel.selectedHandlingOnError = Retail_BE_AccountBulkActions_HandlingErrorOptionsEnum.Skip;


            $scope.scopeModel.onAccountManagementDirectiveReady = function (api) {
                accountManagementDirectiveAPI = api;
                accountManagementDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.execute = function () {

                var bulkActionDraftFinalStatePromise = accountManagementDirectiveAPI.finalizeBulkActionDraft();

                bulkActionDraftFinalStatePromise.then(function (bulkActionDraftFinalState) {

                   var executeAccountBulkActionProcessInput = {
                        AccountBEDefinitionId: accountBEDefinitionId,
                        AccountBulkActions: [{
                            AccountBulkActionId: bulkAction.AccountBulkActionId,
                            Settings: accountManagementDirectiveAPI.getRuntimeDirectiveData()
                        }],
                        HandlingErrorOption: $scope.scopeModel.selectedHandlingOnError.value,
                        BulkActionFinalState: bulkActionDraftFinalState
                    };
                   return Retail_BE_AccountBEAPIService.ExecuteAccountBulkActions(executeAccountBulkActionProcessInput).then(function (response) {
                        if (response.Succeed) {
                            $scope.modalContext.closeModal();
                            BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                        }
                        else {
                            VRNotificationService.showError(response.OutputMessage, $scope);
                        }
                    });
                });
        };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function loadAccountManagementDirective() {
                    var accountManagementDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    accountManagementDirectiveReadyDeferred.promise.then(function () {
                        var payload = {
                            viewId: viewId,
                            accountBEDefinitionId: accountBEDefinitionId,
                            bulkAction: bulkAction
                        };
                        VRUIUtilsService.callDirectiveLoad(accountManagementDirectiveAPI, payload, accountManagementDirectiveLoadDeferred);
                    });
                    return accountManagementDirectiveLoadDeferred.promise;
                }

                function setTitle() {
                    $scope.title = 'Bulk Action: ' + bulkAction.Title;
                }

                return UtilsService.waitMultipleAsyncOperations([loadAccountManagementDirective, setTitle]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }
    }
    appControllers.controller('Retail_BE_AccountBulkActionsEditorController', AccountBulkActionsController);

})(appControllers);
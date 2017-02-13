(function (appControllers) {

    'use strict';

    ActionRuntimeEditorController.$inject = ['$scope', 'Retail_BE_ActionDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_BPInstanceAPIService','BusinessProcess_BPInstanceService','WhS_BP_CreateProcessResultEnum','Retail_BE_AccountBEAPIService'];

    function ActionRuntimeEditorController($scope, Retail_BE_ActionDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum, Retail_BE_AccountBEAPIService) {

        var accountActionDefinition;
        var accountBEDefinitionId;
        var accountId;
        var directiveAPI;
        var directiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountId = parameters.accountId;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountActionDefinition = parameters.accountActionDefinition;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.selectedEditor = accountActionDefinition.ActionDefinitionSettings.BPDefinitionSettings.RuntimeEditor;
            $scope.scopeModel.extensionConfigs = [];


            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var directivePayload = undefined;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.start = function () {
                var inputArguments = {
                    $type: "Retail.BusinessEntity.Entities.ActionBPInputArgument, Retail.BusinessEntity.Entities",
                    ActionDefinitionId: accountActionDefinition.AccountActionDefinitionId,
                    AccountBEDefinitionId: accountBEDefinitionId,
                    ActionBPSettings: directiveAPI.getData(),
                    AccountId: accountId
                };
                var input = {
                    InputArguments: inputArguments
                };
               return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                        var context = {
                            onClose: function () {
                                if ($scope.onActionExecuted != undefined && typeof ($scope.onActionExecuted) == 'function') {
                                    Retail_BE_AccountBEAPIService.GetAccountDetail(accountBEDefinitionId, accountId).then(function (response) {
                                        return $scope.onActionExecuted(response);
                                    });
                                }
                              
                            }
                        };
                        $scope.modalContext.closeModal();
                        BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, context);
                    }
                });

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
           
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            var actionDefinitionName = (accountActionDefinition != undefined) ? accountActionDefinition.Name : undefined;
            $scope.title = actionDefinitionName;
        }


        function loadDirective() {
            if (accountActionDefinition != undefined && accountActionDefinition.ActionDefinitionSettings != undefined && accountActionDefinition.ActionDefinitionSettings.BPDefinitionSettings != undefined) {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;
                    var directivePayload = {
                        bpDefinitionSettings: accountActionDefinition.ActionDefinitionSettings.BPDefinitionSettings
                    }; 
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });
                return directiveLoadDeferred.promise;
            }

        }

    }

    appControllers.controller('Retail_BE_ActionRuntimeEditorController', ActionRuntimeEditorController);

})(appControllers);
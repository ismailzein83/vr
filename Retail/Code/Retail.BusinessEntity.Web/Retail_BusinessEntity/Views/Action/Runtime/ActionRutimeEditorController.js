(function (appControllers) {

    'use strict';

    ActionRuntimeEditorController.$inject = ['$scope', 'Retail_BE_ActionDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountAPIService'];

    function ActionRuntimeEditorController($scope, Retail_BE_ActionDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountAPIService) {

        var actionDefinitionId;
        var actionDefinitionEntity;

        var accountEntity;
        var accountId;

        var directiveAPI;
        var directiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                actionDefinitionId = parameters.actionDefinitionId;
                accountId = parameters.accountId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.extensionConfigs = [];

            $scope.scopeModel.selectedExtensionConfig;

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var directivePayload = undefined;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };


            $scope.scopeModel.save = function () {
               // return (isEditMode) ? updateActionDefinition() : insertActionDefinition();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([loadActionBPDefinitionExtensionConfigs, getActionDefinition, getAccount]).then(function () {
                loadAllControls().finally(function () {
                    actionDefinitionEntity = undefined;
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getActionDefinition() {
            return Retail_BE_ActionDefinitionAPIService.GetActionDefinition(actionDefinitionId).then(function (response) {
                actionDefinitionEntity = response;
            });
        }
        function getAccount() {
            return Retail_BE_AccountAPIService.GetAccount(accountId).then(function (response) {
                accountEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            var actionDefinitionName = (actionDefinitionEntity != undefined) ? actionDefinitionEntity.Name : undefined;
            var accountName = (accountEntity != undefined)? accountEntity.Name: undefined;
            $scope.title = actionDefinitionName + " : " + accountName;
        }

        function loadActionBPDefinitionExtensionConfigs() {
            return Retail_BE_ActionDefinitionAPIService.GetActionBPDefinitionExtensionConfigs().then(function (response) {
                if (response != undefined) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.extensionConfigs.push(response[i]);
                    }
                    if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined && actionDefinitionEntity.Settings.BPDefinitionSettings != undefined)
                        $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, actionDefinitionEntity.Settings.BPDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                }
            });
        }

        function loadDirective() {
            if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined && actionDefinitionEntity.Settings.BPDefinitionSettings != undefined) {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;
                    var directivePayload = {
                        bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings
                    };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });
                return directiveLoadDeferred.promise;
            }

        }



    }

    appControllers.controller('Retail_BE_ActionRuntimeEditorController', ActionRuntimeEditorController);

})(appControllers);
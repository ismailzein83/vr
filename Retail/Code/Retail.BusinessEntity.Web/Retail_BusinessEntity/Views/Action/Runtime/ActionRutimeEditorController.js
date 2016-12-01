(function (appControllers) {

    'use strict';

    ActionRuntimeEditorController.$inject = ['$scope', 'Retail_BE_ActionDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountAPIService','BusinessProcess_BPInstanceAPIService','BusinessProcess_BPInstanceService','WhS_BP_CreateProcessResultEnum'];

    function ActionRuntimeEditorController($scope, Retail_BE_ActionDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountAPIService, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum) {

        var actionDefinitionId;
        var actionDefinitionEntity;
        var entityId;

        var directiveAPI;
        var directiveReadyDeferred;

        var onActionExecuted;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                actionDefinitionId = parameters.actionDefinitionId;
                entityId = parameters.entityId;
                onActionExecuted = parameters.onActionExecuted;
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

            $scope.scopeModel.start = function () {
                var inputArguments = {
                    $type: "Retail.BusinessEntity.Entities.ActionBPInputArgument, Retail.BusinessEntity.Entities",
                    ActionDefinitionId: actionDefinitionEntity.ActionDefinitionId,
                    ActionEntityId: entityId,
                    ActionBPSettings: directiveAPI.getData(),
                };
                var input = {
                    InputArguments: inputArguments
                };
               return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                        var context = {
                            onClose: function () {
                                if (onActionExecuted != undefined && typeof (onActionExecuted) == 'function') {
                                    return onActionExecuted();
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

            UtilsService.waitMultipleAsyncOperations([getActionDefinition]).then(function () {
                loadAllControls().finally(function () {
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

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadDirective, loadActionBPDefinitionExtensionConfigs]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            var actionDefinitionName = (actionDefinitionEntity != undefined) ? actionDefinitionEntity.Name : undefined;
            $scope.title = actionDefinitionName;
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
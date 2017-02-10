'use strict';
app.directive('retailBeAccountactiondefinitionsBackendexecutorEditor', ['UtilsService', 'VRUIUtilsService','Retail_BE_AccountBEDefinitionAPIService', 
    function (UtilsService, VRUIUtilsService, Retail_BE_AccountBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new BackendExecutorEditorCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/Templates/BackendExecutorEditorTemplate.html';
            }

        };

        function BackendExecutorEditorCtor(ctrl, $scope) {
            var accountBEDefinitionId;

            var accountActionDefinitionEntity;

            var accountActionDefinitionAPI;
            var accountActionDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedAccountActionDefinitionSelectorReadyDeferred;


            var backendExecutorSettingEditorAPI;
            var backendExecutorSettingEditorReadyDeferred;

            var accountActionBackendExecutorEntity;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountActionDefinitionSelectorReady = function (api) {
                    accountActionDefinitionAPI = api;
                    accountActionDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountActionDefinitionSelectionChanged = function (value) {
                    if (value != undefined)
                    {
                        getAccountActionDefinition(value.AccountActionDefinitionId);
                    }
                };

                $scope.scopeModel.onBackendExecutorSettingEditorReady = function (api) {
                    backendExecutorSettingEditorAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var backendExecutorSettingEditorPayload = {
                        actionDefinitionSettings: accountActionDefinitionEntity.ActionDefinitionSettings
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, backendExecutorSettingEditorAPI, backendExecutorSettingEditorPayload, setLoader, backendExecutorSettingEditorReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;

                        accountActionBackendExecutorEntity = payload.accountActionBackendExecutorEntity;

                        if (accountActionBackendExecutorEntity != undefined) {
                            promises.push(getAccountActionDefinition(accountActionBackendExecutorEntity.ActionDefinitionId));
                            backendExecutorSettingEditorReadyDeferred = UtilsService.createPromiseDeferred();
                            function loadBackendExecutorSetting() {
                                var backendExecutorSettingEditorLoadDeferred = UtilsService.createPromiseDeferred();
                                backendExecutorSettingEditorReadyDeferred.promise.then(function () {
                                    backendExecutorSettingEditorReadyDeferred = undefined;
                                    var backendExecutorSettingEditorPayload = {
                                        accountActionBackendExecutorEntity: accountActionBackendExecutorEntity,
                                        actionDefinitionSettings: accountActionDefinitionEntity.ActionDefinitionSettings
                                    };
                                    VRUIUtilsService.callDirectiveLoad(backendExecutorSettingEditorAPI, backendExecutorSettingEditorPayload, backendExecutorSettingEditorLoadDeferred);
                                });
                                return backendExecutorSettingEditorLoadDeferred.promise;
                            }

                            promises.push(loadBackendExecutorSetting());
                          
                        }
                    }

                    function loadAccountActionDefinitionSelector() {
                        var accountActionDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        accountActionDefinitionSelectorReadyDeferred.promise.then(function () {
                            var accountActionDefinitionSelectorPayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                filter: {
                                    VisibleInBalanceAlertRule: true
                                },
                            };
                            if (accountActionBackendExecutorEntity) {
                                accountActionDefinitionSelectorPayload.selectedIds = accountActionBackendExecutorEntity.ActionDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountActionDefinitionAPI, accountActionDefinitionSelectorPayload, accountActionDefinitionSelectorLoadDeferred);
                        });
                        return accountActionDefinitionSelectorLoadDeferred.promise;
                    }
                 
                    promises.push(loadAccountActionDefinitionSelector());
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var actionBPSettings =  backendExecutorSettingEditorAPI.getData();
                    if (actionBPSettings != undefined)
                    {
                        actionBPSettings.ActionDefinitionId = accountActionDefinitionAPI.getSelectedIds();
                        actionBPSettings.ActionName = "Account Action (" +  $scope.scopeModel.selectedAccountActionDefinition.Name +" )";
                    }
                      

                    return actionBPSettings;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
         
            function getAccountActionDefinition(accountActionDefinitionId)
            {
               return Retail_BE_AccountBEDefinitionAPIService.GetAccountActionDefinition(accountBEDefinitionId, accountActionDefinitionId).then(function (response) {
                   accountActionDefinitionEntity = response;
                   $scope.scopeModel.backendExecutorSettingEditor = accountActionDefinitionEntity.ActionDefinitionSettings.BackendExecutorSettingEditor;
                });
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);
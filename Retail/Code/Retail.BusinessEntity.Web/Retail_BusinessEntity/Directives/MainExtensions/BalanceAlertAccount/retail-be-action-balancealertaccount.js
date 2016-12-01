'use strict';
app.directive('retailBeActionBalancealertaccount', ['UtilsService','VRUIUtilsService','Retail_BE_ActionDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, Retail_BE_ActionDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new BalanceAlertAccountActionCtor(ctrl, $scope);
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
                return '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/BalanceAlertAccount/Templates/BalanceAlertAccountActionTemplate.html';
            }

        };

        function BalanceAlertAccountActionCtor(ctrl, $scope) {

            var actionDefinitionAPI;
            var actionDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var actionDefinitionRuntimeAPI;
            var actionDefinitionRuntimeReadyDeferred;

            var actionDefinitionEntity;
            var vrActionEntity;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.extensionConfigs = [];

                $scope.scopeModel.onActionDefinitionSelectorReady = function (api) {
                    actionDefinitionAPI = api;
                    actionDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onActionDeinitionSelectionChanged = function () {
                    var selectedActionDefinitionId = actionDefinitionAPI.getSelectedIds();
                    if (selectedActionDefinitionId != undefined) {
                        getActionDefinitionById(selectedActionDefinitionId).then(function () {



                            if (actionDefinitionRuntimeAPI != undefined) {
                                var directivePayload = { bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings };
                                var setLoader = function (value) {
                                    $scope.scopeModel.isLoadingDirective = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, actionDefinitionRuntimeAPI, directivePayload, setLoader, actionDefinitionRuntimeReadyDeferred);
                            }


                        })
                    }

                };

                $scope.scopeModel.onActionDefinitionRuntimeReady = function (api) {
                    actionDefinitionRuntimeAPI = api;
                    var directivePayload = { bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, actionDefinitionRuntimeAPI, directivePayload, setLoader, actionDefinitionRuntimeReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        vrActionEntity = payload.vrActionEntity;
                    }
                    var promises = [];
                    var actionDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    actionDefinitionSelectorReadyDeferred.promise.then(function () {
                        var actionDefinitionSelectorPayload = {
                            showFullName: true,
                        };
                        if (vrActionEntity) {
                            actionDefinitionSelectorPayload.selectedIds = vrActionEntity.ActionDefinitionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(actionDefinitionAPI, actionDefinitionSelectorPayload, actionDefinitionSelectorLoadDeferred);
                    });
                    promises.push(actionDefinitionSelectorLoadDeferred.promise);

                    var loadActionBPDefinitionExtensionConfigsPromise = loadActionBPDefinitionExtensionConfigs();
                    promises.push(loadActionBPDefinitionExtensionConfigsPromise);

                    if (vrActionEntity) {
                        var promiseDiffered = UtilsService.createPromiseDeferred();
                        promises.push(promiseDiffered.promise);
                        loadActionBPDefinitionExtensionConfigsPromise.then(function () {
                            if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined && actionDefinitionEntity.Settings.BPDefinitionSettings != undefined) {
                                loadDirective().then(function () {
                                    promiseDiffered.resolve();
                                }).catch(function (error) {
                                    promiseDiffered.reject(error);
                                });
                            } else {
                                promiseDiffered.resolve();
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var actionBPSettings = actionDefinitionRuntimeAPI.getData();
                    if (actionBPSettings != undefined)
                        actionBPSettings.ConfigId = $scope.scopeModel.selectedExtensionConfig.ExtensionConfigurationId;

                    return {
                        $type: "Retail.BusinessEntity.Business.Extensions.BalanceAlertAccountAction, Retail.BusinessEntity.Business",
                        ActionDefinitionId: actionDefinitionAPI.getSelectedIds(),
                        ActionBPSettings: actionBPSettings,
                        ActionName: $scope.scopeModel.selectedActionDefinition.nameValue
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getActionDefinitionById(actionDefinitionId)
            {
                return Retail_BE_ActionDefinitionAPIService.GetActionDefinition(actionDefinitionId).then(function (response) {
                    actionDefinitionEntity = response;

                    if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined && actionDefinitionEntity.Settings.BPDefinitionSettings != undefined)
                        $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, actionDefinitionEntity.Settings.BPDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                });
            }

            function loadActionBPDefinitionExtensionConfigs() {
                return Retail_BE_ActionDefinitionAPIService.GetActionBPDefinitionExtensionConfigs().then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.extensionConfigs.push(response[i]);
                        }
                       
                    }
                });
            }

            function loadDirective() {
                
                var actionDefinitionRuntimeLoadDeferred = UtilsService.createPromiseDeferred();
                actionDefinitionRuntimeReadyDeferred = UtilsService.createPromiseDeferred();
                actionDefinitionRuntimeReadyDeferred.promise.then(function () {
                    actionDefinitionRuntimeReadyDeferred = undefined;
                    var directivePayload = {
                        bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings
                    };
                    VRUIUtilsService.callDirectiveLoad(actionDefinitionRuntimeAPI, directivePayload, actionDefinitionRuntimeLoadDeferred);
                });
                return actionDefinitionRuntimeLoadDeferred.promise;
            }


            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);
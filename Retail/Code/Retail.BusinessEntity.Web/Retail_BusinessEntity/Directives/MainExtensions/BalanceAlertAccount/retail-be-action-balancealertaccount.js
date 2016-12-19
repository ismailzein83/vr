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
            var selectedActionDefinitionSelectorReadyDeferred;
            var actionDefinitionRuntimeAPI;
            var actionDefinitionRuntimeReadyDeferred = UtilsService.createPromiseDeferred();

            var actionDefinitionEntity;
            var vrActionEntity;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.extensionConfigs = [];

                $scope.scopeModel.onActionDefinitionSelectorReady = function (api) {
                    actionDefinitionAPI = api;
                    actionDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onActionDefinitionSelectionChanged = function () {
                    if (selectedActionDefinitionSelectorReadyDeferred != undefined)
                    {
                        selectedActionDefinitionSelectorReadyDeferred.resolve();
                    } else
                    {
                        var selectedActionDefinitionId = actionDefinitionAPI.getSelectedIds();
                        if (selectedActionDefinitionId != undefined) {
                            getActionDefinitionById(selectedActionDefinitionId).finally(function () {
                                $scope.scopeModel.isLoadingDirective = true;
                                actionDefinitionRuntimeReadyDeferred.promise.then(function(){
                                    var directivePayload = { bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings };
                                    var setLoader = function (value) {
                                        $scope.scopeModel.isLoadingDirective = value;
                                    };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, actionDefinitionRuntimeAPI, directivePayload, setLoader);
                                });
                            });
                        }
                    }
                };
                $scope.scopeModel.onActionDefinitionRuntimeReady = function (api) {
                    actionDefinitionRuntimeAPI = api;
                    actionDefinitionRuntimeReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    if (payload != undefined) {
                        vrActionEntity = payload.vrActionEntity;
                        if(vrActionEntity != undefined)
                        {
                            selectedActionDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                        }
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




                    if (vrActionEntity != undefined) {
                        var actionDefinitionRuntimeLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(actionDefinitionRuntimeLoadDeferred.promise);

                        var promise = getActionDefinitionById(vrActionEntity.ActionDefinitionId);
                        UtilsService.waitMultiplePromises([loadActionBPDefinitionExtensionConfigsPromise, promise]).then(function () {
                            UtilsService.waitMultiplePromises([selectedActionDefinitionSelectorReadyDeferred.promise, actionDefinitionRuntimeReadyDeferred.promise]).then(function () {
                                selectedActionDefinitionSelectorReadyDeferred = undefined;
                                var selectedActionDefinitionId = actionDefinitionAPI.getSelectedIds();
                                getActionDefinitionById(selectedActionDefinitionId).then(function () {
                                    var directivePayload = {
                                        bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings,
                                        vrActionEntity: vrActionEntity
                                    };
                                    VRUIUtilsService.callDirectiveLoad(actionDefinitionRuntimeAPI, directivePayload, actionDefinitionRuntimeLoadDeferred);
                                });
                            });
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

 


            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);
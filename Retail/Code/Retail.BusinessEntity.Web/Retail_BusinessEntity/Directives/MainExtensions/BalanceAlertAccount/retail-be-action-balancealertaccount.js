'use strict';
app.directive('retailBeActionBalancealertaccount', ['UtilsService','VRUIUtilsService','Retail_BE_VRAccountBalanceAPIService',
    function (UtilsService, VRUIUtilsService, Retail_BE_VRAccountBalanceAPIService) {

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

            var actionBackendExecutorEditorAPI;
            var actionBackendExecutorEditorReadyDeferred = UtilsService.createPromiseDeferred();
           
            var actionDefinitionEntity;
            var context;
            var accountBEDefinitionId;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onActionBackendExecutorEditorReady = function (api) {
                    actionBackendExecutorEditorAPI = api;
                    actionBackendExecutorEditorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var vrActionEntity;
                    var promises = [];
                    if (payload != undefined) {
                        vrActionEntity = payload.vrActionEntity;
                        if (payload.context != undefined)
                        {
                            context = payload.context;
                            var actionBackendExecutorEditorLoadDeferred = UtilsService.createPromiseDeferred();
                            promises.push(actionBackendExecutorEditorLoadDeferred.promise);
                            var alertRuleTypeSettings = context.getAlertRuleTypeSettings();
                            var promise = Retail_BE_VRAccountBalanceAPIService.GetAccountBEDefinitionIdByAccountTypeId(alertRuleTypeSettings.AccountTypeId).then(function (repsponse) {
                                accountBEDefinitionId = repsponse;
                                actionBackendExecutorEditorReadyDeferred.promise.then(function () {
                                    var actionDefinitionSelectorPayload = {
                                        accountBEDefinitionId: accountBEDefinitionId,
                                    };

                                    if (vrActionEntity != undefined) {
                                        actionDefinitionSelectorPayload.accountActionBackendExecutorEntity = vrActionEntity.ActionExecutor;
                                    }
                                    VRUIUtilsService.callDirectiveLoad(actionBackendExecutorEditorAPI, actionDefinitionSelectorPayload, actionBackendExecutorEditorLoadDeferred);
                                });
                            });
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var actionExecutor = actionBackendExecutorEditorAPI.getData();
                    return {
                        $type: "Retail.BusinessEntity.Business.Extensions.BalanceAlertAccountAction, Retail.BusinessEntity.Business",
                        ActionExecutor: actionExecutor,
                        ActionName: actionExecutor.ActionName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);
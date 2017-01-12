(function (app) {

    'use strict';

    SingleRuleDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function SingleRuleDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SingleRuleCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRuleSet/MainExtensions/Templates/SingleRuleSetSettingsTemplate.html'
        };

        function SingleRuleCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            var recurringChargeDefinitionSelectorAPI;
            var recurringChargeDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountChargeEvaluatorSelectiveAPI;
            var accountChargeEvaluatorSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var accountConditionSelectiveAPI;
            var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecurringChargeDefinitionSelectorReady = function (api) {
                    recurringChargeDefinitionSelectorAPI = api;
                    recurringChargeDefinitionSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onAccountChargeEvaluatorSelectiveReady = function (api) {
                    accountChargeEvaluatorSelectiveAPI = api;
                    accountChargeEvaluatorSelectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onAccountConditionSelectiveReady = function (api) {
                    accountConditionSelectiveAPI = api;
                    accountConditionSelectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountRecurringChargeRuleSetSettings;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountRecurringChargeRuleSetSettings = payload.accountRecurringChargeRuleSetSettings;
                    }

                    //Loading RecurringChargeDefinition Selector
                    var recurringChargeDefinitionSelectorLoadPromise = getRecurringChargeDefinitionSelectorLoadPromise();
                    promises.push(recurringChargeDefinitionSelectorLoadPromise);

                    //Loading AccountChargeEvaluator Selective
                    var accountChargeEvaluatorSelectiveLoadPromise = getAccountChargeEvaluatorSelectiveLoadPromise();
                    promises.push(accountChargeEvaluatorSelectiveLoadPromise);

                    //Loading AccountCondition Selective
                    var accountConditionSelectiveLoadPromise = getAccountConditionSelectiveLoadPromise();
                    promises.push(accountConditionSelectiveLoadPromise);


                    function getRecurringChargeDefinitionSelectorLoadPromise() {
                        var recurringChargeDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        recurringChargeDefinitionSelectorReadyDeferred.promise.then(function () {

                            var recurringChargeDefinitionSelectorPayload;
                            if (accountRecurringChargeRuleSetSettings != undefined) {
                                recurringChargeDefinitionSelectorPayload = {
                                    selectedIds: accountRecurringChargeRuleSetSettings.RecurringChargeDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(recurringChargeDefinitionSelectorAPI, recurringChargeDefinitionSelectorPayload, recurringChargeDefinitionSelectorLoadDeferred);
                        });

                        return recurringChargeDefinitionSelectorLoadDeferred.promise;
                    }
                    function getAccountChargeEvaluatorSelectiveLoadPromise() {
                        var accountChargeEvaluatorSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        accountChargeEvaluatorSelectiveReadyDeferred.promise.then(function () {

                            var accountChargeEvaluatorSelectivePayload;
                            if (accountRecurringChargeRuleSetSettings != undefined) {
                                accountChargeEvaluatorSelectivePayload = {
                                    chargeEvaluator: accountRecurringChargeRuleSetSettings.ChargeEvaluator
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(accountChargeEvaluatorSelectiveAPI, accountChargeEvaluatorSelectivePayload, accountChargeEvaluatorSelectiveLoadDeferred);
                        });

                        return accountChargeEvaluatorSelectiveLoadDeferred.promise;
                    }
                    function getAccountConditionSelectiveLoadPromise() {
                        var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        accountConditionSelectiveReadyDeferred.promise.then(function () {

                            var accountConditionSelectivePayload = {
                                accountBEDefinitionId: accountBEDefinitionId
                            };
                            if (accountRecurringChargeRuleSetSettings != undefined) {
                                accountConditionSelectivePayload.beFilter = accountRecurringChargeRuleSetSettings.Condition;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
                        });

                        return accountConditionSelectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountRecurringChargeRuleSets.SingleRuleAccountRecurringChargeRuleSetSettings, Retail.BusinessEntity.MainExtensions",
                        RecurringChargeDefinitionId: recurringChargeDefinitionSelectorAPI.getSelectedIds(),
                        ChargeEvaluator: accountChargeEvaluatorSelectiveAPI.getData(),
                        Condition: accountConditionSelectiveAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRecurringchargerulesetsettingsSinglerule', SingleRuleDirective);

})(app);
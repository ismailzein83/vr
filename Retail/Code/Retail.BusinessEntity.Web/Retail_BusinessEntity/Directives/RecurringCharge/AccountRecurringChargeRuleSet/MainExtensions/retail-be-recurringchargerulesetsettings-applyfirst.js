(function (app) {

    'use strict';

    ApplyFirstDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function ApplyFirstDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ApplyFirstCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRuleSet/MainExtensions/Templates/ApplyFirstRuleSetSettingsTemplate.html'
        };

        function ApplyFirstCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var recurringChargeRulesDirectiveAPI;
            var recurringChargeRulesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecurringChargeRulesDirectiveReady = function (api) {
                    recurringChargeRulesDirectiveAPI = api;
                    recurringChargeRulesDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountRecurringChargeRuleSetSettings;

                    if (payload != undefined) {
                        accountRecurringChargeRuleSetSettings = payload.accountRecurringChargeRuleSetSettings;
                    }

                    //Loading RecurringChargeRules Directive
                    var recurringChargeRulesDirectiveLoadPromise = getRecurringChargeRulesDirectiveLoadPromise();
                    promises.push(recurringChargeRulesDirectiveLoadPromise);

                    function getRecurringChargeRulesDirectiveLoadPromise() {
                        var recurringChargeRulesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        recurringChargeRulesDirectiveReadyDeferred.promise.then(function () {

                            var recurringChargeRulesDirectivePayload;
                            if (accountRecurringChargeRuleSetSettings != undefined) {
                                recurringChargeRulesDirectivePayload = {
                                    chargeRules: accountRecurringChargeRuleSetSettings.ChargeRules
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(recurringChargeRulesDirectiveAPI, recurringChargeRulesDirectivePayload, recurringChargeRulesDirectiveLoadDeferred);
                        });

                        return recurringChargeRulesDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountRecurringChargeRuleSets.ApplyFirstAccountRecurringChargeRuleSetSettings, Retail.BusinessEntity.MainExtensions",
                        ChargeRules: recurringChargeRulesDirectiveAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRecurringchargerulesetsettingsApplyfirst', ApplyFirstDirective);

})(app);
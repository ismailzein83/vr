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

            var recurringChargeDefinitionSelectorAPI;
            var recurringChargeDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecurringChargeDefinitionSelectorReady = function (api) {
                    recurringChargeDefinitionSelectorAPI = api;
                    recurringChargeDefinitionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    console.log(payload);

                    var accountRecurringChargeRuleSetSettings;

                    if (payload != undefined) {
                        accountRecurringChargeRuleSetSettings = payload.accountRecurringChargeRuleSetSettings;
                    }

                    //Loading RecurringChargeDefinition Selector
                    var recurringChargeDefinitionSelectorLoadPromise = getRecurringChargeDefinitionSelectorLoadPromise();
                    promises.push(recurringChargeDefinitionSelectorLoadPromise);


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

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountRecurringChargeRuleSets.SingleRuleAccountRecurringChargeRuleSetSettings, Retail.BusinessEntity.MainExtensions",
                        RecurringChargeDefinitionId: recurringChargeDefinitionSelectorAPI.getSelectedIds()
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
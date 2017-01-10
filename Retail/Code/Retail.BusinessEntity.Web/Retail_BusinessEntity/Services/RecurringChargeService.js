(function (appControllers) {

    'use stict';

    RecurringChargeService.$inject = ['VRModalService'];

    function RecurringChargeService(VRModalService) {

        function addAccountRecurringChargeRuleSet(recurringChargeRuleSetNames, onRecurringChargeRuleSetAdded) {

            var parameters = {
                recurringChargeRuleSetNames: recurringChargeRuleSetNames
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeRuleSetAdded = onRecurringChargeRuleSetAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRuleSet/Templates/RecurringChargeRuleSetEditor.html', parameters, settings);
        };
        function editAccountRecurringChargeRuleSet(recurringChargeRuleSet, recurringChargeRuleSetNames, onRecurringChargeRuleSetUpdated) {

            var parameters = {
                recurringChargeRuleSet: recurringChargeRuleSet,
                recurringChargeRuleSetNames: recurringChargeRuleSetNames
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeRuleSetUpdated = onRecurringChargeRuleSetUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRuleSet/Templates/RecurringChargeRuleSetEditor.html', parameters, settings);
        }

        function addAccountRecurringChargeRule(recurringChargeRuleNames, onRecurringChargeRuleAdded) {

            var parameters = {
                recurringChargeRuleNames: recurringChargeRuleNames
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeRuleAdded = onRecurringChargeRuleAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRule/Templates/RecurringChargeRuleEditor.html', parameters, settings);
        };
        function editAccountRecurringChargeRule(recurringChargeRule, recurringChargeRuleNames, onRecurringChargeRuleUpdated) {

            var parameters = {
                recurringChargeRule: recurringChargeRule,
                recurringChargeRuleNames: recurringChargeRuleNames
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeRuleUpdated = onRecurringChargeRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRule/Templates/RecurringChargeRuleEditor.html', parameters, settings);
        }


        return {
            addAccountRecurringChargeRuleSet: addAccountRecurringChargeRuleSet,
            editAccountRecurringChargeRuleSet: editAccountRecurringChargeRuleSet,
            addAccountRecurringChargeRule: addAccountRecurringChargeRule,
            editAccountRecurringChargeRule: editAccountRecurringChargeRule
        };
    }

    appControllers.service('Retail_BE_RecurringChargeService', RecurringChargeService);

})(appControllers);
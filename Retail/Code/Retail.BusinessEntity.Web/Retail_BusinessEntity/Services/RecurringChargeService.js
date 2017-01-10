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


        return {
            addAccountRecurringChargeRuleSet: addAccountRecurringChargeRuleSet,
            editAccountRecurringChargeRuleSet: editAccountRecurringChargeRuleSet
        };
    }

    appControllers.service('Retail_BE_RecurringChargeService', RecurringChargeService);

})(appControllers);
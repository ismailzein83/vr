(function (appControllers) {

    'use stict';

    RecurringChargeService.$inject = ['VRModalService'];

    function RecurringChargeService(VRModalService) {

        function addAccountRecurringChargeRuleSet(onRecurringChargeRuleSetAdded) {

            var parameters = {
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeRuleSetAdded = onRecurringChargeRuleSetAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRuleSet/Templates/RecurringChargeRuleSetEditor.html', parameters, settings);
        };
        function editAccountRecurringChargeRuleSet(recurringChargeRuleSet, onRecurringChargeRuleSetUpdated) {

            var parameters = {
                recurringChargeRuleSet: recurringChargeRuleSet
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
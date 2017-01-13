(function (appControllers) {

    'use stict';

    recurringChargeDefinitionService.$inject = ['VRModalService'];

    function recurringChargeDefinitionService(VRModalService) {

        function addRecurringChargeDefinition(onRecurringChargeDefinitionAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeDefinitionAdded = onRecurringChargeDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/RecurringChargeDefinition/RecurringChargeDefinitionEditor.html', null, settings);
        };
        function editRecurringChargeDefinition(recurringChargeDefinitionId, onRecurringChargeDefinitionUpdated) {

            var parameters = {
                recurringChargeDefinitionId: recurringChargeDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRecurringChargeDefinitionUpdated = onRecurringChargeDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/RecurringChargeDefinition/RecurringChargeDefinitionEditor.html', parameters, settings);
        }
        return {
            addRecurringChargeDefinition: addRecurringChargeDefinition,
            editRecurringChargeDefinition: editRecurringChargeDefinition
        };
    }

    appControllers.service('Retail_BE_RecurringChargeDefinitionService', recurringChargeDefinitionService);

})(appControllers);
(function (appControllers) {

    'use strict';

    AccountConditionService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function AccountConditionService(VRModalService, VRNotificationService, UtilsService, VRUIUtilsService) {

        function addAccountCondition(accountBEDefinitionId, onAccountConditionItemAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountConditionItemAdded = onAccountConditionItemAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountCondition/MainExtensions/Templates/AccountConditionItemEditor.html', parameters, settings);
        };

        function editAccountCondition(accountConditionItemEntity, accountBEDefinitionId, onAccountConditionItemUpdated) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountConditionItemEntity: accountConditionItemEntity,
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountConditionItemUpdated = onAccountConditionItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountCondition/MainExtensions/Templates/AccountConditionItemEditor.html', parameters, settings);
        };
    
        return {
            addAccountCondition: addAccountCondition,
            editAccountCondition: editAccountCondition,
          
        };

    }

    appControllers.service('Retail_BE_AccountConditionService', AccountConditionService);

})(appControllers);
﻿(function (appControllers) {

    'use stict';

    AccountBEDefinitionService.$inject = ['VRModalService'];

    function AccountBEDefinitionService(VRModalService) {

        function addGridColumnDefinition(accountBEDefinitionId, onColumnDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionAdded = onColumnDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/GridColumnDefinitionEditor.html', parameters, modalSettings);
        };
        function editGridColumnDefinition(columnDefinition, accountBEDefinitionId, onColumnDefinitionUpdated) {

            var parameters = {
                columnDefinition: columnDefinition,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionUpdated = onColumnDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/GridColumnDefinitionEditor.html', parameters, modalSettings);
        }

        function addAccountViewDefinition(accountBEDefinitionId, onAccountViewDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountViewDefinitionAdded = onAccountViewDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/Templates/AccountViewDefinitionEditor.html', parameters, modalSettings);

        }
        function editAccountViewDefinition(accountViewDefinitionEntity, accountBEDefinitionId, onAccountViewDefinitionUpdated) {

            var parameters = {
                accountViewDefinitionEntity: accountViewDefinitionEntity,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountViewDefinitionUpdated = onAccountViewDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/Templates/AccountViewDefinitionEditor.html', parameters, modalSettings);
        }

        function addAccountActionDefinition(accountBEDefinitionId, onAccountActionDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountActionDefinitionAdded = onAccountActionDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/Templates/AccountActionDefinitionEditor.html', parameters, modalSettings);

        }
        function editAccountActionDefinition(accountActionDefinitionEntity, accountBEDefinitionId, onAccountActionDefinitionUpdated) {

            var parameters = {
                accountActionDefinitionEntity: accountActionDefinitionEntity,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountActionDefinitionUpdated = onAccountActionDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/Templates/AccountActionDefinitionEditor.html', parameters, modalSettings);
        }

        return {
            addGridColumnDefinition: addGridColumnDefinition,
            editGridColumnDefinition: editGridColumnDefinition,
            addAccountViewDefinition: addAccountViewDefinition,
            editAccountViewDefinition: editAccountViewDefinition,
            addAccountActionDefinition: addAccountActionDefinition,
            editAccountActionDefinition: editAccountActionDefinition
        };
    }

    appControllers.service('Retail_BE_AccountBEDefinitionService', AccountBEDefinitionService);

})(appControllers);
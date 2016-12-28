﻿(function (appControllers) {

    'use stict';

    AccountService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function AccountService(VRModalService, VRNotificationService, UtilsService, VRUIUtilsService) {

        function addAccount(parentAccountId, onAccountAdded) {
            var parameters = {
                parentAccountId: parentAccountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountAdded = onAccountAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        };

        function editAccount(accountId, parentAccountId, onAccountUpdated) {
            var parameters = {
                accountId: accountId,
                parentAccountId: parentAccountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountUpdated = onAccountUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        };

        function defineAccountViewTabs(accountBEDefinitionId, account, gridAPI, accountViewDefinitions) {
            if (account == undefined || account.AvailableAccountViews == undefined || account.AvailableAccountViews.length == 0)
                return;

            var drillDownTabs = [];

            for (var index = 0; index < account.AvailableAccountViews.length; index++) {

                var currentAccountViewDefinitionId = account.AvailableAccountViews[index];
                var accountViewDefinition = UtilsService.getItemByVal(accountViewDefinitions, currentAccountViewDefinitionId, "AccountViewDefinitionId");

                addDrillDownTab(accountViewDefinition);
            }

            setDrillDownTabs();

            function addDrillDownTab(accountViewDefinition) {
                if (accountViewDefinition == undefined || accountViewDefinition.DrillDownSectionName == undefined ||
                    accountViewDefinition.Settings == undefined || accountViewDefinition.Settings.RuntimeEditor == undefined)
                    return;

                var drillDownTab = {};

                drillDownTab.title = accountViewDefinition.DrillDownSectionName;
                drillDownTab.directive = accountViewDefinition.Settings.RuntimeEditor;

                drillDownTab.loadDirective = function (accountViewGridAPI, account) {
                    account.accountViewGridAPI = accountViewGridAPI;

                    return account.accountViewGridAPI.load(buildAccountViewQuery());
                };

                function buildAccountViewQuery() {

                    var payload = {
                        accountBEDefinitionId: accountBEDefinitionId,
                        parentAccountId: account.Entity.AccountId
                    };
                    return payload;
                }

                drillDownTabs.push(drillDownTab);
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(account);
            }
        }

        function openAccount360DegreeEditor(accountBEDefinitionId, accountId) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/Account360DegreeEditor.html', parameters, settings);
        };


        return {
            addAccount: addAccount,
            editAccount: editAccount,
            defineAccountViewTabs: defineAccountViewTabs,
            openAccount360DegreeEditor: openAccount360DegreeEditor
        };
    }

    appControllers.service('Retail_BE_AccountService', AccountService);

})(appControllers);
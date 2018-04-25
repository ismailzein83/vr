(function (appControllers) {

    'use strict';

    AccountBEService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function AccountBEService(VRModalService, VRNotificationService, UtilsService, VRUIUtilsService) {

        function addAccount(accountBEDefinitionId, parentAccountId, onAccountAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountAdded = onAccountAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        }

        function editAccount(accountBEDefinitionId, accountId, parentAccountId, sourceId, onAccountUpdated) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                parentAccountId: parentAccountId,
                sourceId: sourceId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountUpdated = onAccountUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        }

        function getEntityUniqueName(definitionId) {
            return "Retail_BusinessEntity_AccountBE_" + definitionId;
        }

        function defineAccountViewTabs(accountBEDefinitionId, account, gridAPI, accountViewDefinitions) {
            if (accountBEDefinitionId == undefined || account == undefined || account.AvailableAccountViews == undefined || account.AvailableAccountViews.length == 0)
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

                    return account.accountViewGridAPI.load(buildAccountViewPayload());
                };

                function buildAccountViewPayload() {

                    var payload = {
                        accountViewDefinition: accountViewDefinition,
                        accountBEDefinitionId: accountBEDefinitionId,
                        parentAccountId: account.AccountId
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
        }

        function openBulkActionsEditor(viewId, bulkAction, accountBEDefinitionId) {
            var parameters = {
                viewId: viewId,
                bulkAction: bulkAction,
                accountBEDefinitionId: accountBEDefinitionId,
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {

            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/BulkActionsEditor.html', parameters, settings);
        }

        return {
            addAccount: addAccount,
            editAccount: editAccount,
            getEntityUniqueName: getEntityUniqueName,
            defineAccountViewTabs: defineAccountViewTabs,
            openAccount360DegreeEditor: openAccount360DegreeEditor,
            openBulkActionsEditor: openBulkActionsEditor
        };
         
    }

    appControllers.service('Retail_BE_AccountBEService', AccountBEService);

})(appControllers);
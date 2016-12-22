(function (appControllers) {

    'use stict';

    AccountService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountService(VRModalService, VRNotificationService)
    {
        function addAccount(parentAccountId, onAccountAdded)
        {
            var parameters = {
                parentAccountId: parentAccountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountAdded = onAccountAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        };

        function editAccount(accountId, parentAccountId, onAccountUpdated)
        {
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

        function defineAccountItemTabsAndMenuActions(account, gridAPI) {
            if (account.Entity.Settings == null || account.Entity.Settings.ItemsConfig == null)
                return;

            var drillDownTabs = [];
            //var menuActions = [];

            for (var i = 0; i < account.Entity.Settings.ItemsConfig.length; i++) {
                var dataAnalysisItemDefinitionConfig = account.Entity.Settings.ItemsConfig[i];

                addDrillDownTab(dataAnalysisItemDefinitionConfig);
                //addMenuAction(dataAnalysisItemDefinitionConfig, i);
            }

            setDrillDownTabs();
            //setMenuActions();


            function addDrillDownTab(dataAnalysisItemDefinitionConfig) {
                var drillDownTab = {};

                drillDownTab.title = dataAnalysisItemDefinitionConfig.Title;
                drillDownTab.directive = dataAnalysisItemDefinitionConfig.GridDirective;

                drillDownTab.loadDirective = function (dataAnalysisItemDefinitionGridAPI, account) {
                    account.dataAnalysisItemDefinitionGridAPI = dataAnalysisItemDefinitionGridAPI;

                    return account.dataAnalysisItemDefinitionGridAPI.load(buildDataAnalysisItemDefinitionQuery());
                };

                function buildDataAnalysisItemDefinitionQuery() {

                    var dataAnalysisItemDefinitionQuery = {};

                    dataAnalysisItemDefinitionQuery.DataAnalysisDefinitionId = account.Entity.DataAnalysisDefinitionId;
                    dataAnalysisItemDefinitionQuery.ItemDefinitionTypeId = dataAnalysisItemDefinitionConfig.TypeId;

                    return dataAnalysisItemDefinitionQuery;
                }

                drillDownTabs.push(drillDownTab);
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(account);
            }

            function addMenuAction(dataAnalysisItemDefinitionConfig, dataAnalysisItemDefinitionConfigIndex) {
                var menuAction = {};

                menuAction.name = 'New ' + dataAnalysisItemDefinitionConfig.Title;
                menuAction.clicked = function (account) {

                    account.drillDownExtensionObject.drillDownDirectiveTabs[dataAnalysisItemDefinitionConfigIndex].setTabSelected(account);

                    var itemDefinitionTypeId = dataAnalysisItemDefinitionConfig.TypeId;

                    var onDataAnalysisItemDefinitionAdded = function (addedDataAnalysisItemDefinition) {
                        account.dataAnalysisItemDefinitionGridAPI.onItemAdded(addedDataAnalysisItemDefinition);
                    };

                    VR_Analytic_DataAnalysisItemDefinitionService.addDataAnalysisItemDefinition(account.Entity.DataAnalysisDefinitionId,
                                                                                                itemDefinitionTypeId,
                                                                                                onDataAnalysisItemDefinitionAdded);
                };

                menuActions.push(menuAction);
            }
            function setMenuActions() {
                account.menuActions = [];
                for (var i = 0; i < menuActions.length; i++)
                    account.menuActions.push(menuActions[i]);
            }
        }

        return {
            addAccount: addAccount,
            editAccount: editAccount,
            defineAccountItemTabsAndMenuActions: defineAccountItemTabsAndMenuActions
        };
    }

    appControllers.service('Retail_BE_AccountService', AccountService);

})(appControllers);
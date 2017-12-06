(function (appControllers) {

    'use strict';

    AccountManagerService.$inject = ['VRModalService', 'VRUIUtilsService'];

    function AccountManagerService(VRModalService, VRUIUtilsService)
    {

        return ({
            addAssignmentDefinition: addAssignmentDefinition,
            editAssignmentDefinition: editAssignmentDefinition,
            addAccountManager: addAccountManager,
            editAccountmanager: editAccountmanager,
            addSubView: addSubView,
            editSubView: editSubView,
            defineAccountManagerSubViewTabs: defineAccountManagerSubViewTabs,
            getEntityUniqueName: getEntityUniqueName
        });
        function addAssignmentDefinition(onAssignmentDefinitionAdded)
       {
            var settings = {
            };

            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignmentDefinitionAdded = onAssignmentDefinitionAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AssignmentDefinitionEditor.html', parameters, settings);
        }
        function addSubView(onSubViewAdded,context) {
            var settings = {
            };

            var parameters = {
                context : context
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubViewAdded = onSubViewAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerSubViewDefinitionEditor.html', parameters, settings);
        }
        function editSubView(subViewEntity, onSubViewUpdated,subViews, context) {
            var parameters = {
                subViewEntity: subViewEntity,
                context:context
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSubViewUpdated = onSubViewUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerSubViewDefinitionEditor.html', parameters, settings);
        }
        function addAccountManager(onAccountManagerAdded, accountDefinitionId) {
            var settings = {
            };

            var parameters = {
                accountManagerDefinitionId: accountDefinitionId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAdded = onAccountManagerAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerEditor.html', parameters, settings);
        }
        function editAccountmanager(accountManagerObject, onAccountManagerUpdated) {
            var settings = {
            };
            var parameters = {
                accountManagerDefinitionId: accountManagerObject.AccountManagerDefinitionId,
                accountManagerId:accountManagerObject.AccountManagerId

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerUpdated = onAccountManagerUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerEditor.html', parameters, settings);
        }
        function editAssignmentDefinition(assignmentDefinitionEntity, onAssignmentDefinitionUpdated) {
            var parameters = {
                assignmentDefinitionEntity: assignmentDefinitionEntity
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignmentDefinitionUpdated = onAssignmentDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AssignmentDefinitionEditor.html', parameters, settings);
        }

        function defineAccountManagerSubViewTabs(accountManagerDefinitionId, accountManager, gridAPI, accountManagerSubViewsDefinitions) {

            var drillDownTabs = [];

            for (var index = 0; index < accountManagerSubViewsDefinitions.length; index++) {
                var accountManagerSubViewDefinition = accountManagerSubViewsDefinitions[index];
                addDrillDownTab(accountManagerSubViewDefinition);
            }

            setDrillDownTabs();

            function addDrillDownTab(accountManagerSubViewDefinition) {
                var drillDownTab = {};
                if (accountManagerSubViewDefinition != undefined && accountManagerSubViewDefinition.Settings != undefined) {
                    drillDownTab.title = accountManagerSubViewDefinition.Name;
                    drillDownTab.directive = accountManagerSubViewDefinition.Settings.RuntimeEditor;
                    drillDownTab.loadDirective = function (accountManagerSubViewGridAPI, accountManager) {
                        accountManager.accountManagerSubViewGridAPI = accountManagerSubViewGridAPI;

                        return accountManager.accountManagerSubViewGridAPI.load(buildAccountViewPayload());
                    };
                    function buildAccountViewPayload() {

                        var payload = {
                            accountManagerSubViewDefinition: accountManagerSubViewDefinition,
                            accountManagerDefinitionId: accountManagerDefinitionId,
                            accountManagerId: accountManager.AccountManagerId
                        };
                        return payload;
                    }

                    drillDownTabs.push(drillDownTab);
                }
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(accountManager);
            }
           
        }
        function getEntityUniqueName(accountManagerDefinitionId) {
            return "VR_AccountManager_AccountManager_" + accountManagerDefinitionId;
        }


    }

    appControllers.service('VR_AccountManager_AccountManagerService', AccountManagerService);

})(appControllers);

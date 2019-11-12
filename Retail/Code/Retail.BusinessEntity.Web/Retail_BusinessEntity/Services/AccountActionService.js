'use strict';

app.service('Retail_BE_AccountActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'Retail_BE_AccountBEService', 'Retail_BE_ActionRuntimeService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_ChangeStatusActionAPIService', 'Retail_BE_AccountPackageAPIService', 'Retail_BE_AccountManagerAssignmentService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, Retail_BE_AccountBEService, Retail_BE_ActionRuntimeService, Retail_BE_AccountBEAPIService, Retail_BE_ChangeStatusActionAPIService, Retail_BE_AccountPackageAPIService, Retail_BE_AccountManagerAssignmentService) {

        var actionTypes = [];

        function defineAccountMenuActions(accountBEDefinitionId, account, gridAPI, accountViewDefinitions, accountActionDefinitions, accountActionGroups) {

            account.menuActions = [];
            if (account.AvailableAccountActions != undefined) {
                var groupedActionsByGroupId = getGroupedActions(account.AvailableAccountActions);
                for (var i = 0; i < account.AvailableAccountActions.length; i++) {
                    var availableAccountActionId = account.AvailableAccountActions[i];
                    var accountActionDefinition = UtilsService.getItemByVal(accountActionDefinitions, availableAccountActionId, "AccountActionDefinitionId");
                    if (accountActionDefinition != undefined) {
                        var groupedActions = groupedActionsByGroupId[accountActionDefinition.AccountActionGroupId];
                        if (groupedActions != undefined) {
                            if (!groupedActions.isUsed) {
                                groupedActions.isUsed = true;
                                var groupGridAction = UtilsService.getItemByVal(accountActionGroups, accountActionDefinition.AccountActionGroupId, "AccountActionGroupId");
                                account.menuActions.push({
                                    name: groupGridAction.Title,
                                    childsactions: groupedActions.childActions
                                });
                            }

                        } else {
                            var actionType = getActionTypeIfExist(accountActionDefinition.ActionDefinitionSettings.ClientActionName);
                            if (actionType != undefined) {
                                account.menuActions.push(addGridMenuAction(accountActionDefinition, actionType));
                            }
                        }
                    }
                }
            }

            function getGroupedActions(availableAccountActionIds) {
                var groupActions = {};
                if (availableAccountActionIds != undefined) {
                    for (var j = 0; j < availableAccountActionIds.length; j++) {
                        var actionId = availableAccountActionIds[j];
                        var accountActionDefinition = UtilsService.getItemByVal(accountActionDefinitions, actionId, "AccountActionDefinitionId");
                        if (accountActionDefinition != undefined && accountActionDefinition.AccountActionGroupId != undefined) {
                            var actionType = getActionTypeIfExist(accountActionDefinition.ActionDefinitionSettings.ClientActionName);
                            if (actionType != undefined) {
                                if (groupActions[accountActionDefinition.AccountActionGroupId] == undefined)
                                    groupActions[accountActionDefinition.AccountActionGroupId] = {
                                        isUsed: false,
                                        childActions: []
                                    };
                                groupActions[accountActionDefinition.AccountActionGroupId].childActions.push(addGridMenuAction(accountActionDefinition, actionType));
                            }
                        }
                    }
                }
                return groupActions;
            }

            function addGridMenuAction(accountActionDefinition, actionType) {
                return {
                    name: accountActionDefinition.Name,
                    clicked: function (selectedAccount) {
                        Retail_BE_AccountBEAPIService.GetAccount(accountBEDefinitionId, account.AccountId).then(function (response) {
                            var payload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                account: response,
                                accountActionDefinition: accountActionDefinition,
                                onItemUpdated: function (updatedItem) {
                                    Retail_BE_AccountBEService.defineAccountViewTabs(accountBEDefinitionId, updatedItem, gridAPI, accountViewDefinitions);
                                    defineAccountMenuActions(accountBEDefinitionId, updatedItem, gridAPI, accountViewDefinitions, accountActionDefinitions, accountActionGroups);
                                    gridAPI.itemUpdated(updatedItem);
                                },
                                showGridLoader: function (value) {
                                    if (value) {
                                        gridAPI.showLoader();
                                    } else {
                                        gridAPI.hideLoader();
                                    }
                                }
                            };
                            var promise = actionType.ExecuteAction(payload);
                        });
                    }
                };
            }

        }

        function getActionTypeIfExist(actionTypeName) {
            for (var i = 0; i < actionTypes.length; i++) {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }
        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }

        function registerEditAccount() {

            var actionType = {
                ActionTypeName: "Edit",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var account = payload.account;
                    var onItemUpdated = payload.onItemUpdated;

                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    Retail_BE_AccountBEService.editAccount(accountBEDefinitionId, account.AccountId, account.ParentAccountId, account.SourceId, onAccountUpdated);
                }
            };
            registerActionType(actionType);
        }
        function registerOpen360DegreeAccount() {

            var actionType = {
                ActionTypeName: "Open360Degree",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var account = payload.account;

                    Retail_BE_AccountBEService.openAccount360DegreeEditor(accountBEDefinitionId, account.AccountId);
                }
            };
            registerActionType(actionType);
        }
        function registerBPActionAccount() {

            var actionType = {
                ActionTypeName: "BPAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountId = payload.account.AccountId;
                    var accountActionDefinition = payload.accountActionDefinition;
                    var onItemUpdated = payload.onItemUpdated;

                    var onActionExecuted = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    Retail_BE_ActionRuntimeService.openActionRuntime(accountBEDefinitionId, accountActionDefinition, accountId, onActionExecuted);
                }
            };
            registerActionType(actionType);
        }
        function registerAccountManagerAssignment() {
            var actionType = {
                ActionTypeName: "AssignAccountManagerAccountAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountId = payload.account.AccountId;
                    var accountActionDefinition = payload.accountActionDefinition;
                    var onItemUpdated = payload.onItemUpdated;

                    var onActionExecuted = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    Retail_BE_AccountManagerAssignmentService.openAccountManagerAssignmentEditor(accountBEDefinitionId, accountId, accountActionDefinition, onItemUpdated);
                }
            };
            registerActionType(actionType);
        }

        function registerChangeStatusAction() {

            var actionType = {
                ActionTypeName: "ChangeStatusAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var account = payload.account;
                    var onItemUpdated = payload.onItemUpdated;
                    var accountActionDefinition = payload.accountActionDefinition;
                    openChangeStatusEditor(onItemUpdated, accountBEDefinitionId, account.AccountId, accountActionDefinition.AccountActionDefinitionId);
                }
            };
            registerActionType(actionType);
        }

        function registerExportRatesAction() {
            var actionType = {
                ActionTypeName: "ExportRatesAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var account = payload.account;
                    Retail_BE_AccountPackageAPIService.ExportRates(accountBEDefinitionId, account.AccountId).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };

            registerActionType(actionType);
        }




        function openChangeStatusEditor(onItemUpdated, accountBEDefinitionId, accountId, accountActionDefinitionId) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onItemUpdated = onItemUpdated;
            };
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountActionDefinitionId: accountActionDefinitionId,
                accountId: accountId
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/ChangeStatusActionEditor.html', parameters, settings);
        }

        function registerStartBPProcessAction() {

            var actionType = {
                ActionTypeName: "StartBPProcess",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var account = payload.account;
                    var actionDefinitionSettings = payload.accountActionDefinition.ActionDefinitionSettings;

                    Retail_BE_AccountBEService.startBPProcessAction(accountBEDefinitionId, account.AccountId, actionDefinitionSettings);
                }
            };
            registerActionType(actionType);
        }

        return ({
            defineAccountMenuActions: defineAccountMenuActions,
            getActionTypeIfExist: getActionTypeIfExist,
            registerActionType: registerActionType,
            registerEditAccount: registerEditAccount,
            registerOpen360DegreeAccount: registerOpen360DegreeAccount,
            registerBPActionAccount: registerBPActionAccount,
            registerChangeStatusAction: registerChangeStatusAction,
            registerExportRatesAction: registerExportRatesAction,
            registerAccountManagerAssignment: registerAccountManagerAssignment,
            registerStartBPProcessAction: registerStartBPProcessAction
        });
    }]);

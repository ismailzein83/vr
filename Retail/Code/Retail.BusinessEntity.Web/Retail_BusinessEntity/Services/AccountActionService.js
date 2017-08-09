'use strict';

app.service('Retail_BE_AccountActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'Retail_BE_AccountBEService', 'Retail_BE_ActionRuntimeService', 'Retail_BE_AccountBEAPIService','Retail_BE_ChangeStatusActionAPIService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, Retail_BE_AccountBEService, Retail_BE_ActionRuntimeService, Retail_BE_AccountBEAPIService, Retail_BE_ChangeStatusActionAPIService) {

        var actionTypes = [];

        function defineAccountMenuActions(accountBEDefinitionId, account, gridAPI, accountViewDefinitions, accountActionDefinitions) {

            account.menuActions = [];

            if (account.AvailableAccountActions != undefined) {
                for (var j = 0; j < account.AvailableAccountActions.length; j++) {
                    var actionId = account.AvailableAccountActions[j];
                    var accountActionDefinition = UtilsService.getItemByVal(accountActionDefinitions, actionId, "AccountActionDefinitionId");
                    if (accountActionDefinition != undefined) {
                        var actionType = getActionTypeIfExist(accountActionDefinition.ActionDefinitionSettings.ClientActionName);
                        if (actionType != undefined) {
                            addGridMenuAction(accountActionDefinition, actionType);
                        }
                    }
                }
            }

            function addGridMenuAction(accountActionDefinition, actionType) {
                account.menuActions.push({
                    name: accountActionDefinition.Name,
                    clicked: function (selectedAccount) {

                        Retail_BE_AccountBEAPIService.GetAccount(accountBEDefinitionId, account.AccountId).then(function (response) {
                            var payload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                account: response,
                                accountActionDefinition: accountActionDefinition,
                                onItemUpdated: function (updatedItem) {
                                    Retail_BE_AccountBEService.defineAccountViewTabs(accountBEDefinitionId, updatedItem, gridAPI, accountViewDefinitions);
                                    defineAccountMenuActions(accountBEDefinitionId, updatedItem, gridAPI, accountViewDefinitions, accountActionDefinitions);
                                    gridAPI.itemUpdated(updatedItem);
                                }
                            };

                            var promise = actionType.ExecuteAction(payload);
                        });
                    }
                });
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

                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            return Retail_BE_ChangeStatusActionAPIService.ChangeAccountStatus(accountBEDefinitionId, account.AccountId, accountActionDefinition.AccountActionDefinitionId).then(function (response) {
                                if (VRNotificationService.notifyOnItemUpdated('Account', response, 'Name')) {
                                    if (onItemUpdated != undefined)
                                        onItemUpdated(response.UpdatedObject);
                                }
                            }).catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                        }
                    });
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
            registerChangeStatusAction: registerChangeStatusAction
        });
    }]);

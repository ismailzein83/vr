
app.service('Retail_BE_AccountActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'Retail_BE_AccountBEService','Retail_BE_ActionRuntimeService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, Retail_BE_AccountBEService, Retail_BE_ActionRuntimeService) {

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
                    clicked: function (account) {
                        var payload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            account: account,
                            accountActionDefinition: accountActionDefinition,
                            onItemUpdated: function (updatedItem) {
                                Retail_BE_AccountBEService.defineAccountViewTabs(accountBEDefinitionId, updatedItem, gridAPI, accountViewDefinitions);
                                defineAccountMenuActions(accountBEDefinitionId, updatedItem, gridAPI, accountViewDefinitions, accountActionDefinitions);
                                gridAPI.itemUpdated(updatedItem);
                            }
                        };

                        //var promiseDeffered = UtilsService.createPromiseDeferred();

                        var promise = actionType.ExecuteAction(payload);
                        //if (promise != undefined && promise.then != undefined) {
                        //    promise.then(function (response) {
                        //        if (response) {
                        //            gridAPI.showLoader();
                        //            var invoiceId = account.Entity.InvoiceId;
                        //            return VR_Invoice_InvoiceAPIService.GetInvoiceDetail(invoiceId).then(function (response) {
                        //                gridAPI.itemUpdated(response);
                        //                defineInvoiceTabsAndMenuActions(response, gridAPI, subSections, subSectionConfigs, invoiceTypeId, invoiceActions);
                        //                promiseDeffered.resolve();
                        //            }).catch(function (error) {
                        //                promiseDeffered.reject(error);
                        //            });
                        //        } else {
                        //            promiseDeffered.resolve();
                        //        }

                        //        promiseDeffered.resolve(); 

                        //    }).catch(function (error) {
                        //        promiseDeffered.reject(error);
                        //    }).finally(function () {
                        //        gridAPI.hideLoader();
                        //    });
                        //} else {
                        //    promiseDeffered.resolve();
                        //}

                        //return promiseDeffered.promise;
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
                    Retail_BE_AccountBEService.editAccount(accountBEDefinitionId, account.Entity.AccountId, account.Entity.ParentAccountId, onAccountUpdated);
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

                    Retail_BE_AccountBEService.openAccount360DegreeEditor(accountBEDefinitionId, account.Entity.AccountId);
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
                    var accountActionDefinition = payload.accountActionDefinition;
                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountId = payload.account.Entity.AccountId;
                    var onItemUpdated = payload.onItemUpdated;
                    var onActionExecuted = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    Retail_BE_ActionRuntimeService.openActionRuntime(accountBEDefinitionId,accountActionDefinition,accountId,  onActionExecuted);
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
            registerBPActionAccount: registerBPActionAccount
        });
    }]);


app.service('Retail_Teles_TelesAccountActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'Retail_BE_AccountBEService', 'Retail_BE_AccountActionService', 'Retail_Teles_TelesAccountAPIService', 'InsertOperationResultEnum',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, Retail_BE_AccountBEService, Retail_BE_AccountActionService, Retail_Teles_TelesAccountAPIService, InsertOperationResultEnum) {


        function registerMappingTelesAccount() {

            var actionType = {
                ActionTypeName: "MappingTelesAccount",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountActionId = payload.accountActionDefinition.AccountActionDefinitionId;
                    var onItemUpdated = payload.onItemUpdated;
                    var account = payload.account;
                    var enterpriseInfoEntity = account.ExtendedSettings != undefined ? account.ExtendedSettings["Retail.Teles.Entities.EnterpriseAccountMappingInfo"] : undefined;

                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    mapTelesAccountEditor(accountBEDefinitionId, accountActionId, account.AccountId, enterpriseInfoEntity, onAccountUpdated);
                }
            };
            Retail_BE_AccountActionService.registerActionType(actionType);
        }

        function mapTelesAccountEditor(accountBEDefinitionId, accountActionId, accountId, enterpriseInfoEntity, onMappingAccount) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                actionDefinitionId: accountActionId,
                accountId: accountId,
                enterpriseInfoEntity: enterpriseInfoEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onMappingAccount = onMappingAccount;
            };

            VRModalService.showModal('/Client/Modules/Retail_Teles/Directives/AccountAction/MappingTelesAccountAction/Templates/MappingTelesAccountEditor.html', parameters, modalSettings);
        }

        function registerUnmappingTelesAccount() {

            var actionType = {
                ActionTypeName: "UnmappingTelesAccount",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountActionId = payload.accountActionDefinition.AccountActionDefinitionId;
                    var onItemUpdated = payload.onItemUpdated;
                    var account = payload.account;
                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    payload.showGridLoader(true);
                    unmapTelesAccountEditor(accountBEDefinitionId, accountActionId, account.AccountId, onAccountUpdated).finally(function () {
                        payload.showGridLoader(false);
                    });
                }
            };
            Retail_BE_AccountActionService.registerActionType(actionType);
        }

        function unmapTelesAccountEditor(accountBEDefinitionId, accountActionId, accountId, onUnmappingAccount) {
            var parameters = {
                AccountBEDefinitionId: accountBEDefinitionId,
                ActionDefinitionId: accountActionId,
                AccountId: accountId
            };

            var deferred = UtilsService.createPromiseDeferred();
            VRNotificationService.showConfirmation('Are you sure you want to unmap Teles account?').then(function (response) {
                if (response) {
                    Retail_Teles_TelesAccountAPIService.UnmapTelesAccount(parameters).then(function (response) {
                        if (response) {
                            switch (response.Result) {
                                case InsertOperationResultEnum.Succeeded.value:
                                    VRNotificationService.showSuccess("Unmapping succeeded");
                                    if (onUnmappingAccount != undefined) {
                                        onUnmappingAccount(response.UpdatedObject);
                                    }
                                    break;
                                case InsertOperationResultEnum.Failed.value:
                                    VRNotificationService.showError("Unmapping Failed");
                                    break;
                                case InsertOperationResultEnum.SameExists.value:
                                    VRNotificationService.showWarning("Same Account unmapped.");
                            }
                        }
                        deferred.resolve();
                    }).catch(function (error) {
                        deferred.reject(error);
                    });
                } else {
                    deferred.resolve();
                }
            });
            return deferred.promise;
        }

        function registerMappingTelesSite() {

            var actionType = {
                ActionTypeName: "MappingTelesSite",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountActionId = payload.accountActionDefinition.AccountActionDefinitionId;
                    var onItemUpdated = payload.onItemUpdated;
                    var account = payload.account;
                    var siteAccountMappingInfo = account.ExtendedSettings != undefined ? account.ExtendedSettings["Retail.Teles.Entities.SiteAccountMappingInfo"] : undefined;

                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    mapTelesSiteEditor(accountBEDefinitionId, accountActionId, account.AccountId, siteAccountMappingInfo, onAccountUpdated);
                }
            };
            Retail_BE_AccountActionService.registerActionType(actionType);
        }

        function mapTelesSiteEditor(accountBEDefinitionId, accountActionId, accountId, telesInfoEntity, onMappingAccount) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                actionDefinitionId: accountActionId,
                accountId: accountId,
                telesInfoEntity: telesInfoEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onMappingAccount = onMappingAccount;
            };

            VRModalService.showModal('/Client/Modules/Retail_Teles/Directives/AccountAction/MappingTelesAccountAction/Templates/MappingTelesSiteEditor.html', parameters, modalSettings);
        }

        function registerMappingTelesUser() {

            var actionType = {
                ActionTypeName: "MappingTelesUser",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountActionId = payload.accountActionDefinition.AccountActionDefinitionId;
                    var onItemUpdated = payload.onItemUpdated;
                    var account = payload.account;
                    var userAccountMappingInfo = account.ExtendedSettings != undefined ? account.ExtendedSettings["Retail.Teles.Entities.UserAccountMappingInfo"] : undefined;

                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    mapTelesUserEditor(accountBEDefinitionId, accountActionId, account.AccountId, userAccountMappingInfo, onAccountUpdated);
                }
            };
            Retail_BE_AccountActionService.registerActionType(actionType);
        }

        function mapTelesUserEditor(accountBEDefinitionId, accountActionId, accountId, telesInfoEntity, onMappingUser) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                actionDefinitionId: accountActionId,
                accountId: accountId,
                telesInfoEntity: telesInfoEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onMappingUser = onMappingUser;
            };

            VRModalService.showModal('/Client/Modules/Retail_Teles/Directives/AccountAction/MappingTelesAccountAction/Templates/MappingTelesUserEditor.html', parameters, modalSettings);
        }

        function registerChangeUserRoutingGroup() {

            var actionType = {
                ActionTypeName: "ChangeUserRoutingGroup",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var accountBEDefinitionId = payload.accountBEDefinitionId;
                    var accountActionId = payload.accountActionDefinition.AccountActionDefinitionId;
                    var onItemUpdated = payload.onItemUpdated;
                    var account = payload.account;
                    var userAccountMappingInfo = account.ExtendedSettings != undefined ? account.ExtendedSettings["Retail.Teles.Entities.UserAccountMappingInfo"] : undefined;

                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    ChangeUserRoutingGroupEditor(accountBEDefinitionId, accountActionId, account.AccountId, userAccountMappingInfo, onAccountUpdated);
                }
            };
            Retail_BE_AccountActionService.registerActionType(actionType);
        }
        function ChangeUserRoutingGroupEditor(accountBEDefinitionId, accountActionId, accountId, telesInfoEntity, onChangeUserRoutingGroup) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                actionDefinitionId: accountActionId,
                accountId: accountId,
                telesInfoEntity: telesInfoEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onChangeUserRoutingGroup = onChangeUserRoutingGroup;
            };
            VRModalService.showModal('/Client/Modules/Retail_Teles/Directives/AccountAction/MappingTelesAccountAction/Templates/ChangeUserRoutingGroupEditor.html', parameters, modalSettings);
        }
        return ({
            registerMappingTelesAccount: registerMappingTelesAccount,
            registerMappingTelesSite: registerMappingTelesSite,
            registerMappingTelesUser: registerMappingTelesUser,
            registerChangeUserRoutingGroup: registerChangeUserRoutingGroup,
            registerUnmappingTelesAccount: registerUnmappingTelesAccount
        });
    }]);

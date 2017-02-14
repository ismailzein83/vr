
app.service('Retail_Teles_TelesAccountActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'Retail_BE_AccountBEService', 'Retail_BE_AccountActionService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, Retail_BE_AccountBEService, Retail_BE_AccountActionService) {

      
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
                    var enterpriseInfoEntity = account.Entity.ExtendedSettings != undefined ? account.Entity.ExtendedSettings["Retail.Teles.Entities.EnterpriseAccountMappingInfo"] : undefined;

                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    mapTelesAccountEditor(accountBEDefinitionId, accountActionId, account.Entity.AccountId, enterpriseInfoEntity, onAccountUpdated);
                }
            };
            Retail_BE_AccountActionService.registerActionType(actionType);
        }

        function mapTelesAccountEditor(accountBEDefinitionId, accountActionId, accountId, enterpriseInfoEntity, onMappingAccount)
        {
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

        return ({
            registerMappingTelesAccount: registerMappingTelesAccount,
        });
    }]);

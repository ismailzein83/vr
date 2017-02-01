
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

                    var onAccountUpdated = function (updatedAccount) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedAccount);
                    };
                    mapTelesAccountEditor(accountBEDefinitionId, accountActionId, onAccountUpdated);
                }
            };
            Retail_BE_AccountActionService.registerActionType(actionType);
        }

        function mapTelesAccountEditor(accountBEDefinitionId, accountActionId, onMappingAccount)
        {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                actionDefinitionId: accountActionId
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

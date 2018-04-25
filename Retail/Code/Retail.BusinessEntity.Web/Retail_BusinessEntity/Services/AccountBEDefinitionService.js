(function (appControllers) {

    'use stict';

    AccountBEDefinitionService.$inject = ['VRModalService'];

    function AccountBEDefinitionService(VRModalService) {

        function addGridColumnDefinition(accountBEDefinitionId, onColumnDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionAdded = onColumnDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/GridColumnDefinitionEditor.html', parameters, modalSettings);
        }
        function editGridColumnDefinition(columnDefinition, accountBEDefinitionId, onColumnDefinitionUpdated) {

            var parameters = {
                columnDefinition: columnDefinition,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionUpdated = onColumnDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/GridColumnDefinitionEditor.html', parameters, modalSettings);
        }

        function addGridColumnExportExcelDefinition(accountBEDefinitionId, onColumnDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionAdded = onColumnDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/GridColumnDefinitionExportExcelEditor.html', parameters, modalSettings);
        }
        function editGridColumnExportExcelDefinition(columnDefinition, accountBEDefinitionId, onColumnDefinitionUpdated) {

            var parameters = {
                columnDefinition: columnDefinition,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionUpdated = onColumnDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/GridColumnDefinitionExportExcelEditor.html', parameters, modalSettings);
        }

        function addAccountViewDefinition(accountBEDefinitionId, onAccountViewDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountViewDefinitionAdded = onAccountViewDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/Templates/AccountViewDefinitionEditor.html', parameters, modalSettings);

        }
        function editAccountViewDefinition(accountViewDefinitionEntity, accountBEDefinitionId, onAccountViewDefinitionUpdated) {

            var parameters = {
                accountViewDefinitionEntity: accountViewDefinitionEntity,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountViewDefinitionUpdated = onAccountViewDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/Templates/AccountViewDefinitionEditor.html', parameters, modalSettings);
        }

        function addAccountActionDefinition(accountBEDefinitionId, onAccountActionDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountActionDefinitionAdded = onAccountActionDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/Templates/AccountActionDefinitionEditor.html', parameters, modalSettings);

        }
        function editAccountActionDefinition(accountActionDefinitionEntity, accountBEDefinitionId, onAccountActionDefinitionUpdated) {

            var parameters = {
                accountActionDefinitionEntity: accountActionDefinitionEntity,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountActionDefinitionUpdated = onAccountActionDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/Templates/AccountActionDefinitionEditor.html', parameters, modalSettings);
        }

        function addAccountExtraFieldDefinition(accountBEDefinitionId, onAccountExtraFieldDefinitionAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountExtraFieldDefinitionAdded = onAccountExtraFieldDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountExtraFieldDefinition/Templates/AccountExtraFieldDefinitionEditor.html', parameters, modalSettings);

        }
        function editAccountExtraFieldDefinition(accountExtraFieldDefinitionEntity, accountBEDefinitionId, onAccountExtraFieldDefinitionUpdated) {

            var parameters = {
                accountExtraFieldDefinitionEntity: accountExtraFieldDefinitionEntity,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountExtraFieldDefinitionUpdated = onAccountExtraFieldDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountExtraFieldDefinition/Templates/AccountExtraFieldDefinitionEditor.html', parameters, modalSettings);
        }
        function addAccountBulkAction(accountBEDefinitionId, onAccountBulkActionAdded) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountBulkActionAdded = onAccountBulkActionAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountBulkActions/Templates/AccountBulkActionEditor.html', parameters, modalSettings);
        }

        function editAccountBulkAction(accountBulkActionEntity, accountBEDefinitionId, onAccountBulkActionUpdated) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountBulkActionEntity: accountBulkActionEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountBulkActionUpdated = onAccountBulkActionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountBulkActions/Templates/AccountBulkActionEditor.html', parameters, modalSettings);
        }

        return {
            addGridColumnDefinition: addGridColumnDefinition,
            editGridColumnDefinition: editGridColumnDefinition,
            addGridColumnExportExcelDefinition: addGridColumnExportExcelDefinition,
            editGridColumnExportExcelDefinition: editGridColumnExportExcelDefinition,
            addAccountViewDefinition: addAccountViewDefinition,
            editAccountViewDefinition: editAccountViewDefinition,
            addAccountActionDefinition: addAccountActionDefinition,
            editAccountActionDefinition: editAccountActionDefinition,
            addAccountExtraFieldDefinition: addAccountExtraFieldDefinition,
            editAccountExtraFieldDefinition: editAccountExtraFieldDefinition,
            addAccountBulkAction: addAccountBulkAction,
            editAccountBulkAction: editAccountBulkAction
        };
    }

    appControllers.service('Retail_BE_AccountBEDefinitionService', AccountBEDefinitionService);

})(appControllers);
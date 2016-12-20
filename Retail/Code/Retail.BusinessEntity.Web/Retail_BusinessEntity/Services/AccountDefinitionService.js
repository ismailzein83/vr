(function (appControllers) {

    'use stict';

    AccountDefinitionService.$inject = ['VRModalService'];

    function AccountDefinitionService(VRModalService) {

        function addGridColumnDefinition(onColumnDefinitionAdded) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionAdded = onColumnDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountDefinition/AccountGridDefinition/GridColumnDefinitionEditor.html', null, modalSettings);
        };
        function editGridColumnDefinition(columnDefinition, onColumnDefinitionUpdated) {

            var parameters = {
                columnDefinition: columnDefinition
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionUpdated = onColumnDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountDefinition/AccountGridDefinition/GridColumnDefinitionEditor.html', parameters, modalSettings);
        }

        function addAccountViewDefinition(onAccountViewDefinitionAdded) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountViewDefinitionAdded = onAccountViewDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountDefinition/AccountViewDefinition/AccountViewDefinitionEditor.html', null, modalSettings);

        }
        function editAccountViewDefinition(accountViewDefinitionEntity, onAccountViewDefinitionUpdated) {

            var parameters = {
                accountViewDefinitionEntity: accountViewDefinitionEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountViewDefinitionUpdated = onAccountViewDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountDefinition/AccountViewDefinition/AccountViewDefinitionEditor.html', parameters, modalSettings);
        }


        return {
            addGridColumnDefinition: addGridColumnDefinition,
            editGridColumnDefinition: editGridColumnDefinition,
            addAccountViewDefinition: addAccountViewDefinition,
            editAccountViewDefinition: editAccountViewDefinition
        };
    }

    appControllers.service('Retail_BE_AccountDefinitionService', AccountDefinitionService);

})(appControllers);
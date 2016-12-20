(function (appControllers) {

    'use stict';

    ColumnDefinitionService.$inject = ['VRModalService'];

    function ColumnDefinitionService(VRModalService) {

        function addColumnDefinitions(onColumnDefinitionAdded) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionAdded = onColumnDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ColumnDefinition/ColumnDefinitionEditor.html', null, modalSettings);
        };

        function editColumnDefinitions(columnDefinition, onColumnDefinitionUpdated) {

            var parameters = {
                columnDefinition: columnDefinition
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionUpdated = onColumnDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ColumnDefinition/ColumnDefinitionEditor.html', parameters, modalSettings);
        }

        return {
            addColumnDefinition: addColumnDefinition,
            editColumnDefinition: editColumnDefinition
        };
    }

    appControllers.service('Retail_BE_ColumnDefinitionService', ColumnDefinitionService);

})(appControllers);
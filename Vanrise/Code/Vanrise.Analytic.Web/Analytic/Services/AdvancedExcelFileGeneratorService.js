(function (appControllers) {
    'use stict';
    advancedExcelFileGeneratorService.$inject = ['VRModalService'];
    function advancedExcelFileGeneratorService(VRModalService) {


        function addTableDefinition(onTableDefinitionAdded) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTableDefinitionAdded = onTableDefinitionAdded;
            };

            var modalParameters = {
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorEditorTemplate.html', modalParameters, modalSettings);
        }

        function editTableDefinition(object, onTableDefinitionUpdated) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modelScope) {
                modelScope.onTableDefinitionUpdated = onTableDefinitionUpdated;
            };
            var modalParameters = {
                Entity: object
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorEditorTemplate.html', modalParameters, modalSettings);
        }

        return {
            addTableDefinition: addTableDefinition,
            editTableDefinition: editTableDefinition,
        };
    }

    appControllers.service('VRAnalytic_AdvancedExcelFileGeneratorService', advancedExcelFileGeneratorService);

})(appControllers);

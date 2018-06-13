(function (appControllers) {
    'use stict';
    advancedExcelFileGeneratorService.$inject = ['VRModalService'];
    function advancedExcelFileGeneratorService(VRModalService) {


        function addTableDefinition(onTableDefinitionAdded, context) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTableDefinitionAdded = onTableDefinitionAdded;
            };

            var modalParameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorEditorTemplate.html', modalParameters, modalSettings);
        }

        function editTableDefinition(object, onTableDefinitionUpdated, context) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modelScope) {
                modelScope.onTableDefinitionUpdated = onTableDefinitionUpdated;
            };
            var modalParameters = {
                Entity: object,
                context: context
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

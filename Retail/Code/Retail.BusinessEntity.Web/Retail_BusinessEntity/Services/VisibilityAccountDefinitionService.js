(function (appControllers) {

    'use stict';

    VisibilityAccountDefinition.$inject = ['VRModalService'];

    function VisibilityAccountDefinition(VRModalService) {

        function addVisibilityAccountDefinition(excludedAccountBEDefinitionIds, onVisibilityAccountDefinitionAdded) {

            var parameters = {
                excludedAccountBEDefinitionIds: excludedAccountBEDefinitionIds
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVisibilityAccountDefinitionAdded = onVisibilityAccountDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/Templates/VisibilityAccountDefinitionEditor.html', parameters, modalSettings);
        };
        function editVisibilityAccountDefinition(visibilityAccountDefinition, retailBEVisibilityEditorRuntime, excludedAccountBEDefinitionIds, onVisibilityAccountDefinitionUpdated) {

            var parameters = {
                visibilityAccountDefinition: visibilityAccountDefinition,
                retailBEVisibilityEditorRuntime: retailBEVisibilityEditorRuntime,
                excludedAccountBEDefinitionIds: excludedAccountBEDefinitionIds
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVisibilityAccountDefinitionUpdated = onVisibilityAccountDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/Templates/VisibilityAccountDefinitionEditor.html', parameters, modalSettings);
        }


        return {
            addVisibilityAccountDefinition: addVisibilityAccountDefinition,
            editVisibilityAccountDefinition: editVisibilityAccountDefinition
        };
    }

    appControllers.service('Retail_BE_VisibilityAccountDefinitionService', VisibilityAccountDefinition);

})(appControllers);
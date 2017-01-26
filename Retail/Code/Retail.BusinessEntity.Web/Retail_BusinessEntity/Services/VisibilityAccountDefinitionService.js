(function (appControllers) {

    'use stict';

    VisibilityAccountDefinition.$inject = ['VRModalService'];

    function VisibilityAccountDefinition(VRModalService) {

        function addVisibilityAccountDefinition(onVisibilityAccountDefinitionAdded) {

            var parameters = {
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVisibilityAccountDefinitionAdded = onVisibilityAccountDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/Templates/VisibilityAccountDefinitionEditor.html', parameters, modalSettings);
        };
        function editVisibilityAccountDefinition(visibilityAccountDefinition, onVisibilityAccountDefinitionUpdated) {

            var parameters = {
                visibilityAccountDefinition: visibilityAccountDefinition
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
(function (appControllers) {

    "use strict";

    StyleDefinitionService.$inject = ['VRModalService'];

    function StyleDefinitionService(VRModalService) {

        function addStyleDefinition(onStyleDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onStyleDefinitionAdded = onStyleDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/StyleDefinition/StyleDefinitionEditor.html', null, settings);
        };

        function editStyleDefinition(styleDefinitionId, onStyleDefinitionUpdated) {
            var settings = {};

            var parameters = {
                styleDefinitionId: styleDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onStyleDefinitionUpdated = onStyleDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/StyleDefinition/StyleDefinitionEditor.html', parameters, settings);
        }


        return {
            addStyleDefinition: addStyleDefinition,
            editStyleDefinition: editStyleDefinition
        };
    }

    appControllers.service('VRCommon_StyleDefinitionService', StyleDefinitionService);

})(appControllers);
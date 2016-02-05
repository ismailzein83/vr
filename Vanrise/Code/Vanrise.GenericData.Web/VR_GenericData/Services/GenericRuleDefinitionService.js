(function (appControllers) {

    'use strict';

    GenericRuleDefinitionService.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService'];

    function GenericRuleDefinitionService(VR_GenericData_GenericRuleDefinitionAPIService, VRModalService, VRNotificationService) {
        return {
            addGenericRuleDefinition: addGenericRuleDefinition,
            editGenericRuleDefinition: editGenericRuleDefinition
        };

        function addGenericRuleDefinition(onGenericRuleDefinitionAdded, settingsTypeName) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionAdded = onGenericRuleDefinitionAdded;
            };
            
            var parameters;
            if (settingsTypeName != undefined)
            {
                parameters = {
                    SettingsTypeName: settingsTypeName
                };
            }

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html', parameters, modalSettings);
        }

        function editGenericRuleDefinition(genericRuleDefinitionId, onGenericRuleDefinitionUpdated) {
            var modalParameters = {
                GenericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionUpdated = onGenericRuleDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionService', GenericRuleDefinitionService);

})(appControllers);
(function (appControllers) {

    'use strict';

    genericRule.$inject = ['VRModalService'];

    function genericRule(VRModalService) {
        return ({
            addGenericRule: addGenericRule,
            editGenericRule: editGenericRule
        });

        function addGenericRule(genericRuleDefinitionId, onGenericRuleAdded) {
            var modalParameters = {
                genericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleAdded = onGenericRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }

        function editGenericRule(genericRuleId, genericRuleDefinitionId, onGenericRuleUpdated) {
            var modalParameters = {
                genericRuleId: genericRuleId,
                genericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleUpdated = onGenericRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_GenericData_GenericRule', genericRule);

})(appControllers);

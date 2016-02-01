(function (appControllers) {

    'use strict';

    genericRule.$inject = ['VRModalService'];

    function genericRule(VRModalService) {
        return ({
            addGenericRule: addGenericRule
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
    };

    appControllers.service('VR_GenericData_GenericRule', genericRule);

})(appControllers);

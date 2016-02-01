(function (appControllers) {

    'use strict';

    genericRule.$inject = ['VRModalService'];

    function genericRule(VRModalService) {
        return ({
            addGenericRule: addGenericRule
        });

        function addGenericRule(onGenericRuleAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleAdded = onGenericRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', null, modalSettings);
        }
    };

    appControllers.service('VR_GenericData_GenericRule', genericRule);

})(appControllers);

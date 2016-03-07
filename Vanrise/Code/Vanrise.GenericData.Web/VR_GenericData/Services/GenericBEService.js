(function (appControllers) {

    'use strict';

    GenericBEService.$inject = ['VRModalService', 'VRNotificationService'];

    function GenericBEService( VRModalService, VRNotificationService) {
        return {
            addGenericBE: addGenericBE,
        };

        function addGenericBE(onGenericBEDefinitionAdded) {
            var modalParameters = {
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEDefinitionAdded = onGenericBEDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEEditorDefintion.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_GenericBEService', GenericBEService);

})(appControllers);
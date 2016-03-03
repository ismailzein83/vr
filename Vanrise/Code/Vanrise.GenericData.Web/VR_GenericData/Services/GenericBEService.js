﻿(function (appControllers) {

    'use strict';

    GenericBEService.$inject = ['VR_GenericData_GenericEditorAPIService', 'VRModalService', 'VRNotificationService'];

    function GenericBEService(VR_GenericData_GenericEditorAPIService, VRModalService, VRNotificationService) {
        return {
            editGenericBE: editGenericBE,
            addGenericBE: addGenericBE,
        };

        function editGenericBE(genericEditorDefinitionId, onGenericEditorUpdated) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericEditorUpdated = onGenericEditorUpdated;
            };

            var parameters = {
                genericEditorDefinitionId: genericEditorDefinitionId
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericBEEditor.html', parameters, modalSettings);
        }

        function addGenericBE(onGenericBEAdded) {
            var modalParameters = {
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEAdded = onGenericBEAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericBEEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_GenericBEService', GenericBEService);

})(appControllers);
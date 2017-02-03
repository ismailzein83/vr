﻿(function (appControllers) {

    'use strict';

   OutPutHandlerService.$inject = ['VRModalService'];

    function OutPutHandlerService(VRModalService) {
        return ({
            addOutPutHandler: addOutPutHandler,
            editOutPutHandler: editOutPutHandler,
        });

        function addOutPutHandler(onOutPutHandlerAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOutPutHandlerAdded = onOutPutHandlerAdded;
            };

            VRModalService.showModal('/Client/Modules/Mediation_Generic/Views/MediationDefinition/OutPutHandlerEditor.html', null, modalSettings);
        }

        function editOutPutHandler(outPutHandler, onOutPutHandlerUpdated) {
            var modalParameters = {
                outPutHandler: outPutHandler
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOutPutHandlerUpdated = onOutPutHandlerUpdated;
            };

            VRModalService.showModal('/Client/Modules/Mediation_Generic/Views/MediationDefinition/OutPutHandlerEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('Mediation_Generic_OutPutHandlerService',OutPutHandlerService);

})(appControllers);

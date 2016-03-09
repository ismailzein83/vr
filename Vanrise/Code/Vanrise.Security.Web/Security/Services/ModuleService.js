(function (appControllers) {

    'use strict';

    ModuleService.$inject = ['VRModalService'];

    function ModuleService(VRModalService) {
        return ({
            addModule: addModule,
            editModule: editModule,
        });

        function addModule(onModuleAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onModuleAdded = onModuleAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Menu/ModuleEditor.html', null, modalSettings);
        }

        function editModule(moduleId, onModuleUpdated) {
            var modalParameters = {
                moduleId: moduleId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onModuleUpdated = onModuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Menu/ModuleEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_Sec_ModuleService', ModuleService);

})(appControllers);

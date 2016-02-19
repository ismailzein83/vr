(function (appControllers) {

    'use strict';

    PopService.$inject = ['VRModalService'];

    function PopService(VRModalService) {
        return ({
            addPop: addPop,
            editPop: editPop
        });

        function addPop(onPopAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPopAdded = onPopAdded;
            };
            var parameters;
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/Pop/PopEditor.html', parameters, settings);
        }

        function editPop(popId, onPopUpdated) {
            var modalSettings = {
            };
            var parameters = {
                PopId: popId
           };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPopUpdated = onPopUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/Pop/PopEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('Demo_PopService', PopService);

})(appControllers);

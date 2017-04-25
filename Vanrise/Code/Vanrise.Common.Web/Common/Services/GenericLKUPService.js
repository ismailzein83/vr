
(function (appControllers) {

    'use stict';

    GenericLKUPService.$inject = ['VRModalService'];

    function GenericLKUPService(VRModalService) {
        
        function addGenericLKUP(onGenericLKUPAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericLKUPAdded = onGenericLKUPAdded
            };

            VRModalService.showModal('/Client/Modules/Common/Views/GenericLKUP/GenericLKUPEditor.html', null, settings);
        }

        function editGenericLKUP(genericLKUPItemId, onGenericLKUPUpdated) {
            var settings = {};

            var parameters = {
                genericLKUPItemId: genericLKUPItemId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericLKUPUpdated = onGenericLKUPUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/GenericLKUP/GenericLKUPEditor.html', parameters, settings);
        }
       

        return {
            addGenericLKUP: addGenericLKUP,
            editGenericLKUP: editGenericLKUP,
            
        };
    }

    appControllers.service('VR_Common_GenericLKUPService', GenericLKUPService);

})(appControllers);
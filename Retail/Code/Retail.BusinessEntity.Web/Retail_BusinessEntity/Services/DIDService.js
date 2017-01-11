(function (appControllers) {

    'use stict';

    DIDService.$inject = ['VRModalService'];

    function DIDService(VRModalService) {

        function addDID(onDIDAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDIDAdded = onDIDAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/DID/DIDEditor.html', null, settings);
        };
        function editDID(dIDId, onDIDUpdated) {

            var parameters = {
               dIDId: dIDId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDIDUpdated = onDIDUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/DID/DIDEditor.html', parameters, settings);
        }
        return {
            addDID: addDID,
            editDID: editDID
        };
    }

    appControllers.service('Retail_BE_DIDService', DIDService);

})(appControllers);
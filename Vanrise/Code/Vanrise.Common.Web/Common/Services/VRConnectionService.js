
(function (appControllers) {

    "use strict";

    VRConnectionService.$inject = ['VRModalService'];

    function VRConnectionService(VRModalService) {

        function addVRConnection(onVRConnectionAdded) {
            var settings = {};
            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRConnectionAdded = onVRConnectionAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRConnection/VRConnectionEditor.html', parameters, settings);
        };

        function editVRConnection( vrConnectionId, onVRConnectionUpdated) {
            var settings = {};

            var parameters = {
                vrConnectionId: vrConnectionId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRConnectionUpdated = onVRConnectionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRConnection/VRConnectionEditor.html', parameters, settings);
        }


        return {
            addVRConnection: addVRConnection,
            editVRConnection: editVRConnection
        };
    }

    appControllers.service('VRCommon_VRConnectionService', VRConnectionService);

})(appControllers);
(function (appControllers) {

    'use strict';

    SwitchConnectivityService.$inject = ['WhS_BE_SwitchConnectivityAPIService', 'VRModalService', 'VRNotificationService'];

    function SwitchConnectivityService(WhS_BE_SwitchConnectivityAPIService, VRModalService, VRNotificationService) {
        var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/SwitchConnectivity/SwitchConnectivityEditor.html';

        function addSwitchConnectivity(onSwitchConnectivityAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchConnectivityAdded = onSwitchConnectivityAdded;
            };

            VRModalService.showModal(editorUrl, null, settings);
        }

        function editSwitchConnectivity(switchConnectivityId, onSwitchConnectivityUpdated) {
            var parameters = {
                switchConnectivityId: switchConnectivityId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchConnectivityUpdated = onSwitchConnectivityUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        return {
            addSwitchConnectivity: addSwitchConnectivity,
            editSwitchConnectivity: editSwitchConnectivity
        };
    }

    appControllers.service('WhS_BE_SwitchConnectivityService', SwitchConnectivityService);

})(appControllers);
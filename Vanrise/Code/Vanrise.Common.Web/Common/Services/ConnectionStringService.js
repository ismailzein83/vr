(function (appControllers) {

    'use strict';

    ConnectionStringService.$inject = ['VRModalService'];

    function ConnectionStringService(VRModalService) {
        return {
            addConnectionString: addConnectionString,
            editConnectionString: editConnectionString
        };

        function addConnectionString(onConnectionAdded) {
            var modalParameters = {
               
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConnectionAdded = onConnectionAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CurrencyExchangeRate/ConnectionEditor.html', modalParameters, modalSettings);
        }

        function editConnectionString(connection,  onConnectionUpdated) {
            var modalParameters = {
                Connection: connection
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConnectionUpdated = onConnectionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CurrencyExchangeRate/ConnectionEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_ConnectionStringService', ConnectionStringService);

})(appControllers);
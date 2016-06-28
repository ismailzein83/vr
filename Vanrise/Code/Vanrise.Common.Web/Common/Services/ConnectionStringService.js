(function (appControllers) {

    'use strict';

    ConnectionStringService.$inject = ['VRModalService'];

    function ConnectionStringService(VRModalService) {
        return {
            addConnectionString: addConnectionString,
            editConnectionString: editConnectionString
        };

        function addConnectionString(onConnectionStringAdded) {
            //var modalParameters = {
               
            //};
            //var modalSettings = {};

            //modalSettings.onScopeReady = function (modalScope) {
            //    modalScope.onConnectionStringAdded = onConnectionStringAdded;
            //};

            //VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/DataRecordSourceEditor.html', modalParameters, modalSettings);
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
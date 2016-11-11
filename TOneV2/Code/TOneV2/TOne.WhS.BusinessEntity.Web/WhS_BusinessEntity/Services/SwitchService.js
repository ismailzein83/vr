(function (appControllers) {

    'use strict';

    SwitchService.$inject = ['WhS_BE_SwitchAPIService', 'VRModalService', 'VRNotificationService'];

    function SwitchService(WhS_BE_SwitchAPIService, VRModalService, VRNotificationService) {
        return ({
            addSwitch: addSwitch,
            editSwitch: editSwitch,
            deleteSwitch: deleteSwitch
        });

        function addSwitch(onSwitchAdded) {
            var settings = {
            };

            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchAdded = onSwitchAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Switch/SwitchEditor.html', parameters, settings);
        }

        function editSwitch(switchId, onSwitchUpdated) {
            var modalSettings = {
            };
            var parameters = {
                switchId: switchId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSwitchUpdated = onSwitchUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Switch/SwitchEditor.html', parameters, modalSettings);
        }

        function deleteSwitch(scope, switchId, onSwitchDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                   
                    if (response) {
                        return WhS_BE_SwitchAPIService.DeleteSwitch(switchId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Switch", deletionResponse);
                                onSwitchDeleted();
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_BE_SwitchService', SwitchService);

})(appControllers);

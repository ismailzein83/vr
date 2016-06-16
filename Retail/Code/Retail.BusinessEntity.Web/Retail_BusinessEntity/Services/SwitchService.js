(function (appControllers) {

    'use stict';

    SwitchService.$inject = ['VRModalService'];

    function SwitchService(VRModalService) {

        function addSwitch(onSwitchAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchAdded = onSwitchAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Switch/SwitchEditor.html', null, settings);
        };

        function editSwitch(switchEntity, onSwitchUpdated) {
            var modalSettings = {
            };

            var parameters = {
                switchEntity: switchEntity,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSwitchUpdated = onSwitchUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Switch/SwitchEditor.html', parameters, modalSettings);
        }

        return {
            addSwitch: addSwitch,
            editSwitch: editSwitch
        };
    }

    appControllers.service('Retail_BE_SwitchService', SwitchService);

})(appControllers);
(function (appControllers) {

    'use stict';

    SwitchService.$inject = ['VRModalService'];

    function SwitchService(VRModalService) {
        function addSwitch(onSwitchAdded) {
            var parameters = {
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchAdded = onSwitchAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Switch/SwitchEditor.html', parameters, settings);
        };

        return {
            addSwitch: addSwitch
        };
    }

    appControllers.service('Retail_BE_SwitchService', SwitchService);

})(appControllers);

(function (appControllers) {

    'use stict';

    StatusDefinitionService.$inject = ['VRModalService'];

    function StatusDefinitionService(VRModalService) {

        //function addSwitch(onSwitchAdded) {
        //    var settings = {};

        //    settings.onScopeReady = function (modalScope) {
        //        modalScope.onSwitchAdded = onSwitchAdded
        //    };

        //    VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Switch/SwitchEditor.html', null, settings);
        //};

        //function editSwitch(switchId, onSwitchUpdated) {
        //    var modalSettings = {
        //    };

        //    var parameters = {
        //        switchId: switchId,
        //    };

        //    modalSettings.onScopeReady = function (modalScope) {
        //        modalScope.onSwitchUpdated = onSwitchUpdated;
        //    };
        //    VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Switch/SwitchEditor.html', parameters, modalSettings);
        //}

        //return {
        //    addSwitch: addSwitch,
        //    editSwitch: editSwitch
        //};
    }

    appControllers.service('Retail_BE_StatusDefinitionService', StatusDefinitionService);

})(appControllers);
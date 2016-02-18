(function (appControllers) {

    'use stict';

    OperatorConfigurationService.$inject = ['VRModalService'];

    function OperatorConfigurationService(VRModalService) {
        return ({
            addOperatorConfiguration: addOperatorConfiguration,
            editOperatorConfiguration: editOperatorConfiguration
        });

        function addOperatorConfiguration(onOperatorConfigurationAdded) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onOperatorConfigurationAdded = onOperatorConfigurationAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorConfiguration/OperatorConfigurationEditor.html', null, settings);
        }

        function editOperatorConfiguration(operatorConfigurationObj, onOperatorConfigurationUpdated) {
            var modalSettings = {
            };

            var parameters = {
                OperatorConfigurationId: operatorConfigurationObj.Entity.OperatorConfigurationId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOperatorConfigurationUpdated = onOperatorConfigurationUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorConfiguration/OperatorConfigurationEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('Demo_OperatorConfigurationService', OperatorConfigurationService);

})(appControllers);

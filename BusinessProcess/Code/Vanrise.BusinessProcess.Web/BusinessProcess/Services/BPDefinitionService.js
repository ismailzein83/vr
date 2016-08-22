(function (appControllers) {

    "use strict";
    BusinessProcess_BPDefinitionService.$inject = ['VRModalService'];

    function BusinessProcess_BPDefinitionService(VRModalService) {

        return ({
            editBusinessProcessDefinition: editBusinessProcessDefinition
        });

        function editBusinessProcessDefinition(businessProcessDefinitionId, onBPDefenitionUpdated) {
            var modalParameters = {
                businessProcessDefinitionId: businessProcessDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBPDefenitionUpdated = onBPDefenitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPDefinition/BPTechnicalDefinitionEditor.html', modalParameters, modalSettings);
        }
    }
    appControllers.service('BusinessProcess_BPDefinitionService', BusinessProcess_BPDefinitionService);

})(appControllers);
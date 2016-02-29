(function (appControllers) {

    "use strict";

    BusinessProcess_BPDefinitionService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum', 'VRModalService', 'UtilsService'];

    function BusinessProcess_BPDefinitionService(LabelColorsEnum, BPInstanceStatusEnum, VRModalService, UtilsService) {

        return ({
            startNewInstance: startNewInstance
        });

        function startNewInstance(bpDefinitionObj) {
            var modalSettings = {
            };

            var parameters = {
                BPDefinitionID: bpDefinitionObj.Entity.BPDefinitionID
            };


            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/NewInstanceEditor/NewInstanceEditor.html', parameters, modalSettings);
        }
    }
    appControllers.service('BusinessProcess_BPDefinitionService', BusinessProcess_BPDefinitionService);

})(appControllers);
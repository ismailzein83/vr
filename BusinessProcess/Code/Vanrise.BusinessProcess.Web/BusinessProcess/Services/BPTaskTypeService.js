(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskTypeService.$inject = [ 'VRModalService','BusinessProcess_BPTaskTypeAPIService'];
    function BusinessProcess_BPTaskTypeService(VRModalService, BusinessProcess_BPTaskTypeAPIService) {

        function addBPTaskType(onTaskTypeAdded) {
            var modalParameters = {
                businessEntityDefinitionId: "d33fd65a-721f-4ae1-9d41-628be9425796"
            };

            var modalSettings = {
                size:"medium"
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEAdded = onTaskTypeAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', modalParameters, modalSettings);
        }



        return {
            addBPTaskType: addBPTaskType
        };
    }
    appControllers.service('BusinessProcess_BPTaskTypeService', BusinessProcess_BPTaskTypeService);

})(appControllers);
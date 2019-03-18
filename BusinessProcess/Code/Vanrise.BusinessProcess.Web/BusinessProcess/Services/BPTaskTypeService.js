(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskTypeService.$inject = [ 'VRModalService','BusinessProcess_BPTaskTypeAPIService'];
    function BusinessProcess_BPTaskTypeService(VRModalService, BusinessProcess_BPTaskTypeAPIService) {

        function addBPTaskType(onTaskTypeAdded) {

            var modalParameters = {
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTaskTypeAdded = onTaskTypeAdded;
            };

            VRModalService.showModal("/Client/Modules/BusinessProcess/Directives/BPTask/Templates/BPTaskTypeStaticEditorTemplate.html", modalParameters, modalSettings);
        }



        return {
            addBPTaskType: addBPTaskType
        };
    }
    appControllers.service('BusinessProcess_BPTaskTypeService', BusinessProcess_BPTaskTypeService);

})(appControllers);
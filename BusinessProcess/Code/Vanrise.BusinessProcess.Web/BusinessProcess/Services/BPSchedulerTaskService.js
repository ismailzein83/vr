


(function (appControllers) {

    "use strict";
    BusinessProcess_BPSchedulerTaskService.$inject = ['VRModalService'];
    function BusinessProcess_BPSchedulerTaskService(VRModalService) {

        return ({
            showAddTaskModal: showAddTaskModal
        });

        function showAddTaskModal(bpDefinitionObj) {
            var settings = {
            };
            //'7a35f562-319b-47b3-8258-ec1a704a82eb' is the related action type id for workflow
            var parameters = {
                additionalParameter: { bpDefinitionID: bpDefinitionObj.Entity.BPDefinitionID, actionTypeId: '7a35f562-319b-47b3-8258-ec1a704a82eb' }
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Schedule Task";
            };

            VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, settings);
  
        }

    }
    appControllers.service('BusinessProcess_BPSchedulerTaskService', BusinessProcess_BPSchedulerTaskService);

})(appControllers);
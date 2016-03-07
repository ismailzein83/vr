


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
            var parameters = {
                additionalParameter: { bpDefinitionID: bpDefinitionObj.Entity.BPDefinitionID }
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Schedule Task";
            };

            VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, settings);
  
        }

    }
    appControllers.service('BusinessProcess_BPSchedulerTaskService', BusinessProcess_BPSchedulerTaskService);

})(appControllers);
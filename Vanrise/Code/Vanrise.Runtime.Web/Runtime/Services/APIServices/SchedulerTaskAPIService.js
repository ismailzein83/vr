app.service('SchedulerTaskAPIService', function (BaseAPIService) {

    return ({
        GetFilteredTasks: GetFilteredTasks,
        GetTask: GetTask,
        GetSchedulerTaskTriggerTypes: GetSchedulerTaskTriggerTypes,
        GetSchedulerTaskActionTypes: GetSchedulerTaskActionTypes,
        AddTask: AddTask,
        UpdateTask: UpdateTask,
        DeleteTask: DeleteTask
    });

    function GetFilteredTasks(input) {
        return BaseAPIService.post("/api/SchedulerTask/GetFilteredTasks", input);
    }

    function GetTask(taskId) {
        return BaseAPIService.get("/api/SchedulerTask/GetTask",
            {
                taskId: taskId
            }
           );
    }

    function GetSchedulerTaskTriggerTypes() {
        return BaseAPIService.get("/api/SchedulerTask/GetSchedulerTaskTriggerTypes");
    }

    function GetSchedulerTaskActionTypes() {
        return BaseAPIService.get("/api/SchedulerTask/GetSchedulerTaskActionTypes");
    }

    function AddTask(task) {
        return BaseAPIService.post("/api/SchedulerTask/AddTask", task);
    }

    function UpdateTask(task) {
        return BaseAPIService.post("/api/SchedulerTask/UpdateTask", task);
    }

    function DeleteTask(taskId) {
        return BaseAPIService.get("/api/SchedulerTask/DeleteTask", {
            taskId: taskId
        });
    }
});
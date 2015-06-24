app.service('SchedulerTaskAPIService', function (BaseAPIService) {

    return ({
        GetFilteredTasks: GetFilteredTasks,
        GetTask: GetTask,
        AddTask: AddTask,
        UpdateTask: UpdateTask
    });

    function GetFilteredTasks(fromRow, toRow, name) {
        return BaseAPIService.get("/api/SchedulerTask/GetFilteredTasks",
            {
                fromRow: fromRow,
                toRow: toRow,
                name: name
            }
           );
    }

    function GetTask(taskId) {
        return BaseAPIService.get("/api/SchedulerTask/GetTask",
            {
                taskId: taskId
            }
           );
    }

    function AddTask(task) {
        return BaseAPIService.post("/api/SchedulerTask/AddTask", task);
    }

    function UpdateTask(task) {
        return BaseAPIService.post("/api/SchedulerTask/UpdateTask", task);
    }

});
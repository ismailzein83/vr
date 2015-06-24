app.service('SchedulerAPIService', function (BaseAPIService) {

    return ({
        GetFilteredTasks: GetFilteredTasks
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

});
var BusinessProcessAPIService = function (BaseAPIService) {

    "use strict";

    function GetDefinitions() {
        return BaseAPIService.get("/api/BusinessProcess/GetDefinitions");
    }

    function GetStatusList() {
        return BaseAPIService.get("/api/BusinessProcess/GetStatusList");
    }

    function GetFilteredBProcess(DefinitionsId, InstanceStatus, FromRow, ToRow, DateFrom, DateTo) {
        return BaseAPIService.post("/api/BusinessProcess/GetFilteredBProcess", {
            DefinitionsId: DefinitionsId,
            InstanceStatus: InstanceStatus,
            FromRow: FromRow,
            ToRow: ToRow,
            DateFrom: DateFrom,
            DateTo: DateTo
        });
    }

    function GetFilteredDefinitions(fromRow, toRow, title) {
        return BaseAPIService.get("/api/BusinessProcess/GetFilteredDefinitions",
            {
                fromRow: fromRow,
                toRow: toRow,
                title: title
            });
    }

    function GetTrackingsByInstanceId(processInstanceID, fromRow, toRow, trackingSeverity,message) {
        return BaseAPIService.post("/api/BusinessProcess/GetTrackingsByInstanceId", {
            ProcessInstanceID: processInstanceID,
            FromRow: fromRow,
            ToRow: toRow,
            TrackingSeverity: trackingSeverity,
            Message: message
        });
    }

    function GetTrackingSeverity() {
        return BaseAPIService.get("/api/BusinessProcess/GetTrackingSeverity");
    }

    return ({
        GetDefinitions: GetDefinitions,
        GetFilteredDefinitions: GetFilteredDefinitions,
        GetStatusList: GetStatusList,
        GetFilteredBProcess: GetFilteredBProcess,
        GetTrackingsByInstanceId: GetTrackingsByInstanceId,
        GetTrackingSeverity: GetTrackingSeverity
    });

};
BusinessProcessAPIService.$inject = ['BaseAPIService'];
appControllers.service('BusinessProcessAPIService', BusinessProcessAPIService);
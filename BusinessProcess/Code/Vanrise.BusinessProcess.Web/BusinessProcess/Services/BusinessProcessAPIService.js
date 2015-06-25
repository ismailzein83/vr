BusinessProcessAPIService.$inject = ['BaseAPIService'];

function BusinessProcessAPIService(BaseAPIService) {

    "use strict";

    function GetOpenedInstances() {
        return BaseAPIService.get("/api/BusinessProcess/GetOpenedInstances");
    }

    function GetDefinitions() {
        return BaseAPIService.get("/api/BusinessProcess/GetDefinitions");
    }

    function GetStatusList() {
        return BaseAPIService.get("/api/BusinessProcess/GetStatusList");
    }

    function StartInstance(processName) {
        return BaseAPIService.post("/api/BusinessProcess/StartInstance", {
            processName: processName
        });
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

    function GetTrackingsByInstanceId(processInstanceID, fromRow, toRow, trackingSeverity, message, lastTrackingId) {
        return BaseAPIService.post("/api/BusinessProcess/GetTrackingsByInstanceId", {
            ProcessInstanceID: processInstanceID,
            FromRow: fromRow,
            ToRow: toRow,
            LastTrackingId: lastTrackingId
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
        GetTrackingSeverity: GetTrackingSeverity,
        GetOpenedInstances: GetOpenedInstances,
        StartInstance: StartInstance
    });

}
appControllers.service('BusinessProcessAPIService', BusinessProcessAPIService);
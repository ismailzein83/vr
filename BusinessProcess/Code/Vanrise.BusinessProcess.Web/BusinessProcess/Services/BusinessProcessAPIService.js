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

    function GetWorkflowTasksByDefinitionId(bpDefinitionId) {
        return BaseAPIService.get("/api/BusinessProcess/GetWorkflowTasksByDefinitionId",
            {
                bpDefinitionId: bpDefinitionId
            }
        );
    }

    function GetWorkflowTasksByDefinitionIds() {
        return BaseAPIService.get("/api/BusinessProcess/GetWorkflowTasksByDefinitionIds");
    }

    function CreateNewProcess(createProcessInput) {
        return BaseAPIService.post("/api/BusinessProcess/CreateNewProcess", createProcessInput);
    }

    function GetFilteredBProcess(input) {
        return BaseAPIService.post("/api/BusinessProcess/GetFilteredBProcess", input);
    }

    function GetFilteredDefinitions(title) {
        return BaseAPIService.get("/api/BusinessProcess/GetFilteredDefinitions",
            {
                title: title
            });
    }

    function GetDefinition(id) {
        return BaseAPIService.get("/api/BusinessProcess/GetDefinition",
            {
                id: id
            });
    }

    function GetTrackingsByInstanceId(processInstanceID, fromRow, toRow, lastTrackingId) {
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

    function GetBPInstance(id) {
        return BaseAPIService.get("/api/BusinessProcess/GetBPInstance", {
            id: id
        });
    }

    function GetNonClosedStatuses() {
        return BaseAPIService.get("/api/BusinessProcess/GetNonClosedStatuses");
    }

    return ({
        GetDefinitions: GetDefinitions,
        GetFilteredDefinitions: GetFilteredDefinitions,
        GetStatusList: GetStatusList,
        GetWorkflowTasksByDefinitionId: GetWorkflowTasksByDefinitionId,
        GetWorkflowTasksByDefinitionIds: GetWorkflowTasksByDefinitionIds,
        GetFilteredBProcess: GetFilteredBProcess,
        GetTrackingsByInstanceId: GetTrackingsByInstanceId,
        GetTrackingSeverity: GetTrackingSeverity,
        GetOpenedInstances: GetOpenedInstances,
        CreateNewProcess: CreateNewProcess,
        GetBPInstance: GetBPInstance,
        GetDefinition: GetDefinition,
        GetNonClosedStatuses: GetNonClosedStatuses
    });

}
appControllers.service('BusinessProcessAPIService', BusinessProcessAPIService);
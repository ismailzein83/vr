(function(appControllers) {

    "use strict";

    businessProcessApiService.$inject = ['BaseAPIService'];

    appControllers.service('BusinessProcessAPIService', businessProcessApiService);

    function businessProcessApiService(BaseAPIService) {

        function GetRecentInstances(statusUpdatedAfter) {
            return BaseAPIService.get("/api/BusinessProcess/GetRecentInstances",
                {
                    StatusUpdatedAfter: statusUpdatedAfter
                });
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

        function GetFilteredTrackings(input) {
            return BaseAPIService.post("/api/BusinessProcess/GetFilteredTrackings", input);
        }

        function GetTrackingsFrom(processInstanceId,fromTrackingId) {
            return BaseAPIService.post("/api/BusinessProcess/GetTrackingsFrom", {
                ProcessInstanceId: processInstanceId,
                FromTrackingId: fromTrackingId
                
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
            GetFilteredTrackings: GetFilteredTrackings,
            GetTrackingSeverity: GetTrackingSeverity,
            GetRecentInstances: GetRecentInstances,
            CreateNewProcess: CreateNewProcess,
            GetBPInstance: GetBPInstance,
            GetDefinition: GetDefinition,
            GetNonClosedStatuses: GetNonClosedStatuses,
            GetTrackingsFrom: GetTrackingsFrom
        });

    }
    
})(appControllers);


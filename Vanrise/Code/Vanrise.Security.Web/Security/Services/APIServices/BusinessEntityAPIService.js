app.service('BusinessEntitiesAPIService', function (BaseAPIService) {

    return ({
        GetEntityNodes: GetEntityNodes,
        ToggleBreakInheritance: ToggleBreakInheritance
    });

    function GetEntityNodes() {
        return BaseAPIService.get("/api/BusinessEntities/GetEntityNodes");
    }

    function ToggleBreakInheritance(entityType, entityId) {
        return BaseAPIService.get("/api/BusinessEntities/ToggleBreakInheritance", 
            {
                entityType: entityType,
                entityId: entityId
            });
    }
});
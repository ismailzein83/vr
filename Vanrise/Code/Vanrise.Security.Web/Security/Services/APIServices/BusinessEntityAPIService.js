app.service('BusinessEntitiesAPIService', function (BaseAPIService) {

    return ({
        GetEntityNodes: GetEntityNodes
    });

    function GetEntityNodes() {
        return BaseAPIService.get("/api/BusinessEntities/GetEntityNodes");
    }
});
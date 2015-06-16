app.service('BusinessEntityAPIService2', function (BaseAPIService) {

    return ({
        GetEntityNodes: GetEntityNodes
    });

    function GetEntityNodes() {
        return BaseAPIService.get("/api/BusinessEntities/GetEntityNodes");
    }
});
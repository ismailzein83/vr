app.service('MenuAPIService', function (BaseAPIService) {

    return ({
        GetMenuItems: GetMenuItems
    });

    function GetMenuItems() {
        return BaseAPIService.get("/api/Menu/GetMenuItems");
    }
});